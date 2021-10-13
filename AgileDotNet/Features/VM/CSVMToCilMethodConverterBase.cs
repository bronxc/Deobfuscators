using System;
using System.Collections.Generic;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using de4dot.blocks;

namespace AgileDotNet.Features.VM
{
    public abstract class CSVMToCilMethodConverterBase
    {
        readonly IContext context;
        readonly protected ModuleDefMD module;
        readonly CilOperandInstructionRestorer operandRestorer = new CilOperandInstructionRestorer();
        readonly Dictionary<Instruction, int> cilToVmIndex = new Dictionary<Instruction, int>();
        readonly Dictionary<int, Instruction> vmIndexToCil = new Dictionary<int, Instruction>();

        private Instruction GetInstruction(int vmIndex) => vmIndexToCil[vmIndex];
        private Instruction GetInstruction(Instruction src, int displ)
        {
            int index = cilToVmIndex[src];
            return vmIndexToCil[index + displ];
        }
        private Instruction GetInstructionEnd(int vmIndex)
        {
            vmIndex++;
            vmIndexToCil.TryGetValue(vmIndex, out Instruction ins);
            return ins;
        }

        private static void UpdateLocalInstruction(Instruction instr, Local local, int index)
        {
            object operand = null;
            OpCode opcode;

            switch (instr.OpCode.Code)
            {
                case Code.Ldloc_S:
                case Code.Ldloc:
                    {
                        if (index == 0)
                            opcode = OpCodes.Ldloc_0;
                        else if (index == 1)
                            opcode = OpCodes.Ldloc_1;
                        else if (index == 2)
                            opcode = OpCodes.Ldloc_2;
                        else if (index == 3)
                            opcode = OpCodes.Ldloc_3;
                        else if(byte.MinValue <= index && index <= byte.MaxValue)
                        {
                            opcode = OpCodes.Ldloc_S;
                            operand = local;
                        }
                        else
                        {
                            opcode = OpCodes.Ldloc;
                            operand = local;
                        }
                    }
                    break;
                case Code.Stloc:
                case Code.Stloc_S:
                    {
                        if (index == 0)
                            opcode = OpCodes.Stloc_0;
                        else if (index == 1)
                            opcode = OpCodes.Stloc_1;
                        else if (index == 2)
                            opcode = OpCodes.Stloc_2;
                        else if (index == 3)
                            opcode = OpCodes.Stloc_3;
                        else if(byte.MinValue <= index && index <= byte.MaxValue)
                        {
                            opcode = OpCodes.Stloc_S;
                            operand = local;
                        }
                        else
                        {
                            opcode = OpCodes.Stloc;
                            operand = local;
                        }
                    }
                    break;
                case Code.Ldloca:
                case Code.Ldloca_S:
                    {
                        if(byte.MinValue <= index && index <= byte.MaxValue)
                        {
                            opcode = OpCodes.Ldloca_S;
                            operand = local;
                        }
                        else
                        {
                            opcode = OpCodes.Ldloca;
                            operand = local;
                        }
                    }
                    break;
                default:
                    throw new ApplicationException("Invalid opcode");
            }

            instr.OpCode = opcode;
            instr.Operand = operand;
        }
        private static void UpdateArgInstruction(Instruction instr, Parameter arg, int index)
        {
            switch (instr.OpCode.Code)
            {
                case Code.Ldarg:
                case Code.Ldarg_S:
                    {
                        if(index == 0)
                        {
                            instr.OpCode = OpCodes.Ldarg_0;
                            instr.Operand = null;
                        }else if(index == 1)
                        {
                            instr.OpCode = OpCodes.Ldarg_1;
                            instr.Operand = null;
                        }else if(index == 2)
                        {
                            instr.OpCode = OpCodes.Ldarg_2;
                            instr.Operand = null;
                        }else if(index == 3)
                        {
                            instr.OpCode = OpCodes.Ldarg_3;
                            instr.Operand = null;
                        }else if(byte.MinValue <= index && index <= byte.MaxValue)
                        {
                            instr.OpCode = OpCodes.Ldarg_S;
                            instr.Operand = arg;
                        }
                        else
                        {
                            instr.OpCode = OpCodes.Ldarg;
                            instr.Operand = arg;
                        }
                    }
                    break;
                case Code.Starg:
                case Code.Starg_S:
                    if(byte.MinValue <= index && index <= byte.MaxValue)
                    {
                        instr.OpCode = OpCodes.Starg_S;
                        instr.Operand = arg;
                    }
                    else
                    {
                        instr.OpCode = OpCodes.Starg;
                        instr.Operand = arg;
                    }
                    break;
                case Code.Ldarga:
                case Code.Ldarga_S:
                    if (byte.MinValue <= index && index <= byte.MaxValue)
                        instr.OpCode = OpCodes.Ldarga_S;
                    else instr.OpCode = OpCodes.Ldarga;

                    instr.Operand = arg;
                    break;
                default: throw new ApplicationException("Invalid opcode");
            }
        }

