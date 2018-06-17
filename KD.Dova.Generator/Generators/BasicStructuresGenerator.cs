﻿using KD.Dova.Generator.Definitions;

namespace KD.Dova.Generator.Generators
{
    /// <summary>
    /// Used to generate basic structure files.
    /// </summary>
    internal class BasicStructuresGenerator : AbstractGenerator
    {
        public override void Generate(string[] lines)
        {
            string lineBeginning = "typedef struct ";

            for (int i = 0; i < lines.Length; ++i)
            {
                string line = lines[i];

                if (line.StartsWith(lineBeginning) && !line.EndsWith(";"))
                {
                    StructureDefinition def = new StructureDefinition();
                    string structName = this.ParseStructureFields(def, lines, i);
                    def.Name = this.ParseStructureName(structName);

                    this.GenerateFile(def);
                }
            }
        }
    }
}
