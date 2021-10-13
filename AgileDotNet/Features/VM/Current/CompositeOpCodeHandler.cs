using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileDotNet.Features.VM.Current
{
    public class CompositeOpCodeHandler
    {
        public List<BlockSigInfo> BlockSigInfos { get; private set; }
        public List<HandlerTypeCode> TypeCodes { get; private set; }

        public CompositeOpCodeHandler(List<BlockSigInfo> blockSigInfos)
        {
            BlockSigInfos = blockSigInfos;
            TypeCodes = new List<HandlerTypeCode>();
        }

        public override string ToString()
        {
            return OpCodeHandlerInfo.GetCompositeName(TypeCodes);
        }
    }
}
