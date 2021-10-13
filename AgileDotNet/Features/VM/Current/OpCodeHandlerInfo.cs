using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileDotNet.Features.VM.Current
{
    public class OpCodeHandlerInfo
    {
        public HandlerTypeCode TypeCode { get; private set; }
        public string Name { get; private set; }

        public OpCodeHandlerInfo(HandlerTypeCode typecode)
        {
            TypeCode = typecode;
            Name = GetHandlerName(typecode);
        }

        //If needed fix this one
        public static string GetHandlerName(HandlerTypeCode type)
        {
            switch (type)
            {
                case HandlerTypeCode.Ldfld_Ldsfld: return "ldfld/ldsfld";
                case HandlerTypeCode.Ldflda_Ldsflda: return "ldflda/ldsflda";
                case HandlerTypeCode.Stfld_Stsfld: return "stfld/stsfld";
                default:
                    {
                        string result = type.ToString().ToLower().Replace("_", ".");
                        return result;
                    }
            }
        }
        public static string GetCompositeName(IList<HandlerTypeCode> typeCodes)
        {
            if (typeCodes.Count == 0)
                return null;
            StringBuilder sb = new StringBuilder();
            foreach(HandlerTypeCode type in typeCodes)
            {
                if (sb.Length != 0)
                    sb.Append(", ");
                sb.Append(GetHandlerName(type));
            }
            return sb.ToString();
        }
    }
}
