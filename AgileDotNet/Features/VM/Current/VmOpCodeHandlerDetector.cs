using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dnlib.DotNet;
using dnlib.DotNet.Emit;

using de4dot.blocks;
using de4dot.blocks.cflow;

namespace AgileDotNet.Features.VM.Current
{
    public class MyDeobfuscator
    {
        private StringDecryptor StringDecryptor;

        private void RestoreMethod(Blocks blocks)
        {
            blocks.GetCode(out IList<Instruction> allInstructions, out IList<ExceptionHandler> allExceptionHandlers);
            DotNetUtils.RestoreBody(blocks.Method, allInstructions, allExceptionHandlers);
        }

        public MyDeobfuscator(ModuleDefMD module)
        {
            StringDecryptor = new StringDecryptor(module.Assembly, GLOBALS.CurrentFile);
        }

        public void DecryptStrings(MethodDef method)
        {
            if(method.HasBody && method.Body.HasInstructions)
            {
                for(int i = 0; i < method.Body.Instructions.Count; i++)
                {
                    if(method.Body.Instructions[i].OpCode == OpCodes.Ldstr && method.Body.Instructions[i + 1].OpCode == OpCodes.Call && method.Body.Instructions[i + 1].Operand.ToString().Contains(StringDecryptor.GetMethod()))
                    {
                        string encrypted = method.Body.Instructions[i].Operand.ToString();
                        string decrypted = StringDecryptor.Decrypt(encrypted);

                        method.Body.Instructions[i].Operand = decrypted;
                        method.Body.Instructions[i + 1].OpCode = OpCodes.Nop;

                        i++;
                    }
                }
            }
        }
        public void DecryptStrings(Blocks blocks) => DecryptStrings(blocks.Method);

        public void Deobfuscate(MethodDef method) => DecryptStrings(method);
    }

    public class VmOpCodeHandlerDetector
    {
        private const int HANDLERS = 78;
        private ModuleDefMD module;
        private List<VmOpCode> vmOpcodes;
        private MyDeobfuscator deobfuscator;

        public IList<VmOpCode> Handlers => vmOpcodes;

