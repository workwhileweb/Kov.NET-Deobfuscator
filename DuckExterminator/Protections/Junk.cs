using de4dot.blocks;
using de4dot.blocks.cflow;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckExterminator.Protections
{
    internal class Junk
    {
        private static ModuleDefMD Module = DeobfuscatorContext.Module;
        public static void Cleaner(MethodDef method)
        {
            try
            {
                BlocksCflowDeobfuscator blocksCflowDeobfuscator = new BlocksCflowDeobfuscator();
                de4dot.blocks.Blocks blocks = new de4dot.blocks.Blocks(method);
                blocks.Method.Body.SimplifyBranches();
                blocks.Method.Body.OptimizeBranches();
                blocks.RemoveDeadBlocks();
                blocks.RepartitionBlocks();
                blocks.UpdateBlocks();
                blocksCflowDeobfuscator.Initialize(blocks);
                blocksCflowDeobfuscator.Deobfuscate();
                blocks.RepartitionBlocks();
                IList<Instruction> instructions;
                IList<ExceptionHandler> exceptionHandlers;
                blocks.GetCode(out instructions, out exceptionHandlers);
                DotNetUtils.RestoreBody(method, instructions, exceptionHandlers);
            }
            catch
            {

            }
        }
        public static void Fix()
        {
            int Fixed = 0;
            foreach (var type in Module.GetTypes())
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody || !method.Body.HasInstructions)
                        continue;

                    Cleaner(method);
                }
            }
        }
    }
}
