using KD.Dova.Proxy.Natives;
using System;
using System.Runtime.InteropServices;

namespace KD.Dova.Core
{
    /// <summary>
    /// Represents Java Environment.
    /// </summary>
    public unsafe class JavaEnvironment : IDisposable
    {
        public JavaVM JVM { get; private set; }
        public JNIEnvironment JNIEnv { get; private set; }

        public JavaEnvironment(IntPtr ptr)
        {
            this.JNIEnv = new JNIEnvironment(ptr);
        }

        public int GetVersion()
        {
            IntPtr ret = this.JNIEnv.GetVersion();
            int version = ret.ToInt32();
            return version;
        }

        public int GetMajorVersion()
        {
            return GetVersion() >> 16;
        }

        public int GetMinorVersion()
        {
            return GetVersion() % 65536;
        }

        public JavaVM GetJavaVM()
        {
            if (this.JVM == null)
            {
                IntPtr jvm;
                this.JNIEnv.GetJavaVM(out jvm);
                this.JVM = new JavaVM(jvm);
            }

            return this.JVM;
        }

        public string ReadJavaString(IntPtr javaString)
        {
            if (javaString != null)
            {
                byte b;
                IntPtr chars = this.JNIEnv.GetStringChars(javaString, &b);
                string ret = Marshal.PtrToStringUni(chars);
                this.JNIEnv.ReleaseStringChars(javaString, chars);
                return ret;
            }

            return null;
        }

        public void Dispose()
        {
            if (this.JVM != null)
            {
                this.JVM.Dispose();
                this.JVM = null;
            }

            if (this.JNIEnv != null)
            {
                Marshal.FreeCoTaskMem(this.JNIEnv.NativePointer);
                this.JNIEnv = null;
            }
        }
    }
}
