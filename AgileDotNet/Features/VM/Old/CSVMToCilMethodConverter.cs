using System.Collections.Generic;
using System.IO;

using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace AgileDotNet.Features.VM.Old
{
    public class CSVMToCilMethodConverter : CSVMToCilMethodConverterBase
    {
        VmOpCodeHandlerDetector opCodeDetector;

        public CSVMToCilMethodConverter(IContext context, ModuleDefMD module, VmOpCodeHandlerDetector detector) : base(context, module) => opCodeDetector = detector;

        protected override List<Instruction> ReadInstructions(MethodDef cilMethod, VirtualMethodDef csvmMethod)
        {
            GenericParamContext gpc = GenericParamContext.Create(cilMethod);
            BinaryReader reader = new BinaryReader(new MemoryStream(csvmMethod.CodeStream));
            List<Instruction> instructions = new List<Instruction>();

            uint offsets = 0;

            while(reader.BaseStream.Position < reader.BaseStream.Length)
            {
                ushort opCode = reader.ReadUInt16();
                var inst = opCodeDetector.Handlers[opCode].Read(reader, module, gpc);

                inst.Offset = offsets;

                offsets += (uint)GetInstructionSize(inst);

                SetCilToVmIndex(inst, instructions.Count);
                SetVmIndexToCil(inst, instructions.Count);

                instructions.Add(inst);
            }

            return instructions;
        }
    }
}
