using KD.Dova.Utils;

namespace KD.Dova.Extensions
{
    internal static class JavaTypeExtensions
    {
        internal static string ToFullJniSignature(this JavaType[] source, string javaReturnType)
        {
            string ret = "(";

            foreach (JavaType jt in source)
            {
                ret += jt.JniField;
            }

            ret += ")";

            ret += javaReturnType.ToJniSignatureString();

            return ret;
        }
    }
}
