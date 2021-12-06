using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckExterminator.Protections
{
    internal class LocalToFields
    {
        private static ModuleDefMD Module = DeobfuscatorContext.Module;
        public static void Fix()
        {
            int Fixed = 0;
            Dictionary<string, TypeSig> fieldList = new Dictionary<string, TypeSig>();
            Dictionary<string, Local> fixedLocalList = new Dictionary<string, Local>();
            TypeDef GlobalType = Module.GlobalType;

            foreach (TypeDef type in Module.GetTypes())
            {
                foreach (FieldDef fields in type.Fields)
                {
                    if (!fields.IsStatic) { continue; }
                    fieldList.Add(fields.Name, fields.FieldSig.GetFieldType());
                }
                foreach (MethodDef methods in type.Methods)
                {
                    for (int x = 0; x < methods.Body.Instructions.Count; x++)
                    {
                        Instruction inst = methods.Body.Instructions[x];

                        switch (inst.OpCode.Code)
                        {
                            case Code.Ldsfld:
                            case Code.Stsfld:
                            case Code.Ldsflda:
                                if (inst.Operand is FieldDef)
                                {
                                    string fieldName = ((FieldDef)inst.Operand).Name;

                                    if (fieldList.ContainsKey(((FieldDef)inst.Operand).Name))
                                    {
                                        TypeSig temp_typeSig = null;

                                        fieldList.TryGetValue(fieldName, out temp_typeSig);

                                        Local fixedLocal = new Local(temp_typeSig, fieldName);

                                        methods.Body.Variables.Add(fixedLocal);

                                        GlobalType.Fields.Remove((FieldDef)inst.Operand);

                                        inst.OpCode = GetField(inst.OpCode.Code);

                                        if (!fixedLocalList.ContainsKey(fieldName))
                                        {
                                            inst.Operand = fixedLocal;
                                            fixedLocalList.Add(fieldName, fixedLocal);
                                        }
                                        else
                                        {
                                            inst.Operand = fixedLocalList[fieldName];
                                        }
                                        Fixed++;
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            Console.WriteLine("Fields to Locals Fixed : " + Fixed);
        }

        static OpCode GetField(Code OpCode)
        {
            switch (OpCode)
            {
                case Code.Stsfld:
                    return OpCodes.Stloc;
                case Code.Ldsfld:
                    return OpCodes.Ldloc;
                case Code.Ldsflda:
                    return OpCodes.Ldloca;
            }
            return null;
        }
    }
}
