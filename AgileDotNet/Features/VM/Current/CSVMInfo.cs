using System.Collections.Generic;

using dnlib.DotNet;
using dnlib.DotNet.Emit;
using de4dot.blocks;

namespace AgileDotNet.Features.VM.Current
{
    public class CSVMInfo
    {
        ModuleDef module;

        public TypeDef VmHandlerBaseType;

        public MethodDef LogicalOpShrUn;
        public MethodDef LogicalOpShl;
        public MethodDef LogicalOpShr;
        public MethodDef LogicalOpAnd;
        public MethodDef LogicalOpXor;
        public MethodDef LogicalOpOr;

        public MethodDef CompareLt;
        public MethodDef CompareLte;
        public MethodDef CompareGt;
        public MethodDef CompareGte;
        public MethodDef CompareEq;
        public MethodDef CompareEqz;

        public MethodDef ArithmeticSubOvfUn;
        public MethodDef ArithmeticMulOvfUn;
        public MethodDef ArithmeticRemUn;
        public MethodDef ArithmeticRem;
        public MethodDef ArithmeticDivUn;
        public MethodDef ArithmeticDiv;
        public MethodDef ArithmeticMul;
        public MethodDef ArithmeticMulOvf;
        public MethodDef ArithmeticSub;
        public MethodDef ArithmeticSubOvf;
        public MethodDef ArithmeticAddOvfUn;
        public MethodDef ArithmeticAddOvf;
        public MethodDef ArithmeticAdd;

        public MethodDef UnaryNot;
        public MethodDef UnaryNeg;

        public MethodDef ArgsGet;
        public MethodDef ArgsSet;
        public MethodDef LocalsGet;
        public MethodDef LocalsSet;


        private static bool CheckLogicalMethodSig(MethodDef method) =>
            method != null &&
            method.IsStatic &&
            method.MethodSig.GetParamCount() == 2 &&
            method.MethodSig.RetType.GetElementType() == ElementType.Object &&
            method.MethodSig.Params[0].GetElementType() == ElementType.Object &&
            method.MethodSig.Params[1].GetElementType() == ElementType.Object;
        private static bool CheckCompareMethodSig(MethodDef method)
        {
            if (method == null || !method.IsStatic)
                return false;

            MethodSig sig = method.MethodSig;

            if (sig == null || sig.GetParamCount() != 3)
                return false;
            if (sig.RetType.GetElementType() != ElementType.Boolean)
                return false;
            if (sig.Params[0].GetElementType() != ElementType.Object)
                return false;
            if (sig.Params[1].GetElementType() != ElementType.Object)
                return false;

            ValueTypeSig arg = sig.Params[2] as ValueTypeSig;
            if (arg == null || arg.TypeDef == null || !arg.TypeDef.IsEnum)
                return false;

            return true;
        }
        private static bool CheckCompareEqMethodSig(MethodDef method) =>
            method != null &&
            method.IsStatic &&
            method.MethodSig.GetParamCount() == 2 &&
            method.MethodSig.RetType.GetElementType() == ElementType.Boolean &&
            method.MethodSig.Params[0].GetElementType() == ElementType.Object &&
            method.MethodSig.Params[1].GetElementType() == ElementType.Object;
        private static bool CheckCompareEqzMethodSig(MethodDef method) =>
            method != null &&
            method.IsStatic &&
            method.MethodSig.GetParamCount() == 1 &&
            method.MethodSig.RetType.GetElementType() == ElementType.Boolean &&
            method.MethodSig.Params[0].GetElementType() == ElementType.Object;
        private static bool CheckArithmeticUnMethodSig(MethodDef method) =>
            method != null &&
            method.IsStatic &&
            method.MethodSig.GetParamCount() == 2 &&
            method.MethodSig.RetType.GetElementType() == ElementType.Object &&
            method.MethodSig.Params[0].GetElementType() == ElementType.Class &&
            method.MethodSig.Params[1].GetElementType() == ElementType.Class;
        private static bool CheckArithmeticOtherMethodSig(MethodDef method) =>
            method != null &&
            method.IsStatic &&
            method.MethodSig.GetParamCount() == 2 &&
            method.MethodSig.RetType.GetElementType() == ElementType.Object &&
            method.MethodSig.Params[0].GetElementType() == ElementType.Object &&
            method.MethodSig.Params[1].GetElementType() == ElementType.Object;
        private static bool CheckCallvirt(Instruction instruction, string returnType, string parameters)
        {
            if (instruction.OpCode.Code != Code.Callvirt)
                return false;
            return DotNetUtils.IsMethod(instruction.Operand as IMethod, returnType, parameters);
        }

