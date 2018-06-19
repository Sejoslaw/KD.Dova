using KD.Dova.Generator.Definitions;

namespace KD.Dova.Generator.Generators
{
    internal class ConstantsGenerator : AbstractGenerator
    {
        public override void Generate(string[] lines)
        {
            ClassWithConstantsDefinition classDef = new ClassWithConstantsDefinition();

            this.Generate(lines, "#define", null, (index, line) =>
            {
                FieldDefinition fieldDef = new FieldDefinition();
                this.ParseConstant(fieldDef, line);

                if (!string.IsNullOrEmpty(fieldDef.Name) &&
                    !fieldDef.Name.StartsWith("_"))
                {
                    classDef.Fields.Add(fieldDef);
                }
            });

            this.GenerateFile(classDef);
        }
    }
}
