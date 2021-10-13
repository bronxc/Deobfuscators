using System;
using System.Collections.Generic;

using dnlib.DotNet;

namespace AgileDotNet.Features.VM.Old
{
    public class FieldsInfo
    {
        private Dictionary<string, int> fieldTypes = new Dictionary<string, int>(StringComparer.Ordinal);
        private int Enums = 0;

        public static readonly object EnumType = new object();

        private void Add(string typeName)
        {
            fieldTypes.TryGetValue(typeName, out int count);
            fieldTypes[typeName] = count + 1;
        }
        private void Add(TypeSig type) => Add(type.GetFullName());

        private void AddEnum() => Enums++;

        public FieldsInfo(IEnumerable<FieldDef> fields)
        {
            foreach(FieldDef field in fields)
            {
                TypeDef fieldTypeDef = field.FieldSig.GetFieldType().TryGetTypeDef();

                if (fieldTypeDef != null && fieldTypeDef.IsEnum)
                    AddEnum();
                else
                    Add(field.FieldSig.GetFieldType());
            }
        }
        public FieldsInfo(TypeDef typedef) : this(typedef.Fields) { }
        public FieldsInfo(object[] fieldTypes)
        {
            foreach(object o in fieldTypes)
            {
                if (o == EnumType)
                    AddEnum();
                else
                    Add((string)o);
            }
        }

        public bool IsSame(FieldsInfo other)
        {
            if (Enums != other.Enums)
                return false;
            if (fieldTypes.Count != other.fieldTypes.Count)
                return false;

            foreach(KeyValuePair<string,int> kv in fieldTypes)
            {
                if (!other.fieldTypes.TryGetValue(kv.Key, out int n))
                    return false;
                if (kv.Value != n)
                    return false;
            }

            return true;
        }
    }
}
