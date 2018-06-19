// This file has been machine generated.
// Generated by: KD.Dova.Generator
// For more information go to: https://github.com/Sejoslaw/KD.Dova


using System;
using System.Runtime.InteropServices;

namespace KD.Dova.Natives.Structures
{
    internal unsafe struct JNIInvokeInterface_
    {
        public IntPtr reserved0;
        public IntPtr reserved1;
        public IntPtr reserved2;

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate IntPtr AttachCurrentThread(JavaVM_ vm,IntPtr penv,IntPtr args);
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate IntPtr AttachCurrentThreadAsDaemon(JavaVM_ vm,IntPtr penv,IntPtr args);
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate IntPtr DestroyJavaVM(JavaVM_ vm);
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate IntPtr DetachCurrentThread(JavaVM_ vm);
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate IntPtr GetEnv(JavaVM_ vm,IntPtr penv,IntPtr version);
    }
}
