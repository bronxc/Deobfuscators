using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using de4dot.blocks;

public class Utils
{
    public static void Write(ConsoleColor cc, string mess)
    {
        ConsoleColor oCC = Console.ForegroundColor;

        Console.ForegroundColor = cc;
        Console.WriteLine(mess);
        Console.ForegroundColor = oCC;
    }
    public static void Info(string message)
    {
        Write(ConsoleColor.Cyan, message);
    }
    public static void Warn(string message)
    {
        Write(ConsoleColor.Yellow, message);
    }
    public static void Error(string message)
    {
        Write(ConsoleColor.Red, message);
    }

    public static void Debug(string message)
    {
#if (DEBUG)
        Write(ConsoleColor.Magenta, message);
#endif
    }
    public static void Debug(object obj)
    {
        Debug(obj.ToString());
    }
    public static void Debug<T>(IList<T> list)
    {
        if (list == null)
            return;

        string msg = "List: {Count: '" + list.Count + "', Type: '" + list[0].GetType().FullName + "'}: [\n";

        for(int i = 0; i < list.Count; i++)
        {
            string m = list[0].ToString();

            if (!(i >= (list.Count-1)))
                m += ",";

            msg += (i == 0 ? "" : "\n") + m;
        }

        msg += "\n]";

        Debug(msg);
    }
    public static void Debug<T,K>(Dictionary<T,K> dict)
    {
        if (dict == null)
            return;

        string msg = "Dictionary: {Keys: '" + dict.Keys.Count + "', Values: '" + dict.Values.Count + "'}: [\n";

        int i = 0;

        foreach(T key in dict.Keys)
        {
            string m = "  { Key: '" + key.ToString() + "', Value: ";

            if (!dict.TryGetValue(key, out K val))
                m += "'null' }";
            else m += "'" + val.ToString() + "' }";

            if (!(i >= (dict.Keys.Count-1)))
                m += ",";

            msg += (i == 0 ? "" : "\n") +m;

            i++;
        }

        msg += "\n]";

        Debug(msg);
    }
    public static void DebugN(string s, object obj)
    {
        string msg = $"IsNull: '{s}' = {obj == null}";
        Debug(msg);
    }
    public static void DebugN<T>(IEnumerable<T> ei)
    {
        if(typeof(T) == typeof(UTF8String))
        {
            IEnumerable<UTF8String> ie2 = (IEnumerable<UTF8String>)ei;
            List<string> strings = new List<string>();

            foreach(UTF8String str in ie2)
            {
                strings.Add(str.String);
            }

            Debug<string>(strings);
            return;
        }

        Debug<T>(ei.ToList());
    }
    public static void HandleError(Exception e)
    {
        string caller = Assembly.GetCallingAssembly().GetName().Name;
        Error($"Got an error from '{caller}' Exception type: '{e.GetType().FullName}' Error: {e.ToString()}");
    }

    public static IEnumerable<T> Unique<T>(IEnumerable<T> values)
    {
        Dictionary<T, bool> dict = new Dictionary<T, bool>();

        foreach (T val in values)
            dict[val] = true;

        return dict.Keys;
    }
}

