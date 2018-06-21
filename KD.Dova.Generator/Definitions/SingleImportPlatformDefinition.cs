namespace KD.Dova.Generator.Definitions
{
    /// <summary>
    /// Describes imports for single platform.
    /// </summary>
    internal class SingleImportPlatformDefinition
    {
        public string PlatformName { get; set; }
        public string DllImport { get; set; }

        public string PlatformFullName
        {
            get
            {
                return $"JNINativeImports_{ this.PlatformName }";
            }
        }

        /// <summary>
        /// This should return platform corresponding function from <see cref="KD.Dova.Commons.OS"/>.
        /// </summary>
        public string FunctionName
        {
            get
            {
                return $"OS.Is{ this.PlatformName }";
            }
        }
    }
}
