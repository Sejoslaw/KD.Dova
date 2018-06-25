using System;
using KD.Dova.Core;
using KD.Dova.Core.Callers;
using KD.Dova.Extensions;
using KD.Dova.Utils;

namespace KD.Dova.Api
{
    /// <summary>
    /// Provides basic methods to operate on JNI directly.
    /// </summary>
    internal class GatewayManager : IGateway
    {
        public JavaRuntime Runtime { get; }

        private CallersFactory CallersFactory { get; }

        internal GatewayManager(JavaRuntime runtime)
        {
            this.Runtime = runtime;
            this.CallersFactory = new CallersFactory(this);
        }

        public T GetFieldValue<T>(IntPtr objectId, string fieldName, string fieldType = null)
        {
            return this.ReturnFieldValue<T>(objectId, fieldName, fieldType, false,
                (id, name, sig) => this.Runtime.JavaEnvironment.JNIEnv.GetFieldID(id, name, sig));
        }

        public T GetStaticFieldValue<T>(JClass javaType, string fieldName, string fieldType = null)
        {
            return this.ReturnFieldValue<T>(javaType.JavaClass, fieldName, fieldType, true,
                (id, name, sig) => this.Runtime.JavaEnvironment.JNIEnv.GetStaticFieldID(id, name, sig));
        }

        public T InvokeMethod<T>(IntPtr objectId, string methodName, string javaReturnType, params object[] parameters)
        {
            return this.ReturnMethodInvoke<T>(objectId, methodName, javaReturnType, parameters, false,
                (id, name, sig) => this.Runtime.JavaEnvironment.JNIEnv.GetMethodID(id, name, sig));
        }

        public T InvokeStaticMethod<T>(JClass javaType, string methodName, string javaReturnType, params object[] parameters)
        {
            return this.ReturnMethodInvoke<T>(javaType.JavaClass, methodName, javaReturnType, parameters, false,
                (id, name, sig) => this.Runtime.JavaEnvironment.JNIEnv.GetStaticMethodID(id, name, sig));
        }

        public JClass LoadClass(string className, params object[] genericTypes)
        {
            try
            {
                string jniSignature = className.ToJniSignatureString();
                IntPtr javaClass = this.Runtime.JavaEnvironment.JNIEnv.FindClass(jniSignature);
                JClass type = new JClass(this, className, javaClass);
                return type;
            }
            catch
            {
                throw new ArgumentException(this.Runtime.JavaEnvironment.CatchJavaException());
            }
        }

        public JObject New(string typeName, params object[] parameters)
        {
            JClass type = this.LoadClass(typeName);
            JObject obj = type.New(parameters);
            return obj;
        }

        public JObject New(JClass type, params object[] parameters)
        {
            try
            {
                IntPtr methodId = IntPtr.Zero;

                // Call constructor with no parameters
                if (parameters == null ||
                    parameters.Length == 0)
                {
                    methodId = this.Runtime.JavaEnvironment.JNIEnv.GetMethodID(type.JavaClass, "<init>", "()V");
                }
                else
                {
                    // TODO: Add support for calling constructor with parameters.
                }

                IntPtr javaObject = this.Runtime.JavaEnvironment.JNIEnv.NewObject(type.JavaClass, methodId, new NativeValue());
                return new JObject(this, type.JavaClassName, type.JavaClass, javaObject);
            }
            catch
            {
                throw new ArgumentException(this.Runtime.JavaEnvironment.CatchJavaException());
            }
        }

        public void SetFieldValue(IntPtr objectId, string fieldName, object newValue)
        {
            throw new NotSupportedException();
        }

        public void SetStaticFieldValue(JClass javaType, string fieldName, object newValue)
        {
            throw new NotSupportedException();
        }

        private T ReturnFieldValue<T>(IntPtr ptr, string fieldName, string fieldType, bool isStatic, Func<IntPtr, string, string, IntPtr> GetFieldId)
        {
            return this.ReturnValue<T>(ptr, fieldName,
                (id, name, sig) =>
                {
                    if (fieldType != null)
                    {
                        sig = fieldType.ToJniSignatureString();
                    }

                    IntPtr fieldId = GetFieldId(id, fieldName, sig);

                    return this.CallersFactory.GetFieldValue<T>(ptr, fieldId, isStatic);
                });
        }

        private T ReturnMethodInvoke<T>(IntPtr ptr, string methodName, string javaReturnType, object[] parameters, bool isStatic, Func<IntPtr, string, string, IntPtr> GetMethodId)
        {
            return this.ReturnValue<T>(ptr, methodName,
                (id, name, returnTypeJniSignature) =>
                {
                    string sig = parameters.ToJniSignatureArray().ToFullJniSignature(javaReturnType);
                    IntPtr methodId = GetMethodId(id, methodName, sig);
                    NativeValue[] args = parameters.ToNativeValueArray(this.Runtime);

                    return this.CallersFactory.InvokeMethod<T>(ptr, methodId, args, isStatic);
                });
        }

        private T ReturnValue<T>(IntPtr id, string name, Func<IntPtr, string, string, T> GetId)
        {
            if (GetId == null)
            {
                return default(T);
            }

            try
            {
                Type retType = typeof(T);
                JavaType sigType = retType.ToJniSignature();
                string jniSignature = sigType.JniField;

                return GetId(id, name, jniSignature);
            }
            catch
            {
                throw new ArgumentException(this.Runtime.JavaEnvironment.CatchJavaException());
            }
        }
    }
}
