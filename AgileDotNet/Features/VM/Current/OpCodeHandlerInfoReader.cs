using System;
using System.Collections.Generic;
using System.IO;

using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace AgileDotNet.Features.VM.Current
{
    public class OpCodeHandlerInfoReader
    {
        private class ConvInfo
        {
            public byte Type { get; private set; }
            public bool Second { get; private set; }
            public bool Third { get; private set; }

            public OpCode opCode { get; private set; }

            public ConvInfo(byte type, bool second, bool third, OpCode code)
            {
                Type = type;
                Second = second;
                Third = third;
                opCode = code;
            }
        }

        private IInstructionOperandResolver resolver;
        private Dictionary<HandlerTypeCode, Func<BinaryReader, Instruction>> readHandlers;

        private readonly GenericParamContext gpc;

        private readonly static List<ConvInfo> instructionInfos = new List<ConvInfo>
        {
            new ConvInfo(0, false, false, OpCodes.Conv_I1),
            new ConvInfo(1, false, false, OpCodes.Conv_I2),
            new ConvInfo(2, false, false, OpCodes.Conv_I4),
            new ConvInfo(3, false, false, OpCodes.Conv_I8),
            new ConvInfo(4, false, false, OpCodes.Conv_R4),
            new ConvInfo(5, false, false, OpCodes.Conv_R8),
            new ConvInfo(6, false, false, OpCodes.Conv_U1),
            new ConvInfo(7, false, false, OpCodes.Conv_U2),
            new ConvInfo(8, false, false, OpCodes.Conv_U4),
            new ConvInfo(9, false, false, OpCodes.Conv_U8),
            new ConvInfo(10, false, false, OpCodes.Conv_I),
            new ConvInfo(11, false, false, OpCodes.Conv_U),

            new ConvInfo(0, true, false, OpCodes.Conv_Ovf_I1),
            new ConvInfo(1, true, false, OpCodes.Conv_Ovf_I2),
            new ConvInfo(2, true, false, OpCodes.Conv_Ovf_I4),
            new ConvInfo(3, true, false, OpCodes.Conv_Ovf_I8),
            new ConvInfo(6, true, false, OpCodes.Conv_Ovf_U1),
            new ConvInfo(7, true, false, OpCodes.Conv_Ovf_U2),
            new ConvInfo(8, true, false, OpCodes.Conv_Ovf_U4),
            new ConvInfo(9, true, false, OpCodes.Conv_Ovf_U8),
            new ConvInfo(10, true, false, OpCodes.Conv_Ovf_I),
            new ConvInfo(11, true, false, OpCodes.Conv_Ovf_U),

            new ConvInfo(0, true, true, OpCodes.Conv_Ovf_I1_Un),
            new ConvInfo(1, true, true, OpCodes.Conv_Ovf_I2_Un),
            new ConvInfo(2, true, true, OpCodes.Conv_Ovf_I4_Un),
            new ConvInfo(3, true, true, OpCodes.Conv_Ovf_I8_Un),
            new ConvInfo(6, true, true, OpCodes.Conv_Ovf_U1_Un),
            new ConvInfo(7, true, true, OpCodes.Conv_Ovf_U2_Un),
            new ConvInfo(8, true, true, OpCodes.Conv_Ovf_U4_Un),
            new ConvInfo(9, true, true, OpCodes.Conv_Ovf_U8_Un),
            new ConvInfo(10, true, true, OpCodes.Conv_Ovf_I_Un),
            new ConvInfo(11, true, true, OpCodes.Conv_Ovf_U_Un),
            new ConvInfo(12, true, true, OpCodes.Conv_R_Un)
        };

        #region Handlers

        private Instruction Handler_Add(BinaryReader reader) => OpCodes.Add.ToInstruction();
        private Instruction Handler_Add_Ovf(BinaryReader reader) => OpCodes.Add_Ovf.ToInstruction();
        private Instruction Handler_Add_Ovf_Un(BinaryReader reader) => OpCodes.Add_Ovf_Un.ToInstruction();
        private Instruction Handler_And(BinaryReader reader) => OpCodes.And.ToInstruction();
        private Instruction Handler_Beq(BinaryReader reader) => new Instruction(OpCodes.Beq, new TargetDisplOperand(reader.ReadInt32()));
        private Instruction Handler_Bge(BinaryReader reader) => new Instruction(OpCodes.Bge, new TargetDisplOperand(reader.ReadInt32()));
        private Instruction Handler_Bge_Un(BinaryReader reader) => new Instruction(OpCodes.Bge_Un, new TargetDisplOperand(reader.ReadInt32()));
        private Instruction Handler_Bgt(BinaryReader reader) => new Instruction(OpCodes.Bgt, new TargetDisplOperand(reader.ReadInt32()));
        private Instruction Handler_Bgt_Un(BinaryReader reader) => new Instruction(OpCodes.Bgt_Un, new TargetDisplOperand(reader.ReadInt32()));
        private Instruction Handler_Ble(BinaryReader reader) => new Instruction(OpCodes.Ble, new TargetDisplOperand(reader.ReadInt32()));
        private Instruction Handler_Ble_Un(BinaryReader reader) => new Instruction(OpCodes.Ble_Un, new TargetDisplOperand(reader.ReadInt32()));
        private Instruction Handler_Blt(BinaryReader reader) => new Instruction(OpCodes.Blt, new TargetDisplOperand(reader.ReadInt32()));
        private Instruction Handler_Blt_Un(BinaryReader reader) => new Instruction(OpCodes.Blt_Un, new TargetDisplOperand(reader.ReadInt32()));
        private Instruction Handler_Bne_Un(BinaryReader reader) => new Instruction(OpCodes.Bne_Un, new TargetDisplOperand(reader.ReadInt32()));
        private Instruction Handler_Box(BinaryReader reader) => OpCodes.Box.ToInstruction(resolver.ResolveToken(reader.ReadUInt32(), gpc) as ITypeDefOrRef);
        private Instruction Handler_Br(BinaryReader reader) => new Instruction(OpCodes.Br, new TargetDisplOperand(reader.ReadInt32()));
        private Instruction Handler_Brfalse(BinaryReader reader) => new Instruction(OpCodes.Brfalse, new TargetDisplOperand(reader.ReadInt32()));
        private Instruction Handler_Brtrue(BinaryReader reader) => new Instruction(OpCodes.Brtrue, new TargetDisplOperand(reader.ReadInt32()));
        private Instruction Handler_Call(BinaryReader reader) => OpCodes.Call.ToInstruction(resolver.ResolveToken(reader.ReadUInt32(), gpc) as IMethod);
        private Instruction Handler_Callvirt(BinaryReader reader) => OpCodes.Callvirt.ToInstruction(resolver.ResolveToken(reader.ReadUInt32(), gpc) as IMethod);
        private Instruction Handler_Castclass(BinaryReader reader) => OpCodes.Castclass.ToInstruction(resolver.ResolveToken(reader.ReadUInt32(), gpc) as ITypeDefOrRef);
        private Instruction Handler_Ceq(BinaryReader reader) => new Instruction(OpCodes.Ceq, new TargetDisplOperand(reader.ReadInt32()));
        private Instruction Handler_Cgt(BinaryReader reader) => new Instruction(OpCodes.Cgt, new TargetDisplOperand(reader.ReadInt32()));
        private Instruction Handler_Cgt_Un(BinaryReader reader) => new Instruction(OpCodes.Cgt_Un, new TargetDisplOperand(reader.ReadInt32()));
        private Instruction Handler_Clt(BinaryReader reader) => new Instruction(OpCodes.Clt, new TargetDisplOperand(reader.ReadInt32()));
        private Instruction Handler_Clt_Un(BinaryReader reader) => new Instruction(OpCodes.Clt_Un, new TargetDisplOperand(reader.ReadInt32()));
        private Instruction Handler_Conv(BinaryReader reader)
        {
            byte type = reader.ReadByte();
            bool second = reader.ReadBoolean();
            bool third = reader.ReadBoolean();

            Instruction instruction = null;

            foreach(ConvInfo info in instructionInfos)
            {
                if (type != info.Type || info.Second != second || info.Third == third)
                    continue;

                instruction = new Instruction { OpCode = info.opCode };
                break;
            }
            if (instruction == null)
                throw new ApplicationException("Invalid opcode");

            return instruction;
        }
        private Instruction Handler_Div(BinaryReader reader) => OpCodes.Div.ToInstruction();
        private Instruction Handler_Div_Un(BinaryReader reader) => OpCodes.Div_Un.ToInstruction();
        private Instruction Handler_Dup(BinaryReader reader) => OpCodes.Dup.ToInstruction();
        private Instruction Handler_Endfinally(BinaryReader reader) => OpCodes.Endfinally.ToInstruction();
        private Instruction Handler_Initobj(BinaryReader reader) => OpCodes.Initobj.ToInstruction(resolver.ResolveToken(reader.ReadUInt32(), gpc) as ITypeDefOrRef);
        private Instruction Handler_Isinst(BinaryReader reader) => OpCodes.Isinst.ToInstruction(resolver.ResolveToken(reader.ReadUInt32(), gpc) as ITypeDefOrRef);
        private Instruction Handler_Ldarg(BinaryReader reader) => new Instruction(OpCodes.Ldarg, new ArgOperand(reader.ReadUInt16()));
        private Instruction Handler_Ldarga(BinaryReader reader) => new Instruction(OpCodes.Ldarga, new ArgOperand(reader.ReadUInt16()));
        private Instruction Handler_Ldc(BinaryReader reader)
        {
            switch ((ElementType)reader.ReadByte())
            {
                case ElementType.I4: return Instruction.CreateLdcI4(reader.ReadInt32());
                case ElementType.I8: return OpCodes.Ldc_I8.ToInstruction(reader.ReadInt64());
                case ElementType.R4: return OpCodes.Ldc_R4.ToInstruction(reader.ReadSingle());
                case ElementType.R8: return OpCodes.Ldc_R8.ToInstruction(reader.ReadDouble());
                case ElementType.Object: return OpCodes.Ldnull.ToInstruction();
                default: throw new ApplicationException("Invalid instruction");
            }
        }
        private Instruction Handler_Ldelem(BinaryReader reader) => new Instruction(OpCodes.Ldelem, null);
        private Instruction Handler_Ldelema(BinaryReader reader) => new Instruction(OpCodes.Ldelema, null);
        private Instruction Handler_Ldfld_Ldsfld(BinaryReader reader)
        {
            IField field = resolver.ResolveToken(reader.ReadUInt32(), gpc) as IField;
            return new Instruction(null, new FieldInstructionOperand(OpCodes.Ldsfld, OpCodes.Ldfld, field));
        }
        private Instruction Handler_Ldflda_Ldsflda(BinaryReader reader)
        {
            IField field = resolver.ResolveToken(reader.ReadUInt32(), gpc) as IField;
            return new Instruction(null, new FieldInstructionOperand(OpCodes.Ldsflda, OpCodes.Ldflda, field));
        }
        private Instruction Handler_Ldftn(BinaryReader reader) => OpCodes.Ldftn.ToInstruction(resolver.ResolveToken(reader.ReadUInt32(), gpc) as IMethod);
        private Instruction Handler_Ldlen(BinaryReader reader) => OpCodes.Ldlen.ToInstruction();
        private Instruction Handler_Ldloc(BinaryReader reader) => new Instruction(OpCodes.Ldloc, new LocalOperand(reader.ReadUInt16()));
        private Instruction Handler_Ldloca(BinaryReader reader) => new Instruction(OpCodes.Ldloca, new LocalOperand(reader.ReadUInt16()));
        private Instruction Handler_Ldobj(BinaryReader reader) => new Instruction(OpCodes.Ldobj, null);
        private Instruction Handler_Ldstr(BinaryReader reader) => OpCodes.Ldstr.ToInstruction(reader.ReadString());
        private Instruction Handler_Ldtoken(BinaryReader reader) => OpCodes.Ldtoken.ToInstruction(resolver.ResolveToken(reader.ReadUInt32(), gpc) as ITokenOperand);
        private Instruction Handler_Ldvirtftn(BinaryReader reader)
        {
            IMethod method = resolver.ResolveToken(reader.ReadUInt32(), gpc) as IMethod;
            reader.ReadUInt32();
            return OpCodes.Ldvirtftn.ToInstruction(method);
        }
        private Instruction Handler_Leave(BinaryReader reader) => new Instruction(OpCodes.Leave, new TargetDisplOperand(reader.ReadInt32()));
        private Instruction Handler_Mul(BinaryReader reader) => OpCodes.Mul.ToInstruction();
        private Instruction Handler_Mul_Ovf(BinaryReader reader) => OpCodes.Mul_Ovf.ToInstruction();
        private Instruction Handler_Mul_Ovf_Un(BinaryReader reader) => OpCodes.Mul_Ovf_Un.ToInstruction();
        private Instruction Handler_Neg(BinaryReader reader) => OpCodes.Neg.ToInstruction();
        private Instruction Handler_newarr(BinaryReader reader) => OpCodes.Newarr.ToInstruction(resolver.ResolveToken(reader.ReadUInt32(), gpc) as ITypeDefOrRef);
        private Instruction Handler_Newobj(BinaryReader reader) => OpCodes.Newobj.ToInstruction(resolver.ResolveToken(reader.ReadUInt32(), gpc) as IMethod);
        private Instruction Handler_Nop(BinaryReader reader) => OpCodes.Nop.ToInstruction();
        private Instruction Handler_Not(BinaryReader reader) => OpCodes.Not.ToInstruction();
        private Instruction Handler_Or(BinaryReader reader) => OpCodes.Or.ToInstruction();
        private Instruction Handler_Pop(BinaryReader reader) => OpCodes.Pop.ToInstruction();
        private Instruction Handler_Rem(BinaryReader reader) => OpCodes.Rem.ToInstruction();
        private Instruction Handler_Rem_Un(BinaryReader reader) => OpCodes.Rem_Un.ToInstruction();
        private Instruction Handler_Ret(BinaryReader reader) => OpCodes.Ret.ToInstruction();
        private Instruction Handler_Rethrow(BinaryReader reader) => OpCodes.Rethrow.ToInstruction();
        private Instruction Handler_Shl(BinaryReader reader) => OpCodes.Shl.ToInstruction();
        private Instruction Handler_Shr(BinaryReader reader) => OpCodes.Shr.ToInstruction();
        private Instruction Handler_Shr_Un(BinaryReader reader) => OpCodes.Shr_Un.ToInstruction();
        private Instruction Handler_Starg(BinaryReader reader) => new Instruction(OpCodes.Starg, new ArgOperand(reader.ReadUInt16()));
        private Instruction Handler_Stelem(BinaryReader reader) => new Instruction(OpCodes.Stelem, null);
        private Instruction Handler_Stfld_Stsfld(BinaryReader reader)
        {
            IField field = resolver.ResolveToken(reader.ReadUInt32(), gpc) as IField;
            return new Instruction(null, new FieldInstructionOperand(OpCodes.Stsfld, OpCodes.Stfld, field));
        }
        private Instruction Handler_Stloc(BinaryReader reader)
        {
            ushort loc = reader.ReadUInt16();
            reader.ReadInt32();
            return new Instruction(OpCodes.Stloc, new LocalOperand(loc));
        }
        private Instruction Handler_Stobj(BinaryReader reader) => new Instruction(OpCodes.Stobj, null);
        private Instruction Handler_Sub(BinaryReader reader) => OpCodes.Sub.ToInstruction();
        private Instruction Handler_Sub_Ovf(BinaryReader reader) => OpCodes.Sub_Ovf.ToInstruction();
        private Instruction Handler_Sub_Ovf_Un(BinaryReader reader) => OpCodes.Sub_Ovf_Un.ToInstruction();
        private Instruction Handler_Switch(BinaryReader reader)
        {
            int size = reader.ReadInt32();
            int[] offsets = new int[size];

            for (int i = 0; i < size; i++)
                offsets[i] = reader.ReadInt32();

            return new Instruction(OpCodes.Switch, new SwitchTargetDisplOperand(offsets));
        }
        private Instruction Handler_Throw(BinaryReader reader) => OpCodes.Throw.ToInstruction();
        private Instruction Handler_Unbox_Any(BinaryReader reader) => OpCodes.Unbox_Any.ToInstruction(resolver.ResolveToken(reader.ReadUInt32(), gpc) as ITypeDefOrRef);
        private Instruction Handler_Xor(BinaryReader reader) => OpCodes.Xor.ToInstruction();

        #endregion

        public OpCodeHandlerInfoReader(IInstructionOperandResolver resolver, GenericParamContext gpc)
        {
            this.resolver = resolver;
            this.gpc = gpc;

            readHandlers = new Dictionary<HandlerTypeCode, Func<BinaryReader, Instruction>>
            {
                {HandlerTypeCode.Add, Handler_Add },
                {HandlerTypeCode.Add_Ovf, Handler_Add_Ovf },
                {HandlerTypeCode.Add_Ovf_Un, Handler_Add_Ovf_Un },
                {HandlerTypeCode.And, Handler_And },
                {HandlerTypeCode.Beq, Handler_Beq },
                {HandlerTypeCode.Bge, Handler_Bge },
                {HandlerTypeCode.Bge_Un, Handler_Bge_Un },
                {HandlerTypeCode.Bgt, Handler_Bgt },
                {HandlerTypeCode.Bgt_Un, Handler_Bgt_Un },
                {HandlerTypeCode.Ble, Handler_Ble },
                {HandlerTypeCode.Ble_Un, Handler_Ble_Un },
                {HandlerTypeCode.Blt, Handler_Blt },
                {HandlerTypeCode.Blt_Un, Handler_Blt_Un },
                {HandlerTypeCode.Bne_Un, Handler_Bne_Un },
                {HandlerTypeCode.Box, Handler_Box },
                {HandlerTypeCode.Br, Handler_Br },
                {HandlerTypeCode.Brfalse, Handler_Brfalse },
                {HandlerTypeCode.Brtrue, Handler_Brtrue },
                {HandlerTypeCode.Call, Handler_Call},
                {HandlerTypeCode.Callvirt, Handler_Callvirt },
                {HandlerTypeCode.Castclass, Handler_Castclass },
                {HandlerTypeCode.Ceq, Handler_Ceq },
                {HandlerTypeCode.Cgt, Handler_Cgt },
                {HandlerTypeCode.Cgt_Un, Handler_Cgt_Un },
                {HandlerTypeCode.Clt, Handler_Clt },
                {HandlerTypeCode.Clt_Un, Handler_Clt_Un },
                {HandlerTypeCode.Conv, Handler_Conv },
                {HandlerTypeCode.Div, Handler_Div },
                {HandlerTypeCode.Div_Un, Handler_Div_Un },
                {HandlerTypeCode.Dup, Handler_Dup },
                {HandlerTypeCode.Endfinally, Handler_Endfinally },
                {HandlerTypeCode.Initobj, Handler_Initobj },
                {HandlerTypeCode.Isinst, Handler_Isinst },
                {HandlerTypeCode.Ldarg, Handler_Ldarg },
                {HandlerTypeCode.Ldarga, Handler_Ldarga },
                {HandlerTypeCode.Ldc, Handler_Ldc},
                {HandlerTypeCode.Ldelem, Handler_Ldelem },
                {HandlerTypeCode.Ldelema, Handler_Ldelema },
                {HandlerTypeCode.Ldflda_Ldsflda, Handler_Ldflda_Ldsflda },
                {HandlerTypeCode.Ldfld_Ldsfld, Handler_Ldfld_Ldsfld },
                {HandlerTypeCode.Ldftn, Handler_Ldftn },
                {HandlerTypeCode.Ldlen, Handler_Ldlen },
                {HandlerTypeCode.Ldloc, Handler_Ldloc },
                {HandlerTypeCode.Ldloca, Handler_Ldloca },
                {HandlerTypeCode.Ldobj, Handler_Ldobj },
                {HandlerTypeCode.Ldtoken,Handler_Ldtoken },
                {HandlerTypeCode.Ldvirtftn, Handler_Ldvirtftn },
                {HandlerTypeCode.Leave, Handler_Leave },
                {HandlerTypeCode.Mul, Handler_Mul },
                {HandlerTypeCode.Mul_Ovf, Handler_Mul_Ovf },
                {HandlerTypeCode.Mul_Ovf_Un, Handler_Mul_Ovf_Un },
                {HandlerTypeCode.Neg, Handler_Neg },
                {HandlerTypeCode.Newarr, Handler_newarr },
                {HandlerTypeCode.Newobj, Handler_Newobj },
                {HandlerTypeCode.Nop, Handler_Nop },
                {HandlerTypeCode.Not, Handler_Not },
                {HandlerTypeCode.Or, Handler_Or },
                {HandlerTypeCode.Pop, Handler_Pop },
                {HandlerTypeCode.Rem, Handler_Rem },
                {HandlerTypeCode.Rem_Un, Handler_Rem_Un },
                {HandlerTypeCode.Ret, Handler_Ret },
                {HandlerTypeCode.Rethrow, Handler_Rethrow},
                {HandlerTypeCode.Shl, Handler_Shl },
                {HandlerTypeCode.Shr, Handler_Shr },
                {HandlerTypeCode.Shr_Un, Handler_Shr_Un },
                {HandlerTypeCode.Starg, Handler_Starg },
                {HandlerTypeCode.Stelem, Handler_Stelem },
                {HandlerTypeCode.Stfld_Stsfld, Handler_Stfld_Stsfld },
                {HandlerTypeCode.Stloc, Handler_Stloc },
                {HandlerTypeCode.Stobj, Handler_Stobj },
                {HandlerTypeCode.Sub, Handler_Sub},
                {HandlerTypeCode.Sub_Ovf, Handler_Sub_Ovf },
                {HandlerTypeCode.Sub_Ovf_Un, Handler_Sub_Ovf_Un },
                {HandlerTypeCode.Switch, Handler_Switch},
                {HandlerTypeCode.Throw, Handler_Throw },
                {HandlerTypeCode.Unbox_any, Handler_Unbox_Any },
                {HandlerTypeCode.Xor, Handler_Xor }
            };
        }

        public Instruction Read(HandlerTypeCode typeCode, BinaryReader reader)
        {
            if (!readHandlers.TryGetValue(typeCode, out Func<BinaryReader, Instruction> readHandler))
                throw new ApplicationException("Invalid handler type");

            return readHandler(reader);
        }
    }
}
