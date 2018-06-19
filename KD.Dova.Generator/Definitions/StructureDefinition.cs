using System.Collections.Generic;

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
            fileLines.Add("using KD.Dova;");
            fileLines.Add("using System.Security;");
            fileLines.Add("using System.Runtime.InteropServices;");
            fileLines.Add("using System.Runtime.CompilerServices;");
        }

        internal override void AddMainContent(List<string> fileLines)
        {
            if (this.Functions.Count > 0)
            {
                this.BuildDelegatesStructure(fileLines);
                fileLines.Add("");
            }

            this.BuildFunctionPointersStructure(fileLines);
        }

        private void BuildDelegatesStructure(List<string> fileLines)
        {
            fileLines.Add($"    internal unsafe struct { this.Name }");
            fileLines.Add("    {");

            if (this.Functions.Count > 0) // Functions
            {
                foreach (FunctionDefinition funcDef in this.Functions)
                {
                    fileLines.Add($"        [UnmanagedFunctionPointer(CallingConvention.Winapi)]");

                    if (this.Name.Equals("JNIInvokeInterface_")) // Special attribute for this structure
                    {
                        fileLines.Add("        [SuppressUnmanagedCodeSecurity]");
                    }

                    fileLines.Add($"        public delegate { funcDef.ToString() }");
                    fileLines.Add("");
                }
            }

            fileLines.Add("    }");
        }

        private void BuildFunctionPointersStructure(List<string> fileLines)
        {
            if (this.Functions.Count > 0)
            {
                fileLines.Add("    [StructLayout(LayoutKind.Sequential), NativeCppClass]");
            }

            string name = this.Name;

            if (name.EndsWith("_") &&
                (!name.Equals("JavaVM_") && !name.Equals("JNIEnv_"))) // Java specific structures
            {
                name = this.Name.Substring(0, this.Name.Length - 1); // This should remove underline from the end of the name
            }

            fileLines.Add($"    internal unsafe struct { name }");
            fileLines.Add("    {");

            if (this.Fields.Count > 0) // Fields
            {
                foreach (FieldDefinition fieldDef in this.Fields)
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

            if (this.Functions.Count > 0) // Function Pointers
            {
                fileLines.Add("");

                foreach (FunctionDefinition funcDef in this.Functions)
                {
                    fileLines.Add($"        public { AbstractGenerator.POINTER } { funcDef.Name };");
                }
            }

            fileLines.Add("    }");
        }
    }
}
