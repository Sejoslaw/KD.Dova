﻿using System.Collections.Generic;
using System.Linq;

namespace KD.Dova.Generator.Definitions
{
    /// <summary>
    /// Contains all informations about single structure.
    /// </summary>
    internal class StructureDefinition
    {
        public string Name { get; set; }
        public ICollection<FieldDefinition> Fields { get; }
        public ICollection<FunctionDefinition> Functions { get; }

        public StructureDefinition()
        {
            this.Fields = new List<FieldDefinition>();
            this.Functions = new List<FunctionDefinition>();
        }

        public List<string> ToFileDefinition()
        {
            List<string> fileLines = new List<string>();

            // File definition
            fileLines.Add("// This file has been machine generated.");
            fileLines.Add("// Generated by: KD.Dova.Generator");
            fileLines.Add("// For more information go to: https://github.com/Sejoslaw/KD.Dova");
            fileLines.Add("");
            fileLines.Add("");
            fileLines.Add("using System;");

            // Additional using for UnmanagedFunctionPointer
            if (this.Functions.Count > 0)
            {
                fileLines.Add("using System.Runtime.InteropServices;");
            }

            fileLines.Add("");
            fileLines.Add("namespace KD.Dova.Natives.Structures");
            fileLines.Add("{");
            fileLines.Add($"    internal unsafe struct { this.Name }");
            fileLines.Add("    {");

            if (this.Fields.Count > 0)
            {
                foreach (var fieldDef in this.Fields)
                {
                    fileLines.Add($"        public { fieldDef.Type } { fieldDef.Name };");
                }
            }

            if (this.Functions.Count > 0)
            {
                fileLines.Add("");

                var sorted = this.Functions.OrderBy(func => func.Name).ToList();
                foreach (var funcDef in sorted)
                {
                    fileLines.Add($"        [UnmanagedFunctionPointer(CallingConvention.Winapi)]");
                    fileLines.Add($"        public delegate { funcDef.ToString() }");
                }
            }

            fileLines.Add("    }");
            fileLines.Add("}");

            return fileLines;
        }
    }
}
