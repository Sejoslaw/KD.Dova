namespace KD.Dova.Generator
{
    /// <summary>
    /// Represents single generator.
    /// </summary>
    public interface IGenerator
    {
        /// <summary>
        /// Generate files.
        /// </summary>
        /// <param name="lines"> Lines read from "jni.h". </param>
        void Generate(string[] lines);
    }
}