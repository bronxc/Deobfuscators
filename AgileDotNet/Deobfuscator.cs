using AgileDotNet.Features;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AgileDotNet
{
    public struct DeobfuscatorSettings
    {
        public bool DecryptStrings;
        public bool DelegateFixer;
        public bool AntiTamper;
        public bool ControlFlow;
        public bool DeVirtualize;
        public bool ClearClasses;
    }
    public class Deobfuscator
    {
        private DeobfuscatorSettings m_settings;

        private bool IsAllowed(string name)
        {
            List<string> AllowedNames = new List<string>()
            {
                "<Module>"
            };

            foreach(string allowedName in AllowedNames)
            {
                if (allowedName == name)
                    return true;
            }
            return false;
        }

        public Deobfuscator(DeobfuscatorSettings setting)
        {
            this.m_settings = setting;
        }

        public void UpdateSettings(DeobfuscatorSettings settings)
        {
            this.m_settings = settings;
        }
        public void Deobfuscate(string file)
        {
            AssemblyDef AsmDef = AssemblyDef.Load(file);

            string FinalMessage = "";

            if (m_settings.DelegateFixer)
            {
                DelegateFix df = new DelegateFix(AsmDef);

                FinalMessage += "[+] Delegates fixed: " + df.FixDelegates() + ";\n";
            }
            if (m_settings.DecryptStrings)
            {
                StringDecryptor sd = new StringDecryptor(AsmDef, file);

                int DecryptedStrings = 0;

                foreach (ModuleDef module in AsmDef.Modules)
                {
                    foreach (TypeDef type in module.GetTypes())
                    {
                        foreach (MethodDef method in type.Methods)
                        {
                            if (method.HasBody && method.Body.HasInstructions)
                            {
                                for (int i = 0; i < method.Body.Instructions.Count; i++)
                                {
                                    if (method.Body.Instructions[i].OpCode == OpCodes.Ldstr && method.Body.Instructions[i + 1].OpCode == OpCodes.Call && method.Body.Instructions[i + 1].Operand.ToString().Contains(sd.GetMethod()))
                                    {
                                        string encrypted = method.Body.Instructions[i].Operand.ToString();
                                        string decrypted = sd.Decrypt(encrypted);

                                        method.Body.Instructions[i].Operand = decrypted;
                                        method.Body.Instructions[i + 1].OpCode = OpCodes.Nop;

                                        DecryptedStrings++;

                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }

                FinalMessage += "[+] Decrypted strings: " + DecryptedStrings + ";\n";
            }
            if (m_settings.ControlFlow)
            {
                ControlFlow cf = new ControlFlow(AsmDef);

                ControlFlow.ControlFlowInfo cfInfo = cf.DoControlFlow();

                string message = "[+] ControlFlow : {\n  Fixed Arithmetics: "+cfInfo.FixedArithmetics+";\n  Cleared Control Flows: "+cfInfo.ClearedControlFlows+";\n  Total: "+cfInfo.TotalFixed+";\n}\n";

                FinalMessage += message;
            }
            if (m_settings.AntiTamper)
            {
                FinalMessage += "[+] Tampers removed: " + AntiTamper.RemoveTampers(AsmDef) + ";\n";
            }
            if (m_settings.ClearClasses && m_settings.DecryptStrings && m_settings.DelegateFixer)
            {
                List<TypeDef> types = AsmDef.Modules[0].GetTypes().ToList();
                foreach (TypeDef type in types)
                {
                    if(type.Namespace == "" && !IsAllowed(type.Name))
                    {
                        AsmDef.Modules[0].Types.Remove(type);
                    }
                }
            }



            ModuleWriterOptions options = new ModuleWriterOptions(AsmDef.ManifestModule);

            options.MetadataOptions.Flags = options.MetadataOptions.Flags | MetadataFlags.PreserveAll;
            options.MetadataOptions.PreserveHeapOrder(AsmDef.ManifestModule, true);

            string DeFile = Path.GetFileNameWithoutExtension(file) + "_deobfed" + Path.GetExtension(file);

            AsmDef.Write("deobfed" + "/" + DeFile);

            Utils.Info(FinalMessage);
        }
    }
}
