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
        public List<FieldDefinition> Params { get; }

        public FunctionDefinition()
        {
            this.Params = new List<FieldDefinition>();
        }

        public override string ToString()
        {
            string func = $"{ this.ReturnType } { this.Name }(";
            string parameters = this.BuildParameters();
            func += parameters;

            if (!func.EndsWith(")"))
            {
                func += ")";
            }

            func += ";";

            return func;
        }

        public string BuildParameters()
        {
            string ret = "";

            for (int i = 0; i < this.Params.Count - 1; ++i)
            {
                FieldDefinition fieldDef = this.Params[i];

                if (fieldDef.IsOut) // out pointer
                {
                    ret += $"out ";
                }

                ret += $"{ fieldDef.Type } { fieldDef.Name }, ";
            }

            FieldDefinition field = this.Params.Last();
            ret += $"{ field.Type } { field.Name }";

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
                    !field.Type.Equals(AbstractGenerator.POINTER))
                {
                    field.Type += "_";
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
                    return;
                }
            }

            if (this.Name.ToLower().StartsWith("set") || this.Name.ToLower().StartsWith("new"))
            {
                FieldDefinition def = this.Params.LastOrDefault();
                if (def == null)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(prim))
                {
                    if (this.Name.ToLower().Contains("array") &&
                        (def.Name.Equals("len") || (def.Name.Equals("len)"))))
                    {
                        def.Type = "int";
                        return;
                    }
                    else
                    {
                        def.Type = prim;
                        return;
                    }
                }
            }

            if ((this.Name.ToLower().StartsWith("get") || this.Name.ToLower().StartsWith("call")) &&
                !string.IsNullOrEmpty(prim))
            {
                if (!this.Name.ToLower().Contains("array"))
                {
                    this.ReturnType = prim;
                    return;
                }
            }

            if (this.Name.ToLower().EndsWith("length"))
            {
                this.ReturnType = "int";
                return;
            }
        }

        private string GetPrimitiveType(string str)
        {
            if (str.Contains("boolean")) return "bool";
            if (str.Contains("byte")) return "byte";
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
