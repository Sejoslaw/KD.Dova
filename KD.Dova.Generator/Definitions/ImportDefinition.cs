using System.Collections.Generic;

namespace KD.Dova.Generator.Definitions
{
    internal class ImportDefinition : FileConvertable
    {
        public List<SingleImportDefinition> Imports { get; }
        public List<SingleImportPlatformDefinition> Platforms { get; }

        public ImportDefinition()
        {
            this.Imports = new List<SingleImportDefinition>();
            this.Platforms = new List<SingleImportPlatformDefinition>();

            this.ImportImports();
            this.ImportPlatforms();
            this.Name = "JNINativeImports";
        }

        internal override void AddLibraries(List<string> fileLines)
        {
            fileLines.Add("using KD.Dova.Commons;");
            fileLines.Add("using System.Runtime.InteropServices;");
        }

        internal override void AddMainContent(List<string> fileLines)
        {
            //Platform specific DllImport classes
            foreach (var platform in this.Platforms)
            {
                fileLines.Add($"    internal static unsafe class { platform.PlatformFullName }");
                fileLines.Add("    {");

                foreach (SingleImportDefinition def in this.Imports)
                {
                    fileLines.Add($"        { platform.DllImport }");
                    fileLines.Add($"        internal static extern { def.ToString() }");
                    fileLines.Add("");
                }

                fileLines.Add("    }");
                fileLines.Add("");
            }

            // Main class which will handle different platforms

            fileLines.Add($"    internal static unsafe class JNINativeImports");
            fileLines.Add("    {");

            foreach (SingleImportDefinition def in this.Imports)
            {
                string functionName = def.ToString();
                fileLines.Add($"        internal static { functionName.Substring(0, functionName.Length - 1) }");
                fileLines.Add("        {");

                foreach (var platform in this.Platforms)
                {
                    fileLines.Add($"            if ({ platform.FunctionName })");
                    fileLines.Add("            {");
                    fileLines.Add($"                return { platform.PlatformFullName }.{ def.ToString(false, false, false) }");
                    fileLines.Add("            }");
                }

                fileLines.Add("            throw new ArgumentException(\"Unknown OS. Please check if there is an implementation of KD.Dova for your operating system.\");");
                fileLines.Add("        }");
                fileLines.Add("");
            }

            fileLines.Add("    }");
            fileLines.Add("");
        }

        private void ImportPlatforms()
        {
            this.Platforms.Add(new SingleImportPlatformDefinition
            {
                PlatformName = "Windows",
                DllImport = "[DllImport(\"jvm.dll\", CallingConvention = CallingConvention.Winapi)]"
            });

            this.Platforms.Add(new SingleImportPlatformDefinition
            {
                PlatformName = "Linux",
                DllImport = "[DllImport(\"libjvm.so.6\", CallingConvention = CallingConvention.Winapi)]"
            });
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
                        new FieldDefinition { CustomAttribute = "[Out]", IsOut = true, Type = "int", Name = "jSize2" }
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
