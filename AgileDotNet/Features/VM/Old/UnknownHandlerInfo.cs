using System;
using System.Collections.Generic;

using dnlib.DotNet;
using dnlib.DotNet.Emit;

using de4dot.blocks;

namespace AgileDotNet.Features.VM.Old
{
    public class UnknownHandlerInfo
    {
        private TypeDef type;
        private CSVMInfo csvmInfo;
        private FieldsInfo fieldsInfo;
        private MethodDef readMethod, execMethod;

        private int pStaticMethods, pInstanceMethods, pVirtualMethods, pCtors;
        private int execMethodThrows, execMethodPops;

        public MethodDef ReadMethod => readMethod;
        public MethodDef ExecMethod => execMethod;

        public int StaticMethods => pStaticMethods;
        public int InstanceMethods => pInstanceMethods;
        public int VirtualMethods => pVirtualMethods;
        public int ExecMethodThrows => execMethodThrows;
        public int ExecMethodPops => execMethodPops;
        public int Ctors => pCtors;

        private static IEnumerable<FieldDef> GetFields(TypeDef type)
        {
            FieldDefAndDeclaringTypeDict<FieldDef> typeFields = new FieldDefAndDeclaringTypeDict<FieldDef>();

            foreach (FieldDef field in type.Fields)
                typeFields.Add(field, field);

            Dictionary<FieldDef, bool> realFields = new Dictionary<FieldDef, bool>();

            foreach(MethodDef method in type.Methods)
            {
                if (method.Body == null)
                    continue;

                foreach(Instruction inst in method.Body.Instructions)
                {
                    IField field = inst.Operand as IField;

                    if (field == null)
                        continue;

                    FieldDef field2 = typeFields.Find(field);

                    if (field2 == null)
                        continue;

                    realFields[field2] = true;
                }
            }

            return realFields.Keys;
        }
        private void CountMethods()
        {
            foreach(MethodDef method in type.Methods)
            {
                if (method.Name == ".ctor")
                    pCtors++;
                else if (method.IsStatic)
                    pStaticMethods++;
                else if (method.IsVirtual)
                    pVirtualMethods++;
                else
                    pInstanceMethods++;
            }
        }
        private void FindOverrideMethods()
        {
            foreach(MethodDef method in type.Methods)
            {
                if (!method.IsVirtual)
                    continue;

                if(DotNetUtils.IsMethod(method, "System.Void", "(System.IO.BinaryReader)"))
                {
                    if (readMethod != null)
                        throw new Exception("Found another read method");

                    readMethod = method;
                }else if(!DotNetUtils.HasReturnValue(method) && method.MethodSig.GetParamCount() == 1)
                {
                    if (execMethod != null)
                        throw new Exception("Found another exec method");

                    execMethod = method;
                }
            }

            if (readMethod == null)
                throw new Exception("Failed to find read method");

            if (execMethod == null)
                throw new Exception("Failed to find exec method");
        }
        private static int CountThrows(MethodDef method)
        {
            int count = 0;

            foreach (Instruction inst in method.Body.Instructions)
                if (inst.OpCode.Code == Code.Throw)
                    count++;

            return count;
        }
        private int CountPops(MethodDef method)
        {
            int count = 0;

            foreach(Instruction inst in method.Body.Instructions)
            {
                if (inst.OpCode.Code != Code.Call && inst.OpCode.Code != Code.Callvirt)
                    continue;

                IMethod called = inst.Operand as IMethod;

                if (!MethodEqualityComparer.CompareDeclaringTypes.Equals(called, csvmInfo.PopMethod))
                    continue;

                count++;
            }

            return count;
        }

        public UnknownHandlerInfo(TypeDef type, CSVMInfo csvmInfo)
        {
            this.type = type;
            this.csvmInfo = csvmInfo;

            fieldsInfo = new FieldsInfo(GetFields(type));

            CountMethods();
            FindOverrideMethods();

            execMethodThrows = CountThrows(execMethod);
            execMethodPops = CountPops(execMethod);
        }

        public bool HasSameFieldTypes(object[] fieldTypes) => new FieldsInfo(fieldTypes).IsSame(fieldsInfo);
    }
}
