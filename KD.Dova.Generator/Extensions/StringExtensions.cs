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

        public static string RemoveFirstParameter(this string source)
        {
            if (string.IsNullOrEmpty(source) ||
                source.Equals("env") ||
                source.Equals("vm"))
            {
                return string.Empty;
            }

            if (source.Contains(","))
            {
                int index = source.IndexOf(",");
                string ret = source.Substring(index + 2, source.Length - index - 2);
                return ret;
            }

            return source;
        }

        public static string RemoveFirstArgument(this string source)
        {
            int indexOpenning = source.IndexOf("(");
            int comma = source.IndexOf(",");
            int indexEnding = source.IndexOf(")");

            string ret = source.Substring(0, indexOpenning + 1);

            if (comma < 0) // Function with only one parameter - environment
            {
                ret += source.Substring(indexEnding, source.Length - indexEnding);
                return ret;
            }
            else // More than one parameter
            {
                ret += source.Substring(comma + 2, source.Length - comma - 2);
                return ret;
            }
        }
    }
}
