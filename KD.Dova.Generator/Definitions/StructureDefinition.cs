using System.Collections.Generic;
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
            // Additional using for UnmanagedFunctionPointer
            if (this.Functions.Count > 0)
            {
                fileLines.Add("using System.Runtime.InteropServices;");
            }
        }

        internal override void AddMainContent(List<string> fileLines)
        {
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
        }
    }
}
