namespace KD.Dova.Extensions
{
    /// <summary>
    /// Contains various extensions for <see cref="string"/> type.
    /// </summary>
    public static class StringExtensions
    {
        public static string RemoveSpaces(this string source)
        {
            return source.Replace(" ", "");
        }

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

        public static string WithFirstCharLower(this string source)
        {
            return source.ToLower().ToCharArray()[0] + source.Substring(1);
        }

        public static bool IsKeyWord(this string source)
        {
            return (source.Equals("throw"));
        }

        public static string ReplaceKeyWord(this string source)
        {
            return "_" + source;
        }

        public static string ReplaceIfKeyWord(this string source)
        {
            if (source.IsKeyWord())
            {
                source = source.ReplaceKeyWord();
            }

            return source;
        }
    }
}
