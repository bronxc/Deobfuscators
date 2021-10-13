using System.IO;
using System.Collections.Generic;

using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace AgileDotNet.Features.VM.Current
{
    public class CsvmToCilMethodConverter : CSVMToCilMethodConverterBase
    {
        VmOpCodeHandlerDetector opCodeDetector;

        public CsvmToCilMethodConverter(IContext context, ModuleDefMD module, VmOpCodeHandlerDetector opCodeDetector) : base(context, module) => this.opCodeDetector = opCodeDetector;

        protected override List<Instruction> ReadInstructions(MethodDef cilMethod, VirtualMethodDef csvmMethod)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(csvmMethod.CodeStream));
            List<Instruction> instructions = new List<Instruction>();
            GenericParamContext gpc = GenericParamContext.Create(cilMethod);
            OpCodeHandlerInfoReader handlerInfoReader = new OpCodeHandlerInfoReader(module, gpc);

            int VmInstructionsCount = reader.ReadInt32();
            ushort[] vmInstructions = new ushort[VmInstructionsCount];

            for (int i = 0; i < VmInstructionsCount; i++)
                vmInstructions[i] = reader.ReadUInt16();

            uint offset = 0;
            for(int i = 0; i < VmInstructionsCount; i++)
            {
                VmOpCode composite = opCodeDetector.Handlers[vmInstructions[i]];
                IList<HandlerTypeCode> handlerInfos = composite.HandlerTypeCodes;

                if (handlerInfos.Count == 0)
                    handlerInfos = new HandlerTypeCode[] { HandlerTypeCode.Nop };

                for(int j = 0; j < handlerInfos.Count; j++)
                {
                    Instruction instruction = handlerInfoReader.Read(handlerInfos[j], reader);

                    instruction.Offset = offset;
                    offset += (uint)GetInstructionSize(instruction);

                    SetCilToVmIndex(instruction, i);

                    if (j == 0)
                        SetVmIndexToCil(instruction, i);

                    instructions.Add(instruction);
                }
            }

            return instructions;
        }
    }
}
