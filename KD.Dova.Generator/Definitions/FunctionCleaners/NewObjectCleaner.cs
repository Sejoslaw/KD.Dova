namespace KD.Dova.Generator.Definitions.FunctionCleaners
{
    internal class NewObjectCleaner : FunctionCleaner
    {
        public NewObjectCleaner(string wantedFunctionName) : base(wantedFunctionName)
        {
        }

        protected override void StartCleaning(FunctionDefinition funcDef)
        {
            var field = this.FindField(funcDef, "args");
            if (field != null)
            {
                field.Type = AbstractGenerator.PARAMS_FIELD;
            }
        }
    }
}
