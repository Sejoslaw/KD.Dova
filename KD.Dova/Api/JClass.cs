using System;

namespace KD.Dova.Api
{
    /// <summary>
    /// Wrapper for single Java class.
    /// </summary>
    public sealed class JClass : IDisposable
    {
        /// <summary>
        /// Name of this Java class.
        /// </summary>
        public string JavaClassName { get; }

        private IGateway Gateway { get; }
        /// <summary>
        /// Pointer to Java type.
        /// </summary>
        internal IntPtr JavaClass { get; }

        internal JClass(IGateway gateway, string javaClassName, IntPtr javaClass)
        {
            this.Gateway = gateway;
            this.JavaClassName = javaClassName;
            this.JavaClass = javaClass;
        }

        /// <summary>
        /// Creates new object of this Java class.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public JObject New(params object[] parameters)
        {
            return this.Gateway.New(this, parameters);
        }

        /// <summary>
        /// Invokes static method from current Java class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T InvokeStaticMethod<T>(string methodName, string javaReturnType, params object[] parameters)
        {
            return this.Gateway.InvokeStaticMethod<T>(this, methodName, javaReturnType, parameters);
        }

        /// <summary>
        /// Returns the value of static field from current Java class. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public T GetStaticFieldValue<T>(string fieldName, string fieldType = null)
        {
            return this.Gateway.GetStaticFieldValue<T>(this, fieldName, fieldType);
        }

        /// <summary>
        /// Sets the value of the static field of current Java class.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="newValue"></param>
        public void SetStaticFieldValue(string fieldName, object newValue)
        {
            this.Gateway.SetStaticFieldValue(this, fieldName, newValue);
        }

        public void Dispose()
        {
            this.Gateway.Runtime.JavaEnvironment.JNIEnv.DeleteLocalRef(this.JavaClass);
        }
    }
}
