using KD.Dova.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

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
            fileLines.Add("using KD.Dova.Core;");
            fileLines.Add("using KD.Dova.Utils;");
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
                this.GenerateWrapperForEnvironment(fileLines);
            }
            else if (this.Name.Equals("JNIInvokeInterface_"))
            {
                this.GenerateWrapperForVirtualMachine(fileLines);
            }
        }

        private void GenerateWrapperForVirtualMachine(List<string> fileLines)
        {
            fileLines.Add("");

            string name = this.Name.Substring(0, this.Name.Length - 1);

            fileLines.Add($"    internal unsafe class JavaVirtualMachine");
            fileLines.Add("    {");

            fileLines.Add("        /* Pointer to this object in unmanaged code. */");
            fileLines.Add("        internal IntPtr NativePointer { get; }");
            fileLines.Add("        internal JNIInvokeInterface_ InvokeInterface { get; }");
            fileLines.Add("");

            fileLines.Add("        internal JavaVirtualMachine(IntPtr jvm)");
            fileLines.Add("        {");
            fileLines.Add("            this.NativePointer = jvm;");
            fileLines.Add("            this.InvokeInterface = *(*(JavaVM_*)jvm.ToPointer()).functions;");
            fileLines.Add("        }");
            fileLines.Add("");

            this.BuildFunctionsFromDelegates(fileLines, "InvokeInterface", "JNIInvokeInterface");
        }

        private void GenerateWrapperForEnvironment(List<string> fileLines)
        {
            fileLines.Add("");

            string name = this.Name.Substring(0, this.Name.Length - 1);

            fileLines.Add($"    internal unsafe class JNIEnvironment");
            fileLines.Add("    {");

            fileLines.Add("        /* Pointer to this object in unmanaged code. */");
            fileLines.Add("        internal IntPtr NativePointer { get; }");
            fileLines.Add("        internal JNINativeInterface_ NativeInterface { get; }");
            fileLines.Add("");

            fileLines.Add("        internal JNIEnvironment(IntPtr jniEnv)");
            fileLines.Add("        {");
            fileLines.Add("            this.NativePointer = jniEnv;");
            fileLines.Add("            this.NativeInterface = *(*(JNIEnv_*) jniEnv.ToPointer()).functions;");
            fileLines.Add("        }");
            fileLines.Add("");

            this.BuildFunctionsFromDelegates(fileLines, "NativeInterface", "JNINativeInterface");
        }

        private void BuildFunctionsFromDelegates(List<string> fileLines, string innerFieldName, string structWithDelegates)
        {
            if (this.Functions.Count > 0)
            {
                foreach (FunctionDefinition func in this.Functions)
                {
                    string function = func.ToString();
                    function = function.Substring(0, function.Length - 1);
                    string variableName = func.Name.WithFirstCharLower().ReplaceIfKeyWord();

                    string parameters = func.BuildParameters(false, false);
                    if (parameters.EndsWith(")"))
                    {
                        parameters = parameters.Substring(0, parameters.Length - 1);
                    }

                    function = function.RemoveFirstArgument();
                    parameters = parameters.RemoveFirstParameter();

                    if (string.IsNullOrEmpty(parameters))
                    {
                        parameters += "this.NativePointer";
                    }
                    else
                    {
                        parameters = "this.NativePointer, " + parameters;
                    }

                    string functionDelaration = "";
                    if (func.Params.Where(param => param.Type.Contains(AbstractGenerator.JAVA_ARGS)).Count() > 0)
                    {
                        functionDelaration += "        internal";
                    }
                    else
                    {
                        functionDelaration += "        public";
                    }
                    functionDelaration += $" { function }";

                    fileLines.Add(functionDelaration);
                    fileLines.Add("        {");
                    fileLines.Add($"            if ({ variableName } == null)");
                    fileLines.Add("            {");
                    fileLines.Add($"                JavaConverter.GetDelegateForFunctionPointer(this.{ innerFieldName }.{ func.Name }, ref { variableName });");
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

            if (this.Functions.Count > 0)
            {
                foreach (FunctionDefinition func in this.Functions)
                {
                    string variableName = func.Name.WithFirstCharLower().ReplaceIfKeyWord();

                    fileLines.Add($"        internal { structWithDelegates }.{ func.Name } { variableName };");
                }
            }

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