        private static int CountThrows(MethodDef method)
        {
            if (method == null || method.Body == null)
                return 0;

            int count = 0;

            foreach (Instruction ins in method.Body.Instructions)
                if (ins.OpCode.Code == Code.Throw)
                    count++;

            return count;
        }
        private static int CountVirtual(TypeDef type)
        {
            int count = 0;

            foreach (MethodDef method in type.Methods)
                if (method.IsVirtual)
                    count++;

            return count;
        }

        private bool CheckUnboxAny(Instruction instruction, ElementType expectedType)
        {
            if (instruction == null || instruction.OpCode.Code != Code.Unbox_Any)
                return false;

            CorLibTypeSig sig = module.CorLibTypes.GetCorLibTypeSig(instruction.Operand as ITypeDefOrRef);

            return sig.GetElementType() == expectedType;
        }
        private bool CheckBox(Instruction instruction, ElementType expectedType)
        {
            if (instruction == null || instruction.OpCode.Code != Code.Box)
                return false;

            CorLibTypeSig sig = module.CorLibTypes.GetCorLibTypeSig(instruction.Operand as ITypeDefOrRef);

            return sig.GetElementType() == expectedType;
        }


        private MethodDef FindLogicalOpMethod(TypeDef type, ElementType e1, ElementType e2, ElementType e3, Code code)
        {
            foreach(MethodDef method in type.Methods)
            {
                if (!CheckLogicalMethodSig(method))
                    continue;
                if (method.Body == null)
                    continue;

                IList<Instruction> instructions = method.Body.Instructions;
                for(int i = 0;i<instructions.Count - 7; i++)
                {
                    Instruction ldarg0 = instructions[i];

                    if (!ldarg0.IsLdarg() || ldarg0.GetParameterIndex() != 0)
                        continue;
                    if (!CheckUnboxAny(instructions[i + 1], e1))
                        continue;

                    Instruction ldarg1 = instructions[i + 2];

                    if (!ldarg1.IsLdarg() || ldarg1.GetParameterIndex() != 1)
                        continue;
                    if (!CheckUnboxAny(instructions[i + 3], e2))
                        continue;

                    Instruction ldci4 = instructions[i + 4];

                    if (!ldci4.IsLdcI4() || ldci4.GetLdcI4Value() != 0x1F)
                        continue;
                    if (instructions[i + 5].OpCode.Code != Code.And)
                        continue;
                    if (instructions[i + 6].OpCode.Code != code)
                        continue;
                    if (!CheckBox(instructions[i + 7], e3))
                        continue;

                    return method;
                }
            }

            return null;
        }
        private MethodDef FindLogicalOpMethod(TypeDef type, Code code)
        {
            foreach(MethodDef method in type.Methods)
            {
                if (!CheckLogicalMethodSig(method))
                    continue;
                if (method.Body == null)
                    continue;

                IList<Instruction> instructions = method.Body.Instructions;
                
                for(int i = 0;i<instructions.Count - 5; i++)
                {
                    Instruction ldarg0 = instructions[i];

                    if (!ldarg0.IsLdarg() || ldarg0.GetParameterIndex() != 0)
                        continue;
                    if (!CheckUnboxAny(instructions[i + 1], ElementType.I4))
                        continue;

                    Instruction ldarg1 = instructions[i + 2];
                    if (!ldarg1.IsLdarg() || ldarg1.GetParameterIndex() != 1)
                        continue;
                    if (!CheckUnboxAny(instructions[i + 3], ElementType.I4))
                        continue;
                    if (instructions[i + 4].OpCode.Code != code)
                        continue;
                    if (!CheckBox(instructions[i + 5], ElementType.I4))
                        continue;

                    return method;
                }
            }

            return null;
        }
        private MethodDef FindCompareMethod(TypeDef type, Code code, bool invert)
        {
            foreach(MethodDef method in type.Methods)
            {
                if (!CheckCompareMethodSig(method))
                    continue;
                if (method.Body == null)
                    continue;

                IList<Instruction> instructions = method.Body.Instructions;
                int end = instructions.Count - 6;

                if (invert)
                    end -= 2;

                for(int i = 0; i < end; i++)
                {
                    int index = i;
                    Instruction ldarg0 = instructions[index++];

                    if (!ldarg0.IsLdarg() || ldarg0.GetParameterIndex() != 0)
                        continue;
                    if (!CheckUnboxAny(instructions[index++], ElementType.I4))
                        continue;

                    Instruction ldargs1 = instructions[index++];

                    if (!ldargs1.IsLdarg() || ldargs1.GetParameterIndex() != 1)
                        continue;
                    if (!CheckUnboxAny(instructions[index++], ElementType.I4))
                        continue;
                    if (instructions[index++].OpCode.Code != code)
                        continue;

                    if (invert)
                    {
                        Instruction ldic4 = instructions[index++];

                        if (!ldic4.IsLdcI4() || ldic4.GetLdcI4Value() != 0)
                            continue;
                        if (instructions[index++].OpCode.Code != Code.Ceq)
                            continue;
                    }

                    if (!instructions[index++].IsStloc())
                        continue;

                    return method;
                }
            }

            return null;
        }
        private MethodDef FindArithmeticOpUn(TypeDef type, Code code)
        {
            foreach(MethodDef method in type.Methods)
            {
                if (!CheckArithmeticUnMethodSig(method))
                    continue;
                if (method.Body == null)
                    continue;

                IList<Instruction> instructions = method.Body.Instructions;

                for(int i = 0; i < instructions.Count; i++)
                {
                    Instruction ldarg0 = instructions[i];

                    if (!ldarg0.IsLdarg() || ldarg0.GetParameterIndex() != 0)
                        continue;
                    if (!CheckCallvirt(instructions[i + 1], "System.Int32", "()"))
                        continue;
                    if (instructions[i + 2].OpCode.Code != Code.Conv_Ovf_U4)
                        continue;

                    Instruction ldarg1 = instructions[i + 3];

                    if (!ldarg1.IsLdarg() || ldarg1.GetParameterIndex() != 1)
                        continue;
                    if (!CheckCallvirt(instructions[i + 4], "System.Int32", "()"))
                        continue;
                    if (instructions[i + 5].OpCode.Code != Code.Conv_Ovf_U4)
                        continue;
                    if (instructions[i + 6].OpCode.Code != code)
                        continue;
                    if (!CheckBox(instructions[i + 7], ElementType.U4))
                        continue;
                    if (!instructions[i + 8].IsStloc())
                        continue;

                    return method;
                }
            }

            return null;
        }
        private MethodDef FindArithmeticDivOrRemUn(TypeDef type, Code code)
        {
            foreach(MethodDef method in type.Methods)
            {
                if (!CheckArithmeticUnMethodSig(method))
                    continue;
                if (method.Body == null)
                    continue;

                IList<Instruction> instructions = method.Body.Instructions;

                for(int i = 0; i < instructions.Count; i++)
                {
                    Instruction ldarg0 = instructions[i];

                    if (!ldarg0.IsLdarg() || ldarg0.GetParameterIndex() != 0)
                        continue;
                    if (!CheckCallvirt(instructions[i + 1], "System.Int32", "()"))
                        continue;

                    Instruction ldarg1 = instructions[i + 2];

                    if (!ldarg1.IsLdarg() || ldarg1.GetParameterIndex() != 1)
                        continue;
                    if (!CheckCallvirt(instructions[i + 3], "System.Int32", "()"))
                        continue;
                    if (instructions[i + 4].OpCode.Code != code)
                        continue;
                    if (!CheckBox(instructions[i + 5], ElementType.U4))
                        continue;
                    if (!instructions[i + 6].IsStloc())
                        continue;

                    return method;
                }
            }

            return null;
        }
        private MethodDef FindArithmeticOther(TypeDef type,Code code)
        {
            foreach(MethodDef method in type.Methods)
            {
                if (!CheckArithmeticOtherMethodSig(method))
                    continue;
                if (method.Body == null)
                    continue;

                IList<Instruction> instructions = method.Body.Instructions;

                for(int i = 0; i < instructions.Count; i++)
                {
                    Instruction ldarg0 = instructions[i];

                    if (!ldarg0.IsLdarg() || ldarg0.GetParameterIndex() != 0)
                        continue;
                    if (!CheckUnboxAny(instructions[i + 1], ElementType.I4))
                        continue;

                    Instruction ldarg1 = instructions[i + 2];

                    if (!ldarg1.IsLdarg() || ldarg1.GetParameterIndex() != 1)
                        continue;
                    if (!CheckUnboxAny(instructions[i + 3], ElementType.I4))
                        continue;
                    if (instructions[i + 4].OpCode.Code != code)
                        continue;
                    if (!CheckBox(instructions[i + 5], ElementType.I4))
                        continue;

                    return method;
                }
            }

            return null;
        }