public struct CRC32
{
    static readonly uint[] table = new uint[256] {
            0x00000000, 0x77073096, 0xEE0E612C, 0x990951BA,
            0x076DC419, 0x706AF48F, 0xE963A535, 0x9E6495A3,
            0x0EDB8832, 0x79DCB8A4, 0xE0D5E91E, 0x97D2D988,
            0x09B64C2B, 0x7EB17CBD, 0xE7B82D07, 0x90BF1D91,
            0x1DB71064, 0x6AB020F2, 0xF3B97148, 0x84BE41DE,
            0x1ADAD47D, 0x6DDDE4EB, 0xF4D4B551, 0x83D385C7,
            0x136C9856, 0x646BA8C0, 0xFD62F97A, 0x8A65C9EC,
            0x14015C4F, 0x63066CD9, 0xFA0F3D63, 0x8D080DF5,
            0x3B6E20C8, 0x4C69105E, 0xD56041E4, 0xA2677172,
            0x3C03E4D1, 0x4B04D447, 0xD20D85FD, 0xA50AB56B,
            0x35B5A8FA, 0x42B2986C, 0xDBBBC9D6, 0xACBCF940,
            0x32D86CE3, 0x45DF5C75, 0xDCD60DCF, 0xABD13D59,
            0x26D930AC, 0x51DE003A, 0xC8D75180, 0xBFD06116,
            0x21B4F4B5, 0x56B3C423, 0xCFBA9599, 0xB8BDA50F,
            0x2802B89E, 0x5F058808, 0xC60CD9B2, 0xB10BE924,
            0x2F6F7C87, 0x58684C11, 0xC1611DAB, 0xB6662D3D,
            0x76DC4190, 0x01DB7106, 0x98D220BC, 0xEFD5102A,
            0x71B18589, 0x06B6B51F, 0x9FBFE4A5, 0xE8B8D433,
            0x7807C9A2, 0x0F00F934, 0x9609A88E, 0xE10E9818,
            0x7F6A0DBB, 0x086D3D2D, 0x91646C97, 0xE6635C01,
            0x6B6B51F4, 0x1C6C6162, 0x856530D8, 0xF262004E,
            0x6C0695ED, 0x1B01A57B, 0x8208F4C1, 0xF50FC457,
            0x65B0D9C6, 0x12B7E950, 0x8BBEB8EA, 0xFCB9887C,
            0x62DD1DDF, 0x15DA2D49, 0x8CD37CF3, 0xFBD44C65,
            0x4DB26158, 0x3AB551CE, 0xA3BC0074, 0xD4BB30E2,
            0x4ADFA541, 0x3DD895D7, 0xA4D1C46D, 0xD3D6F4FB,
            0x4369E96A, 0x346ED9FC, 0xAD678846, 0xDA60B8D0,
            0x44042D73, 0x33031DE5, 0xAA0A4C5F, 0xDD0D7CC9,
            0x5005713C, 0x270241AA, 0xBE0B1010, 0xC90C2086,
            0x5768B525, 0x206F85B3, 0xB966D409, 0xCE61E49F,
            0x5EDEF90E, 0x29D9C998, 0xB0D09822, 0xC7D7A8B4,
            0x59B33D17, 0x2EB40D81, 0xB7BD5C3B, 0xC0BA6CAD,
            0xEDB88320, 0x9ABFB3B6, 0x03B6E20C, 0x74B1D29A,
            0xEAD54739, 0x9DD277AF, 0x04DB2615, 0x73DC1683,
            0xE3630B12, 0x94643B84, 0x0D6D6A3E, 0x7A6A5AA8,
            0xE40ECF0B, 0x9309FF9D, 0x0A00AE27, 0x7D079EB1,
            0xF00F9344, 0x8708A3D2, 0x1E01F268, 0x6906C2FE,
            0xF762575D, 0x806567CB, 0x196C3671, 0x6E6B06E7,
            0xFED41B76, 0x89D32BE0, 0x10DA7A5A, 0x67DD4ACC,
            0xF9B9DF6F, 0x8EBEEFF9, 0x17B7BE43, 0x60B08ED5,
            0xD6D6A3E8, 0xA1D1937E, 0x38D8C2C4, 0x4FDFF252,
            0xD1BB67F1, 0xA6BC5767, 0x3FB506DD, 0x48B2364B,
            0xD80D2BDA, 0xAF0A1B4C, 0x36034AF6, 0x41047A60,
            0xDF60EFC3, 0xA867DF55, 0x316E8EEF, 0x4669BE79,
            0xCB61B38C, 0xBC66831A, 0x256FD2A0, 0x5268E236,
            0xCC0C7795, 0xBB0B4703, 0x220216B9, 0x5505262F,
            0xC5BA3BBE, 0xB2BD0B28, 0x2BB45A92, 0x5CB36A04,
            0xC2D7FFA7, 0xB5D0CF31, 0x2CD99E8B, 0x5BDEAE1D,
            0x9B64C2B0, 0xEC63F226, 0x756AA39C, 0x026D930A,
            0x9C0906A9, 0xEB0E363F, 0x72076785, 0x05005713,
            0x95BF4A82, 0xE2B87A14, 0x7BB12BAE, 0x0CB61B38,
            0x92D28E9B, 0xE5D5BE0D, 0x7CDCEFB7, 0x0BDBDF21,
            0x86D3D2D4, 0xF1D4E242, 0x68DDB3F8, 0x1FDA836E,
            0x81BE16CD, 0xF6B9265B, 0x6FB077E1, 0x18B74777,
            0x88085AE6, 0xFF0F6A70, 0x66063BCA, 0x11010B5C,
            0x8F659EFF, 0xF862AE69, 0x616BFFD3, 0x166CCF45,
            0xA00AE278, 0xD70DD2EE, 0x4E048354, 0x3903B3C2,
            0xA7672661, 0xD06016F7, 0x4969474D, 0x3E6E77DB,
            0xAED16A4A, 0xD9D65ADC, 0x40DF0B66, 0x37D83BF0,
            0xA9BCAE53, 0xDEBB9EC5, 0x47B2CF7F, 0x30B5FFE9,
            0xBDBDF21C, 0xCABAC28A, 0x53B39330, 0x24B4A3A6,
            0xBAD03605, 0xCDD70693, 0x54DE5729, 0x23D967BF,
            0xB3667A2E, 0xC4614AB8, 0x5D681B02, 0x2A6F2B94,
            0xB40BBE37, 0xC30C8EA1, 0x5A05DF1B, 0x2D02EF8D,
        };

