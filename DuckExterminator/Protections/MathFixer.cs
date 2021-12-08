using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckExterminator.Protections
{
    internal class MathFixer
    {
        private static ModuleDefMD asm = DeobfuscatorContext.Module;
        /// <summary>
        /// Get from since icba to make one myself : https://github.com/miso-xyz/DuckiKov/blob/main/Program.cs#L241
        /// </summary>
        /// <param name="fixCalls"></param>
        public static void Fix(bool fixCalls = true)
        {
            int Fixed = 0;
            foreach (TypeDef type in asm.Types)
            {
                foreach (MethodDef methods in type.Methods)
                {
                    for (int x = 0; x < methods.Body.Instructions.Count; x++)
                    {
                        Instruction inst = methods.Body.Instructions[x];
                        switch (inst.OpCode.Code)
                        {
                            case Code.Call:
                                if (fixCalls)
                                {
                                    if (inst.Operand is MemberRef)
                                    {
                                        switch (((MemberRef)inst.Operand).Name)
                                        {
                                            case "Sin":
                                                if (methods.Body.Instructions[x + 1].OpCode.Equals(OpCodes.Conv_I4))
                                                {
                                                    inst.OpCode = OpCodes.Ldc_I4;
                                                    inst.Operand = Convert.ToInt32(Math.Sin(Convert.ToDouble(methods.Body.Instructions[x - 1].Operand.ToString())));
                                                    methods.Body.Instructions.RemoveAt(x + 1);
                                                    Fixed++;
                                                }
                                                else
                                                {
                                                    inst.OpCode = OpCodes.Ldc_R8;
                                                    inst.Operand = Math.Sin(Convert.ToDouble(methods.Body.Instructions[x - 1].Operand.ToString())); Fixed++;
                                                }
                                                methods.Body.Instructions.RemoveAt(x - 1);
                                                x--;
                                                break;
                                            case "Cos":
                                                if (methods.Body.Instructions[x + 1].OpCode.Equals(OpCodes.Conv_I4))
                                                {
                                                    inst.OpCode = OpCodes.Ldc_I4;
                                                    inst.Operand = Convert.ToInt32(Math.Cos(Convert.ToDouble(methods.Body.Instructions[x - 1].Operand.ToString())));
                                                    methods.Body.Instructions.RemoveAt(x + 1); Fixed++;
                                                }
                                                else
                                                {
                                                    inst.OpCode = OpCodes.Ldc_R8;
                                                    inst.Operand = Math.Cos(Convert.ToDouble(methods.Body.Instructions[x - 1].Operand.ToString())); Fixed++;
                                                }
                                                methods.Body.Instructions.RemoveAt(x - 1);
                                                x--;
                                                break;

                                        }
                                    }
                                }
                                break;
                            case Code.Add:
                            case Code.Sub:
                            case Code.Mul:
                            case Code.Div:
                            case Code.Xor:
                            case Code.Rem:
                                int calculated = 0;
                                try
                                {
                                    calculated = Calculate(new Instruction[] { methods.Body.Instructions[x - 1], methods.Body.Instructions[x - 2] }, inst.OpCode.Code);
                                    methods.Body.Instructions.RemoveAt(x - 2);
                                    methods.Body.Instructions.RemoveAt(x - 2);
                                    inst.OpCode = OpCodes.Ldc_I4;
                                    inst.Operand = calculated;
                                    x -= 2; Fixed++;
                                }
                                catch { }
                                break;
                        }
                    }
                }
            }
            Console.WriteLine("Maths Fixed : " + Fixed);
        }
        public static int Calculate(Instruction[] insts, Code calcType)
        {
            switch (calcType)
            {
                case Code.Add:
                    return insts[0].GetLdcI4Value() + insts[1].GetLdcI4Value();
                case Code.Sub:
                    return insts[0].GetLdcI4Value() - insts[1].GetLdcI4Value();
                case Code.Mul:
                    return insts[0].GetLdcI4Value() * insts[1].GetLdcI4Value();
                case Code.Div:
                    return insts[0].GetLdcI4Value() / insts[1].GetLdcI4Value();
                case Code.Xor:
                    return insts[0].GetLdcI4Value() ^ insts[1].GetLdcI4Value();
                case Code.Rem:
                    return insts[0].GetLdcI4Value() % insts[1].GetLdcI4Value();
                default:
                    return int.MinValue;
            }
        }
    }
}
