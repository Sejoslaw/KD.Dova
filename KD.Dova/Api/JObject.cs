using System;

namespace KD.Dova.Api
{
    /// <summary>
    /// Wrapper for a single Java object.
    /// </summary>
    public sealed class JObject
    {
        /// <summary>
        /// Name of the Java class.
        /// </summary>
        public string JavaClassName { get; }

        private IGateway Gateway { get; }
        /// <summary>
        /// Pointer to Java class which this object represents.
        /// </summary>
        private IntPtr JavaClass { get; }
        /// <summary>
        /// Pointer to actual Java object.
        /// </summary>
        private IntPtr JavaObject { get; }

        internal JObject(IGateway gateway, string javaClassName, IntPtr javaClass, IntPtr javaObject)
        {
            this.Gateway = gateway;
            this.JavaClassName = javaClassName;
            this.JavaClass = javaClass;
            this.JavaObject = javaObject;
        }

        /// <summary>
        /// Returns the value of the field using specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public T GetFieldValue<T>(string fieldName)
        {
            return this.Gateway.GetFieldValue<T>(this.JavaObject, fieldName);
        }

        /// <summary>
        /// Returns value of the custom-type field.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public JObject GetFieldRef(string fieldName)
        {
            return this.Gateway.GetFieldRef(this.JavaObject, fieldName);
        }

        /// <summary>
        /// Sets the value of specified field.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="newValue"></param>
        public void SetFieldValue(string fieldName, object newValue)
        {
            this.Gateway.SetFieldValue(this.JavaObject, fieldName, newValue);
        }

        /// <summary>
        /// Invokes specified method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T Invoke<T>(string methodName, params object[] parameters)
        {
            return this.Gateway.InvokeMethod<T>(this.JavaObject, methodName, parameters);
        }
    }
}
