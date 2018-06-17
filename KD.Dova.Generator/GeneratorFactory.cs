using KD.Dova.Generator.Generators;
using System.Collections.Generic;
using System.IO;

namespace KD.Dova.Generator
{
    /// <summary>
    /// Holds informations about all generators.
    /// </summary>
    public sealed class GeneratorFactory
    {
        private static GeneratorFactory _instance;
        public static GeneratorFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GeneratorFactory();
                }

                return _instance;
            }
        }

        /// <summary>
        /// Holds information about all generators.
        /// </summary>
        private List<IGenerator> Generators { get; set; }
        /// <summary>
        /// Path to "jni.h" from which files should be generated.
        /// </summary>
        public string JniFilePath { get; set; }
        /// <summary>
        /// Directory to which all files will be generated.
        /// </summary>
        private string _outputDirectory;
        public string OutputDirectory
        {
            get => this._outputDirectory;
            set
            {
                this._outputDirectory = value;

                if (!Directory.Exists(this._outputDirectory))
                {
                    Directory.CreateDirectory(this._outputDirectory);
                }
            }
        }

        public GeneratorFactory()
        {
            this.Generators = new List<IGenerator>();
            this.InitializeGenerators();
        }

        public void StartGenerators()
        {
            string[] lines = File.ReadAllLines(JniFilePath);

            foreach (IGenerator generator in this.Generators)
            {
                generator.Generate(lines);
            }
        }

        /// <summary>
        /// Initialize all default generators.
        /// Reinitialize this to clear generators list and add basic generators.
        /// </summary>
        public void InitializeGenerators()
        {
            this.Generators.Clear();

            this.Generators.Add(new BasicStructuresGenerator());
            this.Generators.Add(new JniInterfacesGenerator());
        }
    }
}