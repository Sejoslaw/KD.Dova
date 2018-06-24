using KD.Dova.Api;
using KD.Dova.Natives;
using KD.Dova.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KD.Dova.Core.Callers
{
    /// <summary>
    /// Holds data about 
    /// </summary>
    internal class CallersFactory
    {
        private IGateway Gateway { get; }
        private List<object> FieldCallers { get; }
        private List<object> MethodCallers { get; }

        public CallersFactory(IGateway gateway)
        {
            this.Gateway = gateway;
            this.FieldCallers = new List<object>();
            this.MethodCallers = new List<object>();

            JNIEnvironment env = this.Gateway.Runtime.JavaEnvironment.JNIEnv;

            this.FieldCallers.Add(new FieldCaller<IntPtr>(env.GetObjectField, env.GetStaticObjectField));
            this.FieldCallers.Add(new FieldCaller<bool>(env.GetBooleanField, env.GetStaticBooleanField));
            this.FieldCallers.Add(new FieldCaller<byte>(env.GetByteField, env.GetStaticByteField));
            this.FieldCallers.Add(new FieldCaller<ushort>(env.GetCharField, env.GetStaticCharField));
            this.FieldCallers.Add(new FieldCaller<short>(env.GetShortField, env.GetStaticShortField));
            this.FieldCallers.Add(new FieldCaller<int>(env.GetIntField, env.GetStaticIntField));
            this.FieldCallers.Add(new FieldCaller<long>(env.GetLongField, env.GetStaticLongField));
            this.FieldCallers.Add(new FieldCaller<float>(env.GetFloatField, env.GetStaticFloatField));
            this.FieldCallers.Add(new FieldCaller<double>(env.GetDoubleField, env.GetStaticDoubleField));

            this.MethodCallers.Add(new MethodCaller<IntPtr>(env.CallObjectMethod, env.CallStaticObjectMethod));
            this.MethodCallers.Add(new MethodCaller<bool>(env.CallBooleanMethod, env.CallStaticBooleanMethod));
            this.MethodCallers.Add(new MethodCaller<byte>(env.CallByteMethod, env.CallStaticByteMethod));
            this.MethodCallers.Add(new MethodCaller<ushort>(env.CallCharMethod, env.CallStaticCharMethod));
            this.MethodCallers.Add(new MethodCaller<short>(env.CallShortMethod, env.CallStaticShortMethod));
            this.MethodCallers.Add(new MethodCaller<int>(env.CallIntMethod, env.CallStaticIntMethod));
            this.MethodCallers.Add(new MethodCaller<long>(env.CallLongMethod, env.CallStaticLongMethod));
            this.MethodCallers.Add(new MethodCaller<float>(env.CallFloatMethod, env.CallStaticFloatMethod));
            this.MethodCallers.Add(new MethodCaller<double>(env.CallDoubleMethod, env.CallStaticDoubleMethod));
        }

        internal T GetFieldValue<T>(IntPtr id, IntPtr fieldId, bool isStatic)
        {
            Type type = typeof(T);

            if (type == typeof(object) ||
                type == typeof(JObject) ||
                type == typeof(string)) // We need to read string as it is the object.
            {
                var objectCaller = this.FieldCallers.Where(call => call is FieldCaller<IntPtr>).FirstOrDefault();
                if (objectCaller != null)
                {
                    var fieldCaller = objectCaller as FieldCaller<IntPtr>;
                    IntPtr pointer = fieldCaller.Invoke(id, fieldId, isStatic);

                    // For string we need to read it.
                    if (type == typeof(string))
                    {
                        string str = this.Gateway.Runtime.JavaEnvironment.ReadJavaString(pointer);
                        return (T)(object)str;
                    }

                    JavaType javaType = JavaMapper.GetRegisteredJavaTypes().Where(jt => jt.CSharpType == type).FirstOrDefault();
                    if (javaType != null)
                    {
                        IntPtr javaClass = this.Gateway.Runtime.JavaEnvironment.JNIEnv.GetObjectClass(pointer);
                        return (T)(object)new JObject(this.Gateway, javaType.JavaTypeName, javaClass, pointer);
                    }
                }
            }
            else
            {
                var callers = this.FieldCallers.Where(call => call is FieldCaller<T>);
                var caller = callers.FirstOrDefault();

                if (caller != null)
                {
                    return (caller as FieldCaller<T>).Invoke(id, fieldId, isStatic);
                }
            }

            return default(T);
        }

        internal T InvokeMethod<T>(IntPtr id, IntPtr methodId, NativeValue[] parameters, bool isStatic)
        {
            Type type = typeof(T);

            if (type == typeof(object) ||
                type == typeof(JObject) ||
                type == typeof(string)) // We need to read string as it is the object.
            {
                var objectCaller = this.MethodCallers.Where(call => call is MethodCaller<IntPtr>).FirstOrDefault();
                if (objectCaller != null)
                {
                    var methodCaller = objectCaller as MethodCaller<IntPtr>;
                    IntPtr pointer = methodCaller.Invoke(id, methodId, parameters, isStatic);

                    // For string we need to read it.
                    if (type == typeof(string))
                    {
                        string str = this.Gateway.Runtime.JavaEnvironment.ReadJavaString(pointer);
                        return (T)(object)str;
                    }

                    JavaType javaType = JavaMapper.GetRegisteredJavaTypes().Where(jt => jt.CSharpType == type).FirstOrDefault();
                    if (javaType != null)
                    {
                        IntPtr javaClass = this.Gateway.Runtime.JavaEnvironment.JNIEnv.GetObjectClass(pointer);
                        return (T)(object)new JObject(this.Gateway, javaType.JavaTypeName, javaClass, pointer);
                    }
                }
            }
            else
            {
                var callers = this.MethodCallers.Where(call => call is MethodCaller<T>);
                var caller = callers.FirstOrDefault();

                if (caller != null)
                {
                    return (caller as MethodCaller<T>).Invoke(id, methodId, parameters, isStatic);
                }
            }

            return default(T);
        }
    }
}
