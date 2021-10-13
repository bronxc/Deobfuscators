using System;
using System.Reflection; 
using System.Collections.Generic;
using System.IO;

using AgileDotNet.Features.VM;
using AgileDotNet.Features.VM.Old;

using dnlib.DotNet;
using dnlib.IO;
using de4dot.blocks;

namespace AgileDotNet.Features
{
    public class CSVMData
    {
        private DataReader reader;

        public CSVMData(DataReader reader)
        {
            reader.Position = 0;
            this.reader = reader;
        }

        public List<VirtualMethodDef> Read()
        {
            int FuncCount = reader.ReadInt32();
            if (FuncCount < 0)
                throw new ApplicationException("Invalid number of CSVM methods");

            List<VirtualMethodDef> list = new List<VirtualMethodDef>(FuncCount);

            for(int i = 0; i < FuncCount; i++)
            {
                VirtualMethodDef vMethod;
                vMethod.Guid = new Guid(reader.ReadBytes(16));
                vMethod.MethodToken = reader.ReadUInt32();
                vMethod.LocalVarStream = reader.ReadBytes(reader.ReadInt32());
                vMethod.CodeStream = reader.ReadBytes(reader.ReadInt32());
                vMethod.ExceptionHandlerStream = reader.ReadBytes(reader.ReadInt32());
                Utils.Info($"Adding '{vMethod.Guid.ToString()}'");
                list.Add(vMethod);
            }

            return list;
        }
    }
    public class DeVirt
    {
        private IContext context;
        private ModuleDef module;
        private EmbeddedResource resource;
        private AssemblyRef vmAssemblyRef;
        private Assembly Asm;

        private bool CanBeDevirted = false;
        public int RestoredMethods { get; private set; } = 0;

        public bool Detected => resource != null && vmAssemblyRef != null;
        public EmbeddedResource Resource => Detected ? resource : null;

        public int DeVirtualizedFuncs { get; private set; } = 0;

        private ModuleDefMD Convert(ModuleDef module)
        {
            ModuleDefMD mdl = ModuleDefMD.Load(GLOBALS.CurrentFile, ModuleDef.CreateModuleContext());
            return mdl; // mdl.ResolveModule(module.Rid) as ModuleDefMD;
        }
        private AssemblyRef FindVmAssemblyRef()
        {
            foreach (MemberRef member in module.GetMemberRefs())
            {
                MethodSig sig = member.MethodSig;

                if (sig == null)
                    continue;
                if (sig.RetType.GetElementType() != ElementType.Object)
                    continue;
                if (sig.Params.Count != 2)
                    continue;
                if (member.Name != "RunMethod")
                    continue;
                if (member.FullName == "System.Object VMRuntime.Libraries.CSVMRuntime::RunMethod(System.String,System.Object[])")
                    return member.DeclaringType.Scope as AssemblyRef;
            }

            return null;
        }
        private EmbeddedResource FindCSVMResource() => DotNetUtils.GetResource(module, "_CSVM") as EmbeddedResource;

        private VmOpCodeHandlerDetector GetVmOpCodeHandlerDetector()
        {
            string vmFilename = vmAssemblyRef.Name + ".dll";
            string vmModulePath = Path.Combine(Path.GetDirectoryName(module.Location), vmFilename);

            string dataKey = "Ops";
            Dictionary<string, VmOpCodeHandlerDetector> dict = (Dictionary<string, VmOpCodeHandlerDetector>)context.GetData(dataKey);

            if (dict == null)
                context.SetData(dataKey, dict = new Dictionary<string, VmOpCodeHandlerDetector>(StringComparer.OrdinalIgnoreCase));

            if (dict.TryGetValue(vmFilename, out VmOpCodeHandlerDetector detector))
                return detector;

            

            dict[vmFilename] = detector = new VmOpCodeHandlerDetector(ModuleDefMD.Load(vmFilename));

            detector.FindHandlers();
            Utils.Info($"CSVM opcodes {detector.Handlers.Count}");

            return detector;
        }
        private void Restore2()
        {
            Utils.Warn("Restoring CSVM methods");

            VmOpCodeHandlerDetector opcodeDetector = GetVmOpCodeHandlerDetector();

            DataReader dataReader = new DataReader();
            dataReader.CopyTo(new BinaryWriter(Asm.GetManifestResourceStream("_CSVM")));
            List<VirtualMethodDef> csvmMethods = new CSVMData(dataReader).Read(); //resource.CreateReader()
            CSVMToCilMethodConverter converter = new CSVMToCilMethodConverter(context, (ModuleDefMD)module, opcodeDetector);

            foreach(VirtualMethodDef method in csvmMethods)
            {
                MethodDef cilMethod = module.ResolveToken(method.MethodToken) as MethodDef;

                if (cilMethod == null)
                    throw new ApplicationException($"Could not find method '{method.Guid.ToString()}'");

                converter.Convert(cilMethod, method);
                RestoredMethods++;
            }
        }

        public DeVirt(IContext context, ModuleDef module)
        {
            this.module = module;
            this.context = context;
        }
        public DeVirt(IContext context, ModuleDef module, DeVirt other)
        {
            this.context = context;
            this.module = module;

            if (other.resource != null)
                resource = (EmbeddedResource)module.Resources[other.module.Resources.IndexOf(other.resource)];

            if (other.vmAssemblyRef != null)
                vmAssemblyRef = ((ModuleDefMD)module).ResolveAssemblyRef(other.vmAssemblyRef.Rid);
        }

        public void Init(Assembly asm)
        {
            //new DataReader().CopyTo(new BinaryWriter(asm.GetManifestResourceStream("_CSVM")));
            Asm = asm;
            //resource = FindCSVMResource();
            vmAssemblyRef = FindVmAssemblyRef();

            if(asm.GetManifestResourceStream("_CSVM") != null)
            {
                CanBeDevirted = true;
                Utils.Info("Starting CSVM");
            }
            else
            {
                CanBeDevirted = false;
                Utils.Error("Devirtualization will be skipped due to CSVM not being found!");
            }
        }
        public bool Restore()
        {
            /*if (!Detected || !CanBeDevirted)
                return true;*/

            try
            {
                Restore2();
                return true;
            }
            catch(Exception ex)
            {
                Utils.HandleError(ex);
                return false;
            }
        }
    }
}
