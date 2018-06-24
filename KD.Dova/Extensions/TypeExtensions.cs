using KD.Dova.Utils;
using System;
using System.Linq;

namespace KD.Dova.Extensions
{
    internal static class TypeExtensions
    {
        internal static JavaType ToJniSignature(this Type type)
        {
            if (type.IsArray)
            {
                return null;
            }

            // Try to get primitive type
            JavaType jt = JavaMapper.GetRegisteredJavaTypes().Where(javaType => javaType.CSharpType == type).FirstOrDefault();

            if (jt != null)
            {
                return jt;
            }

            string typeName = type.FullName;
            string jniTypeName = typeName.Replace(".", "/");

            jt = new JavaType(typeName, typeName.ToJniSignatureString(), type);
            return jt;
        }
    }
}
