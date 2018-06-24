using System;

namespace KD.Dova.Core.Callers
{
    /// <summary>
    /// Used to call single JNI method.
    /// </summary>
    internal class MethodCaller<T>
    {
        internal Type Type { get; }
        internal Func<IntPtr, IntPtr, NativeValue[], T> CallMethod { get; }
        internal Func<IntPtr, IntPtr, NativeValue[], T> CallStaticMethod { get; }

        internal MethodCaller(
            Func<IntPtr, IntPtr, NativeValue[], T> callMethod,
            Func<IntPtr, IntPtr, NativeValue[], T> callStaticMethod)
        {
            this.Type = typeof(T);
            this.CallMethod = callMethod;
            this.CallStaticMethod = callStaticMethod;
        }

        internal T Invoke(IntPtr clazz, IntPtr methodId, NativeValue[] parameters, bool isStatic)
        {
            if (isStatic)
            {
                return this.CallStaticMethod(clazz, methodId, parameters);
            }
            else
            {
                return this.CallMethod(clazz, methodId, parameters);
            }
        }
    }
}
