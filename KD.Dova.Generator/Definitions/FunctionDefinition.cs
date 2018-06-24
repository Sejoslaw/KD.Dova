using KD.Dova.Generator.Definitions.FunctionCleaners;
using System.Collections.Generic;
using System.Linq;

namespace KD.Dova.Generator.Definitions
{
    /// <summary>
    /// Defines single function signature in structure.
    /// </summary>
    internal class FunctionDefinition
    {
        public string ReturnType { get; set; }
        public string Name { get; set; }
        public List<FieldDefinition> Params { get; set; }
        public List<FunctionCleaner> Cleaners { get; }

        public FunctionDefinition()
        {
            this.Params = new List<FieldDefinition>();
            this.Cleaners = new List<FunctionCleaner>();

            // Add custom functions cleaners
            this.Cleaners.Add(new FindClassCleaner("FindClass"));
            this.Cleaners.Add(new GetIdCleaner("GetField"));
            this.Cleaners.Add(new GetIdCleaner("GetStaticField"));
            this.Cleaners.Add(new GetIdCleaner("GetMethod"));
            this.Cleaners.Add(new GetIdCleaner("GetStaticMethod"));
            this.Cleaners.Add(new NewObjectCleaner("NewObject"));
            this.Cleaners.Add(new NewStringCleaner("NewString"));
        }

        public override string ToString()
        {
            return this.ToString(true, true, true);
        }

        public string ToString(bool includeReturnType, bool includeType, bool includeArgument)
        {
            string func = "";

            if (includeReturnType)
            {
                func += $"{ this.ReturnType } ";
            }

            func += $"{ this.Name }(";
            string parameters = this.BuildParameters(includeType, includeArgument);
            func += parameters;

            if (!func.EndsWith(")"))
            {
                func += ")";
            }

            func += ";";

            return func;
        }

        public string BuildParameters(bool includeType, bool includeArgument)
        {
            string ret = "";

            for (int i = 0; i < this.Params.Count; ++i)
            {
                FieldDefinition fieldDef = this.Params[i];

                if (includeArgument)
                {
                    if (!string.IsNullOrEmpty(fieldDef.CustomAttribute))
                    {
                        ret += $"{ fieldDef.CustomAttribute } ";
                    }
                }

                if (fieldDef.IsOut) // out pointer
                {
                    ret += $"out ";
                }

                if (includeType)
                {
                    ret += $"{ fieldDef.Type } ";
                }

                ret += $"{ fieldDef.Name }, ";
            }

            if (ret.Length > 0)
            {
                ret = ret.Substring(0, ret.Length - 2);
            }

            return ret;
        }

        /// <summary>
        /// Clean values of elements in arrays.
        /// Clean return type by func name.
        /// Clean value argument type by func name.
        /// Clean array lenghts.
        /// </summary>
        public void Clean()
        {
            string prim = this.GetPrimitiveType(this.Name.ToLower());

            foreach (FieldDefinition field in this.Params)
            {
                if (char.IsUpper(field.Type.ToCharArray().First()) &&
                    !field.Type.Equals(AbstractGenerator.POINTER) &&
                    !field.Type.Equals(AbstractGenerator.JAVA_ARGS))
                {
                    field.Type += "_";
                }
                else if (field.Type.Equals(AbstractGenerator.JAVA_ARGS))
                {
                    field.Type += "*";
                }
            }

            if (this.Name.ToLower().StartsWith("release"))
            {
                FieldDefinition def = this.Params.Where(field => field.Name.Equals("elems")).FirstOrDefault();
                if (def == null)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(prim))
                {
                    def.Type = prim + "*";
                }
            }

            foreach (FieldDefinition def in this.Params)
            {
                if (def.Name.StartsWith("len") ||
                    def.Name.StartsWith("capacity") ||
                    def.Name.StartsWith("mode"))
                {
                    def.Type = "int";
                }
                else if (def.Name.StartsWith("buf"))
                {
                    def.Type = "StringBuilder";
                }
            }

            if ((this.Name.ToLower().StartsWith("get") || this.Name.ToLower().StartsWith("call")) &&
                !string.IsNullOrEmpty(prim))
            {
                if (!this.Name.ToLower().Contains("array"))
                {
                    this.ReturnType = prim;
                }
            }

            if (this.Name.ToLower().EndsWith("length"))
            {
                this.ReturnType = "int";
            }

            this.RunFunctionCleaners();
        }

        private void RunFunctionCleaners()
        {
            this.Cleaners.ForEach(cleaner => cleaner.Clean(this));
        }

        private string GetPrimitiveType(string str)
        {
            if (str.Contains("boolean")) return "bool";
            if (str.Contains("byte")) return "byte";
            if (str.EndsWith("chars")) return AbstractGenerator.POINTER;
            if (str.Contains("char")) return "ushort";
            if (str.Contains("double")) return "double";
            if (str.Contains("float")) return "float";
            if (str.Contains("int")) return "int";
            if (str.Contains("long")) return "long";
            if (str.Contains("object")) return "IntPtr";
            if (str.Contains("short")) return "short";

            return string.Empty;
        }
    }
}
