using System;

namespace KD.Dova.Core.Callers
{
    /// <summary>
    /// Used to call single JNI method.
    /// </summary>
    internal class FieldCaller<T>
    {
        internal Type Type { get; }
        internal Func<IntPtr, IntPtr, T> GetFieldId { get; }
        internal Func<IntPtr, IntPtr, T> GetStaticFieldId { get; }

        internal FieldCaller(Func<IntPtr, IntPtr, T> GetFieldId, Func<IntPtr, IntPtr, T> GetStaticFieldId)
        {
            this.Type = typeof(T);
            this.GetFieldId = GetFieldId;
            this.GetStaticFieldId = GetStaticFieldId;
        }

        internal T Invoke(IntPtr clazz, IntPtr fieldId, bool isStatic)
        {
            if (isStatic)
            {
                return this.GetStaticFieldId(clazz, fieldId);
            }
            else
            {
                return this.GetFieldId(clazz, fieldId);
            }
        }
    }
}
