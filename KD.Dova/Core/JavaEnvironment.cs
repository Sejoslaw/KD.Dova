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
        internal JavaVM VirtualMachine { get; set; }
        internal JNIEnvironment JNIEnv { get; private set; }

        internal JavaEnvironment(IntPtr ptr)
        {
            this.JNIEnv = new JNIEnvironment(ptr);
        }

        public string GetJavaVersion()
        {
            int major = this.GetMajorVersion();
            int minor = this.GetMinorVersion();
            return $"{ major }.{ minor }";
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
            if (this.VirtualMachine == null)
            {
                IntPtr jvm;
                this.JNIEnv.GetJavaVM(out jvm);
                this.VirtualMachine = new JavaVM(jvm);
            }

            return this.VirtualMachine;
        }

        public void Dispose()
        {
            if (this.VirtualMachine != null)
            {
                this.VirtualMachine.Dispose();
                this.VirtualMachine = null;
            }

            if (this.JNIEnv != null)
            {
                Marshal.FreeCoTaskMem(this.JNIEnv.NativePointer);
                this.JNIEnv = null;
            }
        }

        internal string ReadJavaString(IntPtr javaString)
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
    }
}