        private IField FixLoadStoreFieldInstruction(Instruction instruction, IField fieldRef, OpCode sOpCode, OpCode iOpCode)
        {
            FieldDef field = context.ResolveField(fieldRef);
            bool isStatic;
            if (field == null)
                isStatic = false;
            else isStatic = field.IsStatic;

            instruction.OpCode = isStatic ? sOpCode : iOpCode;
            return fieldRef;
        }
        private object FixOperand(IList<Instruction> instructions, Instruction instruction, IVmOperand op)
        {
            if (op is TargetDisplOperand)
                return GetInstruction(instruction, ((TargetDisplOperand)op).Displacement);

            if(op is SwitchTargetDisplOperand)
            {
                int[] disps = ((SwitchTargetDisplOperand)op).TargetDisplacements;
                Instruction[] targets = new Instruction[disps.Length];

                for (int i = 0; i < targets.Length; i++)
                    targets[i] = GetInstruction(instruction, disps[i]);

                return targets;
            }

            if (op is ArgOperand || op is LocalOperand)
                return op;

            if (op is FieldInstructionOperand field)
                return FixLoadStoreFieldInstruction(instruction, field.Field, field.StaticOpCode, field.InstanceOpCode);

            throw new ApplicationException($"Unknown operand type: {op.GetType()}");
        }
        private void FixInstructionOperands(IList<Instruction> instructions)
        {
            foreach (Instruction ins in instructions)
                if (ins.Operand is IVmOperand op)
                    ins.Operand = FixOperand(instructions, ins, op);
        }
        private void FixLocals(IList<Instruction> instructions, IList<Local> locals)
        {
            foreach(Instruction ins in instructions)
            {
                LocalOperand op = ins.Operand as LocalOperand;
                if (op == null)
                    continue;

                UpdateLocalInstruction(ins, locals[op.Local], op.Local);
            }
        }
        private void FixArgs(IList<Instruction> instructions, MethodDef method)
        {
            foreach(Instruction ins in instructions)
            {
                ArgOperand ao = ins.Operand as ArgOperand;
                if (ao == null)
                    continue;

                UpdateArgInstruction(ins, method.Parameters[ao.Arg], ao.Arg);
            }
        }

        private TypeSig ReadTypeRef(BinaryReader reader, GenericParamContext gpc)
        {
            ElementType et = (ElementType)reader.ReadInt32();
            ICorLibTypes ic = module.CorLibTypes;
            switch (et)
            {
                case ElementType.Void: return ic.Void;
                case ElementType.Boolean: return ic.Boolean;
                case ElementType.Char: return ic.Char;
                case ElementType.I1: return ic.SByte;
                case ElementType.U1: return ic.Byte;
                case ElementType.I2: return ic.Int16;
                case ElementType.U2: return ic.UInt16;
                case ElementType.I4: return ic.Int32;
                case ElementType.U4: return ic.UInt32;
                case ElementType.I8: return ic.Int64;
                case ElementType.U8: return ic.UInt64;
                case ElementType.R4: return ic.Single;
                case ElementType.R8: return ic.Double;
                case ElementType.String: return ic.String;
                case ElementType.TypedByRef: return ic.TypedReference;
                case ElementType.I: return ic.IntPtr;
                case ElementType.U: return ic.UIntPtr;
                case ElementType.Object: return ic.Object;
                case ElementType.ValueType:
                case ElementType.Var:
                case ElementType.MVar:
                    return (module.ResolveToken(reader.ReadInt32(), gpc) as ITypeDefOrRef).ToTypeSig();
                case ElementType.GenericInst:
                    ElementType et2 = (ElementType)reader.ReadInt32();
                    if (et2 == ElementType.ValueType)
                        return (module.ResolveToken(reader.ReadInt32(), gpc) as ITypeDefOrRef).ToTypeSig();

                    return ic.Object;
                case ElementType.Ptr:
                case ElementType.Class:
                case ElementType.Array:
                case ElementType.FnPtr:
                case ElementType.SZArray:
                case ElementType.ByRef:
                case ElementType.CModReqd:
                case ElementType.CModOpt:
                case ElementType.Internal:
                case ElementType.Sentinel:
                case ElementType.Pinned:
                default: return ic.Object;
            }
        }

