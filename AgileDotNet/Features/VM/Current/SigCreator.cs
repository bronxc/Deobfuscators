using System.Collections.Generic;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using de4dot.blocks;

namespace AgileDotNet.Features.VM.Current
{
    public enum BlockElementHash : int { }
    public class BlockSigInfo
    {
        private readonly List<int> targets;

        public List<BlockElementHash> Hashes { get; private set; }
        public List<int> Targets => targets;
        public bool HasFallThrough { get; set; }
        public bool EndsInRet { get; set; }

        public BlockSigInfo()
        {
            targets = new List<int>();
            Hashes = new List<BlockElementHash>();
        }
        public BlockSigInfo(List<BlockElementHash> hashes, List<int> targets)
        {
            Hashes = hashes;
            this.targets = targets;
        }
    }
    public class MethodSigInfo
    {
        public HandlerTypeCode TypeCode { get; set; }
        public List<BlockSigInfo> BlockSigInfos { get; private set; }

        public MethodSigInfo(List<BlockSigInfo> blockSigInfos) => BlockSigInfos = blockSigInfos;
        public MethodSigInfo(List<BlockSigInfo> blockSigInfos, HandlerTypeCode type)
        {
            BlockSigInfos = blockSigInfos;
            TypeCode = type;
        }

        public override string ToString()
        {
            return OpCodeHandlerInfo.GetHandlerName(TypeCode);
        }
    }
    public class SigCreator
    {
        const int BASE_INDEX = 0x40000000;
        Blocks blocks;
        Dictionary<object, int> objToId = new Dictionary<object, int>();
        CRC32 hasher = new CRC32();

        private static bool IsNonObfuscatedAssembly(IAssembly asm)
        {
            if (asm == null)
                return false;

            if (asm.Name != "mscorlib" && asm.Name != "System")
                return false;

            return true;
        }

        private static bool IsFromNonObfuscatedAssembly(TypeRef tr)
        {
            if (tr == null)
                return false;

            for(int i = 0; i < 100; i++)
            {
                if (tr.ResolutionScope is AssemblyRef asmRef)
                    return IsNonObfuscatedAssembly(asmRef);

                if(tr.ResolutionScope is TypeRef tr2)
                {
                    tr = tr2;
                    continue;
                }

                break;
            }

            return false;
        }
        private static bool IsFromNonObfuscatedAssembly(IMemberRefParent mrp) => IsFromNonObfuscatedAssembly(mrp as TypeRef);