    uint checkSum;

    public void Initialize() => checkSum = uint.MaxValue;

    public void Hash(byte[] data)
    {
        if (data == null)
            return;
        foreach (var b in data)
        {
            int i = (byte)(checkSum ^ b);
            checkSum = (checkSum >> 8) ^ table[i];
        }
    }

    public void Hash(sbyte a)
    {
        int i = (byte)(checkSum ^ a);
        checkSum = (checkSum >> 8) ^ table[i];
    }

    public void Hash(byte a)
    {
        int i = (byte)(checkSum ^ a);
        checkSum = (checkSum >> 8) ^ table[i];
    }

    public void Hash(short a)
    {
        int i = (byte)(checkSum ^ (byte)a);
        checkSum = (checkSum >> 8) ^ table[i];

        i = (byte)(checkSum ^ (byte)(a >> 8));
        checkSum = (checkSum >> 8) ^ table[i];
    }

    public void Hash(ushort a)
    {
        int i = (byte)(checkSum ^ (byte)a);
        checkSum = (checkSum >> 8) ^ table[i];

        i = (byte)(checkSum ^ (byte)(a >> 8));
        checkSum = (checkSum >> 8) ^ table[i];
    }

    public void Hash(int a)
    {
        int i = (byte)(checkSum ^ (byte)a);
        checkSum = (checkSum >> 8) ^ table[i];

        i = (byte)(checkSum ^ (byte)(a >> 8));
        checkSum = (checkSum >> 8) ^ table[i];

        i = (byte)(checkSum ^ (byte)(a >> 16));
        checkSum = (checkSum >> 8) ^ table[i];

        i = (byte)(checkSum ^ (byte)(a >> 24));
        checkSum = (checkSum >> 8) ^ table[i];
    }

    public void Hash(uint a)
    {
        int i = (byte)(checkSum ^ (byte)a);
        checkSum = (checkSum >> 8) ^ table[i];

        i = (byte)(checkSum ^ (byte)(a >> 8));
        checkSum = (checkSum >> 8) ^ table[i];

        i = (byte)(checkSum ^ (byte)(a >> 16));
        checkSum = (checkSum >> 8) ^ table[i];

        i = (byte)(checkSum ^ (byte)(a >> 24));
        checkSum = (checkSum >> 8) ^ table[i];
    }

