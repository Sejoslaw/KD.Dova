namespace KD.Dova.Extensions
{
    /// <summary>
    /// Contains various extensions for <see cref="string"/> type.
    /// </summary>
    public static class StringExtensions
    {
        public static bool IsPrimitive(this string source)
        {
            return source.StartsWith("c") || // char
                   source.StartsWith("s") || // short
                   source.StartsWith("i") || // int
                   source.StartsWith("l") || // long
                   source.StartsWith("f") || // float
                   source.StartsWith("d") || // double
                   source.StartsWith("v"); // void
        }
    }
}
