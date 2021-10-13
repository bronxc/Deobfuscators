using System.Collections.Generic;
using System.IO;

namespace AgileDotNet.Features.VM.Current
{
    public static class OpCodeHandlerInfos
    {
        private static IList<MethodSigInfo> ReadOpCodeHandlerInfos(byte[] data) => Read(new BinaryReader(new MemoryStream(data)));

        public static void Write(BinaryWriter writer, List<MethodSigInfo> handlerInfos)
        {
            writer.Write(1);
            writer.Write(handlerInfos.Count);

            foreach(MethodSigInfo handler in handlerInfos)
            {
                writer.Write((int)handler.TypeCode);
                writer.Write(handler.BlockSigInfos.Count);

                foreach(BlockSigInfo info in handler.BlockSigInfos)
                {
                    writer.Write(info.Targets.Count);

                    foreach (int target in info.Targets)
                        writer.Write(target);

                    writer.Write(info.Hashes.Count);

                    foreach (BlockElementHash hash in info.Hashes)
                        writer.Write((uint)hash);

                    writer.Write(info.HasFallThrough);
                    writer.Write(info.EndsInRet);
                }
            }
        }
        public static List<MethodSigInfo> Read(BinaryReader reader)
        {
            if (reader.ReadInt32() != 1)
                throw new InvalidDataException();

            int HandlerCount = reader.ReadInt32();
            List<MethodSigInfo> list = new List<MethodSigInfo>(HandlerCount);

            for(int i = 0; i < HandlerCount; i++)
            {
                HandlerTypeCode typecode = (HandlerTypeCode)reader.ReadInt32();

                int BlockCount = reader.ReadInt32();
                List<BlockSigInfo> blocks = new List<BlockSigInfo>(BlockCount);

                for(int j = 0; j < BlockCount; j++)
                {
                    int TargetsCount = reader.ReadInt32();
                    List<int> targets = new List<int>(TargetsCount);

                    for (int k = 0; k < TargetsCount; k++)
                        targets.Add(reader.ReadInt32());

                    int HashesCount = reader.ReadInt32();
                    List<BlockElementHash> hashes = new List<BlockElementHash>(HashesCount);

                    for (int k = 0; k < HashesCount; k++)
                        hashes.Add((BlockElementHash)reader.ReadInt32());

                    BlockSigInfo block = new BlockSigInfo(hashes, targets)
                    {
                        HasFallThrough = reader.ReadBoolean(),
                        EndsInRet = reader.ReadBoolean()
                    };

                    blocks.Add(block);
                }

                list.Add(new MethodSigInfo(blocks, typecode));
            }

            return list;
        }

        public static readonly IList<MethodSigInfo>[] HandlerInfos = new IList<MethodSigInfo>[]
        {
            ReadOpCodeHandlerInfos(CSVMResources.CSVM1),
            ReadOpCodeHandlerInfos(CSVMResources.CSVM2),
            ReadOpCodeHandlerInfos(CSVMResources.CSVM3),
            ReadOpCodeHandlerInfos(CSVMResources.CSVM4),
            ReadOpCodeHandlerInfos(CSVMResources.CSVM5),
            ReadOpCodeHandlerInfos(CSVMResources.CSVM6)
        };
    }
}
