using KD.Dova.Core;
using KD.Dova.Utils;
using System.Collections.Generic;

namespace KD.Dova.Extensions
{
    internal static class ObjectExtensions
    {
        internal static JavaType[] ToJniSignatureArray(this object[] source)
        {
            List<JavaType> javaTypes = new List<JavaType>();

            foreach (object obj in source)
            {
                javaTypes.Add(obj.GetType().ToJniSignature());
            }

            return javaTypes.ToArray();
        }

        internal static NativeValue[] ToNativeValueArray(this object[] source, JavaRuntime runtime)
        {
            List<NativeValue> values = new List<NativeValue>();

            foreach (object obj in source)
            {
                values.Add(obj.ToNativeValue(runtime));
            }

            return values.ToArray();
        }

        internal static NativeValue ToNativeValue(this object source, JavaRuntime runtime)
        {
            JavaType jt = source.GetType().ToJniSignature();

            if (jt != null)
            {
                return jt.ToNativeValue(runtime, source);
            }

            return new NativeValue();
        }
    }
}