    public void Hash(long a)
    {
        int i = (byte)(checkSum ^ (byte)a);
        checkSum = (checkSum >> 8) ^ table[i];

        i = (byte)(checkSum ^ (byte)(a >> 8));
        checkSum = (checkSum >> 8) ^ table[i];

        i = (byte)(checkSum ^ (byte)(a >> 16));
        checkSum = (checkSum >> 8) ^ table[i];

        i = (byte)(checkSum ^ (byte)(a >> 24));
        checkSum = (checkSum >> 8) ^ table[i];

        i = (byte)(checkSum ^ (byte)(a >> 32));
        checkSum = (checkSum >> 8) ^ table[i];

        i = (byte)(checkSum ^ (byte)(a >> 40));
        checkSum = (checkSum >> 8) ^ table[i];

        i = (byte)(checkSum ^ (byte)(a >> 48));
        checkSum = (checkSum >> 8) ^ table[i];

        i = (byte)(checkSum ^ (byte)(a >> 56));
        checkSum = (checkSum >> 8) ^ table[i];
    }

    public void Hash(ulong a)
    {
        int i = (byte)(checkSum ^ (byte)a);
        checkSum = (checkSum >> 8) ^ table[i];

        i = (byte)(checkSum ^ (byte)(a >> 8));
        checkSum = (checkSum >> 8) ^ table[i];

        i = (byte)(checkSum ^ (byte)(a >> 16));
        checkSum = (checkSum >> 8) ^ table[i];

        i = (byte)(checkSum ^ (byte)(a >> 24));
        checkSum = (checkSum >> 8) ^ table[i];

        i = (byte)(checkSum ^ (byte)(a >> 32));
        checkSum = (checkSum >> 8) ^ table[i];

        i = (byte)(checkSum ^ (byte)(a >> 40));
        checkSum = (checkSum >> 8) ^ table[i];

        i = (byte)(checkSum ^ (byte)(a >> 48));
        checkSum = (checkSum >> 8) ^ table[i];

        i = (byte)(checkSum ^ (byte)(a >> 56));
        checkSum = (checkSum >> 8) ^ table[i];
    }

    public uint GetHash() => ~checkSum;

    public static uint CheckSum(byte[] data)
    {
        if (data == null)
            return 0;
        uint cs = uint.MaxValue;
        foreach (var b in data)
        {
            int i = (byte)(cs ^ b);
            cs = (cs >> 8) ^ table[i];
        }
        return ~cs;
    }
}

public class PushedArgs
{
    private List<Instruction> args;
    private int nextIndex;

    public bool CanAddMore => nextIndex >= 0;
    public int NumValidArgs => args.Count - (nextIndex + 1);

    public PushedArgs(int numArgs)
    {
        nextIndex = numArgs - 1;
        args = new List<Instruction>(numArgs);
        for (int i = 0; i < numArgs; i++)
            args.Add(null);
    }

    public void Add(Instruction instr) => args[nextIndex--] = instr;
    public void Set(int i, Instruction instr) => args[i] = instr;

    public void Pop() => args[++nextIndex] = null;

    public Instruction Get(int i)
    {
        if (0 <= i && i < args.Count)
            return args[i];

        return null;
    }
    public Instruction GetEnd(int i) => Get(args.Count - 1 - i);
}
public class Branch
{
    public int current;
    public List<int> Variants { get; }
    public Branch() => Variants = new List<int>();
}
public class State
{
    public int index;
    public Branch branch;
    public int validArgs;
    public int skipPushes;
    public int addPushes;
    public HashSet<int> visited;

