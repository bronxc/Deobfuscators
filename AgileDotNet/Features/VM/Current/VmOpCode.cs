using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileDotNet.Features.VM.Current
{
    public class VmOpCode
    {
        public List<HandlerTypeCode> HandlerTypeCodes { get; private set; }

        public VmOpCode(List<HandlerTypeCode> opCodeHandlerInfos)
        {
            HandlerTypeCodes = new List<HandlerTypeCode>();
            HandlerTypeCodes.AddRange(opCodeHandlerInfos);
        }

        public override string ToString()
        {
            return OpCodeHandlerInfo.GetCompositeName(HandlerTypeCodes);
        }
    }
}
