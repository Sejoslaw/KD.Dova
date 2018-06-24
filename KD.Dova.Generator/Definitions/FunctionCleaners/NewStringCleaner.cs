namespace KD.Dova.Generator.Definitions.FunctionCleaners
{
    internal class NewStringCleaner : FunctionCleaner
    {
        public NewStringCleaner(string wantedFunctionName) : base(wantedFunctionName)
        {
        }

        protected override void StartCleaning(FunctionDefinition funcDef)
        {
            this.CleanParams(funcDef, AbstractGenerator.STRING_ATTRIBUTE, "string", "unicode");
        }
    }
}
