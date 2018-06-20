using KD.Dova.Proxy.Natives;
using System;

namespace KD.Dova.Core
{
    /// <summary>
    /// Represents Java Virtual Machine.
    /// </summary>
    public class JavaVM
    {
        private JavaVirtualMachine JVM { get; }

        public JavaVM(IntPtr ptr)
        {
            this.JVM = new JavaVirtualMachine(ptr);
        }
    }
}
