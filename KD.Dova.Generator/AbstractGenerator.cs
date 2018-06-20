using KD.Dova.Extensions;
using KD.Dova.Generator.Definitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KD.Dova.Generator
{
    public abstract class AbstractGenerator : IGenerator
    {
        internal static string POINTER = "IntPtr";
        internal static string JAVA_ARGS = "JavaVMInitArgs";

        internal void Generate(string[] lines, string lineBeginning, string lineEnding, Action<int, string> CheckLine)
        {
            for (int i = 0; i < lines.Length; ++i)
            {
                string line = lines[i];

                if (string.IsNullOrEmpty(lineEnding))
                {
                    if (line.StartsWith(lineBeginning))
                    {
                        CheckLine?.Invoke(i, line);
                    }
                }
                else
                {
                    if (line.StartsWith(lineBeginning) && line.EndsWith(lineEnding))
                    {
                        CheckLine?.Invoke(i, line);
                    }
                }
            }
        }

        internal string ParseImportName(string line)
        {
            return line.Split("(")[0];
        }

        internal void ParseConstant(FieldDefinition fieldDef, string line)
        {
            string parsed = line.Trim();
            string[] parts = parsed.Split(" ");

            fieldDef.Type = "int";
            fieldDef.Name = parts[1];

            if (fieldDef.Name.StartsWith("_"))
            {
                return;
            }

            for (int i = 2; i < parts.Length; ++i)
            {
                string value = parts[i];
                if (!string.IsNullOrEmpty(value))
                {
                    fieldDef.Value = value;
                    break;
                }
            }

            string comment = this.ParseInlineComment(line);
            fieldDef.Comment = comment;
        }

        private string ParseInlineComment(string line)
        {
            string[] parts = line.Split("/");
            foreach (string part in parts)
            {
                if (part.StartsWith("*")) // comment found
                {
                    string comment = part.Substring(1, part.Length - 1);
                    int index = comment.IndexOf("*");
                    comment = comment.Substring(0, index);
                    return comment;
                }
            }
            return string.Empty;
        }

        internal void ParseEnumValues(EnumDefinition def, string[] lines, int startIndex, string line)
        {
            int fieldIndex = startIndex + 1; // set to first property
            string fieldDef = lines[fieldIndex];

            while (!fieldDef.StartsWith("}"))
            {
                if (!string.IsNullOrEmpty(fieldDef))
                {
                    fieldDef = fieldDef.Trim();
                    fieldDef = fieldDef.RemoveSpaces();

                    string[] parts = fieldDef.Split("=");
                    EnumValueDefinition enumValue = new EnumValueDefinition
                    {
                        Name = parts[0],
                        Value = parts[1]
                    };
                    def.Values.Add(enumValue);
                }

                fieldIndex++;
                fieldDef = lines[fieldIndex];
            }

            if (fieldDef.StartsWith("}"))
            {
                fieldDef = fieldDef.RemoveSpaces();
                fieldDef = fieldDef.Substring(1, fieldDef.Length - 2);

                def.Name = fieldDef;
            }
        }

        internal string ParseStructureFields(StructureDefinition def, string[] lines, int startIndex)
        {
            int fieldIndex = startIndex + 1; // set to first property
            string fieldDef = lines[fieldIndex];
            bool isCppDefinitions = false;

            // Read structure properties.
            while (!fieldDef.StartsWith("}"))
            {
                if (fieldDef.Contains("#ifdef __cplusplus"))
                {
                    isCppDefinitions = true;
                }
                else if (fieldDef.Contains("#endif"))
                {
                    isCppDefinitions = false;
                }

                if (!string.IsNullOrEmpty(fieldDef) && !isCppDefinitions)
                {
                    string fieldType = this.ParseFieldType(fieldDef);
                    string fieldName = this.ParseFieldName(fieldDef);

                    if ((!string.IsNullOrEmpty(fieldType) && !fieldType.Contains("/") && !string.IsNullOrEmpty(fieldName)) &&
                        (!fieldName.Contains("(") && !fieldName.Contains(")")) &&
                        (!fieldDef.EndsWith(");")) &&
                        (!fieldDef.Contains("#")))
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
            bool isCppDefinitions = false;

            // Read structure function signatures.
            while (!funcDef.StartsWith("};"))
            {
                if (funcDef.Contains("#ifdef __cplusplus"))
                {
                    isCppDefinitions = true;
                }
                else if (funcDef.Contains("#endif"))
                {
                    isCppDefinitions = false;
                }

                if (!string.IsNullOrEmpty(funcDef) &&
                    funcDef.Contains("(") && // Fields won't have it.
                    !isCppDefinitions)
                {
                    FunctionDefinition func = new FunctionDefinition();
                    func.ReturnType = this.ParseFunctionReturnType(funcDef);
                    func.Name = this.ParseFunctionName(funcDef);

                    if (!this.MethodRegistered(func, def))
                    {
                        this.ParseFunctionParameters(func, funcIndex, lines, funcDef);

                        if (!func.ReturnType.Contains("(") &&
                            !string.IsNullOrEmpty(func.Name))
                        {
                            def.Functions.Add(func);
                        }
                    }

                    func.Clean();
                }

                funcIndex++;
                funcDef = lines[funcIndex];
            }

            return funcDef;
        }

        internal bool MethodRegistered(FunctionDefinition func, StructureDefinition def)
        {
            foreach (FunctionDefinition function in def.Functions)
            {
                if (func.Name.StartsWith(function.Name) && ((func.Name.Length - function.Name.Length) == 1))
                {
                    return true;
                }
            }

            return false;
        }

        internal void ParseFunctionParameters(FunctionDefinition func, int funcIndex, string[] lines, string funcDef)
        {
            funcDef = funcDef.Trim();

            if ((!funcDef.StartsWith("(") && (!funcDef.EndsWith(")"))) ||
                funcDef.EndsWith(");")) // Single-line function.
            {
                string[] splitted = funcDef.Split("(");

                funcDef = splitted.Last();

                funcDef = funcDef.Substring(0, funcDef.Length - 2);
                this.ParseFunctionParametersFromLine(func, funcDef);
            }
            else // Multi-line function
            {
                do
                {
                    funcIndex++;
                    funcDef = lines[funcIndex].Trim();

                    if (!funcDef.EndsWith(")"))
                    {
                        if (!funcDef.StartsWith("j"))
                        {
                            funcDef = funcDef.Substring(1, funcDef.Length - 1);
                        }

                        if (funcDef.EndsWith(","))
                        {
                            funcDef = funcDef.Substring(0, funcDef.Length - 1);
                        }

                        this.ParseFunctionParametersFromLine(func, funcDef);
                    }
                }
                while (!funcDef.EndsWith(");"));
            }
        }

        internal void ParseFunctionParametersFromLine(FunctionDefinition func, string parametersLine)
        {
            string[] parameters = parametersLine.Split(",");
            foreach (string param in parameters)
            {
                string trimmed = param.Trim();

                if (trimmed.StartsWith("("))
                {
                    trimmed = trimmed.Substring(1, trimmed.Length - 1);
                }

                string type = this.ParseFieldType(trimmed);
                string name = this.ParseFieldName(trimmed);

                if (param.Contains("**"))
                {
                    type = POINTER;
                }

                if (type.Contains("..."))
                {
                    type = "params NativeValue[]";
                    name = "args";
                }

                bool isOut = false;
                if (param.Contains("**") ||
                    name.StartsWith("p"))
                {
                    isOut = true;
                }

                FieldDefinition def = new FieldDefinition
                {
                    Name = name,
                    Type = type,
                    IsOut = isOut
                };
                func.Params.Add(def);
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

            if (funcDef.Contains(")(")) // Inline function definition
            {
                funcDef = funcDef.Split("*")[1];
                funcDef = funcDef.Split(")")[0];
                return funcDef;
            }
            else // Multi-line function
            {
                funcDef = funcDef.Split("*").LastOrDefault();

                if (string.IsNullOrEmpty(funcDef))
                {
                    return "";
                }

                if (funcDef.StartsWith("(") ||
                    !funcDef.Contains(")"))
                {
                    return "";
                }

                int funcNameEndIndex = funcDef.IndexOf(")");
                funcDef = funcDef.Substring(0, funcNameEndIndex);
                return funcDef;
            }
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
                string trimmed = line.Trim().RemoveSpaces();
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
                fieldName = fieldName.Split(" ").Last().Trim();
            }

            if (fieldName.Contains("*"))
            {
                fieldName = fieldName.Replace("*", "");
            }

            if (fieldName.EndsWith(";"))
            {
                fieldName = fieldName.Substring(0, fieldName.Length - 1);
            }

            if (fieldName.StartsWith("ref"))
            {
                fieldName = "reference";
            }
            else if (fieldName.StartsWith("string"))
            {
                fieldName = "str";
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
            string fieldType = line;

            if (!fieldType.Contains("const struct"))
            {
                fieldType = line.Trim();
            }
            else
            {
                int index = line.IndexOf("c");
                fieldType = fieldType.Substring(index, fieldType.Length - index);

                string[] parts = fieldType.Split(" ");
                fieldType = parts[2] + parts[3];
            }

            if (fieldType.Contains("*")) // Pointer
            {
                fieldType = fieldType.RemoveSpaces();

                string pointerType = fieldType.Split("*")[0];

                if (line.EndsWith("*args"))
                {
                    return "JavaVMInitArgs";
                }
                else if (pointerType.IsPrimitive() ||
                    (fieldType.StartsWith("JNIEnv") || (fieldType.StartsWith("JavaVM"))) ||
                    pointerType.StartsWith("j"))
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

        internal void GenerateFile(IFileConvertable convert)
        {
            List<string> lines = convert.ToFileDefinition();
            string path = Path.Combine(GeneratorFactory.Instance.OutputDirectory, $"{ convert.Name }.cs");

            File.WriteAllLines(path, lines.ToArray());
        }

        public abstract void Generate(string[] lines);
    }
}
