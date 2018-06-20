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
            fileLines.Add($"    public static class { this.Name }");
            fileLines.Add("    {");

            if (this.Fields.Count > 0)
            {
                foreach (FieldDefinition field in this.Fields)
                {
                    string val = field.ToString();
                    if (!val.Contains("="))
                    {
                        val = val.Substring(0, val.Length);
                        val += " = 0";
                    }

                    fileLines.Add($"        public const { val };");
                }
            }

            fileLines.Add("    }");
        }
    }
}
