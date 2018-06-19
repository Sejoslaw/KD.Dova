using KD.Dova.Extensions;
using System.Collections.Generic;

namespace KD.Dova.Generator.Definitions
{
    /// <summary>
    /// Contains all informations about single structure.
    /// </summary>
    internal class StructureDefinition : FileConvertable
    {
        public List<FieldDefinition> Fields { get; }
        public List<FunctionDefinition> Functions { get; }

        public StructureDefinition()
        {
            this.Fields = new List<FieldDefinition>();
            this.Functions = new List<FunctionDefinition>();
        }

        internal override void AddLibraries(List<string> fileLines)
        {
            fileLines.Add("using KD.Dova;");
            fileLines.Add("using System.Security;");
            fileLines.Add("using System.Runtime.InteropServices;");
            fileLines.Add("using System.Runtime.CompilerServices;");
        }

        internal override void AddMainContent(List<string> fileLines)
        {
            if (this.Functions.Count > 0)
            {
                this.BuildDelegatesStructure(fileLines);
                fileLines.Add("");
            }

            this.BuildFunctionPointersStructure(fileLines);

            if (this.Name.Equals("JNINativeInterface_"))
            {
                this.GenerateWrapperForNativeFunctions(fileLines);
            }
        }

        private void GenerateWrapperForNativeFunctions(List<string> fileLines)
        {
            fileLines.Add("");

            string name = this.Name.Substring(0, this.Name.Length - 1);

            fileLines.Add($"    internal unsafe class JNIEnvironment");
            fileLines.Add("    {");

            fileLines.Add("        public IntPtr Environment { get; private set; }");
            fileLines.Add("        public JNINativeInterface_ NativeInterface { get; private set; }");
            fileLines.Add("");

            fileLines.Add("        internal JNIEnvironment(IntPtr jniEnv)");
            fileLines.Add("        {");
            fileLines.Add("            this.Environment = jniEnv;");
            fileLines.Add("            this.NativeInterface = *(*(JNIEnv_*) jniEnv.ToPointer()).functions;");
            fileLines.Add("        }");
            fileLines.Add("");

            if (this.Functions.Count > 0)
            {
                foreach (FunctionDefinition func in this.Functions)
                {
                    string function = func.ToString();
                    function = function.Substring(0, function.Length - 1);
                    string variableName = func.Name.WithFirstCharLower().ReplaceIfKeyWord();

                    string parameters = func.BuildParameters(false);
                    if (parameters.EndsWith(")"))
                    {
                        parameters = parameters.Substring(0, parameters.Length - 1);
                    }

                    fileLines.Add($"        public { function } ");
                    fileLines.Add("        {");
                    fileLines.Add($"            if ({ variableName } == null)");
                    fileLines.Add("            {");
                    fileLines.Add($"                NativeHelper.GetDelegateForFunctionPointer(this.NativeInterface.{ func.Name }, ref { variableName });");
                    fileLines.Add("            }");

                    if (!func.ReturnType.Equals("void"))
                    {
                        fileLines.Add($"            var ret = { variableName }.Invoke({ parameters });");
                        fileLines.Add($"            return ret;");
                    }
                    else
                    {
                        fileLines.Add($"            { variableName }.Invoke({ parameters });");
                    }

                    fileLines.Add("        }");
                    fileLines.Add("");
                }
            }
            fileLines.Add("");

            if (this.Functions.Count > 0)
            {
                foreach (FunctionDefinition func in this.Functions)
                {
                    string variableName = func.Name.WithFirstCharLower().ReplaceIfKeyWord();

                    fileLines.Add($"        public JNINativeInterface.{ func.Name } { variableName };");
                }
            }
            fileLines.Add("");


            fileLines.Add("    }");
        }

        private void BuildDelegatesStructure(List<string> fileLines)
        {
            string name = this.Name;

            if (name.EndsWith("_") &&
                (!name.Equals("JavaVM_") && !name.Equals("JNIEnv_"))) // Java specific structures
            {
                name = this.Name.Substring(0, this.Name.Length - 1); // This should remove underline from the end of the name
            }

            fileLines.Add($"    internal unsafe struct { name }");
            fileLines.Add("    {");

            if (this.Functions.Count > 0) // Functions
            {
                foreach (FunctionDefinition funcDef in this.Functions)
                {
                    fileLines.Add($"        [UnmanagedFunctionPointer(CallingConvention.Winapi)]");

                    if (this.Name.Equals("JNIInvokeInterface_")) // Special attribute for this structure
                    {
                        fileLines.Add("        [SuppressUnmanagedCodeSecurity]");
                    }

                    fileLines.Add($"        public delegate { funcDef.ToString() }");
                    fileLines.Add("");
                }
            }

            fileLines.Add("    }");
        }

        private void BuildFunctionPointersStructure(List<string> fileLines)
        {
            if (this.Functions.Count > 0)
            {
                fileLines.Add("    [StructLayout(LayoutKind.Sequential), NativeCppClass]");
            }

            fileLines.Add($"    internal unsafe struct { this.Name }");
            fileLines.Add("    {");

            if (this.Fields.Count > 0) // Fields
            {
                foreach (FieldDefinition fieldDef in this.Fields)
                {
                    string field = $"        public { fieldDef.Type }";

                    if (char.IsUpper(fieldDef.Type.ToCharArray()[0]) &&
                        !fieldDef.Type.Equals(AbstractGenerator.POINTER))
                    {
                        field += "*";
                    }

                    string variableName = fieldDef.Name.ReplaceIfKeyWord();

                    field += $" { variableName };";

                    fileLines.Add(field);
                }
            }

            if (this.Functions.Count > 0) // Function Pointers
            {
                fileLines.Add("");

                foreach (FunctionDefinition funcDef in this.Functions)
                {
                    string variableName = funcDef.Name.ReplaceIfKeyWord();

                    fileLines.Add($"        public { AbstractGenerator.POINTER } { variableName };");
                }
            }

            fileLines.Add("    }");
        }
    }
}
