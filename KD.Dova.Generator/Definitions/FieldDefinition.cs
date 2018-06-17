namespace KD.Dova.Generator.Definitions
{
    /// <summary>
    /// Defines single field.
    /// </summary>
    internal class FieldDefinition
    {
        public string Type { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{ this.Type } { this.Name }";
        }
    }
}
