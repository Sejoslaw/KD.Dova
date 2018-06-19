using System;
using System.Runtime.InteropServices;

namespace KD.Dova.Natives
{
    /// <summary>
    /// Representa a Java native value.
    /// This is equal with "jni.h" -> "jvalue" union.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct NativeValue
    {
        [FieldOffset(0)]
        public byte z;
        [FieldOffset(0)]
        public byte b;
        [FieldOffset(0)]
        public char c;
        [FieldOffset(0)]
        public short s;
        [FieldOffset(0)]
        public int i;
        [FieldOffset(0)]
        public long j;
        [FieldOffset(0)]
        public float f;
        [FieldOffset(0)]
        public double d;
        [FieldOffset(0)]
        public IntPtr l;
    }
}
