using System;
using System.Runtime.InteropServices;

namespace KD.Dova.Core
{
    public static class NativeHelper
    {
        /// <summary>
        /// Converts unmanaged function pointer to a delegate.
        /// </summary>
        public static void GetDelegateForFunctionPointer<T>(IntPtr ptr, ref T res)
        {
            res = (T)(object)Marshal.GetDelegateForFunctionPointer(ptr, typeof(T));
        }
    }
}