        private bool IsUnsaryMethod(MethodDef method, Code code)
        {
            if (!method.HasBody || !method.IsStatic)
                return false;
            if (!DotNetUtils.IsMethod(method, "System.Object", "(System.Object)"))
                return false;
            if (CountThrows(method) != 1)
                return false;

            IList<Instruction> instructions = method.Body.Instructions;

            for(int i = 0; i < instructions.Count; i++)
            {
                Instruction ldarg = instructions[i];

                if (!ldarg.IsLdarg() || ldarg.GetParameterIndex() != 0)
                    continue;
                if (!CheckUnboxAny(instructions[i + 1], ElementType.I4))
                    continue;
                if (instructions[i + 2].OpCode.Code != code)
                    continue;
                if (!CheckBox(instructions[i + 3], ElementType.I4))
                    continue;
                if (!instructions[i + 4].IsStloc())
                    continue;

                return true;
            }

            return false;
        }
        private static MethodDef FindSetter(MethodDef ctor, int arg)
        {
            if (ctor == null || !ctor.HasBody)
                return null;

            IList<Instruction> instructions = ctor.Body.Instructions;

            for(int i = 0; i < instructions.Count; i++)
            {
                Instruction ldarg = instructions[i];

                if (!ldarg.IsLdarg() || ldarg.GetParameterIndex() != arg)
                    continue;

                Instruction call = instructions[i + 1];

                if (call.OpCode.Code != Code.Call)
                    continue;

                MethodDef method = call.Operand as MethodDef;

                if (method == null)
                    continue;

                if (method.DeclaringType != ctor.DeclaringType)
                    continue;

                return method;
            }

            return null;
        }
        private static FieldDef GetPropField(MethodDef ctor)
        {
            if (ctor == null || !ctor.HasBody)
                return null;

            foreach(Instruction ins in ctor.Body.Instructions)
            {
                if (ins.OpCode.Code != Code.Stfld)
                    continue;

                FieldDef field = ins.Operand as FieldDef;

                if (field == null || field.DeclaringType != ctor.DeclaringType)
                    continue;

                return field;
            }

            return null;
        }
        private static MethodDef FindGetter(TypeDef type, FieldDef propField)
        {
            foreach(MethodDef method in type.Methods)
            {
                if (method.IsStatic || !method.HasBody)
                    continue;

                foreach(Instruction ins in method.Body.Instructions)
                {
                    if (ins.OpCode.Code != Code.Ldfld)
                        continue;
                    if (ins.Operand != propField)
                        continue;

                    return method;
                }
            }

            return null;
        }