        protected abstract List<Instruction> ReadInstructions(MethodDef cilMethod, VirtualMethodDef csvmMethod);
        private List<Local> ReadLocals(MethodDef cilMethod, VirtualMethodDef csvmMethod)
        {
            List<Local> locals = new List<Local>();
            BinaryReader reader = new BinaryReader(new MemoryStream(csvmMethod.LocalVarStream));

            if (csvmMethod.LocalVarStream.Length == 0)
                return locals;

            int LocalsCount = reader.ReadInt32();
            if (LocalsCount < 0)
                throw new ApplicationException("Invalid number of locals");

            GenericParamContext gpc = GenericParamContext.Create(cilMethod);
            for(int i = 0; i < LocalsCount; i++)
            {
                locals.Add(new Local(ReadTypeRef(reader, gpc)));
            }

            return locals;
        }
        private List<ExceptionHandler> ReadExceptions(MethodDef cilMethod, VirtualMethodDef csvmMethod)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(csvmMethod.ExceptionHandlerStream));
            List<ExceptionHandler> ehs = new List<ExceptionHandler>();

            if (reader.BaseStream.Length == 0)
                return ehs;

            int ehsCount = reader.ReadInt32();
            if (ehsCount < 0)
                throw new ApplicationException("Invalid number of exception handlers");

            GenericParamContext gpc = GenericParamContext.Create(cilMethod);
            for(int i = 0; i < ehsCount; i++)
            {
                ExceptionHandler eh = new ExceptionHandler((ExceptionHandlerType)reader.ReadInt32());
                eh.TryStart = GetInstruction(reader.ReadInt32());
                eh.TryEnd = GetInstructionEnd(reader.ReadInt32());
                eh.HandlerStart = GetInstruction(reader.ReadInt32());
                eh.HandlerEnd = GetInstructionEnd(reader.ReadInt32());

                if (eh.HandlerType == ExceptionHandlerType.Catch)
                    eh.CatchType = module.ResolveToken(reader.ReadUInt32(), gpc) as ITypeDefOrRef;
                else if (eh.HandlerType == ExceptionHandlerType.Filter)
                    eh.FilterStart = GetInstruction(reader.ReadInt32());

                ehs.Add(eh);
            }

            return ehs;
        }

        public CSVMToCilMethodConverterBase(IContext context, ModuleDefMD module)
        {
            this.context = context;
            this.module = module;
        }

        protected void SetCilToVmIndex(Instruction instr, int index) => cilToVmIndex[instr] = index;
        protected void SetVmIndexToCil(Instruction instr, int index) => vmIndexToCil[index] = instr;
        protected static int GetInstructionSize(Instruction instr)
        {
            OpCode opcode = instr.OpCode;
            if (opcode == null)
                return 5;

            SwitchTargetDisplOperand op = instr.Operand as SwitchTargetDisplOperand;
            if (op == null)
                return instr.GetSize();

            return instr.OpCode.Size + (op.TargetDisplacements.Length + 1) * 4;
        }

        private static bool HasPrefix(IList<Instruction> instructions, int index, Code prefix)
        {
            index--;
            for (; index >= 0; index--)
            {
                Instruction instruction = instructions[index];
                if (instruction.OpCode.OpCodeType != OpCodeType.Prefix)
                    break;
                if (instruction.OpCode.Code == prefix)
                    return true;
            }

            return false;
        }
        private static void RestoreConstrainedPrefix(MethodDef method)
        {
            if (method == null || method.Body == null)
                return;

            IList<Instruction> instructions = method.Body.Instructions;

            for(int i = 0; i < instructions.Count; i++)
            {
                Instruction instruction = instructions[i];
                if (instruction.OpCode != OpCodes.Callvirt)
                    continue;

                IMethod called = instruction.Operand as IMethod;
                if (called == null)
                    continue;

                MethodSig sig = called.MethodSig;
                if (sig == null || !sig.HasThis)
                    continue;

                ByRefSig thisType = MethodStack.GetLoadedType(method, instructions, i, sig.Params.Count) as ByRefSig;
                if (thisType == null)
                    continue;

                if (HasPrefix(instructions, i, Code.Constrained))
                    continue;

                instructions.Insert(i, OpCodes.Constrained.ToInstruction(thisType.Next.ToTypeDefOrRef()));
                i++;
            }
        }

        public void Convert(MethodDef cilMethod, VirtualMethodDef csvmMethod)
        {
            cilToVmIndex.Clear();
            vmIndexToCil.Clear();

            List<Instruction> newInstructions = ReadInstructions(cilMethod, csvmMethod);
            ReadLocals(cilMethod, csvmMethod);
            List<ExceptionHandler> newExceptions = ReadExceptions(cilMethod, csvmMethod);

            FixInstructionOperands(newInstructions);
            FixLocals(newInstructions, cilMethod.Body.Variables);
            FixArgs(newInstructions, cilMethod);

            DotNetUtils.RestoreBody(cilMethod, newInstructions, newExceptions);

            if (!operandRestorer.Restore(cilMethod))
                Utils.Error($"Failed to restore instruction operands in CSVM method {cilMethod.MDToken.ToInt32():X8}");

            RestoreConstrainedPrefix(cilMethod);
        }
    }
}