        private void Hash(string s)
        {
            if (s != null)
                hasher.Hash(Encoding.UTF8.GetBytes(s));
        }
        private void Hash(AssemblyRef asmRef)
        {
            if (asmRef == null)
                return;

            bool canWriteAsm = IsNonObfuscatedAssembly(asmRef);
            hasher.Hash(canWriteAsm ? 1 : 0);
            if (canWriteAsm)
            {
                bool hasPk = !PublicKeyBase.IsNullOrEmpty2(asmRef.PublicKeyOrToken);
                if (hasPk)
                    hasher.Hash(PublicKeyBase.ToPublicKeyToken(asmRef.PublicKeyOrToken).Data);

                Hash(asmRef.Name);
                Hash(asmRef.Culture);
            }
        }
        private void Hash(GenericInstMethodSig gims)
        {
            if (gims == null)
                return;

            hasher.Hash((byte)gims.GetCallingConvention());
            foreach (TypeSig ga in gims.GetGenericArguments())
                Hash(ga);
        }
        private void Hash(MethodSig ms)
        {
            if (ms == null)
                return;

            hasher.Hash((byte)ms.GetCallingConvention());
            Hash(ms.GetRetType());

            foreach (TypeSig p in ms.GetParams())
                Hash(p);

            hasher.Hash(ms.GetParamCount());

            if (ms.GetParamsAfterSentinel() != null)
                foreach (TypeSig p in ms.GetParamsAfterSentinel())
                    Hash(p);
        }
        private void Hash(FieldSig fs)
        {
            if (fs == null)
                return;

            hasher.Hash((byte)fs.GetCallingConvention());
            Hash(fs.GetFieldType());
        }
        private void Hash(TypeSpec ts)
        {
            if (ts == null)
                return;

            Hash(ts.TypeSig);
        }
        private void Hash(TypeRef tr)
        {
            if (tr == null)
                return;

            Hash(tr.ResolutionScope);
            if (IsFromNonObfuscatedAssembly(tr))
            {
                Hash(tr.Namespace);
                Hash(tr.Name);
            }
        }
        private void Hash(TypeDef td)
        {
            if (td == null)
                return;

            Hash(td.BaseType);
            TypeAttributes attrMask = TypeAttributes.LayoutMask | TypeAttributes.ClassSemanticsMask | TypeAttributes.Abstract | TypeAttributes.SpecialName | TypeAttributes.Import | TypeAttributes.WindowsRuntime | TypeAttributes.StringFormatMask | TypeAttributes.RTSpecialName;

            hasher.Hash((uint)(td.Attributes & attrMask));
            hasher.Hash(td.GenericParameters.Count);
            hasher.Hash(td.Interfaces.Count);

            foreach (InterfaceImpl If in td.Interfaces)
                Hash(If.Interface);

            int? id = GetId(td);
            if (id != null)
                hasher.Hash(id.Value);
        }
        private void Hash(MemberRef mr)
        {
            if (mr == null)
                return;

            Hash(mr.Class);
            if (IsFromNonObfuscatedAssembly(mr.Class))
                Hash(mr.Name);

            Hash(mr.Signature);
        }
        private void Hash(MethodDef md)
        {
            if (md == null)
                return;

            MethodImplAttributes attrMask1 = MethodImplAttributes.CodeTypeMask | MethodImplAttributes.ManagedMask | MethodImplAttributes.ForwardRef | MethodImplAttributes.PreserveSig | MethodImplAttributes.InternalCall;

            hasher.Hash((ushort)(md == null ? 0 : md.ImplAttributes & attrMask1));
            MethodAttributes attrMask2 = MethodAttributes.Static | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask | MethodAttributes.CheckAccessOnOverride | MethodAttributes.Abstract | MethodAttributes.SpecialName | MethodAttributes.PinvokeImpl | MethodAttributes.UnmanagedExport | MethodAttributes.RTSpecialName;

            hasher.Hash((ushort)(md.Attributes & attrMask2));
            Hash(md.Signature);
            hasher.Hash(md.ParamDefs.Count);
            hasher.Hash(md.GenericParameters.Count);
            hasher.Hash(md.HasImplMap ? 1 : 0);

            int? id = GetId(md);
            if (id != null)
                hasher.Hash(id.Value);
        }
        private void Hash(TypeSig sig, int level)
        {
            if (sig == null)
                return;
            if (level++ > 20)
                return;

            hasher.Hash((byte)0x41);
            ElementType et = sig.GetElementType();
            hasher.Hash((byte)et);

            switch (et)
            {
                case ElementType.Ptr:
                case ElementType.ByRef:
                case ElementType.SZArray:
                case ElementType.Pinned:
                    Hash(sig.Next, level);
                    break;
                case ElementType.Array:
                    ArraySig aS = (ArraySig)sig;
                    hasher.Hash(aS.Rank);
                    hasher.Hash(aS.Sizes.Count);
                    hasher.Hash(aS.LowerBounds.Count);
                    Hash(sig.Next, level);
                    break;
                case ElementType.CModReqd:
                case ElementType.CModOpt:
                    Hash(((ModifierSig)sig).Modifier);
                    Hash(sig.Next, level);
                    break;
                case ElementType.ValueArray:
                    hasher.Hash(((ValueArraySig)sig).Size);
                    Hash(sig.Next, level);
                    break;
                case ElementType.Module:
                    hasher.Hash(((ModuleSig)sig).Index);
                    Hash(sig.Next, level);
                    break;
                case ElementType.GenericInst:
                    GenericInstSig gis = (GenericInstSig)sig;
                    Hash(gis.GenericType, level);
                    foreach (TypeSig ts in gis.GenericArguments)
                        Hash(ts, level);
                    Hash(sig.Next, level);
                    break;
                case ElementType.FnPtr:
                    Hash(((FnPtrSig)sig).Signature);
                    break;
                case ElementType.Var:
                case ElementType.MVar:
                    hasher.Hash(((GenericSig)sig).Number);
                    break;
                case ElementType.ValueType:
                case ElementType.Class:
                    Hash(((TypeDefOrRefSig)sig).TypeDefOrRef);
                    break;
                case ElementType.End:
                case ElementType.Void:
                case ElementType.Boolean:
                case ElementType.Char:
                case ElementType.I1:
                case ElementType.U1:
                case ElementType.I2:
                case ElementType.U2:
                case ElementType.I4:
                case ElementType.U4:
                case ElementType.I8:
                case ElementType.U8:
                case ElementType.R4:
                case ElementType.R8:
                case ElementType.String:
                case ElementType.TypedByRef:
                case ElementType.I:
                case ElementType.U:
                case ElementType.Object:
                case ElementType.Internal:
                case ElementType.Sentinel:
                default: break;
            }
        }
        private void Hash(TypeSig sig) => Hash(sig, 0);
        private void Hash(object op)
        {
            if(op is MethodDef md)
            {
                Hash(md);
            }
            if(op is MemberRef mr)
            {
                Hash(mr);
                return;
            }
            if(op is TypeDef td)
            {
                Hash(td);
                return;
            }
            if(op is TypeRef tr)
            {
                Hash(tr);
                return;
            }
            if(op is TypeSpec ts)
            {
                Hash(ts);
                return;
            }
            if(op is FieldSig fs)
            {
                Hash(fs);
                return;
            }
            if(op is MethodSig ms)
            {
                Hash(ms);
                return;
            }
            if(op is GenericInstMethodSig gism)
            {
                Hash(gism);
                return;
            }
            if(op is AssemblyRef ar)
            {
                Hash(ar);
                return;
            }
            if(op is TypeSig tsi)
            {
                Hash(tsi);
                return;
            }

            return;
        }

