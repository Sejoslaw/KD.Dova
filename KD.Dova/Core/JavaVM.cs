using KD.Dova.Proxy.Natives;
using System;
using System.Runtime.InteropServices;

namespace KD.Dova.Core
{
    /// <summary>
    /// Represents Java Virtual Machine.
    /// </summary>
    public unsafe class JavaVM : IDisposable
    {
        public JavaVirtualMachine JVM { get; private set; }

        internal JavaVM(IntPtr ptr)
        {
            this.JVM = new JavaVirtualMachine(ptr);
        }

        public int GetEnvironment(out JavaEnvironment environment, int version)
        {
            IntPtr env;
            IntPtr result = this.JVM.GetEnv(out env, new IntPtr(version));
            environment = new JavaEnvironment(env);
            return result.ToInt32();
        }

        internal int AttachCurrentThread(out JavaEnvironment env, JavaVMInitArgs? args = null)
        {
            IntPtr envPointer;
            IntPtr result;

            if (args.HasValue)
            {
                JavaVMInitArgs initArgs = args.Value;
                result = this.JVM.AttachCurrentThread(out envPointer, &initArgs);
            }
            else
            {
                result = this.JVM.AttachCurrentThread(out envPointer, null);
            }

            env = new JavaEnvironment(envPointer);
            return result.ToInt32();
        }

        public void Dispose()
        {
            if (this.JVM.NativePointer != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(this.JVM.NativePointer);
                this.JVM = null;
            }
        }
    }
}
