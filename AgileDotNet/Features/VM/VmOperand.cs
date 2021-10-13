using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace AgileDotNet.Features.VM
{
    interface IVmOperand
    {
    }

    class TargetDisplOperand : IVmOperand
    {
        public readonly int Displacement;
        public TargetDisplOperand(int displacement) => Displacement = displacement;
    }
    class SwitchTargetDisplOperand : IVmOperand
    {
        public readonly int[] TargetDisplacements;
        public SwitchTargetDisplOperand(int[] targetDisplacements) => TargetDisplacements = targetDisplacements;
    }
    class ArgOperand : IVmOperand
    {
        public readonly ushort Arg;
        public ArgOperand(ushort arg) => Arg = arg;
    }
    class LocalOperand : IVmOperand
    {
        public readonly ushort Local;
        public LocalOperand(ushort local) => Local = local;
    }
    class FieldInstructionOperand : IVmOperand
    {
        public readonly OpCode StaticOpCode;
        public readonly OpCode InstanceOpCode;
        public readonly IField Field;

        public FieldInstructionOperand(OpCode sOpCode, OpCode iOpCode, IField field)
        {
            StaticOpCode = sOpCode;
            InstanceOpCode = iOpCode;
            Field = field;
        }
    }
}
