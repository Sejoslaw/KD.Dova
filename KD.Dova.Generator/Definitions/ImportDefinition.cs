using System.Collections.Generic;

namespace KD.Dova.Generator.Definitions
{
    internal class ImportDefinition : FileConvertable
    {
        public List<SingleImportDefinition> Imports { get; }

        public ImportDefinition()
        {
            this.Imports = new List<SingleImportDefinition>();
            this.ImportImports();
            this.Name = "JNINativeImports";
        }

        internal override void AddLibraries(List<string> fileLines)
        {
            fileLines.Add("using System.Runtime.InteropServices;");
        }

        internal override void AddMainContent(List<string> fileLines)
        {
            fileLines.Add($"    internal static unsafe class JNINativeImports");
            fileLines.Add("    {");

            foreach (SingleImportDefinition def in this.Imports)
            {
                fileLines.Add($"        { def.ToAttribute() }");
                fileLines.Add($"        internal static extern { def.ToString() }");
                fileLines.Add("");
            }

            fileLines.Add("    }");
        }

        /// <summary>
        /// Manually build import for each main JNI function.
        /// </summary>
        private void ImportImports()
        {
            this.Imports.Add(new SingleImportDefinition
            {
                Name = "JNI_GetDefaultJavaVMInitArgs",
                ReturnType = "int",
                Params = new List<FieldDefinition>(
                    new FieldDefinition[]
                    {
                        new FieldDefinition { Type = "JavaVMInitArgs*", Name = "args" }
                    })
            });

            this.Imports.Add(new SingleImportDefinition
            {
                Name = "JNI_CreateJavaVM",
                ReturnType = "int",
                Params = new List<FieldDefinition>(
                    new FieldDefinition[]
                    {
                        new FieldDefinition { IsOut = true, Type = AbstractGenerator.POINTER, Name = "pVM" },
                        new FieldDefinition { IsOut = true, Type = AbstractGenerator.POINTER, Name = "pEnv" },
                        new FieldDefinition { Type = "JavaVMInitArgs*", Name = "args" }
                    })
            });

            this.Imports.Add(new SingleImportDefinition
            {
                Name = "JNI_GetCreatedJavaVMs",
                ReturnType = "int",
                Params = new List<FieldDefinition>(
                    new FieldDefinition[]
                    {
                        new FieldDefinition { IsOut = true, Type = AbstractGenerator.POINTER, Name = "pVM" },
                        new FieldDefinition { Type = "int", Name = "jSize1" },
                        new FieldDefinition { IsUsingOutAttribute = true, IsOut = true, Type = "int", Name = "jSize2" }
                    })
            });

            this.Imports.Add(new SingleImportDefinition
            {
                Name = "JNI_OnLoad",
                ReturnType = "int",
                Params = new List<FieldDefinition>(
                    new FieldDefinition[]
                    {
                        new FieldDefinition { Type = "JavaVM_*", Name = "javaVM" }
                    })
            });

            this.Imports.Add(new SingleImportDefinition
            {
                Name = "JNI_OnUnload",
                ReturnType = "int",
                Params = new List<FieldDefinition>(
                    new FieldDefinition[]
                    {
                        new FieldDefinition { Type = "JavaVM_*", Name = "javaVM" }
                    })
            });
        }
    }
}
