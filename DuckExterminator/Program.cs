using dnlib.DotNet;
using dnlib.DotNet.Writer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DuckExterminator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Kov.NET Deobfuscator || by Yeetret";
            Console.ForegroundColor = ConsoleColor.Cyan;

            string Filename = "";
            try
            {
                Filename = args[0];
            }
            catch
            {
                Console.WriteLine("Drag and drop your file : ");
                Filename = Console.ReadLine().Replace("\"", "");
            }
           //lazy
            var _module = ModuleDefMD.Load(args[0]);
            var _assembly = System.Reflection.Assembly.LoadFile(Filename);
            DeobfuscatorContext.Asm = _assembly;
            DeobfuscatorContext.Module = _module;
            Protections.ProxyInt.Fix();
            Protections.LocalToFields.Fix(); //from miso
            Protections.SizeOfs.Fix();
            Protections.MathFixer.Fix(true); //from miso
            Protections.IfFlow.Fix(); //from Deob-DotNetPatcher
            Protections.Junk.Fix();
            Protections.StringEncryption.Fix();

            var options = new ModuleWriterOptions(_module);
            options.MetadataLogger = DummyLogger.NoThrowInstance;
            string path = Path.GetFileNameWithoutExtension(Filename) + "-Unpacked" + Path.GetExtension(Filename);
            _module.Write(path, options);
            Console.WriteLine("Saved to : " + path);
            Console.ReadKey();
        }
    }
}
