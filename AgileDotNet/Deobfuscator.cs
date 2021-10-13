using AgileDotNet.Features;
using de4dot.blocks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AgileDotNet.Features.VM.Old;

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
        public bool MakeEditable;
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
        private Instruction[] Replace(int start,int end, IList<Instruction> instrs, Instruction[] replaceWith) { 
            if((end-start) >= replaceWith.Length)
            {
                for(int i = start; i < end; i++)
                {
                    instrs[i].OpCode = replaceWith[i-start].OpCode;
                    instrs[i].Operand = replaceWith[i-start].Operand;
                    instrs[i].Offset = replaceWith[i-start].Offset;
                }
            }
            else
            {
                List<Instruction> OtherInstructs = new List<Instruction>();

                for(int i = end; i < instrs.Count; i++)
                {
                    OtherInstructs.Add(instrs[i]);
                }

                int overWritenInstrs = 0;

                for(int i = start; i < end; i++)
                {
                    if ((end - i) < 0)
                        overWritenInstrs++;

                    Instruction inst = replaceWith[i - start];

                    if ((i + 1) > instrs.Count)
                        instrs.Add(inst);
                    else
                    {
                        instrs[i].OpCode = inst.OpCode;
                        instrs[i].Operand = inst.Operand;
                        instrs[i].Offset = inst.Offset;
                    }
                }
                for(int i = end; i < overWritenInstrs; i++)
                {
                    if ((i + 1) > instrs.Count)
                        instrs.Add(OtherInstructs[i - end]);
                    else
                    {
                        instrs[i + overWritenInstrs].OpCode = OtherInstructs[i - end].OpCode;
                        instrs[i + overWritenInstrs].Operand = OtherInstructs[i - end].Operand;
                    }
                }
            }

            return instrs.ToArray();
        }

        public Deobfuscator(DeobfuscatorSettings setting)
        {
            this.m_settings = setting;
        }

        public void UpdateSettings(DeobfuscatorSettings settings)
        {
            this.m_settings = settings;
        }
        public void Deobfuscate(AssemblyDef AsmDef, string file = "", Assembly asm = null)
        {
            string FinalMessage = "";
            GLOBALS.CurrentFile = file;

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
            if (m_settings.DeVirtualize && file.EndsWith(".dll"))
            {
                foreach(ModuleDef module in AsmDef.Modules)
                {
                    IContext context = new Context();
                    context.SetData("Ops", new Dictionary<string, VmOpCodeHandlerDetector> { });

                    DeVirt deVirt = new DeVirt(context, module);
                    deVirt.Init(Assembly.LoadFrom(file));

                    if (deVirt.Restore())
                    {
                        FinalMessage += $"[+] Restored CSVMs: {deVirt.RestoredMethods}\n";
                    }
                    else Utils.Error("DeVirtualized failed!");
                }
            }
            if (m_settings.ControlFlow)
            {
                ControlFlow cf = new ControlFlow(AsmDef);

                ControlFlow.ControlFlowInfo cfInfo = cf.DoControlFlow();

                string message = "[+] ControlFlow : {\n  Fixed Arithmetics: " + cfInfo.FixedArithmetics + ";\n  Cleared Control Flows: " + cfInfo.ClearedControlFlows + ";\n  Total: " + cfInfo.TotalFixed + ";\n}\n";

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
                    if (type.Namespace == "" && !IsAllowed(type.Name))
                    {
                        AsmDef.Modules[0].Types.Remove(type);
                    }
                }
            }
            if (m_settings.MakeEditable)
            {
                AsmEditor asmEditor = new AsmEditor(AsmDef, asm);
                Utils.Debug<string, int>(asmEditor.FixNames());
            }


            ModuleWriterOptions options = new ModuleWriterOptions(AsmDef.ManifestModule);

            options.MetadataOptions.Flags = options.MetadataOptions.Flags | MetadataFlags.PreserveAll;
            options.MetadataOptions.PreserveHeapOrder(AsmDef.ManifestModule, true);

            string DeFile = Path.GetFileNameWithoutExtension(file) + "_deobfed" + Path.GetExtension(file);

            AsmDef.Write("deobfed" + "/" + DeFile);

            Utils.Info(FinalMessage);
        }
        public void Deobfuscate(string file)
        {
            AssemblyDef AsmDef = AssemblyDef.Load(file);
            Assembly asm = Assembly.UnsafeLoadFrom(file);

            Console.WriteLine(AsmDef.FullName);

            Deobfuscate(AsmDef, file, asm);
        }
    }
}
