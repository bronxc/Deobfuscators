using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileDotNet.Features
{
    public class AntiTamper
    {
        public static int RemoveTampers(AssemblyDef asm)
        {
            int RemovedTampers = 0;

            foreach (ModuleDef module in asm.Modules)
            {
                foreach (TypeDef type in module.GetTypes())
                {
                    foreach (MethodDef method in type.Methods)
                    {
                        if (method.ImplAttributes == MethodImplAttributes.NoInlining)
                        {
                            method.ImplAttributes = MethodImplAttributes.IL;
                            RemovedTampers++;
                        }
                    }
                }
            }

            return RemovedTampers;
        }
    }
}
