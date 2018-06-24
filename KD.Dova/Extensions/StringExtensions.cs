namespace KD.Dova.Extensions
{
    internal static class StringExtensions
    {
        internal static string ToJniSignatureString(this string source)
        {
            string ret = "";

            if (source.Contains("."))
            {
                ret = source.Replace(".", "/");
            }

            return $"L{ ret };";
        }
    }
}
