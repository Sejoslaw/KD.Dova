// This file has been machine generated.
// Generated by: KD.Dova.Generator
// For more information go to: https://github.com/Sejoslaw/KD.Dova


using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace KD.Dova.Natives.Structures
{
    [StructLayout(LayoutKind.Sequential), NativeCppClass]
    internal unsafe struct JavaVMInitArgs
    {
        public IntPtr version;
        public IntPtr nOptions;
        public JavaVMOption* options;
        public IntPtr ignoreUnrecognized;
    }
}
