using System;
using System.Collections.Generic;
using System.IO;

using dnlib.DotNet;
using dnlib.DotNet.Emit;

using de4dot.blocks;

namespace AgileDotNet.Features.VM.Old
{
    public partial class OpCodeHandler
    {
        public string Name { get; set; }
        public OpCodeHandlerSigInfo OpCodeHandlerSigInfo { get; set; }
        public Predicate<UnknownHandlerInfo> Check { get; set; }
        public Func<BinaryReader, IInstructionOperandResolver, GenericParamContext, Instruction> Read { get; set; }

        private static bool Compare(int? val, int val2)
        {
            if (!val.HasValue)
                return true;

            return val.Value == val2;
        }
        
        public bool Detect(UnknownHandlerInfo info)
        {
            var sigInfo = OpCodeHandlerSigInfo;

            if (!Compare(sigInfo.StaticMethods, info.StaticMethods))
                return false;
            if (!Compare(sigInfo.InstanceMethods, info.InstanceMethods))
                return false;
            if (!Compare(sigInfo.VirtualMethods, info.VirtualMethods))
                return false;
            if (!Compare(sigInfo.Ctors, info.Ctors))
                return false;
            if (!Compare(sigInfo.ExecuteMethodThrows, info.ExecMethodThrows))
                return false;
            if (!Compare(sigInfo.ExecuteMethodPops, info.ExecMethodPops))
                return false;
            if (!info.HasSameFieldTypes(sigInfo.RequiredFieldTypes))
                return false;
            if (sigInfo.ExecuteMethodLocals != null && !new LocalTypes(info.ExecMethod).All(sigInfo.ExecuteMethodLocals))
                return false;

            if (Check != null)
                return Check(info);

            return true;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public static partial class OpCodeHandlers
    {
        private class InstructionInfo
        {
            public byte Type { get; set; }
            public bool Second { get; set; }
            public bool Third { get; set; }
            public OpCode OpCode { get; set; }
        }
        private class InstructionInfo2 { 
            public bool First { get; set; }
            public bool Second { get; set; }
            public int Value { get; set; }
            public OpCode OpCode { get; set; }
        }

        private static List<InstructionInfo> instructionInfos = new List<InstructionInfo>
        {
            new InstructionInfo {Type = 0, Second = false, Third = false,OpCode = OpCodes.Conv_I1},
            new InstructionInfo {Type = 1, Second = false, Third = false,OpCode = OpCodes.Conv_I2},
            new InstructionInfo {Type = 2, Second = false, Third = false,OpCode = OpCodes.Conv_I4},
            new InstructionInfo {Type = 3, Second = false, Third = false,OpCode = OpCodes.Conv_I8},
            new InstructionInfo {Type = 4, Second = false, Third = false,OpCode = OpCodes.Conv_R4},
            new InstructionInfo {Type = 5, Second = false, Third = false,OpCode = OpCodes.Conv_R8},
            new InstructionInfo {Type = 6, Second = false, Third = false,OpCode = OpCodes.Conv_U1},
            new InstructionInfo {Type = 7, Second = false, Third = false,OpCode = OpCodes.Conv_U2},
            new InstructionInfo {Type = 8, Second = false, Third = false,OpCode = OpCodes.Conv_U4},
            new InstructionInfo {Type = 9, Second = false, Third = false,OpCode = OpCodes.Conv_U8},
            new InstructionInfo {Type = 10, Second = false, Third = false,OpCode = OpCodes.Conv_I},
            new InstructionInfo {Type = 11, Second = false, Third = false,OpCode = OpCodes.Conv_U},

            new InstructionInfo {Type = 0, Second = true, Third = false,OpCode = OpCodes.Conv_Ovf_I1},
            new InstructionInfo {Type = 1, Second = true, Third = false,OpCode = OpCodes.Conv_Ovf_I2},
            new InstructionInfo {Type = 2, Second = true, Third = false,OpCode = OpCodes.Conv_Ovf_I4},
            new InstructionInfo {Type = 3, Second = true, Third = false,OpCode = OpCodes.Conv_Ovf_I8},
            new InstructionInfo {Type = 6, Second = true, Third = false,OpCode = OpCodes.Conv_Ovf_U1},
            new InstructionInfo {Type = 7, Second = true, Third = false,OpCode = OpCodes.Conv_Ovf_U2},
            new InstructionInfo {Type = 8, Second = true, Third = false,OpCode = OpCodes.Conv_Ovf_U4},
            new InstructionInfo {Type = 9, Second = true, Third = false,OpCode = OpCodes.Conv_Ovf_U8},
            new InstructionInfo {Type = 10, Second = true, Third = false,OpCode = OpCodes.Conv_Ovf_I},
            new InstructionInfo {Type = 11, Second = true, Third = false,OpCode = OpCodes.Conv_Ovf_U},

            new InstructionInfo {Type = 0, Second = true, Third = true,OpCode = OpCodes.Conv_Ovf_I1_Un},
            new InstructionInfo {Type = 1, Second = true, Third = true,OpCode = OpCodes.Conv_Ovf_I2_Un},
            new InstructionInfo {Type = 2, Second = true, Third = true,OpCode = OpCodes.Conv_Ovf_I4_Un},
            new InstructionInfo {Type = 3, Second = true, Third = true,OpCode = OpCodes.Conv_Ovf_I8_Un},
            new InstructionInfo {Type = 6, Second = true, Third = true,OpCode = OpCodes.Conv_Ovf_U1_Un},
            new InstructionInfo {Type = 7, Second = true, Third = true,OpCode = OpCodes.Conv_Ovf_U2_Un},
            new InstructionInfo {Type = 8, Second = true, Third = true,OpCode = OpCodes.Conv_Ovf_U4_Un},
            new InstructionInfo {Type = 9, Second = true, Third = true,OpCode = OpCodes.Conv_Ovf_U8_Un},
            new InstructionInfo {Type = 10, Second = true, Third = true,OpCode = OpCodes.Conv_Ovf_I_Un},
            new InstructionInfo {Type = 11, Second = true, Third = true,OpCode = OpCodes.Conv_Ovf_U_Un},
            new InstructionInfo {Type = 12, Second = true, Third = true,OpCode = OpCodes.Conv_R_Un},
        };
        private static List<InstructionInfo2> instructionInfos2 = new List<InstructionInfo2>
        {
            new InstructionInfo2 {First = false, Second = true, Value = 24, OpCode = OpCodes.Stelem_I},
            new InstructionInfo2 {First = false, Second = true, Value = 4, OpCode = OpCodes.Stelem_I1},
            new InstructionInfo2 {First = false, Second = true, Value = 6, OpCode = OpCodes.Stelem_I2},
            new InstructionInfo2 {First = false, Second = true, Value = 8, OpCode = OpCodes.Stelem_I4},
            new InstructionInfo2 {First = false, Second = true, Value = 10, OpCode = OpCodes.Stelem_I8},
            new InstructionInfo2 {First = false, Second = true, Value = 12, OpCode = OpCodes.Stelem_R4},
            new InstructionInfo2 {First = false, Second = true, Value = 13, OpCode = OpCodes.Stelem_R8},
            new InstructionInfo2 {First = false, Second = true, Value = 28, OpCode = OpCodes.Stelem_Ref},
            new InstructionInfo2 {First = false, Second = false, Value = 0, OpCode = OpCodes.Stelem},

            new InstructionInfo2 {First = true, Second = true, Value = 24, OpCode = OpCodes.Ldelem_I},
            new InstructionInfo2 {First = true, Second = true, Value = 4, OpCode = OpCodes.Ldelem_I1},
            new InstructionInfo2 {First = true, Second = true, Value = 6, OpCode = OpCodes.Ldelem_I2},
            new InstructionInfo2 {First = true, Second = true, Value = 8, OpCode = OpCodes.Ldelem_I4},
            new InstructionInfo2 {First = true, Second = true, Value = 10, OpCode = OpCodes.Ldelem_I8},
            new InstructionInfo2 {First = true, Second = true, Value = 5, OpCode = OpCodes.Ldelem_U1},
            new InstructionInfo2 {First = true, Second = true, Value = 7, OpCode = OpCodes.Ldelem_U2},
            new InstructionInfo2 {First = true, Second = true, Value = 9, OpCode = OpCodes.Ldelem_U4},
            new InstructionInfo2 {First = true, Second = true, Value = 12, OpCode = OpCodes.Ldelem_R4},
            new InstructionInfo2 {First = true, Second = true, Value = 13, OpCode = OpCodes.Ldelem_R8},
            new InstructionInfo2 {First = true, Second = true, Value = 28, OpCode = OpCodes.Ldelem_Ref},
            new InstructionInfo2 {First = true, Second = false, Value = 0, OpCode = OpCodes.Ldelem},

        };

        private static Instruction arithmetic_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc)
        {
            switch (reader.ReadByte())
            {
                case 0: return OpCodes.Add.ToInstruction();
                case 1: return OpCodes.Add_Ovf.ToInstruction();
                case 2: return OpCodes.Add_Ovf_Un.ToInstruction();
                case 3: return OpCodes.Sub.ToInstruction();
                case 4: return OpCodes.Sub_Ovf.ToInstruction();
                case 5: return OpCodes.Sub_Ovf_Un.ToInstruction();
                case 6: return OpCodes.Mul.ToInstruction();
                case 7: return OpCodes.Mul_Ovf.ToInstruction();
                case 8: return OpCodes.Mul_Ovf_Un.ToInstruction();
                case 9: return OpCodes.Div.ToInstruction();
                case 10: return OpCodes.Div_Un.ToInstruction();
                case 11: return OpCodes.Rem.ToInstruction();
                case 12: return OpCodes.Rem_Un.ToInstruction();
                default: throw new Exception("Invalid opcode");
            }
        }
        private static bool newarr_check(UnknownHandlerInfo info) => DotNetUtils.CallsMethod(info.ExecMethod, "System.Type System.Reflection.Module::ResolveType(System.Int32)");
        private static Instruction newarr_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc) => new Instruction(OpCodes.Newarr, resolver.ResolveToken(reader.ReadUInt32(), gpc));
        private static Instruction box_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc)
        {
            Instruction inst = new Instruction();

            switch (reader.ReadByte())
            {
                case 0: inst.OpCode = OpCodes.Box;break;
                case 1: inst.OpCode = OpCodes.Unbox_Any;break;
                default: throw new Exception("Invalid opcode");
            }

            inst.Operand = resolver.ResolveToken(reader.ReadUInt32(), gpc);

            return inst;
        }
        private static Instruction call_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc)
        {
            Instruction inst = new Instruction();

            switch (reader.ReadByte())
            {
                case 0: inst.OpCode = OpCodes.Newobj;break;
                case 1: inst.OpCode = OpCodes.Call;break;
                case 2: inst.OpCode = OpCodes.Callvirt;break;
                default: throw new Exception("Invalid opcodes");
            }

            inst.Operand = resolver.ResolveToken(reader.ReadUInt32(), gpc);

            return inst;
        }
        private static Instruction cast_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc)
        {
            Instruction inst = new Instruction();

            switch (reader.ReadByte())
            {
                case 0: inst.OpCode = OpCodes.Castclass;break;
                case 1: inst.OpCode = OpCodes.Isinst;break;
                default: throw new Exception("Invalid opcodes");
            }

            inst.Operand = resolver.ResolveToken(reader.ReadUInt32(), gpc);

            return inst;
        }
        private static Instruction compare_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc)
        {
            int type = reader.ReadInt32();
            Instruction inst = new Instruction();

            switch (type)
            {
                case 0: inst.OpCode = OpCodes.Br;break;
                case 1: inst.OpCode = OpCodes.Brtrue;break;
                case 2: inst.OpCode = OpCodes.Brfalse;break;
                case 3: inst.OpCode = OpCodes.Beq;break;
                case 4: inst.OpCode = OpCodes.Bge;break;
                case 5: inst.OpCode = OpCodes.Bgt;break;
                case 6: inst.OpCode = OpCodes.Ble;break;
                case 7: inst.OpCode = OpCodes.Blt;break;
                case 8: inst.OpCode = OpCodes.Bne_Un;break;
                case 9: inst.OpCode = OpCodes.Bge_Un;break;
                case 10: inst.OpCode = OpCodes.Bgt_Un;break;
                case 11: inst.OpCode = OpCodes.Ble_Un;break;
                case 12: inst.OpCode = OpCodes.Blt_Un;break;
                case 13: inst.OpCode = OpCodes.Ceq;break;
                case 14: inst.OpCode = OpCodes.Cgt;break;
                case 15: inst.OpCode = OpCodes.Clt;break;
                case 16: inst.OpCode = OpCodes.Cgt_Un;break;
                case 17: inst.OpCode = OpCodes.Clt_Un;break;
                default: throw new Exception("Invalid opcode");
            }

            if (type < 13)
                inst.Operand = new TargetDisplOperand(reader.ReadInt32());

            return inst;
        }
        private static Instruction convert_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc)
        {
            byte type = reader.ReadByte();
            bool second = reader.ReadBoolean();
            bool third = reader.ReadBoolean();

            Instruction inst = null;

            foreach(InstructionInfo info in instructionInfos)
            {
                if (type != info.Type || info.Second != second || info.Third != third)
                    continue;

                inst = new Instruction(info.OpCode);
                break;
            }

            if (inst == null)
                throw new Exception("Invalid opcode");

            return inst;
        }
        private static Instruction dup_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc)
        {
            switch (reader.ReadByte())
            {
                case 0: return OpCodes.Dup.ToInstruction();
                case 1: return OpCodes.Pop.ToInstruction();
                default: throw new Exception("Invalid opcode");
            }
        }
        private static Instruction ldelem_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc)
        {
            Instruction inst = null;

            bool first = reader.ReadBoolean();
            bool second = reader.ReadBoolean();

            int value = reader.ReadInt32();

            foreach(InstructionInfo2 info in instructionInfos2)
            {
                if (info.First != first || info.Second != second)
                    continue;
                if (second && value != info.Value)
                    continue;

                if (second)
                    inst = new Instruction(info.OpCode);
                else inst = new Instruction(info.OpCode, resolver.ResolveToken((uint)value, gpc));

                break;
            }

            if (inst == null)
                throw new Exception("Invalid opcode");

            return inst;
        }
        private static bool endfinally_check(UnknownHandlerInfo info) => DotNetUtils.CallsMethod(info.ExecMethod, "System.Reflection.MethodInfo System.Type::GetMethod(System.String,System.Reflection.BindingFlags)");
        private static Instruction endfinally_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc) => OpCodes.Endfinally.ToInstruction();
        private static Instruction ldfld_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc)
        {
            byte b = reader.ReadByte();
            IField field = resolver.ResolveToken(reader.ReadUInt32(), gpc) as IField;

            switch (b)
            {
                case 0: return new Instruction(null, new FieldInstructionOperand(OpCodes.Ldsfld, OpCodes.Ldfld, field));
                case 1: return new Instruction(null, new FieldInstructionOperand(OpCodes.Ldsflda, OpCodes.Ldflda, field));
                case 2: return new Instruction(null, new FieldInstructionOperand(OpCodes.Stsfld, OpCodes.Stfld, field));
                default: throw new Exception("Invalid opcode");
            }
        }
        private static Instruction initobj_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc) => new Instruction(OpCodes.Initobj, resolver.ResolveToken(reader.ReadUInt32(), gpc));
        private static Instruction ldloc_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc)
        {
            bool isLdarg = reader.ReadBoolean();
            ushort index = reader.ReadUInt16();

            Instruction inst = new Instruction();

            if (isLdarg)
            {
                inst.OpCode = OpCodes.Ldarg;
                inst.Operand = new ArgOperand(index);
            }
            else
            {
                inst.OpCode = OpCodes.Ldloc;
                inst.Operand = new LocalOperand(index);
            }

            return inst;
        }
        private static Instruction ldloca_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc)
        {
            Instruction inst = new Instruction();

            if (reader.ReadBoolean())
            {
                inst.OpCode = OpCodes.Ldarga;
                inst.Operand = new ArgOperand(reader.ReadUInt16());
            }
            else
            {
                inst.OpCode = OpCodes.Ldloca;
                inst.Operand = new LocalOperand(reader.ReadUInt16());
            }

            return inst;
        }
        private static Instruction ldelema_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc) => new Instruction(OpCodes.Ldelema, null);
        private static Instruction ldlen_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc) => OpCodes.Ldlen.ToInstruction();
        private static Instruction ldobj_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc) => new Instruction(OpCodes.Ldobj, null);
        private static Instruction ldstr_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc) => OpCodes.Ldstr.ToInstruction(reader.ReadString());
        private static bool ldtoken_check(UnknownHandlerInfo info) => DotNetUtils.CallsMethod(info.ExecMethod, "System.Reflection.MemberInfo System.Reflection.Module::ResolveMember(System.Int32)");
        private static Instruction ldtoken_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc) => new Instruction(OpCodes.Ldtoken, resolver.ResolveToken(reader.ReadUInt32(), gpc));
        private static bool leave_check(UnknownHandlerInfo info) => !DotNetUtils.CallsMethod(info.ExecMethod, "System.Reflection.MethodBase System.Reflection.Module::ResolveMethod(System.Int32)") && !DotNetUtils.CallsMethod(info.ExecMethod, "System.Type System.Reflection.Module::ResolveType(System.Int32)") && !DotNetUtils.CallsMethod(info.ExecMethod, "System.Reflection.MemberInfo System.Reflection.Module::ResolveMember(System.Int32)");
        private static Instruction leave_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc) => new Instruction(OpCodes.Leave, new TargetDisplOperand(reader.ReadInt32()));
        private static Instruction ldc_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc)
        {
            switch ((ElementType)reader.ReadByte())
            {
                case ElementType.I4: return Instruction.CreateLdcI4(reader.ReadInt32());
                case ElementType.I8: return OpCodes.Ldc_I8.ToInstruction(reader.ReadInt64());
                case ElementType.R4: return OpCodes.Ldc_R4.ToInstruction(reader.ReadSingle());
                case ElementType.R8: return OpCodes.Ldc_R8.ToInstruction(reader.ReadDouble());
                case ElementType.Object: return OpCodes.Ldnull.ToInstruction();
                default: throw new Exception("Invalid opcode");
            }
        }
        private static Instruction ldftn_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc)
        {
            byte code = reader.ReadByte();
            uint token = reader.ReadUInt32();

            switch (code)
            {
                case 0: return new Instruction(OpCodes.Ldftn, resolver.ResolveToken(token, gpc));
                case 1: reader.ReadInt32(); return new Instruction(OpCodes.Ldvirtftn, resolver.ResolveToken(token, gpc));
                default: throw new Exception("Invalid opcode");
            }
        }
        private static Instruction logical_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc)
        {
            switch (reader.ReadByte())
            {
                case 0: return OpCodes.And.ToInstruction();
                case 1: return OpCodes.Or.ToInstruction();
                case 2: return OpCodes.Xor.ToInstruction();
                case 3: return OpCodes.Shl.ToInstruction();
                case 4: return OpCodes.Shr.ToInstruction();
                case 5: return OpCodes.Shr_Un.ToInstruction();
                default: throw new Exception("Invalid opcode");
            }
        }
        private static bool IsEmptyMethod(MethodDef method)
        {
            foreach(Instruction inst in method.Body.Instructions)
            {
                if (inst.OpCode.Code == Code.Ret)
                    return true;
                if (inst.OpCode.Code != Code.Nop)
                    break;
            }
            return false;
        }
        private static bool nop_check(UnknownHandlerInfo info) => IsEmptyMethod(info.ReadMethod) && IsEmptyMethod(info.ReadMethod);
        private static Instruction nop_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc) => OpCodes.Nop.ToInstruction();
        private static bool ret_check(UnknownHandlerInfo info) => DotNetUtils.CallsMethod(info.ExecMethod, "System.Reflection.MethodBase System.Reflection.Module::ResolveMethod(System.Int32)");
        private static Instruction ret_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc)
        {
            reader.ReadInt32();
            return OpCodes.Ret.ToInstruction();
        }
        private static bool rethrow_check(UnknownHandlerInfo info) => info.ExecMethod.Body.Variables.Count == 0;
        private static Instruction rethrow_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc) => OpCodes.Rethrow.ToInstruction();
        private static Instruction stloc_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc)
        {
            bool isStarg = reader.ReadBoolean();
            ushort index = reader.ReadUInt16();

            Instruction inst = new Instruction();

            if (isStarg)
            {
                inst.OpCode = OpCodes.Starg;
                inst.Operand = new ArgOperand(index);
            }
            else
            {
                inst.OpCode = OpCodes.Stloc;
                inst.Operand = new LocalOperand(index);
                reader.ReadInt32();
            }

            return inst;
        }
        private static Instruction stobj_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc) => new Instruction(OpCodes.Stobj, null);
        private static Instruction switch_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc)
        {
            int targets = reader.ReadInt32();

            int[] targetDisps = new int[targets];

            for (int i = 0; i < targetDisps.Length; i++)
                targetDisps[i] = reader.ReadInt32();

            return new Instruction(OpCodes.Switch, new SwitchTargetDisplOperand(targetDisps));
        }
        private static bool throw_check(UnknownHandlerInfo info) => !DotNetUtils.CallsMethod(info.ExecMethod, "System.Reflection.MethodInfo System.Type::GetMethod(System.String,System.Reflection.BindingFlags)");
        private static Instruction throw_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc) => OpCodes.Throw.ToInstruction();
        private static Instruction neg_read(BinaryReader reader, IInstructionOperandResolver resolver, GenericParamContext gpc)
        {
            switch (reader.ReadByte())
            {
                case 0: return OpCodes.Neg.ToInstruction();
                case 1: return OpCodes.Not.ToInstruction();
                default: throw new Exception("Invalid opcode");
            }
        }
    }
}
