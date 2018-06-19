﻿using System.Collections.Generic;
using System.Linq;

namespace KD.Dova.Generator.Definitions
{
    /// <summary>
    /// Contains all informations about single structure.
    /// </summary>
    internal class StructureDefinition : FileConvertable
    {
        public List<FieldDefinition> Fields { get; }
        public List<FunctionDefinition> Functions { get; }

        public StructureDefinition()
        {
            this.Fields = new List<FieldDefinition>();
            this.Functions = new List<FunctionDefinition>();
        }

        internal override void AddLibraries(List<string> fileLines)
        {
            fileLines.Add("using System.Runtime.InteropServices;");
            fileLines.Add("using System.Runtime.CompilerServices;");
        }

        internal override void AddMainContent(List<string> fileLines)
        {
            if (this.Functions.Count == 0)
            {
                fileLines.Add("    [StructLayout(LayoutKind.Sequential), NativeCppClass]");
            }

            fileLines.Add($"    internal unsafe struct { this.Name }");
            fileLines.Add("    {");

            if (this.Fields.Count > 0)
            {
                foreach (var fieldDef in this.Fields)
                {
                    string field = $"        public { fieldDef.Type }";

                    if (char.IsUpper(fieldDef.Type.ToCharArray()[0]) &&
                        !fieldDef.Type.Equals(AbstractGenerator.POINTER))
                    {
                        field += "*";
                    }

                    field += $" { fieldDef.Name };";

                    fileLines.Add(field);
                }
            }

            if (this.Functions.Count > 0)
            {

            }

            if (this.Functions.Count > 0)
            {
                fileLines.Add("");

                foreach (var funcDef in this.Functions)
                {
                    fileLines.Add($"        [UnmanagedFunctionPointer(CallingConvention.Winapi)]");
                    fileLines.Add($"        public delegate { funcDef.ToString() }");
                }
            }

            fileLines.Add("    }");
        }
    }
}
