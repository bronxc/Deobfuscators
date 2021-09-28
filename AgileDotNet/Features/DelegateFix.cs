using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileDotNet.Features
{
    public class DelegateFix
    {
        private AssemblyDef asmDef;
        private bool flag = false;
        private bool IsFinished = false; //Useless

        private FieldDef getField(TypeDef type)
        {
            foreach(FieldDef field in type.Fields)
            {
                return field;
            }
            return null;
        }
        private MemberRef GetMember(MethodDef method)
        {
            TypeDef type = method.DeclaringType;
            if(type.BaseType.FullName == "System.MulticastDelegate")
            {
                foreach(MethodDef meth in type.Methods)
                {
                    if(meth.HasBody && meth.Body.HasInstructions)
                    {
                        for(int i = 0; i < meth.Body.Instructions.Count; i++)
                        {
                            if (meth.Body.Instructions[i].IsLdcI4())
                            {
                                int value = meth.Body.Instructions[i].GetLdcI4Value();
                                FieldDef fieldDelegate = getField(type);

                                string DelName = fieldDelegate.Name;
                                if (DelName.EndsWith("%"))
                                {
                                    flag = true;
                                    DelName = DelName.TrimEnd(new char[] { '%' });
                                }

                                uint num = BitConverter.ToUInt32(Convert.FromBase64String(DelName), 0);
                                ModuleDefMD mod = (ModuleDefMD)asmDef.ManifestModule;
                                MemberRef solvedMemberRef = mod.ResolveMemberRef((uint)(num + 167772161L) - 167772160U);
                                return solvedMemberRef;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public DelegateFix(AssemblyDef asm)
        {
            asmDef = asm;
        }
        public int FixDelegates()
        {
            int fixedDelegates = 0;
            IsFinished = false;

            foreach (ModuleDef module in asmDef.Modules)
            {
                foreach (TypeDef type in module.GetTypes())
                {
                    foreach (MethodDef method in type.Methods)
                    {
                        if (method.HasBody && method.Body.HasInstructions)
                        {
                            for (int i = 0; i < method.Body.Instructions.Count; i++)
                            {
                                if (method.Body.Instructions[i].OpCode == OpCodes.Call && method.Body.Instructions[i].Operand.ToString().Contains("::Invoke"))
                                {
                                    var operand = method.Body.Instructions[i].Operand;
                                    if (operand is MethodDef method1)
                                    {
                                        MemberRef @ref = GetMember(method1);

                                        method.Body.Instructions[i].Operand = @ref;
                                        fixedDelegates++;
                                    }
                                }
                                if (method.Body.Instructions[i].OpCode == OpCodes.Ldsfld)
                                {
                                    var op = method.Body.Instructions[i].Operand;
                                    if (op is FieldDef field && field.DeclaringType.BaseType.FullName == "System.MulticastDelegate")
                                    {
                                        method.Body.Instructions[i].OpCode = OpCodes.Nop;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            IsFinished = true;
            return fixedDelegates;
        }
        public bool IsDone() => IsFinished; //Useless
    }
}
