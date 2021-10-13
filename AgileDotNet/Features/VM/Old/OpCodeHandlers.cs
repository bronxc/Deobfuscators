using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileDotNet.Features.VM.Old
{
    public static partial class OpCodeHandlers
    {
        public static readonly OpCodeHandler[][] Handlers = new OpCodeHandler[][]
        {
            new OpCodeHandler[]{
                new OpCodeHandler
                {
                    Name = "arithmetic",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            FieldsInfo.EnumType
                        },
                        StaticMethods = 0,
                        InstanceMethods = 14,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = arithmetic_read
                },
                new OpCodeHandler
                {
                    Name = "newarr",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Object",
                            "System.Int32",
                            "System.Type",
                            "System.IntPtr"
                        },
                        ExecuteMethodThrows = 0,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = newarr_check,
                    Read = newarr_read
                },
                new OpCodeHandler
                {
                    Name = "box/unbox",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            FieldsInfo.EnumType,
                            "System.UInt32"
                        },
                        StaticMethods = 0,
                        InstanceMethods = 2,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = box_read
                },
                new OpCodeHandler
                {
                    Name = "Call",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Collections.Generic.Dictionary`2<System.String,System.Int32>",
                            "System.Collections.Generic.Dictionary`2<System.Reflection.MethodInfo,System.Reflection.Emit.DynamicMethod>",
                            "System.Reflection.MethodBase",
                            "System.UInt32",
                            FieldsInfo.EnumType
                        },
                        StaticMethods = 2,
                        InstanceMethods = 4,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = call_read
                },
                new OpCodeHandler
                {
                    Name = "cast",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32",
                            FieldsInfo.EnumType
                        },
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = cast_read
                },
                new OpCodeHandler
                {
                    Name = "compare",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32",
                            FieldsInfo.EnumType
                        },
                        StaticMethods = 1,
                        InstanceMethods = 7,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = compare_read
                },
                new OpCodeHandler
                {
                    Name = "convert",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            FieldsInfo.EnumType,
                            "System.Boolean",
                            "System.Boolean"
                        },
                        StaticMethods = 0,
                        InstanceMethods = 13,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = convert_read
                },
                new OpCodeHandler
                {
                    Name = "dup/pop",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            FieldsInfo.EnumType
                        },
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = dup_read
                },
                new OpCodeHandler
                {
                    Name = "ldelem/stelem",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Boolean",
                            "System.Boolean",
                            FieldsInfo.EnumType,
                            "System.UInt32"
                        },
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldelem_read
                },
                new OpCodeHandler
                {
                    Name = "endfinally",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[0],
                        ExecuteMethodThrows = 2,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = endfinally_check,
                    Read = endfinally_read
                },
                new OpCodeHandler
                {
                    Name = "load/store field",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32",
                            FieldsInfo.EnumType
                        },
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldfld_read
                },
                new OpCodeHandler
                {
                    Name = "initobj",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Type"
                        },
                        ExecuteMethodThrows = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = initobj_read
                },
                new OpCodeHandler
                {
                    Name = "load local/arg",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Boolean",
                            "System.UInt16"
                        },
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldloc_read
                },
                new OpCodeHandler
                {
                    Name = "load local/arg address",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Boolean",
                            "System.UInt32"
                        },
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldloca_read
                },
                new OpCodeHandler
                {
                    Name = "ldelema",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[0],
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Int32",
                            "System.Array"
                        },
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldelema_read
                },
                new OpCodeHandler
                {
                    Name = "ldlen",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[0],
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Array",
                            "System.Object"
                        },
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldlen_read
                },
                new OpCodeHandler
                {
                    Name = "ldobj",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[0],
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldobj_read
                },
                new OpCodeHandler
                {
                    Name = "ldstr",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.String"
                        },
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldstr_read
                },
                new OpCodeHandler
                {
                    Name = "ldtoken",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Object",
                            "System.Reflection.MemberInfo"
                        },
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1,
                    },
                    Check = ldtoken_check,
                    Read = ldtoken_read
                },
                new OpCodeHandler
                {
                    Name = "leave",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Int32"
                        },
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = leave_check,
                    Read = leave_read
                },
                new OpCodeHandler
                {
                    Name = "load constant",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Object"
                        },
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldc_read
                },
                new OpCodeHandler
                {
                    Name = "load func",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            FieldsInfo.EnumType,
                            "System.UInt32",
                            "System.UInt32"
                        },
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldftn_read
                },
                new OpCodeHandler
                {
                    Name = "logical",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            FieldsInfo.EnumType
                        },
                        StaticMethods = 0,
                        InstanceMethods = 6,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = logical_read
                },
                new OpCodeHandler
                {
                    Name = "nop",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[0],
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = nop_check,
                    Read = nop_read
                },
                new OpCodeHandler
                {
                    Name = "ret",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32"
                        },
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = ret_check,
                    Read = ret_read
                },
                new OpCodeHandler
                {
                    Name = "rethrow",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[0],
                        ExecuteMethodThrows = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = rethrow_check,
                    Read = rethrow_read
                },
                new OpCodeHandler
                {
                    Name = "store local/arg",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Boolean",
                            "System.UInt32",
                            FieldsInfo.EnumType
                        },
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = stloc_read
                },
                new OpCodeHandler
                {
                    Name = "stobj",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[0],
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 2,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = stobj_read
                },
                new OpCodeHandler
                {
                    Name = "switch",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32",
                            "System.Int32[]"
                        },
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = switch_read
                },
                new OpCodeHandler
                {
                    Name = "throw",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[0],
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Object"
                        },
                        ExecuteMethodThrows = 2,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = throw_check,
                    Read = throw_read
                },
                new OpCodeHandler
                {
                    Name = "neg/not",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            FieldsInfo.EnumType
                        },
                        StaticMethods = 0,
                        InstanceMethods = 2,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = neg_read
                }
            },
            new OpCodeHandler[]
            {
                new OpCodeHandler
                {
                    Name = "arithmetic",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            FieldsInfo.EnumType,
                            "System.Boolean"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Object",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 2,
                        StaticMethods = 0,
                        InstanceMethods = 14,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = arithmetic_read
                },
                new OpCodeHandler
                {
                    Name = "box/unbox",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            FieldsInfo.EnumType,
                            "System.UInt32",
                            FieldsInfo.EnumType
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Object",
                            "System.Type",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 2,
                        StaticMethods = 0,
                        InstanceMethods = 2,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = box_read
                },
                new OpCodeHandler
                {
                    Name = "call",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Collection.Generic.Dictionary`2<System.String,System.Int32>",
                            "System.Collection.Generic.Dictionary`2<System.Reflection.MethodInfo,System.Reflection.Emit.DynamicMethod>",
                            "System.Reflection.MethodBase",
                            "System.UInt32",
                            FieldsInfo.EnumType,
                            FieldsInfo.EnumType,
                            "System.Boolean"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Boolean",
                            "System.Object",
                            "System.Reflection.ParameterInfo[]",
                            "System.Int32",
                            "System.Object[]",
                            "System.Reflection.ConstructorInfo",
                            "System.Reflection.MethodInfo"
                        },
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 2,
                        StaticMethods = 2,
                        InstanceMethods = 4,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = call_read
                },
                new OpCodeHandler
                {
                    Name = "cast",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32",
                            FieldsInfo.EnumType,
                            "System.Reflection.MethodBase"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Type",
                            "System.Object",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 1,
                        VirtualMethods = 2,
                        Ctors = 2
                    },
                    Check = null,
                    Read = cast_read
                },
                new OpCodeHandler
                {
                    Name = "compare",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Int32",
                            FieldsInfo.EnumType
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Int32",
                            "System.Object",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 2,
                        StaticMethods = 1,
                        InstanceMethods = 7,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = compare_read
                },
                new OpCodeHandler
                {
                    Name = "convert",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            FieldsInfo.EnumType,
                            "System.Boolean",
                            "System.Boolean",
                            "System.UInt32"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Object",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 13,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = convert_read
                },
                new OpCodeHandler
                {
                    Name = "dup/pop",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            FieldsInfo.EnumType
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = dup_read
                },
                new OpCodeHandler
                {
                    Name = "endfinally",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Int32"
                        },
                        ExecuteMethodThrows = 2,
                        ExecuteMethodPops = 0,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = endfinally_check,
                    Read = endfinally_read
                },
                new OpCodeHandler
                {
                    Name = "initobj",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32",
                            "System.Boolean"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Type",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = initobj_read
                },
                new OpCodeHandler
                {
                    Name = "ldelem/stelem",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Boolean",
                            "System.Boolean",
                            FieldsInfo.EnumType,
                            "System.UInt32"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Int32",
                            "System.Array",
                            "System.Object",
                            "System.Type",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 5,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldelem_read
                },
                new OpCodeHandler
                {
                    Name = "ldelema",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[0],
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Int32",
                            "System.Array"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 2,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldelema_read
                },
                new OpCodeHandler
                {
                    Name = "ldlen",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[0],
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Array",
                            "System.Object",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldlen_read
                },
                new OpCodeHandler
                {
                    Name = "ldobj",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[0],
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldobj_read
                },
                new OpCodeHandler
                {
                    Name = "ldstr",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.String"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 0,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldstr_read
                },
                new OpCodeHandler
                {
                    Name = "ldtoken",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Object",
                            "System.Reflection.MemberInfo",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 0,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = ldtoken_check,
                    Read = ldtoken_read
                },
                new OpCodeHandler
                {
                    Name = "leave",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Int32"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Int32",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 0,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = leave_check,
                    Read = leave_read
                },
                new OpCodeHandler
                {
                    Name = "load constant",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Object",
                            "System.Boolean",
                            "System.UInt16"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldc_read
                },
                new OpCodeHandler
                {
                    Name = "load func",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            FieldsInfo.EnumType,
                            "System.UInt32",
                            "System.UInt32"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Reflection.MethodBase",
                            "System.IntPtr",
                            "System.Type",
                            "System.Delegate",
                            "System.RuntimeMethodHandle"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldftn_read
                },
                new OpCodeHandler
                {
                    Name = "load local/arg",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Boolean",
                            "System.UInt16",
                            "System.Boolean"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 0,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldloc_read
                },
                new OpCodeHandler
                {
                    Name = "load local/arg address",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Boolean",
                            "System.UInt32"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Array",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 0,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldloca_read
                },
                new OpCodeHandler
                {
                    Name = "load/store field",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32",
                            FieldsInfo.EnumType
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Reflection.FieldInfo",
                            "System.Object",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 4,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldfld_read
                },
                new OpCodeHandler
                {
                    Name = "logical",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            FieldsInfo.EnumType,
                            FieldsInfo.EnumType
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Object",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 2,
                        StaticMethods = 0,
                        InstanceMethods = 6,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = logical_read
                },
                new OpCodeHandler
                {
                    Name = "neg/not",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            FieldsInfo.EnumType
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Object"
                        },
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 2,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = neg_read
                },
                new OpCodeHandler
                {
                    Name = "newarr",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Object",
                            "System.Int32",
                            "System.Type",
                            "System.Boolean",
                            "System.IntPtr"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = newarr_check,
                    Read = newarr_read
                },
                new OpCodeHandler
                {
                    Name = "nop",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[0],
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 0,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = nop_check,
                    Read = nop_read
                },
                new OpCodeHandler
                {
                    Name = "ret",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Reflection.MethodInfo",
                            "System.Type",
                            "System.Object",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = ret_check,
                    Read = ret_read
                },
                new OpCodeHandler
                {
                    Name = "rethrow",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[0],
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 0,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = rethrow_check,
                    Read = rethrow_read
                },
                new OpCodeHandler
                {
                    Name = "stobj",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[0],
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 2,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = stobj_read
                },
                new OpCodeHandler
                {
                    Name = "store local/arg",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Boolean",
                            "System.UInt16",
                            FieldsInfo.EnumType
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Object",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = stloc_read
                },
                new OpCodeHandler
                {
                    Name = "switch",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32",
                            "System.Int32[]"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Int32",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = switch_read
                },
                new OpCodeHandler
                {
                    Name = "throw",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[0],
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Object",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 2,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = throw_check,
                    Read = throw_read
                }
            },
            new OpCodeHandler[]
            {
                new OpCodeHandler
                {
                    Name = "arithmetic",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            FieldsInfo.EnumType,
                            "System.Double"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Object",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 2,
                        StaticMethods = 0,
                        InstanceMethods = 14,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = arithmetic_read
                },
                new OpCodeHandler
                {
                    Name = "box/unbox",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            FieldsInfo.EnumType,
                            "System.UInt32",
                            FieldsInfo.EnumType
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Object",
                            "System.Type",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 2,
                        StaticMethods = 0,
                        InstanceMethods = 2,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = box_read
                },
                new OpCodeHandler
                {
                    Name = "call",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Collections.Generic.Dictionary`2<System.Reflection.Module,System.Collections.Generic.Dictionary`2<System.Int32,System.Reflection.MethodBase>>",
                            "System.Collections.Generic.Dictionary`2<System.String,System.Int32>",
                            "System.Collections.Generic.Dictionary`2<System.Reflection.MethodInfo,System.Reflection.Emit.DynamicMethod>",
                            "System.Reflection.MethodBase",
                            "System.UInt32",
                            FieldsInfo.EnumType,
                            FieldsInfo.EnumType,
                            "System.Boolean"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Boolean",
                            "System.Reflection.Module",
                            "System.Collections.Generic.Dictionary`2<System.Int32,System.Reflection.MethodBase>",
                            "System.Object",
                            "System.Reflection.ParameterInfo[]",
                            "System.Int32",
                            "System.Object[]",
                            "System.Reflection.ConstructorInfo",
                            "System.Reflection.MethodInfo"
                        },
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 2,
                        StaticMethods = 2,
                        InstanceMethods = 4,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = call_read
                },
                new OpCodeHandler
                {
                    Name = "cast",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32",
                            FieldsInfo.EnumType,
                            "System.Reflection.MethodBase"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Type",
                            "System.Object",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = cast_read
                },
                new OpCodeHandler
                {
                    Name = "compare",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Int32",
                            FieldsInfo.EnumType
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Int32",
                            "System.Object",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 2,
                        StaticMethods = 1,
                        InstanceMethods = 7,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = compare_read
                },
                new OpCodeHandler
                {
                    Name = "convert",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            FieldsInfo.EnumType,
                            "System.Boolean",
                            "System.Boolean",
                            "System.UInt16"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Object",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 13,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = convert_read
                },
                new OpCodeHandler
                {
                    Name = "dup/pop",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            FieldsInfo.EnumType
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = dup_read
                },
                new OpCodeHandler
                {
                    Name = "endfinally",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Int32"
                        },
                        ExecuteMethodThrows = 2,
                        ExecuteMethodPops = 0,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = endfinally_check,
                    Read = endfinally_read
                },
                new OpCodeHandler
                {
                    Name = "initobj",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32",
                            "System.Double"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Type",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = initobj_read
                },
                new OpCodeHandler
                {
                    Name = "ldelem/stelem",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Boolean",
                            "System.Boolean",
                            FieldsInfo.EnumType,
                            "System.UInt32"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Int32",
                            "System.Array",
                            "System.Object",
                            "System.Type",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 5,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldelem_read
                },
                new OpCodeHandler
                {
                    Name = "ldelema",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[0],
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Int32",
                            "System.Array"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 2,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldelema_read
                },
                new OpCodeHandler
                {
                    Name = "ldlen",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                       RequiredFieldTypes = new object[0],
                       ExecuteMethodLocals = new string[]
                       {
                           "System.Array",
                           "System.Object",
                           "System.Boolean"
                       },
                       ExecuteMethodThrows = 0,
                       ExecuteMethodPops = 1,
                       StaticMethods = 0,
                       InstanceMethods = 0,
                       VirtualMethods = 2,
                       Ctors = 1
                    },
                    Check = null,
                    Read = ldlen_read
                },
                new OpCodeHandler
                {
                    Name = "ldobj",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[0],
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldobj_read
                },
                new OpCodeHandler
                {
                    Name = "ldstr",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.String"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 0,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldstr_read
                },
                new OpCodeHandler
                {
                    Name = "ldtoken",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Object",
                            "System.Reflection.MemberInfo",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 0,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = ldtoken_check,
                    Read = ldtoken_read
                },
                new OpCodeHandler
                {
                    Name = "leave",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Int32"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Int32",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 0,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = leave_check,
                    Read = leave_read
                },
                new OpCodeHandler
                {
                    Name = "load constant",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Object",
                            "System.Double",
                            "System.UInt16"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldc_read
                },
                new OpCodeHandler
                {
                    Name = "load func",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            FieldsInfo.EnumType,
                            "System.UInt32",
                            "System.UInt32"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Reflection.MethodBase",
                            "System.IntPtr",
                            "System.Type",
                            "System.Delegate",
                            "System.RuntimeMethodHandle"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldftn_read
                },
                new OpCodeHandler
                {
                    Name = "load local/arg",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Boolean",
                            "System.UInt16",
                            "System.UInt16"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 0,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldloc_read
                },
                new OpCodeHandler
                {
                    Name = "load local/arg address",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Boolean",
                            "System.UInt32"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Array",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 0,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldloca_read
                },
                new OpCodeHandler
                {
                    Name = "load/store field",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32",
                            FieldsInfo.EnumType
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Reflection.FieldInfo",
                            "System.Object",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 4,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = ldfld_read
                },
                new OpCodeHandler
                {
                    Name = "logical",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            FieldsInfo.EnumType,
                            FieldsInfo.EnumType
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Object",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 2,
                        StaticMethods = 0,
                        InstanceMethods = 6,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = logical_read
                },
                new OpCodeHandler
                {
                    Name = "neg/not",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            FieldsInfo.EnumType
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Object"
                        },
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 2,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = neg_read
                },
                new OpCodeHandler
                {
                    Name = "newarr",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Object",
                            "System.Int32",
                            "System.Type",
                            "System.Boolean",
                            "System.IntPtr"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = newarr_check,
                    Read = newarr_read
                },
                new OpCodeHandler
                {
                    Name = "nop",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[0],
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 0,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = nop_check,
                    Read = nop_read
                },
                new OpCodeHandler
                {
                    Name = "ret",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Reflection.MethodInfo",
                            "System.Type",
                            "System.Object",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = ret_check,
                    Read = ret_read
                },
                new OpCodeHandler
                {
                    Name = "rethrow",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[0],
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 0,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = rethrow_check,
                    Read = rethrow_read
                },
                new OpCodeHandler
                {
                    Name = "stobj",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[0],
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 1,
                        ExecuteMethodPops = 2,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = stobj_read
                },
                new OpCodeHandler
                {
                    Name = "store local/arg",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.Boolean",
                            "Sysetm.UInt16",
                            FieldsInfo.EnumType
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Object",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = stloc_read
                },
                new OpCodeHandler
                {
                    Name = "switch",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[]
                        {
                            "System.UInt32",
                            "System.Int32[]"
                        },
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Int32",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 0,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = null,
                    Read = switch_read
                },
                new OpCodeHandler
                {
                    Name = "throw",
                    OpCodeHandlerSigInfo = new OpCodeHandlerSigInfo
                    {
                        RequiredFieldTypes = new object[0],
                        ExecuteMethodLocals = new string[]
                        {
                            "System.Object",
                            "System.Boolean"
                        },
                        ExecuteMethodThrows = 2,
                        ExecuteMethodPops = 1,
                        StaticMethods = 0,
                        InstanceMethods = 0,
                        VirtualMethods = 2,
                        Ctors = 1
                    },
                    Check = throw_check,
                    Read = throw_read
                },
            }
        };
    }
}