        private MethodDef FindLogicalOpMethodShrUn(TypeDef type) => FindLogicalOpMethod(type, ElementType.U4, ElementType.I4, ElementType.U4, Code.Shr_Un);
        private MethodDef FindLogicalOpMethodShl(TypeDef type) => FindLogicalOpMethod(type, ElementType.I4, ElementType.I4, ElementType.I4, Code.Shl);
        private MethodDef FindLogicalOpMethodShr(TypeDef type) => FindLogicalOpMethod(type, ElementType.I4, ElementType.I4, ElementType.I4, Code.Shr);
        private MethodDef FindLogicalOpMethodAnd(TypeDef type) => FindLogicalOpMethod(type, Code.Add);
        private MethodDef FindLogicalOpMethodXor(TypeDef type) => FindLogicalOpMethod(type, Code.Xor);
        private MethodDef FindLogicalOpMethodOr(TypeDef type) => FindLogicalOpMethod(type, Code.Or);

        private MethodDef FindCompareLt(TypeDef type) => FindCompareMethod(type, Code.Clt, false);
        private MethodDef FindCompareLte(TypeDef type) => FindCompareMethod(type, Code.Cgt, true);
        private MethodDef FindCompareGt(TypeDef type) => FindCompareMethod(type, Code.Cgt, false);
        private MethodDef FindCompareGte(TypeDef type) => FindCompareMethod(type, Code.Clt, true);
        private MethodDef FindCompareEq(TypeDef type)
        {
            foreach (MethodDef method in type.Methods)
            {
                if (!CheckCompareEqMethodSig(method))
                    continue;
                if (method.Body == null)
                    continue;

                IList<Instruction> instructions = method.Body.Instructions;

                for (int i = 0; i < instructions.Count; i++)
                {
                    Instruction ldarg0 = instructions[i];

                    if (!ldarg0.IsLdarg() || ldarg0.GetParameterIndex() != 0)
                        continue;
                    if (!CheckUnboxAny(instructions[i + 1], ElementType.I4))
                        continue;

                    Instruction ldarg1 = instructions[i + 2];

                    if (!ldarg1.IsLdarg() || ldarg1.GetParameterIndex() != 1)
                        continue;
                    if (!CheckUnboxAny(instructions[i + 3], ElementType.I4))
                        continue;
                    if (instructions[i + 4].OpCode.Code != Code.Ceq)
                        continue;
                    if (!instructions[i + 5].IsStloc())
                        continue;

                    return method;
                }
            }

            return null;
        }
        private MethodDef FindCompareEqz(TypeDef type)
        {
            foreach (MethodDef method in type.Methods)
            {
                if (!CheckCompareEqzMethodSig(method))
                    continue;
                if (method.Body == null)
                    continue;

                IList<Instruction> instructions = method.Body.Instructions;

                for (int i = 0; i < instructions.Count; i++)
                {
                    Instruction ldarg0 = instructions[i];

                    if (!ldarg0.IsLdarg() || ldarg0.GetParameterIndex() != 0)
                        continue;
                    if (!CheckUnboxAny(instructions[i + 1], ElementType.I4))
                        continue;

                    Instruction ldci4 = instructions[i + 2];

                    if (!ldci4.IsLdcI4() || ldci4.GetLdcI4Value() != 0)
                        continue;
                    if (instructions[i + 3].OpCode.Code != Code.Ceq)
                        continue;
                    if (!instructions[i + 4].IsStloc())
                        continue;

                    return method;
                }
            }

            return null;
        }
        private MethodDef FindArithmeticSubOvfUn(TypeDef type) => FindArithmeticOpUn(type, Code.Sub_Ovf_Un);
        private MethodDef FindArithmeticMulOvfUn(TypeDef type) => FindArithmeticOpUn(type, Code.Mul_Ovf_Un);
        private MethodDef FindArithmeticAddOvfUn(TypeDef type) => FindArithmeticOpUn(type, Code.Add_Ovf_Un);
        private MethodDef FindArithmeticRemUn(TypeDef type) => FindArithmeticDivOrRemUn(type, Code.Rem_Un);
        private MethodDef FindArithmeticDivUn(TypeDef type) => FindArithmeticDivOrRemUn(type, Code.Div_Un);
        private MethodDef FindArithmeticRem(TypeDef type) => FindArithmeticOther(type, Code.Rem);
        private MethodDef FindArithmeticDiv(TypeDef type) => FindArithmeticOther(type, Code.Div);
        private MethodDef FindArithmeticMul(TypeDef type) => FindArithmeticOther(type, Code.Mul);
        private MethodDef FindArithmeticMulOvf(TypeDef type) => FindArithmeticOther(type, Code.Mul_Ovf);
        private MethodDef FindArithmeticSub(TypeDef type) => FindArithmeticOther(type, Code.Sub);
        private MethodDef FindArithmeticSubOvf(TypeDef type) => FindArithmeticOther(type, Code.Sub_Ovf);
        private MethodDef FindArithmeticAdd(TypeDef type) => FindArithmeticOther(type, Code.Add);
        private MethodDef FindArithmeticAddOvf(TypeDef type) => FindArithmeticOther(type, Code.Add_Ovf);
        private MethodDef FindUnaryMethod(TypeDef type, Code code)
        {
            foreach(MethodDef method in type.Methods)
            {
                if (!IsUnsaryMethod(method, code))
                    continue;

                return method;
            }

            return null;
        }

        
        private MethodDef FindUnaryOpMethod1(Code code)
        {
            foreach(TypeDef type in module.Types)
            {
                if (type.BaseType != VmHandlerBaseType)
                    continue;
                if (type.Methods.Count != 4)
                    continue;

                MethodDef method = FindUnaryMethod(type, code);

                if (method != null)
                    return method;
            }

            return null;
        } 
        private bool FindUnaryOpMethod2()
        {
            foreach(TypeDef type in module.Types)
            {
                if (type.BaseType == null || type.BaseType.FullName != "System.Object")
                    continue;
                if (type.Methods.Count != 3)
                    continue;

                UnaryNot = FindUnaryMethod(type, Code.Not);
                UnaryNeg = FindUnaryMethod(type, Code.Neg);

                if (UnaryNot != null && UnaryNeg != null)
                    return true;
            }

            return false;
        }
        private TypeDef FindVmState()
        {
            if (VmHandlerBaseType == null)
                return null;

            foreach(MethodDef method in VmHandlerBaseType.Methods)
            {
                if (method.IsStatic || !method.IsAbstract)
                    continue;
                if (method.Parameters.Count != 2)
                    continue;

                TypeDef arg = method.Parameters[1].Type.TryGetTypeDef();

                if (arg == null)
                    continue;

                return arg;
            }

            return null;
        }
        private static bool FindArgsLocals(MethodDef ctor, int arg, out MethodDef getter, out MethodDef setter)
        {
            getter = setter = null;

            if (ctor == null || !ctor.HasBody)
                return false;

            setter = FindSetter(ctor, arg);

            if (setter == null)
                return false;

            FieldDef propField = GetPropField(setter);

            if (propField == null)
                return false;

            getter = FindGetter(ctor.DeclaringType, propField);

            return getter != null;
        }