        private static List<VmOpCode> CreateVMOpCodes(IList<CompositeOpCodeHandler> composites)
        {
            List<VmOpCode> list = new List<VmOpCode>(composites.Count);

            foreach (CompositeOpCodeHandler composite in composites)
                list.Add(new VmOpCode(composite.TypeCodes));

            return list;
        }
        private bool DetectCompositeHandlers(IEnumerable<CompositeOpCodeHandler> composites, IList<MethodSigInfo> handlerInfos)
        {
            CompositeHandlerDetector detector = new CompositeHandlerDetector(handlerInfos);

            foreach (CompositeOpCodeHandler composite in composites)
            {
                if (!detector.FindHandlers(composite))
                {
                    return false;
                }
                    
            }
                

            return true;
        }
        private static void AddTypeId(SigCreator sig, MethodDef method, int id)
        {
            if (method != null)
                sig.AddId(method.DeclaringType, id);
        }
        private static void GetReadAndExecMethods(TypeDef handler, out MethodDef readMethod, out MethodDef execMethod)
        {
            readMethod = execMethod = null;

            foreach(MethodDef method in handler.Methods)
            {
                if (!method.IsVirtual)
                    continue;

                if(DotNetUtils.IsMethod(method, "System.Void", "(System.IO.BinaryReader)"))
                {
                    if (readMethod != null)
                        throw new ApplicationException("Found a second read method");

                    readMethod = method;
                }else if(!DotNetUtils.HasReturnValue(method) && method.MethodSig.GetParamCount() == 1)
                {
                    if (execMethod != null)
                        throw new ApplicationException("Found a second exec method");

                    execMethod = method;
                }
            }

            if (readMethod == null)
                throw new ApplicationException("Did not find a read method");
            if (execMethod == null)
                throw new ApplicationException("Did not find an exec method");
        }
        IEnumerable<TypeDef> GetVMHandlerTypes(TypeDef baseType)
        {
            foreach (TypeDef type in module.Types)
                if (type.BaseType == baseType)
                    yield return type;
        }
        private static MethodDef SimplifyInstructions(MethodDef method)
        {
            if (method.Body == null)
                return method;

            method.Body.SimplifyMacros(method.Parameters);
            return method;
        }
        private List<TypeDef> FindBasicVMHandlerTypes(CSVMInfo csvmInfo)
        {
            List<TypeDef> list = new List<TypeDef>();

            if (csvmInfo.VmHandlerBaseType == null)
                return list;

            foreach(TypeDef type in module.Types)
            {
                if (list.Count == HANDLERS)
                    break;

                if (type.BaseType == csvmInfo.VmHandlerBaseType)
                    list.Add(type);
            }

            return list;
        }
        private static List<TypeDef> FindVMHandlerTypes(MethodDef method)
        {
            List<TypeDef> list = new List<TypeDef>();

            foreach(Instruction instruction in method.Body.Instructions)
            {
                if (instruction.OpCode.Code != Code.Ldtoken)
                    continue;

                TypeDef type = instruction.Operand as TypeDef;

                if (type == null)
                    continue;

                list.Add(type);
            }

            return list;
        }
        private static SigCreator CreateSigCreator(CSVMInfo csvmInfo)
        {
            SigCreator creator = new SigCreator();

            creator.AddId(csvmInfo.LogicalOpShrUn, 1);
            creator.AddId(csvmInfo.LogicalOpShl, 2);
            creator.AddId(csvmInfo.LogicalOpShr, 3);
            creator.AddId(csvmInfo.LogicalOpAnd, 4);
            creator.AddId(csvmInfo.LogicalOpXor, 5);
            creator.AddId(csvmInfo.LogicalOpOr, 6);

            creator.AddId(csvmInfo.CompareLt, 7);
            creator.AddId(csvmInfo.CompareLte, 8);
            creator.AddId(csvmInfo.CompareGt, 9);
            creator.AddId(csvmInfo.CompareGte, 10);
            creator.AddId(csvmInfo.CompareEq, 11);
            creator.AddId(csvmInfo.CompareEqz, 12);

            creator.AddId(csvmInfo.ArithmeticSubOvfUn, 13);
            creator.AddId(csvmInfo.ArithmeticMulOvfUn, 14);
            creator.AddId(csvmInfo.ArithmeticRemUn, 15);
            creator.AddId(csvmInfo.ArithmeticRem, 16);
            creator.AddId(csvmInfo.ArithmeticDivUn, 17);
            creator.AddId(csvmInfo.ArithmeticDiv, 18);
            creator.AddId(csvmInfo.ArithmeticMul, 19);
            creator.AddId(csvmInfo.ArithmeticMulOvf, 20);
            creator.AddId(csvmInfo.ArithmeticSub, 21);
            creator.AddId(csvmInfo.ArithmeticSubOvf, 22);
            creator.AddId(csvmInfo.ArithmeticAddOvfUn, 23);
            creator.AddId(csvmInfo.ArithmeticAddOvf, 24);
            creator.AddId(csvmInfo.ArithmeticAdd, 25);

            creator.AddId(csvmInfo.UnaryNot, 26);
            creator.AddId(csvmInfo.UnaryNeg, 27);

            creator.AddId(csvmInfo.ArgsGet, 28);
            creator.AddId(csvmInfo.ArgsSet, 29);
            creator.AddId(csvmInfo.LocalsGet, 30);
            creator.AddId(csvmInfo.LocalsSet, 31);

            AddTypeId(creator, csvmInfo.LogicalOpShrUn, 32);
            AddTypeId(creator, csvmInfo.CompareLt, 33);
            AddTypeId(creator, csvmInfo.ArithmeticSubOvfUn, 34);
            AddTypeId(creator, csvmInfo.UnaryNot, 35);
            AddTypeId(creator, csvmInfo.ArgsGet, 36);

            return creator;
        }
        private static MethodDef GetExecMethod(MyDeobfuscator deobfuscator, TypeDef type)
        {
            GetReadAndExecMethods(type, out MethodDef readMethod, out MethodDef execMethod);
            deobfuscator.Deobfuscate(execMethod);
            SimplifyInstructions(execMethod);
            return execMethod;
        }
        private MethodDef GetExecMethod(TypeDef type) => GetExecMethod(deobfuscator, type);
        private List<CompositeOpCodeHandler> CreateCompositeOpCodeHandlers(CSVMInfo csvmInfo, List<TypeDef> handlers)
        {
            List<CompositeOpCodeHandler> list = new List<CompositeOpCodeHandler>();
            SigCreator sig = CreateSigCreator(csvmInfo);

            foreach (TypeDef handler in handlers)
                list.Add(new CompositeOpCodeHandler(sig.Create(GetExecMethod(handler))));

            return list;
        }
        private List<TypeDef> FindVMHandlerTypes()
        {
            string[] requiredFields = new string[]
            {
                null,
                "System.Collections.Generic.Dictionary`2<System.UInt16,System.Type>",
                "System.UInt16"
            };

            CflowDeobfuscator cflowDeobfuscator = new CflowDeobfuscator();

            foreach(TypeDef type in module.Types)
            {
                MethodDef cctor = type.FindStaticConstructor();

                if (cctor == null)
                    continue;

                requiredFields[0] = type.FullName;

                FieldTypes fieldType = new FieldTypes(type);

                if (!fieldType.All(requiredFields))
                    continue;

                cflowDeobfuscator.Deobfuscate(cctor);
                List<TypeDef> handlers = FindVMHandlerTypes(cctor);

                return handlers;
            }

            return null;
        }

        public VmOpCodeHandlerDetector(ModuleDefMD module) => this.module = module;

        public void FindHandlers()
        {
            if (vmOpcodes != null)
                return;

            deobfuscator = new MyDeobfuscator(module);
            CSVMInfo csvmInfo = new CSVMInfo(module);
            List<TypeDef> vmHandlerTypes = FindVMHandlerTypes();

            csvmInfo.Initialize();

            if (vmHandlerTypes == null)
                throw new ApplicationException("Could not find CSVM opcode handler type");

            List<CompositeOpCodeHandler> composites = CreateCompositeOpCodeHandlers(csvmInfo, vmHandlerTypes);

            foreach(IList<MethodSigInfo> handlerInfos in OpCodeHandlerInfos.HandlerInfos)
            {
                if (!DetectCompositeHandlers(composites, handlerInfos))
                {
                    Utils.Debug("Skipping");
                    continue;
                }
                    

                vmOpcodes = CreateVMOpCodes(composites);

                break;
            }

            if (vmOpcodes == null)
                throw new ApplicationException("Could not find any CSVM handlers");
        }
    }
}
