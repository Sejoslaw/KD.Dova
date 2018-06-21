using System.Linq;

namespace KD.Dova.Generator.Definitions.FunctionCleaners
{
    /// <summary>
    /// Used to clean single function.
    /// By cleaning we mean setting up attributes and types manually.
    /// </summary>
    internal abstract class FunctionCleaner
    {
        private string WantedFunctionName { get; }

        public FunctionCleaner(string wantedFunctionName)
        {
            this.WantedFunctionName = wantedFunctionName;
        }

        public void Clean(FunctionDefinition funcDef)
        {
            if (funcDef.Name.StartsWith(this.WantedFunctionName))
            {
                this.StartCleaning(funcDef);
            }
        }

        protected FieldDefinition FindField(FunctionDefinition funcDef, string fieldName)
        {
            return funcDef.Params.Where(param => param.Name.Contains(fieldName)).FirstOrDefault();
        }

        protected void CleanParams(FunctionDefinition funcDef, string newAttribute, string newFieldType, params string[] parameters)
        {
            foreach (var field in funcDef.Params)
            {
                foreach (var par in parameters)
                {
                    if (field.Name.Contains(par))
                    {
                        if (newFieldType != null)
                        {
                            field.Type = newFieldType;
                        }

                        field.CustomAttribute = newAttribute;
                    }
                }
            }
        }

        protected abstract void StartCleaning(FunctionDefinition funcDef);
    }
}
