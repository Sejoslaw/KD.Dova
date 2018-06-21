namespace KD.Dova.Generator.Definitions.FunctionCleaners
{
    internal class GetIdCleaner : FunctionCleaner
    {
        public GetIdCleaner(string wantedFunctionName) : base(wantedFunctionName)
        {
        }

        protected override void StartCleaning(FunctionDefinition funcDef)
        {
            this.CleanParams(funcDef, AbstractGenerator.STRING_ATTRIBUTE, "string",
                "name", "sig");
        }
    }
}