    public State(int index, Branch branch, int validArgs, int skipPushes, int addPushes, HashSet<int> visited)
    {
        this.index = index;
        this.branch = branch;
        this.validArgs = validArgs;
        this.skipPushes = skipPushes;
        this.addPushes = addPushes;
        this.visited = visited;
    }
    public State Clone() => new State(index, branch, validArgs, skipPushes, addPushes, new HashSet<int>(visited));
}
public static class MethodStack
{
    private static IList<Instruction> CacheInstructions = null;
    private static Dictionary<int, Branch> CacheBranches = null;
    private enum Update
    {
        Ok,
        Fail,
        Finish
    }

    private static Update UpdateState(IList<Instruction> instrs,State state, PushedArgs args)
    {
        if (state.index < 0 || state.index >= instrs.Count)
            return Update.Fail;

        Instruction instr = instrs[state.index];
        if (!Instr.IsFallThrough(instr.OpCode))
            return Update.Fail;

        instr.CalculateStackUsage(false, out int pushes, out int pops);
        if (pops == -1)
            return Update.Fail;

        bool isDup = instr.OpCode.Code == Code.Dup;
        if (isDup)
        {
            pushes = 1;
            pops = 0;
        }

        if (pushes > 1)
            return Update.Fail;

        if(state.skipPushes > 0)
        {
            state.skipPushes -= pushes;
            if (state.skipPushes < 0)
                return Update.Fail;

            state.skipPushes += pops;
        }
        else
        {
            if(pushes == 1)
            {
                if (isDup)
                    state.addPushes++;
                else
                {
                    for (; state.addPushes > 0; state.addPushes--)
                    {
                        args.Add(instr);
                        state.validArgs++;
                        if (!args.CanAddMore)
                            return Update.Finish;
                    }
                    state.addPushes = 1;
                }
            }
            state.skipPushes += pops;
        }

        return Update.Ok;
    }
    private static Dictionary<int,Branch> GetBranches(IList<Instruction> instrs)
    {
        if (instrs == CacheInstructions) 
            return CacheBranches;

        CacheInstructions = instrs;
        CacheBranches = new Dictionary<int, Branch>();
        
        for(int i = 0; i < instrs.Count; i++)
        {
            Instruction ins = instrs[i];
            if(ins.Operand is Instruction target)
            {
                int t = instrs.IndexOf(target);
                if(!CacheBranches.TryGetValue(t, out Branch br))
                {
                    br = new Branch();
                    CacheBranches.Add(t, br);
                }
            }
        }

        return CacheBranches;
    }

    private static PushedArgs GetPushedArgInstructions(IList<Instruction> instrs, int index, int numArgs)
    {
        PushedArgs pushedArgs = new PushedArgs(numArgs);
        if (!pushedArgs.CanAddMore)
            return pushedArgs;

        Dictionary<int, Branch> branches = null;
        Stack<State> states = new Stack<State>();
        State state = new State(index, null, 0, 0, 1, new HashSet<int>());
        bool isBacktrack = false;

        states.Push(state.Clone());

        while (true)
        {
            while(state.index >= 0)
            {
                if(branches != null && branches.TryGetValue(state.index, out Branch br) && state.visited.Add(state.index)){
                    br.current = 0;
                    State brState = state.Clone();
                    brState.branch = br;
                    states.Push(brState);
                }
                if (!isBacktrack)
                    state.index--;

                isBacktrack = true;

                Update update = UpdateState(instrs, state, pushedArgs);

                if (update == Update.Finish)
                    return pushedArgs;

                if (update == Update.Fail)
                    break;
            }

            if (states.Count == 0)
                return pushedArgs;

            int validArgs = state.validArgs;
            state = states.Pop();

            if(state.validArgs < validArgs)
                for(int i = state.validArgs+1;i<= validArgs; i++)
                {
                    pushedArgs.Pop();
                }

            if (branches == null)
                branches = GetBranches(instrs);
            else
            {
                isBacktrack = true;
                state.index = state.branch.Variants[state.branch.current++];
                if (state.branch.current < state.branch.Variants.Count)
                    states.Push(state.Clone());
                else state.branch = null;
            }
        }
    }

