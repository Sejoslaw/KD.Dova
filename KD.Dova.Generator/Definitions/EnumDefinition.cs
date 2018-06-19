using System.Collections.Generic;

namespace KD.Dova.Generator.Definitions
{
    /// <summary>
    /// Defines single enum.
    /// </summary>
    internal class EnumDefinition : FileConvertable
    {
        public List<EnumValueDefinition> Values { get; }

        public EnumDefinition()
        {
            this.Values = new List<EnumValueDefinition>();
        }

        internal override void AddLibraries(List<string> fileLines)
        {
        }

        internal override void AddMainContent(List<string> fileLines)
        {
            fileLines.Add($"    internal enum { this.Name }");
            fileLines.Add("    {");

            if (this.Values.Count > 0)
            {
                this.Values.ForEach(def => fileLines.Add($"        { def.ToString() }"));
            }

            fileLines.Add("    }");
        }
    }
}
