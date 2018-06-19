namespace KD.Dova.Generator.Definitions
{
    internal class EnumValueDefinition
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"{ this.Name } = { this.Value }";
        }
    }
}
