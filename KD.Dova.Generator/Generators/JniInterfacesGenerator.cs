using KD.Dova.Generator.Definitions;

namespace KD.Dova.Generator.Generators
{
    internal class JniInterfacesGenerator : AbstractGenerator
    {
        public override void Generate(string[] lines)
        {
            this.Generate(lines, "struct ", "{", (index, line) =>
            {
                StructureDefinition def = new StructureDefinition();
                def.Name = this.ParseStructureName(line);

                this.ParseStructureFields(def, lines, index);
                this.ParseStructureFunctions(def, lines, index);

                this.GenerateFile(def);
            });
        }
    }
}