        private bool IsTypeField(FieldDef fd) => fd != null && fd.DeclaringType == blocks.Method.DeclaringType;
        private static int GetFieldId(FieldDef fd)
        {
            if (fd == null)
                return int.MinValue;

            TypeSig fieldType = fd.FieldSig.GetFieldType();

            if (fieldType == null)
                return int.MinValue + 1;

            int result = BASE_INDEX + 0x1000;
            for(int i = 0; i < 100; i++)
            {
                result += (int)fieldType.ElementType;

                if (fieldType.Next == null)
                    break;

                result += 0x100;
                fieldType = fieldType.Next;
            }

            TypeDef td = fieldType.TryGetTypeDef();
            if (td != null && td.IsEnum)
                return result + 0x10000000;

            return result;
        }

        private BlockElementHash GetHash(int val)
        {
            hasher.Hash(val);
            return (BlockElementHash)hasher.GetHash();
        }
        private BlockElementHash GetHash(string s)
        {
            Hash(s);
            return (BlockElementHash)hasher.GetHash();
        }

        private BlockElementHash? CalculateHash(IList<Instr> instrs, ref int index)
        {
            hasher.Initialize();

            Instr ins = instrs[index];
            switch (ins.OpCode.Code)
            {
                case Code.Beq:
                case Code.Beq_S:
                    return GetHash(BASE_INDEX + 0);
                case Code.Bge:
                case Code.Bge_S:
                    return GetHash(BASE_INDEX + 1);
                case Code.Bge_Un:
                case Code.Bge_Un_S:
                    return GetHash(BASE_INDEX + 2);
                case Code.Bgt:
                case Code.Bgt_S:
                    return GetHash(BASE_INDEX + 3);
                case Code.Bgt_Un:
                case Code.Bgt_Un_S:
                    return GetHash(BASE_INDEX + 4);
                case Code.Ble:
                case Code.Ble_S:
                    return GetHash(BASE_INDEX + 5);
                case Code.Ble_Un:
                case Code.Ble_Un_S:
                    return GetHash(BASE_INDEX + 6);
                case Code.Blt:
                case Code.Blt_S:
                    return GetHash(BASE_INDEX + 7);
                case Code.Blt_Un:
                case Code.Blt_Un_S:
                    return GetHash(BASE_INDEX + 8);
                case Code.Bne_Un:
                case Code.Bne_Un_S:
                    return GetHash(BASE_INDEX + 9);
                case Code.Brfalse:
                case Code.Brfalse_S:
                    return GetHash(BASE_INDEX + 10);
                case Code.Brtrue:
                case Code.Brtrue_S:
                    return GetHash(BASE_INDEX + 11);
                case Code.Switch:
                    return GetHash(BASE_INDEX + 12);
                case Code.Ceq:
                    return GetHash(BASE_INDEX + 13);
                case Code.Cgt:
                    return GetHash(BASE_INDEX + 14);
                case Code.Cgt_Un:
                    return GetHash(BASE_INDEX + 15);
                case Code.Clt:
                    return GetHash(BASE_INDEX + 16);
                case Code.Ldc_I4:
                case Code.Ldc_I4_0:
                case Code.Ldc_I4_1:
                case Code.Ldc_I4_2:
                case Code.Ldc_I4_3:
                case Code.Ldc_I4_4:
                case Code.Ldc_I4_5:
                case Code.Ldc_I4_6:
                case Code.Ldc_I4_7:
                case Code.Ldc_I4_8:
                case Code.Ldc_I4_M1:
                case Code.Ldc_I4_S:
                    return GetHash(ins.GetLdcI4Value());
                case Code.Ldstr:
                    return GetHash(ins.Operand as string);
                case Code.Rethrow:
                    return GetHash(BASE_INDEX + 18);
                case Code.Throw:
                    return GetHash(BASE_INDEX + 19);
                case Code.Call:
                case Code.Callvirt:
                    Hash(ins.Operand);
                    return (BlockElementHash)hasher.GetHash();
                case Code.Ldfld:
                    FieldDef field = ins.Operand as FieldDef;
                    if (!IsTypeField(field))
                        return null;

                    if (index + 1 >= instrs.Count || !instrs[index + 1].IsLdcI4())
                        return null;

                    index++;
                    return GetHash(GetFieldId(field));
                default:break;
            }

            return null;
        }

