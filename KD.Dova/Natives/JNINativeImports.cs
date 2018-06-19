// This file has been machine generated.
// Generated by: KD.Dova.Generator
// For more information go to: https://github.com/Sejoslaw/KD.Dova


using System;
using System.Runtime.InteropServices;

namespace KD.Dova.Proxy.Natives
{
    internal static unsafe class JNINativeImports
    {
        [DllImport("jvm.dll", CallingConvention = CallingConvention.Winapi)]
        internal static extern int JNI_GetDefaultJavaVMInitArgs(JavaVMInitArgs* args);

        [DllImport("jvm.dll", CallingConvention = CallingConvention.Winapi)]
        internal static extern int JNI_CreateJavaVM(out IntPtr pVM, out IntPtr pEnv, JavaVMInitArgs* args);

        [DllImport("jvm.dll", CallingConvention = CallingConvention.Winapi)]
        internal static extern int JNI_GetCreatedJavaVMs(out IntPtr pVM, int jSize1, [Out] out int jSize2);

        [DllImport("jvm.dll", CallingConvention = CallingConvention.Winapi)]
        internal static extern int JNI_OnLoad(JavaVM_* javaVM);

        [DllImport("jvm.dll", CallingConvention = CallingConvention.Winapi)]
        internal static extern int JNI_OnUnload(JavaVM_* javaVM);

    }
}
