using System.Collections.Generic;

namespace KD.Dova.Generator.Definitions
{
    internal class ClassWithConstantsDefinition : FileConvertable
    {
        public List<FieldDefinition> Fields { get; }

        public ClassWithConstantsDefinition()
        {
            this.Fields = new List<FieldDefinition>();
            this.Name = "JNIConstants";
        }

        internal override void AddLibraries(List<string> fileLines)
        {
        }

        internal override void AddMainContent(List<string> fileLines)
        {
            fileLines.Add($"    internal static class { this.Name }");
            fileLines.Add("    {");

            if (this.Fields.Count > 0)
            {
                foreach (FieldDefinition field in this.Fields)
                {
                    fileLines.Add($"        public static readonly { field.ToString() };");
                }
            }

            fileLines.Add("    }");
        }
    }
}
