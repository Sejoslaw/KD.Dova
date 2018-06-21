using KD.Dova.Natives;
using System;
using System.Runtime.InteropServices;

namespace KD.Dova.Utils
{
    /// <summary>
    /// Used to convert values from / to Java values. 
    /// </summary>
    public static class JavaConverter
    {
        public static bool ToBool(byte b)
        {
            return b == JNIConstants.JNI_TRUE ? true : false;
        }

        public static byte ToByte(bool b)
        {
            return b ? (byte)JNIConstants.JNI_TRUE : (byte)JNIConstants.JNI_FALSE;
        }

        /// <summary>
        /// Converts unmanaged function pointer to a delegate.
        /// </summary>
        public static void GetDelegateForFunctionPointer<T>(IntPtr ptr, ref T res)
        {
            res = (T)(object)Marshal.GetDelegateForFunctionPointer(ptr, typeof(T));
        }
    }
}
