using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using dnlib.DotNet;

namespace AgileDotNet.Features
{
    public class AsmEditor
    {
        private struct CustomData
        {
            public bool HasData;
            public string CustomName;
        }
        public struct CustomTypeName
        {
            public TypeDef Class;
            public string CSName;
        }

        private static List<char> BlackListedChars = new List<char>
        {
            /*"<",
            ">",*/
            '=',
            '/'
        };
        private static List<char> BlackListedStartingChars = new List<char>
        {
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9'
        };
        private static Dictionary<char, char> CharsToStr = new Dictionary<char, char>
        {
            {'0', 'a'},
            {'1', 'b'},
            {'2', 'c'},
            {'3', 'd'},
            {'4', 'e'},
            {'5', 'f'},
            {'6', 'g'},
            {'7', 'h'},
            {'8', 'i'},
            {'9', 'j'}
        };

        private Dictionary<TypeDef, string> CustomNames = new Dictionary<TypeDef, string>();

        private AssemblyDef asmDef;
        private Assembly asm;

        private bool HasBlackListedChar(string str)
        {
            if (str.Length > 0 && BlackListedStartingChars.Contains(str[0]))
                return true;

            for (int i = 0;i < str.Length; i++){
                if (BlackListedChars.Contains(str[i]))
                    return true;
            }

            return false;
        }
        private string GetValidString(string str)
        {
            if(str.Length > 0 && BlackListedStartingChars.Contains(str[0]))
            {
                string before = str.Substring(1);
                string thing = CharsToStr[str[0]].ToString();

                str = thing + before;
            }

            string valid = "";

            for (int i = 0; i < str.Length; i++)
            {
                if (!BlackListedChars.Contains(str[i]))
                    valid += str[i];
            }

            return valid == "" ? "NotValid"+new Random().Next(1,10000) : valid;
        }
        private CustomData GetCSData(TypeDef t)
        {
            CustomData cd;

            cd.HasData = false;
            cd.CustomName = null;

            if (CustomNames.TryGetValue(t, out string s))
            {
                cd.HasData = true;
                cd.CustomName = s;
            }

            return cd;
        }

        private void Add(ref Dictionary<string,int> dict, string name)
        {
            dict.TryGetValue(name, out int i);
            dict[name] = i + 1;
        }

        public AsmEditor(AssemblyDef asm, Assembly asm2)
        {
            asmDef = asm;
            this.asm = asm2;
        }

        public void SetCustomNames(List<CustomTypeName> list)
        {
            foreach (CustomTypeName ctn in list)
                CustomNames.Add(ctn.Class, ctn.CSName);
        }
        public Dictionary<string,int> FixNames()
        {
            Dictionary<string, int> edited = new Dictionary<string, int>();

            foreach (ModuleDef module in asmDef.Modules)
            {
                foreach(TypeDef type in module.Types)
                {
                    foreach(FieldDef field in type.Fields)
                    {
                        if (HasBlackListedChar(field.Name))
                        {
                            field.Name = GetValidString(field.Name);
                            Add(ref edited, "fields");
                        }
                    }
                    foreach(MethodDef method in type.Methods)
                    {
                        if (HasBlackListedChar(method.Name))
                        {
                            method.Name = GetValidString(method.Name);
                            Add(ref edited, "methods");
                        }
                    }
                    foreach(PropertyDef prop in type.Properties)
                    {
                        if(HasBlackListedChar(prop.Name))
                        {
                            prop.Name = GetValidString(prop.Name);
                            Add(ref edited, "properties");
                        }
                    }

                    CustomData cd = GetCSData(type);

                    if (cd.HasData)
                    {
                        type.Name = GetValidString(cd.CustomName);
                        Add(ref edited, "custom_classes");
                    }else if (HasBlackListedChar(type.Name))
                    {
                        type.Name = GetValidString(type.Name);
                        Add(ref edited, "classes");
                    }

                    if (HasBlackListedChar(type.Namespace))
                    {
                        type.Namespace = GetValidString(type.Namespace);
                        Add(ref edited, "namespaces");
                    }
                }
            }

            return edited;
        }
    }
}
