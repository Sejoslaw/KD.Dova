using System;

namespace KD.Dova.Api
{
    /// <summary>
    /// Wrapper for single Java type.
    /// </summary>
    public sealed class JType
    {
        /// <summary>
        /// Name of this Java type.
        /// </summary>
        public string JavaTypeName { get; }

        private IGateway Gateway { get; }
        /// <summary>
        /// Pointer to Java type.
        /// </summary>
        internal IntPtr JavaType { get; }

        internal JType(IGateway gateway, string javaTypeName, IntPtr javaType)
        {
            this.Gateway = gateway;
            this.JavaTypeName = javaTypeName;
            this.JavaType = javaType;
        }

        /// <summary>
        /// Creates new object of this Java type.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public JObject New(params object[] parameters)
        {
            return this.Gateway.New(this, parameters);
        }

        /// <summary>
        /// Invokes static method from current Java type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T InvokeStaticMethod<T>(string methodName, params object[] parameters)
        {
            return this.Gateway.InvokeStaticMethod<T>(this, methodName, parameters);
        }

        /// <summary>
        /// Returns the value of static field from current Java type. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public T GetStaticFieldValue<T>(string fieldName)
        {
            return this.Gateway.GetStaticFieldValue<T>(this, fieldName);
        }

        /// <summary>
        /// Sets the value of the static field of current Java type.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="newValue"></param>
        public void SetStaticFieldValue(string fieldName, object newValue)
        {
            this.Gateway.SetStaticFieldValue(this, fieldName, newValue);
        }
    }
}
