using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AgileDotNet.Features
{
    public class StringDecryptor
    {
        private string m_File;

        private TypeDef AgileDotNetRT;

        private byte[] DecryptionKey;

        private string CachedMethod = null;

        private void FindAndSetKey()
        {
            FieldDef field = null;
            foreach(FieldDef field2 in AgileDotNetRT.Fields)
            {
                if (field2.Name.EndsWith("="))
                {
#if (DEBUG)
                    Console.WriteLine("Found decryption key");
#endif
                    field = field2;
                    break;
                }
            }

            if(field is null)
            {
                Console.WriteLine("Failed to find decryption key!");
                Thread.Sleep(2500);
                throw new Exception("Failed to find decryption key please esure that the version of the obfuscator is above 6.3");
            }

            if (GLOBALS.CachedDecryptionKey != null)
            {
                this.DecryptionKey = GLOBALS.CachedDecryptionKey;
                return;
            }

            this.DecryptionKey = field.InitialValue; // Set's the decryption key to the one found in the assembly

            if (this.DecryptionKey is null)
            {
                Assembly a = Assembly.UnsafeLoadFrom(m_File);

                this.DecryptionKey = a.ManifestModule.ResolveField(field.MDToken.ToInt32()).GetValue(null) as byte[];
            }

            GLOBALS.CachedDecryptionKey = this.DecryptionKey;
        }

        public StringDecryptor(AssemblyDef asm, string file)
        {
            this.m_File = file;
            foreach(ModuleDef module in asm.Modules)
            {
                foreach(TypeDef @class in module.GetTypes())
                {
                    if(@class.Name.ToLower() == "<agiledotnetrt>") //Doing .ToLower in case they will add some Name bullshit 
                    {
#if (DEBUG)
                        Console.WriteLine("Found <AgileDotNetRT> class");
#endif
                        AgileDotNetRT = @class;
                        break;
                    }
                }
            }

            if(AgileDotNetRT is null)
            {
                throw new Exception("Did not find AgileDotNetRT");
            }

            FindAndSetKey();
        }

        public string Decrypt(string value)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                for(int i = 0; i < value.Length; i++)
                {
                    sb.Append((char)(value[i] ^ (char)DecryptionKey[i % DecryptionKey.Length]));
                }
                return sb.ToString();
            }catch(Exception e) { throw e; }
        }
        public string GetMethod()
        {
            if (!(CachedMethod is null))
                return CachedMethod;

            foreach (MethodDef method in AgileDotNetRT.Methods)
            {
                if (!method.IsConstructor)
                {
#if (DEBUG)
                    Console.WriteLine("Found the method get function '"+method.Name+"'");
#endif
                    CachedMethod = method.Name;
                    return method.Name;
                }
            }

            return "oRM=";
        }
    }
}
