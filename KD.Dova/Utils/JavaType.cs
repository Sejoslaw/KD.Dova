using KD.Dova.Core;
using System;
using System.Reflection;

namespace KD.Dova.Utils
{
    /// <summary>
    /// Represents single Java type.
    /// </summary>
    internal class JavaType
    {
        public string JavaTypeName { get; }
        public string JniField { get; }
        public Type CSharpType { get; }

        internal JavaType(string javaTypeName, string jniField, Type csharpType)
        {
            this.JavaTypeName = javaTypeName;
            this.JniField = jniField;
            this.CSharpType = csharpType;
        }

        public string ToJniArray()
        {
            return $"[{ this.JniField }";
        }

        public override string ToString()
        {
            return this.JniField;
        }

        public NativeValue ToNativeValue(JavaRuntime runtime, object value)
        {
            NativeValue val = new NativeValue();
            if (value is string)
            {
                string str = value.ToString();
                val.l = runtime.JavaEnvironment.JNIEnv.NewString(str, str.Length);
                return val;
            }

            Type type = val.GetType();

            foreach (FieldInfo fieldInfo in type.GetFields())
            {
                if (fieldInfo.FieldType == value.GetType())
                {
                    fieldInfo.SetValue(val, value);
                }
            }

            return val;
        }
    }
}
