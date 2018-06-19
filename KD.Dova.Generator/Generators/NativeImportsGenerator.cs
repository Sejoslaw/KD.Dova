using KD.Dova.Generator.Definitions;

namespace KD.Dova.Generator.Generators
{
    internal class NativeImportsGenerator : AbstractGenerator
    {
        public override void Generate(string[] lines)
        {
            ImportDefinition def = new ImportDefinition();
            this.GenerateFile(def);
        }
    }
}
