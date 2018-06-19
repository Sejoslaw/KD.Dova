namespace KD.Dova.Generator.Definitions
{
    /// <summary>
    /// Defines single field.
    /// </summary>
    internal class FieldDefinition
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Comment { get; set; }
        /// <summary>
        /// Use "out" keyword.
        /// </summary>
        public bool IsOut { get; set; }
        /// <summary>
        /// Use "[Out]" attribute.
        /// </summary>
        public bool IsUsingOutAttribute { get; set; }

        public override string ToString()
        {
            string ret = $"{ this.Type } { this.Name }";

            if (this.Value != null)
            {
                ret += $" = { this.Value }";
            }

            if (this.Comment != null &&
                !string.IsNullOrEmpty(this.Comment))
            {
                ret += $" /* { this.Comment } */";
            }

            return ret;
        }
    }
}
