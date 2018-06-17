using KD.Dova.Extensions;
using KD.Dova.Generator.Definitions;
using System;
using System.Collections.Generic;
using System.IO;

namespace KD.Dova.Generator
{
    public abstract class AbstractGenerator : IGenerator
    {
        internal static string POINTER = "IntPtr";

        internal string ParseStructureFields(StructureDefinition def, string[] lines, int startIndex)
        {
            int fieldIndex = startIndex + 1; // set to first property
            string fieldDef = lines[fieldIndex];

            // Read structure properties.
            while (!fieldDef.StartsWith("}"))
            {
                if (!string.IsNullOrEmpty(fieldDef))
                {
                    string fieldType = this.ParseFieldType(fieldDef);
                    string fieldName = this.ParseFieldName(fieldDef);

                    if ((!string.IsNullOrEmpty(fieldType) && !fieldType.Contains("/") && !string.IsNullOrEmpty(fieldName)) &&
                        (!fieldName.Contains("(") && !fieldName.Contains(")")) &&
                        (!fieldDef.EndsWith(");")))
                    {
                        def.Fields.Add(new FieldDefinition { Type = fieldType, Name = fieldName });
                    }
                }

                fieldIndex++;
                fieldDef = lines[fieldIndex];
            }

            return fieldDef; // Returns last line which may contains structure name.
        }

        internal string ParseStructureFunctions(StructureDefinition def, string[] lines, int startIndex)
        {
            int funcIndex = startIndex + 1;
            string funcDef = lines[funcIndex];

            // Read structure function signatures.
            while (!funcDef.StartsWith("};"))
            {
                if (!string.IsNullOrEmpty(funcDef) &&
                    funcDef.Contains("(")) // Fields won't have it.
                {
                    FunctionDefinition func = new FunctionDefinition();
                    func.ReturnType = this.ParseFunctionReturnType(funcDef);
                    func.Name = this.ParseFunctionName(funcDef);

                    this.ParseFunctionParameters(func, funcIndex, lines, funcDef);

                    if (!func.ReturnType.Contains("(") &&
                        !string.IsNullOrEmpty(func.Name))
                    {
                        def.Functions.Add(func);
                    }
                }

                funcIndex++;
                funcDef = lines[funcIndex];
            }

            return funcDef;
        }

        private void ParseFunctionParameters(FunctionDefinition func, int funcIndex, string[] lines, string funcDef)
        {
            funcDef = funcDef.Trim();

            if (funcDef.EndsWith(");")) // Single-line function.
            {
                string[] splitted = funcDef.Split("(");

                if (splitted.Length > 2)
                {
                    funcDef = splitted[2];
                }
                else
                {
                    funcDef = splitted[1];
                }

                funcDef = funcDef.Substring(0, funcDef.Length - 2);

                string[] parameters = funcDef.Split(",");
                foreach (string param in parameters)
                {
                    string trimmed = param.Trim();
                    string type = this.ParseFieldType(trimmed);
                    string name = this.ParseFieldName(trimmed);

                    func.Params.Add(new FieldDefinition { Name = name, Type = type });
                }
            }
            else // Multi-line function
            {
            }
        }

        internal string ParseFunctionReturnType(string funcDef)
        {
            string trimmed = funcDef.Trim();
            string[] splitted = trimmed.Split(" ");

            string returnType = splitted[0];

            if (returnType.Equals("const"))
            {
                returnType = splitted[1];
            }

            if (returnType.StartsWith("j"))
            {
                return POINTER;
            }
            else if (returnType.IsPrimitive() && !returnType.StartsWith("const"))
            {
                return returnType;
            }
            else
            {
                return returnType;
            }
        }

        internal string ParseFunctionName(string line)
        {
            string funcDef = line.Trim();
            funcDef = funcDef.Split("*")[1];

            if (funcDef.StartsWith("(") ||
                !funcDef.Contains(")"))
            {
                return "";
            }

            int funcNameEndIndex = funcDef.IndexOf(")");
            funcDef = funcDef.Substring(0, funcNameEndIndex);
            return funcDef;
        }

        internal string ParseStructureName(string line)
        {
            string lineBeginning = "struct ";
            string lineEnding = " {";

            if (line.StartsWith(lineBeginning) && line.EndsWith(lineEnding))
            {
                line = line.Split(" ")[1];
                return line;
            }
            else
            {
                string trimmed = line.Trim().Replace(" ", "");
                trimmed = trimmed.Substring(1, trimmed.Length - 2);
                return trimmed;
            }
        }

        internal string ParseFieldName(string line)
        {
            string fieldName = line.Trim();

            if (string.IsNullOrEmpty(fieldName) ||
                line.Contains("("))
            {
                return string.Empty;
            }

            if (fieldName.Contains(" "))
            {
                fieldName = fieldName.Split(" ")[1].Trim();
            }

            if (fieldName.StartsWith("*"))
            {
                fieldName = fieldName.Substring(1);
            }

            if (fieldName.EndsWith(";"))
            {
                fieldName = fieldName.Substring(0, fieldName.Length - 1);
            }

            return fieldName;
        }

        /// <summary>
        /// This should return C# type.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        internal string ParseFieldType(string line)
        {
            string fieldType = line.Trim();

            if (fieldType.Contains("*")) // Pointer
            {
                fieldType = fieldType.Replace(" ", "");

                string pointerType = fieldType.Split("*")[0];

                if (pointerType.IsPrimitive())
                {
                    return POINTER;
                }
                else
                {
                    return pointerType;
                }
            }
            else if (fieldType.StartsWith("j")) // Java primitive
            {
                return POINTER;
            }
            else // C primitive
            {
                string primitiveType = fieldType.Split("*")[0];
                return primitiveType;
            }
        }

        internal void GenerateFile(StructureDefinition def)
        {
            List<string> lines = def.ToFileDefinition();
            string path = Path.Combine(GeneratorFactory.Instance.OutputDirectory, "Structures");
            path = Path.Combine(path, $"{ def.Name }.cs");

            File.WriteAllLines(path, lines.ToArray());
        }

        public abstract void Generate(string[] lines);
    }
}
