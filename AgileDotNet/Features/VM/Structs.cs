using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileDotNet.Features.VM
{
    public struct VirtualMethodDef
    {
        public Guid Guid;
        public uint MethodToken;
        public byte[] LocalVarStream;
        public byte[] CodeStream;
        public byte[] ExceptionHandlerStream;

        public VirtualMethodDef(Guid guid, uint token, byte[] lVar, byte[] code, byte[] exceptionHandlers)
        {
            Guid = guid;
            MethodToken = token;
            LocalVarStream = lVar;
            CodeStream = code;
            ExceptionHandlerStream = exceptionHandlers;
        }

        public override string ToString()
        {
            return $"{MethodToken:X8} - {Guid}";
        }
    }
}
