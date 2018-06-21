using System;
using KD.Dova.Core;

namespace KD.Dova.Api
{
    /// <summary>
    /// Provides basic methods to operate on JNI directly.
    /// </summary>
    internal class GatewayManager : IGateway
    {
        public JavaRuntime Runtime { get; }

        internal GatewayManager(JavaRuntime runtime)
        {
            this.Runtime = runtime;
        }

        public JObject GetFieldRef(IntPtr objectId, string fieldName)
        {
        }

        public T GetFieldValue<T>(IntPtr objectId, string fieldName)
        {
        }

        public T GetStaticFieldValue<T>(JType javaType, string fieldName)
        {
        }

        public T InvokeMethod<T>(IntPtr objectId, string methodName, params object[] parameters)
        {
        }

        public T InvokeStaticMethod<T>(IntPtr typeId, string methodName, params object[] parameters)
        {
        }

        public JType LoadClass(string className, params object[] genericTypes)
        {
            try
            {
                IntPtr javaClass = this.Runtime.JavaEnvironment.JNIEnv.FindClass(className);
                JType type = new JType(this, className, javaClass);
                return type;
            }
            catch
            {
                throw new ArgumentException(this.Runtime.JavaEnvironment.CatchJavaException());
            }
        }

        public JObject New(string typeName, params object[] parameters)
        {
            JType type = this.LoadClass(typeName);
            JObject obj = type.New(parameters);
            return obj;
        }

        public JObject New(JType type, params object[] parameters)
        {
            try
            {
                IntPtr methodId = IntPtr.Zero;

                // Call constructor with no parameters
                if (parameters == null ||
                    parameters.Length == 0)
                {
                    methodId = this.Runtime.JavaEnvironment.JNIEnv.GetMethodID(type.JavaType, "<init>", "()V");
                }
                else
                {
                    // TODO: Add support for calling constructor with parameters.
                }

                IntPtr javaObject = this.Runtime.JavaEnvironment.JNIEnv.NewObject(type.JavaType, methodId, new NativeValue());
                return new JObject(this, type.JavaTypeName, type.JavaType, javaObject);
            }
            catch
            {
                throw new ArgumentException(this.Runtime.JavaEnvironment.CatchJavaException());
            }
        }

        public void SetFieldValue(IntPtr objectId, string fieldName, object newValue)
        {
            // TODO:
        }

        public void SetStaticFieldValue(JType javaType, string fieldName, object newValue)
        {
            // TODO:
        }
    }
}