        public SigCreator() { }

        public void AddId(object key, int id)
        {
            if (key != null)
                objToId[key] = id;
        }
        int? GetId(object key)
        {
            if (key == null)
                return null;

            if (objToId.TryGetValue(key, out int i))
                return i;

            return null;
        }
        public List<BlockSigInfo> Create(MethodDef method)
        {
            blocks = new Blocks(method);
            List<Block> allBlocks = blocks.MethodBlocks.GetAllBlocks();

            List<BlockSigInfo> blockInfos = new List<BlockSigInfo>();

            foreach(Block block in allBlocks)
            {
                BlockSigInfo blockInfo = new BlockSigInfo
                {
                    HasFallThrough = block.FallThrough != null,
                    EndsInRet = block.LastInstr.OpCode.Code == Code.Ret
                };
                blockInfos.Add(blockInfo);

                IList<Instr> instructions = block.Instructions;
                for(int i = 0; i < instructions.Count; i++)
                {
                    BlockElementHash? info = CalculateHash(instructions, ref i);
                    if (info != null)
                        blockInfo.Hashes.Add(info.Value);
                }
            }

            for(int i = 0; i < blockInfos.Count; i++)
            {
                Block block = allBlocks[i];
                BlockSigInfo blockInfo = blockInfos[i];

                if (block.FallThrough != null)
                    blockInfo.Targets.Add(allBlocks.IndexOf(block.FallThrough));

                if (block.Targets != null)
                    foreach (Block target in block.Targets)
                        blockInfo.Targets.Add(allBlocks.IndexOf(target));
            }

            return blockInfos;
        }
    }
}
