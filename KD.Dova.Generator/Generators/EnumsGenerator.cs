using KD.Dova.Generator.Definitions;

namespace KD.Dova.Generator.Generators
{
    internal class EnumsGenerator : AbstractGenerator
    {
        public override void Generate(string[] lines)
        {
            this.Generate(lines, "typedef enum ", " {", (index, line) =>
            {
                EnumDefinition def = new EnumDefinition();
                this.ParseEnumValues(def, lines, index, line);

                this.GenerateFile(def);
            });
        }
    }
}
