namespace KD.Dova.Generator.Definitions
{
    internal class EnumValueDefinition
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            if (this.Value == null)
            {
                return this.Name;
            }
            else
            {
                return $"{ this.Name } = { this.Value }";
            }
        }
    }
}
