using KD.Dova.Generator.Definitions;

namespace KD.Dova.Generator.Generators
{
    /// <summary>
    /// Used to generate basic structure files.
    /// </summary>
    internal class BasicStructuresGenerator : AbstractGenerator
    {
        public override void Generate(string[] lines)
        {
            this.Generate(lines, "typedef struct", "{", (index, line) =>
            {
                StructureDefinition def = new StructureDefinition();
                string structName = this.ParseStructureFields(def, lines, index);
                def.Name = this.ParseStructureName(structName);

                this.GenerateFile(def);
            });
        }
    }
}
