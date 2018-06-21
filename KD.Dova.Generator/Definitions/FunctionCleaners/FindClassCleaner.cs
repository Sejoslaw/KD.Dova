namespace KD.Dova.Generator.Definitions.FunctionCleaners
{
    internal class FindClassCleaner : FunctionCleaner
    {
        public FindClassCleaner(string wantedFunctionName) : base(wantedFunctionName)
        {
        }

        protected override void StartCleaning(FunctionDefinition funcDef)
        {
            this.CleanParams(funcDef, AbstractGenerator.STRING_ATTRIBUTE, "string", "name");
        }
    }
}
