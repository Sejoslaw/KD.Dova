using KD.Dova.Proxy.Natives;
using KD.Dova.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace KD.Dova.Core
{
    /// <summary>
    /// This is the entry point of the whole KD.Dova library.
    /// This will allow You to work with Java.
    /// 
    /// First thing to do after initializing JavaRuntime is to call Load method.
    /// </summary>
    public unsafe sealed class JavaRuntime : IDisposable
    {
        public JavaEnvironment Environment { get; private set; }
        public JavaVM VirtualMachine { get; private set; }

        /// <summary>
        /// Loads Java Environemtn and Virtual Machine.
        /// </summary>
        /// <param name="path"> On Windows: Path to jvm.dll file. </param>
        /// <param name="jniVersion"> Correct JNI version. See: <see cref="JNIConstants"/>. </param>
        /// <param name="parameters"> Optional arguments for virtual machine. </param>
        /// <param name="attachToExistingJVM"></param>
        public void Load(string path, int jniVersion = JNIConstants.JNI_VERSION_1_8, IDictionary<string, string> parameters = null, bool attachToExistingJVM = false)
        {
            if (string.IsNullOrEmpty(path) ||
                !File.Exists(path))
            {
                throw new ArgumentException("You must specify the correct location of JVM library.", "path");
            }

            // Set the working directory to directory with JVM library.
            Directory.SetCurrentDirectory(Path.GetDirectoryName(path));

            var jvmInitArgs = new JavaVMInitArgs
            {
                version = jniVersion,
                ignoreUnrecognized = JavaConverter.ToByte(true)
            };

            if (parameters != null &&
                parameters.Count > 0)
            {
                jvmInitArgs.nOptions = parameters.Count;
                var options = new List<JavaVMOption>();

                foreach (var kvp in parameters)
                {
                    options.Add(new JavaVMOption
                    {
                        optionString = Marshal.StringToHGlobalAnsi($"{ kvp.Key }={ kvp.Value }")
                    });
                }

                JavaVMOption[] parsedOptions = options.ToArray();
                fixed (JavaVMOption* a = &parsedOptions[0])
                {
                    jvmInitArgs.options = a;
                }
            }

            if (!attachToExistingJVM)
            {
                IntPtr environment;
                IntPtr javaVM;

                int result = JNINativeImports.JNI_CreateJavaVM(out javaVM, out environment, &jvmInitArgs);
                if (result != JNIConstants.JNI_OK)
                {
                    throw new InvalidOperationException($"Error when creating Java Virtual Machine. [JNI_CreateJavaVM -> Return code: { result }].");
                }

                this.Environment = new JavaEnvironment(environment);
                this.VirtualMachine = new JavaVM(javaVM);
            }
            else
            {
                this.AttachToExistingJVM(jvmInitArgs);
            }
        }

        private void AttachToExistingJVM(JavaVMInitArgs jvmInitArgs)
        {
            int virtualMachines;
            IntPtr javaVM;

            int result = JNINativeImports.JNI_GetCreatedJavaVMs(out javaVM, 1, out virtualMachines);
            if (result != JNIConstants.JNI_OK)
            {
                throw new InvalidOperationException($"Error when creating Java Virtual Machine. [JNI_GetCreatedJavaVMs -> Return code: { result }].");
            }

            if (virtualMachines > 0)
            {
                this.VirtualMachine = new JavaVM(javaVM);

                JavaEnvironment env;
                result = this.VirtualMachine.AttachCurrentThread(out env, jvmInitArgs);
                this.Environment = env;

                if (result != JNIConstants.JNI_OK)
                {
                    throw new InvalidOperationException($"Error when creating Java Virtual Machine. [AttachCurrentThread -> Return code: { result }].");
                }
            }
        }

        public void Dispose()
        {
            if (this.Environment != null)
            {
                this.Environment.Dispose();
                this.Environment = null;
            }

            if (this.VirtualMachine != null)
            {
                this.VirtualMachine.Dispose();
                this.VirtualMachine = null;
            }
        }
    }
}
