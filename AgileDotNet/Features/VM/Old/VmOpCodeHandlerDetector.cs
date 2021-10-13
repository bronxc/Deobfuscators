using System;
using System.Collections.Generic;

using dnlib.DotNet;
using dnlib.DotNet.Emit;

using de4dot.blocks.cflow;

namespace AgileDotNet.Features.VM.Old
{
    public class OpCodeHandlerSigInfo
    {
        public object[] RequiredFieldTypes { get; set; }
        public string[] ExecuteMethodLocals { get; set; }
        public int? ExecuteMethodThrows { get; set; }
        public int? ExecuteMethodPops { get; set; }
        public int? StaticMethods { get; set; }
        public int? InstanceMethods { get; set; }
        public int? VirtualMethods { get; set; }
        public int? Ctors { get; set; }
    }
    public class CSVMInfo
    {
        public TypeDef StackValue { get; set; }
        public TypeDef Stack { get; set; }
        public MethodDef PopMethod { get; set; }
        public MethodDef PeekMethod { get; set; }
    }
    public class VmOpCodeHandlerDetector
    {
        private ModuleDefMD module;
        private List<OpCodeHandler> opCodeHandlers;

        public List<OpCodeHandler> Handlers => opCodeHandlers;

        private bool IsStackType(TypeDef type)
        {
            if (type.Fields.Count != 2)
                return false;

            int enumTypes = 0;
            int objectTypes = 0;

            foreach(FieldDef field in type.Fields)
            {
                TypeDef fieldType = field.FieldSig.GetFieldType().TryGetTypeDef();

                if (fieldType != null && fieldType.IsEnum)
                    enumTypes++;

                if (field.FieldSig.GetFieldType().GetElementType() == ElementType.Object)
                    objectTypes++;
            }

            if (enumTypes != 1 || objectTypes != 1)
                return false;

            return true;
        }
        private static bool ImplementsInterface(TypeDef type, string iface)
        {
            foreach (InterfaceImpl ifa in type.Interfaces)
                if (ifa.Interface.FullName == iface)
                    return true;

            return false;
        }
        private static bool HasAdd(MethodDef method)
        {
            foreach(Instruction inst in method.Body.Instructions)
            {
                if (inst.OpCode.Code == Code.Add)
                    return true;
            }

            return false;
        }
        private bool IsStackType(TypeDef type, TypeDef stackValueType)
        {
            if (type.Interfaces.Count != 2)
                return false;
            if (!ImplementsInterface(type, "System.Collectrions.ICollection"))
                return false;
            if (!ImplementsInterface(type, "System.Collections.IEnumerable"))
                return false;
            if (type.NestedTypes.Count == 0)
                return false;

            int stackValueTypes = 0;
            int int32Types = 0;
            int objectTypes = 0;

            foreach(FieldDef field in type.Fields)
            {
                if (field.IsLiteral)
                    continue;

                TypeSig sig = field.FieldSig.GetFieldType();

                if (sig == null)
                    continue;
                if (sig.IsSZArray && sig.Next.TryGetTypeDef() == stackValueType)
                    stackValueTypes++;
                if (sig.ElementType == ElementType.I4)
                    int32Types++;
                if (sig.ElementType == ElementType.Object)
                    objectTypes++;
            }

            if (stackValueTypes != 2 || int32Types != 2 || objectTypes != 1)
                return false;

            return true;
        }
        private void InitStackTypeMethods(CSVMInfo info)
        {
            foreach(MethodDef method in info.Stack.Methods)
            {
                MethodSig sig = method.MethodSig;

                if (sig != null && sig.Params.Count == 0 && sig.RetType.TryGetTypeDef() == info.StackValue)
                    if (HasAdd(method))
                        info.PopMethod = method;
                    else info.PeekMethod = method;
            }
        }
        private TypeDef FindStackValueType()
        {
            foreach (TypeDef type in module.Types)
                if (IsStackType(type))
                    return type;

            return null;
        }
        private TypeDef FindStackType(TypeDef stackValueType)
        {
            foreach (TypeDef type in module.Types)
                if (IsStackType(type, stackValueType))
                    return type;

            return null;
        }
        private CSVMInfo CreateCsvmInfo()
        {
            CSVMInfo info = new CSVMInfo();

            info.StackValue = FindStackValueType();
            info.Stack = FindStackType(info.StackValue);

            InitStackTypeMethods(info);

            return info;
        }

        private static List<TypeDef> FindVmHandlerTypes(MethodDef method)
        {
            List<TypeDef> list = new List<TypeDef>();

            foreach(Instruction inst in method.Body.Instructions)
            {
                if (inst.OpCode.Code == Code.Ldtoken)
                    continue;

                TypeDef type = inst.Operand as TypeDef;

                if (type == null)
                    continue;

                list.Add(type);
            }

            return list;
        }
        private List<TypeDef> FindVmHandlerTypes() // Check if error
        {
            string[] requiredFields = new string[]
            {
                null,
                "System.Collections.Generic.Dictionary`2<System.UInt16,System.Type>",
                "System.UInt16"
            };

            CflowDeobfuscator cflow = new CflowDeobfuscator();

            foreach(TypeDef type in module.Types)
            {
                MethodDef cctor = type.FindStaticConstructor();

                if (cctor == null)
                    continue;

                requiredFields[0] = type.FullName;

                Utils.DebugN<string>(new FieldTypes(type).Strings);

                if (!new FieldTypes(type).Exactly(requiredFields))
                    continue;

                cflow.Deobfuscate(cctor);

                List<TypeDef> handlers = FindVmHandlerTypes(cctor);

                Utils.DebugN("Handlers", handlers.Count);

                if (handlers.Count != 31) // 1050
                    continue;

                return handlers;
            }

            return null;
        }
        private void DetectHandlers(List<TypeDef> handlerTypes, CSVMInfo csvmInfo)
        {
            opCodeHandlers = new List<OpCodeHandler>();

            List<OpCodeHandler> detected = new List<OpCodeHandler>();

            foreach(OpCodeHandler[] handlerList in OpCodeHandlers.Handlers)
            {
                opCodeHandlers.Clear();

                foreach(TypeDef handlerType in handlerTypes)
                {
                    UnknownHandlerInfo info = new UnknownHandlerInfo(handlerType, csvmInfo);

                    detected.Clear();

                    foreach (OpCodeHandler opCodeHandler in handlerList)
                        if (opCodeHandler.Detect(info))
                            detected.Add(opCodeHandler);

                    if (detected.Count != 1)
                        goto next;
                }

                if (new List<OpCodeHandler>(Utils.Unique(opCodeHandlers)).Count == opCodeHandlers.Count)
                    return;

                next:;
            }

            throw new Exception("Could not find all VM opcodes handlers");
        }

        public VmOpCodeHandlerDetector(ModuleDefMD module) => this.module = module;

        public void FindHandlers()
        {
            if (opCodeHandlers != null)
                return;

            List<TypeDef> vmHandlerTypes = FindVmHandlerTypes();

            if (vmHandlerTypes == null)
                throw new Exception("Could not find CSVM opcode handler types");

            DetectHandlers(vmHandlerTypes, CreateCsvmInfo());
        }
    }
}
