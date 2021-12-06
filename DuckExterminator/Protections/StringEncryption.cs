using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckExterminator.Protections
{
    internal class StringEncryption
    {
        private static ModuleDefMD Module = DeobfuscatorContext.Module;
        public static void Fix()
        {
            int Fixed = 0;
            foreach (var type in Module.GetTypes())
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody || !method.Body.HasInstructions)
                        continue;


                    var instr = method.Body.Instructions;

                    for (int i = 0; i < instr.Count; i++)
                    {
                        try
                        {


                            if (instr[i].OpCode == OpCodes.Call && instr[i - 1].IsLdcI4() && instr[i - 2].OpCode == OpCodes.Ldstr)
                            {
                                instr[i].OpCode = OpCodes.Ldstr;
                                instr[i].Operand = Decrypt(instr[i - 2].Operand.ToString(), instr[i - 1].GetLdcI4Value());

                                instr[i - 1].OpCode = OpCodes.Nop;
                                instr[i - 2].OpCode = OpCodes.Nop;


                                Fixed++;
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
            Console.WriteLine("Strings Decrypted : "  + Fixed);
        }

            private static string Decrypt(string A_0, int A_1)
            {
                StringBuilder stringBuilder = new StringBuilder(A_0);
                StringBuilder stringBuilder2 = new StringBuilder(A_0.Length);
                for (int i = 0; i < A_0.Length; i++)
                {
                    char c = stringBuilder[i];
                    c = (char)((int)c ^ A_1);
                    stringBuilder2.Append(c);
                }
                return stringBuilder2.ToString();
            }
        }

    }
