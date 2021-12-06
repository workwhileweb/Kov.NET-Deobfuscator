using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckExterminator
{
    public class DeobfuscatorContext
    {
        public static ModuleDefMD Module
        {
            get;
            set;
        }
        public static System.Reflection.Assembly Asm
        {
            get;set;

        }
    }
}
