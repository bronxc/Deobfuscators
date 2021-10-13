using System;
using System.Collections.Generic;
using System.IO;

using dnlib.DotNet;
using de4dot.blocks;

namespace AgileDotNet.Features.VM.Current
{
    public class CSVM
    {
        private IContext context;
        private ModuleDefMD module;
        private EmbeddedResource resource;
        private AssemblyRef vmAssemblyRef;

        public bool Detected => resource != null && vmAssemblyRef != null;
        public EmbeddedResource Resource => Detected ? resource : null;


        private AssemblyRef FindVmAssemblyRef()
        {
            foreach(MemberRef member in module.GetMemberRefs())
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

        public CSVM(IContext context, ModuleDefMD module)
        {
            this.context = context;
            this.module = module;
        }
        public CSVM(IContext context, ModuleDefMD module, CSVM other) {
            this.context = context;
            this.module = module;

            if (other.resource != null)
                resource = (EmbeddedResource)module.Resources[other.module.Resources.IndexOf(other.resource)];

            if (other.vmAssemblyRef != null)
                vmAssemblyRef = module.ResolveAssemblyRef(other.vmAssemblyRef.Rid);
        }

        public void Find()
        {
            resource = FindCSVMResource();
            vmAssemblyRef = FindVmAssemblyRef();
        }
    }
}
