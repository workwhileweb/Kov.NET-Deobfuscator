using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckExterminator.Protections
{
    internal class ProxyInt
    {
        private static ModuleDefMD Module = DeobfuscatorContext.Module;

        public static void Fix()
        {
            int Fixed = 0;
            foreach(var type in Module.GetTypes())
            {
                foreach(var method in type.Methods)
                {
                    if (!method.HasBody || !method.Body.HasInstructions)
                        continue;

                    var instr = method.Body.Instructions;

                    for (int i = 0; i < instr.Count; i++)
                    {
                        if(instr[i].OpCode == OpCodes.Call)
                        {
                            var op = instr[i].Operand as MethodDef;

                            if (op == null)
                                continue;
                            if (op.ReturnType != Module.CorLibTypes.Int32)
                                continue;
                            if (!isIntProxy(op))
                                continue;
                            instr[i].OpCode = OpCodes.Ldc_I4;
                            instr[i].Operand = op.Body.Instructions[0].GetLdcI4Value();
                            op.DeclaringType.Remove(op);
                            Fixed++;
                        }
                    }
                }
            }
            Console.WriteLine("Int Proxy Fixed: " + Fixed.ToString());
        }

        private static bool isIntProxy(MethodDef Method)
        {
            if (Method.Body.Instructions[0].IsLdcI4() && Method.Body.Instructions[1].OpCode == OpCodes.Ret)
                return true;
            return false;
        }
    }
}
