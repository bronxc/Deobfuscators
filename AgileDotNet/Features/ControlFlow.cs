using de4dot.blocks;
using de4dot.blocks.cflow;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileDotNet.Features
{
    public class ControlFlow
    {
        private class MyControlFlow : BlockDeobfuscator
        {
            private Block switchBlock;
            private Local localSwitch;
            private List<Local> AllVariables;

            

            public MethodDef m_Method;//Doin' this by the classic cpp style
            public InstructionEmulator m_InstEmul;

            public MyControlFlow() //This constructor is for initilaizing some of the variables
            {
                AllVariables = new List<Local>();
                m_InstEmul = new InstructionEmulator();
            }

            private bool IsSwitch(Block block)
            {
                if (block.Instructions.Count <= 6)
                    return false;
                if (!block.FirstInstr.IsLdcI4())
                    return false;

                switchBlock = block;
                localSwitch = Instr.GetLocalVar(AllVariables, block.Instructions[block.Instructions.Count - 4]);
                return true;
            }
            private bool IsExpression(Block block)
            {
                if (block.Instructions.Count < 7)
                    return false;
                if (!block.FirstInstr.IsStloc())
                    return false;

                switchBlock = block;
                localSwitch = Instr.GetLocalVar(blocks.Method.Body.Variables.Locals, block.Instructions[block.Instructions.Count - 4]);
                return true;
            }
            private int EmuCase(out int LocalValue)
            {
                m_InstEmul.Emulate(switchBlock.Instructions, 0, switchBlock.Instructions.Count - 1);
                Int32Value val = m_InstEmul.GetLocal(localSwitch) as Int32Value;
                LocalValue = val.Value;
                return ((Int32Value)m_InstEmul.Pop()).Value;
            }
            private bool Replace(Block block, int lVal)
            {
                if (block.IsConditionalBranch())
                {
                    if (block.FallThrough.FallThrough == switchBlock)
                         block = block.FallThrough;
                    else block = block.FallThrough.FallThrough;
                }

                if (block.LastInstr.OpCode == OpCodes.Switch)
                    block = block.FallThrough;
                if (block == switchBlock) return false;

                for(int i = 0; i < block.Instructions.Count; i++)
                {
                    if(block.Instructions[i].Instruction.GetLocal(blocks.Method.Body.Variables) == localSwitch)
                    {
                        block.Instructions[i] = new Instr(Instruction.CreateLdcI4(lVal));
                        return true;
                    }
                }

                return false;
            }
            private bool IsXor(Block block)
            {
                int inst = block.Instructions.Count - 1;
                List<Instr> instrs = block.Instructions;

                if (inst < 4)
                    return false;

                if (instrs[inst].OpCode != OpCodes.Xor)
                    return false;
                if (!instrs[inst - 1].IsLdcI4())
                    return false;
                if (instrs[inst - 2].OpCode != OpCodes.Mul)
                    return false;
                if (instrs[inst - 3].IsLdcI4())
                    return false;
                if (instrs[inst - 4].IsLdcI4())
                    return false;

                return true;
            }

            private bool ClearFlow()
            {
                bool Moded = false;
                List<Block> AllBlocks = new List<Block>();
                List<Block> TargetBlocks = new List<Block>();

                foreach(Block block in allBlocks)
                {
                    if (block.FallThrough == switchBlock)
                        AllBlocks.Add(block);
                }

                TargetBlocks = switchBlock.Targets;
                
                foreach(Block block in AllBlocks)
                {
                    if (block.LastInstr.IsLdcI4())
                    {
                        int val = block.LastInstr.GetLdcI4Value();
                        m_InstEmul.Push(new Int32Value(val));
                        int @case = EmuCase(out int LValue);

                        block.ReplaceLastNonBranchWithBranch(0, TargetBlocks[@case]);
                        Replace(TargetBlocks[@case], LValue);

                        block.Instructions.Add(new Instr(new Instruction(OpCodes.Pop)));
                        Moded = true;
                    }else if (IsXor(block))
                    {
                        m_InstEmul.Emulate(block.Instructions, block.Instructions.Count - 5, block.Instructions.Count);
                        Int32Value i32 = (Int32Value)m_InstEmul.Pop();
                        m_InstEmul.Push(i32);

                        int @case = EmuCase(out int lVal);

                        block.ReplaceLastNonBranchWithBranch(0, TargetBlocks[@case]);
                        Replace(TargetBlocks[@case], lVal);
                        block.Instructions.Add(new Instr(new Instruction(OpCodes.Pop)));

                        Moded = true;
                    }else if(block.Sources.Count == 1 && block.Instructions.Count == 1)
                    {
                        List<Block> srcs = new List<Block>(block.Sources);
                        foreach(Block source in srcs)
                        {
                            int i32 = source.FirstInstr.GetLdcI4Value();
                            m_InstEmul.Push(new Int32Value(i32));
                            int @case = EmuCase(out int lVal);

                            source.ReplaceLastNonBranchWithBranch(0, TargetBlocks[@case]);
                            Replace(TargetBlocks[@case], lVal);
                            source.Instructions[1] = new Instr(new Instruction(OpCodes.Pop));

                            Moded = true;
                        }
                    }else if(block.LastInstr.OpCode == OpCodes.Xor)
                    {
                        if(block.Instructions[block.Instructions.Count - 2].OpCode == OpCodes.Mul)
                        {
                            List<Instr> Instructs = block.Instructions;

                            int InstrL = Instructs.Count;

                            if (!Instructs[InstrL - 4].IsLdcI4())
                                continue;

                            List<Block> Sources = new List<Block>(block.Sources);

                            foreach(Block source in Sources)
                            {
                                if (source.FirstInstr.IsLdcI4())
                                {
                                    int i32 = source.FirstInstr.GetLdcI4Value();
                                    try
                                    {
                                        Instructs[InstrL - 5] = new Instr(new Instruction(OpCodes.Ldc_I4, i32));
                                    }
                                    catch
                                    {
                                        Instructs.Insert(InstrL - 4, new Instr(new Instruction(OpCodes.Ldc_I4, i32)));
                                        InstrL++;
                                    }

                                    m_InstEmul.Emulate(Instructs, InstrL - 5, InstrL);

                                    int @case = EmuCase(out int lVal);

                                    source.ReplaceLastNonBranchWithBranch(0, TargetBlocks[@case]);
                                    Replace(TargetBlocks[@case], lVal);

                                    try
                                    {
                                        source.Instructions[1] = new Instr(new Instruction(OpCodes.Pop));
                                    }
                                    catch
                                    {
                                        source.Instructions.Add(new Instr(new Instruction(OpCodes.Pop)));
                                    }

                                    Moded = true;
                                }
                            }
                        }
                    }
                }

                return Moded;
            }

            protected override bool Deobfuscate(Block block) //The name explains it self
            {
                bool Moded = false;

                if(block.LastInstr.OpCode == OpCodes.Switch)
                {
                    AllVariables = blocks.Method.Body.Variables.ToList();

                    if(IsSwitch(block))
                    {
                        m_InstEmul.Initialize(blocks.Method);
                        Moded = ClearFlow();
                    }

                    if (IsExpression(block))
                    {
                        m_InstEmul.Initialize(blocks.Method);
                        Moded = ClearFlow();
                        while (ClearFlow())
                        {
                            Moded = ClearFlow();
                        }
                    }
                }

                return Moded;
            }
        }
        public struct ControlFlowInfo
        {
            public int FixedArithmetics;
            public int ClearedControlFlows;
            public int TotalFixed;
        }

        private AssemblyDef Asm;

        private bool IsControlFlowed(MethodDef m)
        {
            for(int i = 0; i < m.Body.Instructions.Count; i++)
            {
                if (m.Body.Instructions[i].OpCode == OpCodes.Switch)
                    return true;
            }

            return false;
        }

        private int FixMath()
        {
            int FixedMaths = 0;
            try
            {
                foreach(ModuleDef module in Asm.Modules)
                {
                    foreach(TypeDef type in module.GetTypes())
                    {
                        foreach(MethodDef method in type.Methods)
                        {
                            if (method.HasBody && method.Body.HasInstructions)
                            {
                                for(int i = 0; i < method.Body.Instructions.Count; i++)
                                {
                                    CilBody body = method.Body;
                                    bool flag = body.Instructions[i].OpCode == OpCodes.Ldc_I4 && body.Instructions[i + 1].OpCode == OpCodes.Call && body.Instructions[i + 1].Operand.ToString().Contains("Math::Abs") && body.Instructions[i + 2].IsStloc() && body.Instructions[i + 3].IsBr();

                                    if (flag)
                                    {
                                        method.Body.Instructions[i].Operand = Math.Abs(Convert.ToInt32(body.Instructions[i].Operand));
                                        method.Body.Instructions[i + 1].OpCode = OpCodes.Nop;
                                        FixedMaths++;
                                    }

                                    body = method.Body;

                                    flag = false;
                                    flag = body.Instructions[i].OpCode == OpCodes.Ldc_I4 && body.Instructions[i + 1].OpCode == OpCodes.Call && body.Instructions[i + 1].Operand.ToString().Contains("Math::Abs") && body.Instructions[i + 2].IsStloc() && body.Instructions[i + 3].IsLdloc() && body.Instructions[i + 4].OpCode == OpCodes.Switch;

                                    if (flag)
                                    {
                                        method.Body.Instructions[i].Operand = Math.Abs(Convert.ToInt32(body.Instructions[i].Operand));
                                        method.Body.Instructions[i + 1].OpCode = OpCodes.Nop;
                                        FixedMaths++;
                                    }
                                }
                            }
                        }
                    }
                }
            }catch(Exception e)
            {
                Utils.HandleError(e);
            }
            return FixedMaths;
        }
        private int ClearControlFlows()
        {
            int ClearedFlows = 0;

            try
            {
                foreach(ModuleDef module in Asm.Modules)
                {
                    foreach(TypeDef type in module.GetTypes())
                    {
                        foreach(MethodDef method in type.Methods)
                        {
                            if (method.HasBody && IsControlFlowed(method))
                            {
                                BlocksCflowDeobfuscator bcfd = new BlocksCflowDeobfuscator();
                                Blocks blocks = new Blocks(method);

                                blocks.RemoveDeadBlocks();
                                blocks.RepartitionBlocks();

                                blocks.UpdateBlocks();
                                blocks.Method.Body.SimplifyBranches();
                                blocks.Method.Body.OptimizeBranches();

                                bcfd.Initialize(blocks);
                                bcfd.Add(new MyControlFlow());

                                bcfd.Deobfuscate();
                                blocks.RepartitionBlocks();

                                IList<Instruction> instructs;
                                IList<ExceptionHandler> exHandlers;

                                blocks.GetCode(out instructs, out exHandlers);
                                DotNetUtils.RestoreBody(method, instructs, exHandlers);

                                ClearedFlows++;
                            }
                        }
                    }
                }
            }catch(Exception e)
            {
                Utils.HandleError(e);
            }

            return ClearedFlows;
        }

        public ControlFlow(AssemblyDef def)
        {
            this.Asm = def;
        }

        public ControlFlowInfo DoControlFlow()
        {
            ControlFlowInfo cfi = default;
            cfi.FixedArithmetics = FixMath();
            cfi.ClearedControlFlows = ClearControlFlows();
            cfi.TotalFixed = cfi.FixedArithmetics + cfi.ClearedControlFlows;

            return cfi;
        }
    }
}
