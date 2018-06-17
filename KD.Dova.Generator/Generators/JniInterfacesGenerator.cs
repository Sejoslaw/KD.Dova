using KD.Dova.Generator.Definitions;

namespace KD.Dova.Generator.Generators
{
    internal class JniInterfacesGenerator : AbstractGenerator
    {
        public override void Generate(string[] lines)
        {
            string lineBeginning = "struct ";
            string lineEnding = " {";

            for (int i = 0; i < lines.Length; ++i)
            {
                string line = lines[i];

                if (line.StartsWith(lineBeginning) && line.EndsWith(lineEnding))
                {
                    StructureDefinition def = new StructureDefinition();
                    def.Name = this.ParseStructureName(line);

                    this.ParseStructureFields(def, lines, i);
                    this.ParseStructureFunctions(def, lines, i);

                    this.GenerateFile(def);
                }
            }
        }
    }
}