    public static PushedArgs GetPushedArgInstructions(IList<Instruction> instrs, int index)
    {
        try
        {
            instrs[index].CalculateStackUsage(false, out int pushes, out int pops);
            if (pops != -1)
                return GetPushedArgInstructions(instrs, index, pops);
        }catch(Exception ex) { }
        return new PushedArgs(0);
    }

    private static ByRefSig CreateByRefType(ITypeDefOrRef et)
    {
        if (et == null)
            return null;

        return new ByRefSig(et.ToTypeSig());
    }
    private static ByRefSig CreateByRefType(TypeSig et)
    {
        if (et == null)
            return null;
        return new ByRefSig(et);
    }

    public static TypeSig GetLoadedType(MethodDef method, IList<Instruction> instrs, int instrIndex, int argIndexFromEnd, out bool wasNewobj)
    {
        wasNewobj = false;
        PushedArgs args = GetPushedArgInstructions(instrs, instrIndex);

        Instruction pushInstr = args.GetEnd(argIndexFromEnd);
        if (pushInstr == null)
            return null;

        TypeSig type = null;
        Local local;

        ICorLibTypes corLibTypes = method.DeclaringType.Module.CorLibTypes;
        switch (pushInstr.OpCode.Code)
        {
            case Code.Ldstr:
                type = corLibTypes.String;
                break;
            case Code.Conv_Ovf_I_Un:
                type = corLibTypes.IntPtr;
                break;
            case Code.Conv_Ovf_U_Un:
                type = corLibTypes.UIntPtr;
                break;
            case Code.Conv_Ovf_I8_Un:
                type = corLibTypes.Int64;
                break;
            case Code.Conv_Ovf_U8_Un:
                type = corLibTypes.UInt64;
                break;
            case Code.Ldind_R8:
                type = corLibTypes.Double;
                break;
            case Code.Callvirt:
                IMethod called = pushInstr.Operand as IMethod;
                if (called == null)
                    return null;

                type = called.MethodSig.GetRetType();
                break;
            case Code.Newarr:
                ITypeDefOrRef t = pushInstr.Operand as ITypeDefOrRef;
                if (t == null)
                    return null;

                type = new SZArraySig(t.ToTypeSig());
                wasNewobj = true;
                break;
            case Code.Ldobj:
                type = (pushInstr.Operand as ITypeDefOrRef).ToTypeSig();
                break;
            case Code.Ldarg_3:
                type = pushInstr.GetArgumentType(method.MethodSig, method.DeclaringType);
                break;
            case Code.Ldloc_3:
                local = pushInstr.GetLocal(method.Body.Variables);
                if (local == null)
                    return null;
                type = local.Type.RemovePinned();
                break;
            case Code.Ldloca_S:
                local = pushInstr.Operand as Local;
                if (local == null)
                    return null;
                type = CreateByRefType(local.Type.RemovePinned());
                break;
            case Code.Ldarga_S:
                type = CreateByRefType(pushInstr.GetArgumentType(method.MethodSig, method.DeclaringType));
                break;
            case Code.Ldsfld:
                IField field = pushInstr.Operand as IField;
                if (field == null)
                    return null;
                type = field.FieldSig.GetFieldType();
                break;
            case Code.Ldsflda:
                IField field2 = pushInstr.Operand as IField;
                if (field2 == null)
                    return null;
                type = CreateByRefType(field2.FieldSig.GetFieldType());
                break;
            case Code.Unbox:
                type = CreateByRefType(pushInstr.Operand as ITypeDefOrRef);
                break;
        }
        return type;
    }
    public static TypeSig GetLoadedType(MethodDef method, IList<Instruction> instrs, int instrIndex, int argIndexFromEnd) => GetLoadedType(method, instrs, instrIndex, argIndexFromEnd, out bool wasNewobj);
    public static TypeSig GetLoadedType(MethodDef method, IList<Instruction> instrs, int instrIndex) => GetLoadedType(method, instrs, instrIndex, 0, out bool wasNewobj);
}