        public CSVMInfo(ModuleDef module) => this.module = module;

        public bool FindVMHandlerBase()
        {
            foreach(TypeDef type in module.Types)
            {
                if (!type.IsPublic || !type.IsAbstract)
                    continue;
                if (type.HasProperties || type.HasEvents)
                    continue;
                if (type.BaseType == null || type.BaseType.FullName != "System.Object")
                    continue;
                if (CountVirtual(type) != 2)
                    continue;

                VmHandlerBaseType = type;

                return true;
            }

            return false;
        }
        public bool FindLocalOpsMethods()
        {
            foreach(TypeDef type in module.Types)
            {
                if (type.BaseType == null || type.BaseType.FullName != "System.Object")
                    continue;
                if (type.Methods.Count != 6 && type.Methods.Count != 7)
                    continue;

                LogicalOpShrUn = FindLogicalOpMethodShrUn(type);

                if (LogicalOpShrUn == null)
                    continue;

                LogicalOpShl = FindLogicalOpMethodShl(type);
                LogicalOpShr = FindLogicalOpMethodShr(type);
                LogicalOpAnd = FindLogicalOpMethodAnd(type);
                LogicalOpXor = FindLogicalOpMethodXor(type);
                LogicalOpOr = FindLogicalOpMethodOr(type);

                Utils.DebugN("LogicalShrUn", LogicalOpShrUn);
                Utils.DebugN("LogicalOpShl", LogicalOpShl);
                Utils.DebugN("LogicalOpShr", LogicalOpShr);
                Utils.DebugN("LogicalOpAnd", LogicalOpAnd);
                Utils.DebugN("LogicalOpXor", LogicalOpXor);
                Utils.DebugN("LogicalOpOr", LogicalOpOr);

                if (LogicalOpShrUn != null && LogicalOpShl != null && LogicalOpShr != null && LogicalOpAnd != null && LogicalOpXor != null && LogicalOpOr != null)
                    return true;
            }

            return false;
        }
        public bool FindComparerMethods()
        {
            foreach(TypeDef type in module.Types)
            {
                if (type.BaseType == null || type.BaseType.FullName != "System.Object")
                    continue;
                if (type.Methods.Count != 9)
                    continue;

                CompareLt = FindCompareLt(type);

                if (CompareLt == null)
                    continue;

                CompareLte = FindCompareLte(type);
                CompareGt = FindCompareGt(type);
                CompareGte = FindCompareGte(type);
                CompareEq = FindCompareEq(type);
                CompareEqz = FindCompareEqz(type);

                Utils.DebugN("Lt", CompareLt);
                Utils.DebugN("Lte", CompareLte);
                Utils.DebugN("Gt", CompareGt);
                Utils.DebugN("Gte", CompareGte);
                Utils.DebugN("Eq", CompareEq);
                Utils.DebugN("Eqz", CompareEqz);

                if (CompareLt != null && CompareLte != null && CompareGt != null && CompareGte != null && CompareEq != null && CompareEqz != null) 
                    return true;
            }

            return false;
        }
        public bool FindArithmeticMethods()
        {
            foreach(TypeDef type in module.Types)
            {
                if (type.BaseType == null || type.BaseType.FullName != "System.Object")
                    continue;
                if (type.Methods.Count != 15)
                    continue;

                ArithmeticSubOvfUn = FindArithmeticSubOvfUn(type);

                if (ArithmeticSubOvfUn == null)
                    continue;

                ArithmeticMulOvfUn = FindArithmeticMulOvfUn(type);
                ArithmeticRemUn = FindArithmeticRemUn(type);
                ArithmeticRem = FindArithmeticRem(type);
                ArithmeticDivUn = FindArithmeticDivUn(type);
                ArithmeticDiv = FindArithmeticDiv(type);
                ArithmeticMul = FindArithmeticMul(type);
                ArithmeticMulOvf = FindArithmeticMulOvf(type);
                ArithmeticSub = FindArithmeticSub(type);
                ArithmeticSubOvf = FindArithmeticSubOvf(type);
                ArithmeticAddOvfUn = FindArithmeticAddOvfUn(type);
                ArithmeticAddOvf = FindArithmeticAddOvf(type);
                ArithmeticAdd = FindArithmeticAdd(type);

                if (ArithmeticSubOvfUn != null && ArithmeticMulOvfUn != null && ArithmeticRemUn != null && ArithmeticRem != null && ArithmeticDivUn != null && ArithmeticDiv != null && ArithmeticMul != null && ArithmeticMulOvf != null && ArithmeticSub != null && ArithmeticSubOvf != null && ArithmeticAddOvfUn != null && ArithmeticAddOvf != null && ArithmeticAdd != null)
                    return true;
            }

            return false;
        }
        public bool FindUnaryOpsMethods()
        {
            UnaryNot = FindUnaryOpMethod1(Code.Not);
            UnaryNeg = FindUnaryOpMethod1(Code.Neg);

            if (UnaryNot != null && UnaryNeg != null)
                return true;

            return FindUnaryOpMethod2();
        }
        public bool FindArgsLocals()
        {
            TypeDef vmState = FindVmState();

            if (vmState == null)
                return false;

            MethodDef ctor = vmState.FindMethod(".ctor");

            return FindArgsLocals(ctor, 1, out ArgsGet, out ArgsSet) && FindArgsLocals(ctor, 2, out LocalsGet, out LocalsSet);
        }

        public bool Initialize()
        {
            // FindVMHandlerBase() && FindLocalOpsMethods() && FindComparerMethods() && FindArithmeticMethods() && FindUnaryOpsMethods() && FindArgsLocals()

            bool foundVmHandlerBase = FindVMHandlerBase();
            bool foundLocalOps = FindLocalOpsMethods();
            bool foundComparers = FindComparerMethods();
            bool foundArithmetics = FindArithmeticMethods();
            bool foundUnary = FindUnaryOpsMethods();
            bool foundArgs = FindArgsLocals();

            Utils.Warn(foundVmHandlerBase.ToString());
            Utils.Warn(foundLocalOps.ToString()); // False (And)
            Utils.Warn(foundComparers.ToString());
            Utils.Warn(foundArithmetics.ToString());
            Utils.Warn(foundUnary.ToString());
            Utils.Warn(foundArgs.ToString());

            return foundVmHandlerBase && foundLocalOps && foundComparers && foundArithmetics && foundUnary && foundArgs;
        }
    }
}
