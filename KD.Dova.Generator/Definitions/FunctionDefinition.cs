using System.Collections.Generic;

namespace KD.Dova.Generator.Definitions
{
    /// <summary>
    /// Defines single function signature in structure.
    /// </summary>
    internal class FunctionDefinition
    {
        public string ReturnType { get; set; }
        public string Name { get; set; }
        public ICollection<FieldDefinition> Params { get; }

        public FunctionDefinition()
        {
            this.Params = new List<FieldDefinition>();
        }

        public override string ToString()
        {
            string func = $"{ this.ReturnType } { this.Name }(";
            string parameters = string.Join(',', this.Params);
            func += parameters + ");";
            return func;
        }
    }
}
