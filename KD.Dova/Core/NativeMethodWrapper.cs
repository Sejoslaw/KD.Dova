using KD.Dova.Proxy.Natives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace KD.Dova.Core
{
    /// <summary>
    /// Wraps Java native method.
    /// </summary>
    internal unsafe class NativeMethodWrapper : IDisposable
    {
        internal JNINativeMethod method;

        public void Dispose()
        {
            if (this.method.name != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(this.method.name);
                this.method.name = IntPtr.Zero;
            }

            if (this.method.signature != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(this.method.signature);
                this.method.signature = IntPtr.Zero;
            }

            GC.SuppressFinalize(this);
        }

        public static NativeMethodWrapper NewNativeMethod(string javaName, string javaSignature, IntPtr functionPointer)
        {
            NativeMethodWrapper wrapper = new NativeMethodWrapper();

            wrapper.method = new JNINativeMethod
            {
                fnPtr = functionPointer,
                name = Marshal.StringToHGlobalAnsi(javaName),
                signature = Marshal.StringToHGlobalAnsi(javaSignature)
            };

            return wrapper;
        }

        public static NativeMethodWrapper NewNativeMethod(Type fromType, string javaName, string csharpMethodName, string javaSignature)
        {
            MethodInfo methodInfo = fromType.GetMethod(csharpMethodName, BindingFlags.NonPublic | BindingFlags.Static);
            if (methodInfo == null)
            {
                throw new ArgumentException($"Unknown method [{ csharpMethodName }] for type [{ fromType }]");
            }
            return NewNativeMethod(javaName, javaSignature, Marshal.GetFunctionPointerForDelegate(Delegate.CreateDelegate(GetDelegateType(methodInfo), methodInfo)));
        }

        public static Type GetDelegateType(MethodInfo method)
        {
            List<Type> args = new List<Type>(method.GetParameters().Select(p => p.ParameterType));

            Type delegateType;
            if (method.ReturnType == typeof(void))
            {
                delegateType = Expression.GetActionType(args.ToArray());
            }
            else
            {
                args.Add(method.ReturnType);
                delegateType = Expression.GetFuncType(args.ToArray());
            }
            return delegateType;
        }
    }
}
