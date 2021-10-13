using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace AgileDotNet.Features.VM
{
    class CilOperandInstructionRestorer
    {
        private MethodDef method;

        private static TypeSig GetPtrElementType(TypeSig type)
        {
            if (type == null)
                return null;
            if (type.IsPointer || type.IsByRef)
                return type.Next;

            return null;
        }
        private bool IsValidType(TypeSig type)
        {
            type = type.RemovePinnedAndModifiers();
            if (type == null)
                return false;
            if (type.ElementType == ElementType.Void)
                return false;

            while(type != null)
            {
                switch (type.ElementType)
                {
                    case ElementType.MVar:
                        GenericMVar gm = (GenericMVar)type;
                        if (gm.Number >= method.MethodSig.GetGenParamCount())
                            return false;
                        break;
                    case ElementType.Var:
                        GenericVar g = (GenericVar)type;
                        TypeDef dt = method.DeclaringType;
                        if (dt == null || g.Number >= dt.GenericParameters.Count)
                            return false;
                        break;
                    default:
                        return false;
                }
                if (type.Next == null)
                    break;

                type = type.Next;
            }

            return type != null;
        }

        public bool Restore(MethodDef method)
        {
            this.method = method;
            bool Failed = false;

            if (method == null || method.Body == null)
                return !Failed;

            var instrs = method.Body.Instructions;
            for(int i = 0; i < instrs.Count; i++)
            {
                var instr = instrs[i];

                if (instr.Operand != null)
                    continue;

                TypeSig operandType = null, operandTypeTmp;
                OpCode newOpCode = null;
                SZArraySig arrayType;

                switch (instr.OpCode.Code)
                {
                    case Code.Ldelem:
                        {
                            arrayType = MethodStack.GetLoadedType(method, instrs, i, 1) as SZArraySig;
                            if (arrayType == null)
                                break;
                            operandTypeTmp = arrayType.Next;
                            if (operandTypeTmp == null)
                                newOpCode = OpCodes.Ldelem_Ref;
                            else
                            {
                                switch (operandTypeTmp.ElementType)
                                {
                                    case ElementType.Boolean: newOpCode = OpCodes.Ldelem_I1;break;
                                    case ElementType.Char: newOpCode = OpCodes.Ldelem_U2;break;
                                    case ElementType.I: newOpCode = OpCodes.Ldelem_I;break;
                                    case ElementType.I1: newOpCode = OpCodes.Ldelem_I1;break;
                                    case ElementType.I2: newOpCode = OpCodes.Ldelem_I2;break;
                                    case ElementType.I4: newOpCode = OpCodes.Ldelem_I4;break;
                                    case ElementType.I8: newOpCode = OpCodes.Ldelem_I8;break;
                                    case ElementType.U: newOpCode = OpCodes.Ldelem_I;break;
                                    case ElementType.U1: newOpCode = OpCodes.Ldelem_U1;break;
                                    case ElementType.U2: newOpCode = OpCodes.Ldelem_U2;break;
                                    case ElementType.U4: newOpCode = OpCodes.Ldelem_U4;break;
                                    case ElementType.U8: newOpCode = OpCodes.Ldelem_I8;break;
                                    case ElementType.R4: newOpCode = OpCodes.Ldelem_R4;break;
                                    case ElementType.R8: newOpCode = OpCodes.Ldelem_R8;break;
                                    default: newOpCode = OpCodes.Ldelem_Ref;break;
                                }
                            }
                        }
                        break;
                    case Code.Stelem:
                        {
                            arrayType = MethodStack.GetLoadedType(method, instrs, i, 2) as SZArraySig;
                            if (arrayType == null)
                                break;
                            operandTypeTmp = arrayType.Next;
                            if (operandTypeTmp == null)
                                newOpCode = OpCodes.Stelem_Ref;
                            else
                            {
                                switch (operandTypeTmp.ElementType)
                                {
                                    case ElementType.I: newOpCode = OpCodes.Stelem_I;break;
                                    case ElementType.I1: newOpCode = OpCodes.Stelem_I1;break;
                                    case ElementType.I2: newOpCode = OpCodes.Stelem_I2;break;
                                    case ElementType.I4: newOpCode = OpCodes.Stelem_I4;break;
                                    case ElementType.I8: newOpCode = OpCodes.Stelem_I8;break;
                                    case ElementType.R4: newOpCode = OpCodes.Stelem_R4;break;
                                    case ElementType.R8: newOpCode = OpCodes.Stelem_R8;break;
                                    default: newOpCode = OpCodes.Stelem_Ref;break;
                                }
                            }
                        }
                        break;
                    case Code.Ldelema:
                        arrayType = MethodStack.GetLoadedType(method, instrs, i, 1) as SZArraySig;
                        if (arrayType == null)
                            break;
                        operandType = arrayType.Next;
                        if (!operandType.IsValueType)
                        {
                            newOpCode = OpCodes.Ldelem_Ref;
                            operandType = null;
                        }
                        break;
                    case Code.Ldobj:
                        operandType = GetPtrElementType(MethodStack.GetLoadedType(method, instrs, i, 0));
                        break;
                    case Code.Stobj:
                        operandType = MethodStack.GetLoadedType(method, instrs, i, 0);
                        if (!IsValidType(operandType))
                            operandType = GetPtrElementType(MethodStack.GetLoadedType(method, instrs, i, 1));
                        break;
                    default:
                        continue;
                }
                if(newOpCode == null && !IsValidType(operandType))
                {
                    Failed = true;
                    continue;
                }

                instr.Operand = operandType.ToTypeDefOrRef();
                if (newOpCode != null)
                    instr.OpCode = newOpCode;
            }

            return !Failed;
        }
    }
}