public interface IContext
{
    void Clear();
    void SetData(string name, object data);
    object GetData(string name);
    void ClearData(string name);
    TypeDef ResolveType(ITypeDefOrRef type);
    MethodDef ResolveMethod(IMethod method);
    FieldDef ResolveField(IField field);
}
public class Context : IContext
{
    Dictionary<string, object> dataDict = new Dictionary<string, object>(StringComparer.Ordinal);

    private static ITypeDefOrRef GetNonGenericTypeRef(ITypeDefOrRef type)
    {
        TypeSpec ts = type as TypeSpec;
        if (ts == null)
            return type;

        GenericInstSig gis = ts.TryGetGenericInstSig();

        if (gis == null || gis.GenericType == null)
            return type;

        return gis.GenericType.TypeDefOrRef;
    }

    public void Clear() => dataDict.Clear();
    public void SetData(string name, object data) => dataDict.Add(name, data);
    public object GetData(string name)
    {
        if (dataDict.TryGetValue(name, out object obj))
            return obj;
        else return null;
    }
    public void ClearData(string name) => dataDict.Remove(name);

    public TypeDef ResolveType(ITypeDefOrRef type)
    {
        if (type == null)
            return null;
        type = GetNonGenericTypeRef(type);

        if (type is TypeDef typedef)
            return typedef;

        if (type is TypeRef tr)
            return tr.Resolve();

        return null;
    }
    public MethodDef ResolveMethod(IMethod method)
    {
        if (method == null)
            return null;

        if (method is MethodDef md)
            return md;

        MemberRef mr = method as MemberRef;
        if (mr == null || !mr.IsMethodRef)
            return null;

        TypeDef type = ResolveType(mr.DeclaringType);
        if (type == null)
            return null;

        return type.Resolve(mr) as MethodDef;
    }
    public FieldDef ResolveField(IField field)
    {
        if (field == null)
            return null;

        if (field is FieldDef fd)
            return fd;

        MemberRef mr = field as MemberRef;
        if (mr == null || !mr.IsFieldRef)
            return null;

        TypeDef type = ResolveType(mr.DeclaringType);
        if (type == null)
            return null;

        return type.Resolve(mr) as FieldDef;
    }
}

public class StringCounts
{
    Dictionary<string, int> strings = new Dictionary<string, int>(StringComparer.Ordinal);

    public IEnumerable<string> Strings => strings.Keys;
    public int StringsCount => strings.Count;

    public void Add(string s)
    {
        strings.TryGetValue(s, out int count);
        strings[s] = count + 1;
    }
    public bool Exists(string s)
    {
        if (s == null)
            return false;

        return strings.ContainsKey(s);
    }
    public bool All(IList<string> list)
    {
        foreach (string s in list)
            if (!Exists(s))
                return false;

        return true;
    }
    public bool Exactly(IList<string> list)
    {
        if (!(list.Count == strings.Count))
            return false;
        return All(list);
    }
    public int Count(string s)
    {
        strings.TryGetValue(s, out int count);
        return count;
    }
}
public class FieldTypes : StringCounts
{
    private void Initialize(IEnumerable<FieldDef> fields)
    {
        if (fields == null)
            return;

        foreach(FieldDef field in fields)
        {
            TypeSig type = field.FieldSig.GetFieldType();
            if (type != null)
                Add(type.FullName);
        }
    }

    public FieldTypes(TypeDef type) => Initialize(type.Fields);
    public FieldTypes(IEnumerable<FieldDef> fields) => Initialize(fields);
}
public class LocalTypes : StringCounts
{
    private void Initialize(IEnumerable<Local> locals)
    {
        if (locals == null)
            return;

        foreach (Local local in locals)
            Add(local.Type.FullName);
    }

    public LocalTypes(MethodDef method)
    {
        if (method != null && method.Body != null)
            Initialize(method.Body.Variables);
    }
    public LocalTypes(IEnumerable<Local> locals) => Initialize(locals);
}

public static class GLOBALS
{
    public static string CurrentFile = "";
    public static byte[] CachedDecryptionKey;
}