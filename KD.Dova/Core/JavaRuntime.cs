using KD.Dova.Api;
using KD.Dova.Natives;
using KD.Dova.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        internal JavaEnvironment JavaEnvironment { get; private set; }

        private IGateway Gateway { get; set; }

        public void Load(IDictionary<string, string> parameters = null, int jniVersion = JNIConstants.JNI_VERSION_1_8, bool attachToExistingJVM = false)
        {
            string path = "";

            if (OS.IsWindows)
            {
                path = this.GetWindowsPath();
            }

            this.Load(path, parameters, jniVersion, attachToExistingJVM);
        }

        /// <summary>
        /// Loads Java Environment and Virtual Machine.
        /// </summary>
        /// <param name="path"> On Windows: Path to jvm.dll file. </param>
        /// <param name="parameters"> Optional arguments for virtual machine. </param>
        /// <param name="jniVersion"> Correct JNI version. See: <see cref="JNIConstants"/>. </param>
        /// <param name="attachToExistingJVM"></param>
        public void Load(string path, IDictionary<string, string> parameters = null, int jniVersion = JNIConstants.JNI_VERSION_1_8, bool attachToExistingJVM = false)
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

                this.JavaEnvironment = new JavaEnvironment(environment);
                this.JavaEnvironment.VirtualMachine = new JavaVM(javaVM);
            }
            else
            {
                this.AttachToExistingJVM(jvmInitArgs);
            }

            this.Gateway = new GatewayManager(this);
        }

        public string GetJavaVersion()
        {
            return this.JavaEnvironment.GetJavaVersion();
        }

        public void Dispose()
        {
            if (this.JavaEnvironment != null)
            {
                this.JavaEnvironment.Dispose();
                this.JavaEnvironment = null;
            }
        }

        #region Methods used to work with Java itself.

        /// <summary>
        /// Adds archive to runtime.
        /// </summary>
        /// <param name="pathOrName"></param>
        public void AddArchive(string pathOrName)
        {
            JClass javaClass = this.Gateway.LoadClass("java.lang.System");
            javaClass.InvokeStaticMethod<object>("loadLibrary", "V", pathOrName);
        }

        /// <summary>
        /// <see cref="IGateway"/>
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public JObject New(string typeName, params object[] parameters)
        {
            JObject obj = this.Gateway.New(typeName, parameters);
            return obj;
        }

        /// <summary>
        /// Returns Java type with optional generic parameters.
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="genericTypes"></param>
        /// <returns></returns>
        public JClass GetClass(string typeName, params object[] genericTypes)
        {
            JClass jt = this.Gateway.LoadClass(typeName, genericTypes);
            return jt;
        }

        #endregion

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
                this.JavaEnvironment.VirtualMachine = new JavaVM(javaVM);

                JavaEnvironment env;
                result = this.JavaEnvironment.VirtualMachine.AttachCurrentThread(out env, jvmInitArgs);
                this.JavaEnvironment = env;

                if (result != JNIConstants.JNI_OK)
                {
                    throw new InvalidOperationException($"Error when creating Java Virtual Machine. [AttachCurrentThread -> Return code: { result }].");
                }
            }
        }

        private string GetWindowsPath()
        {
            string path = @"C:\Program Files\Java";

            var subdirs = Directory.EnumerateDirectories(path);
            string newestJavaVersion = subdirs.OrderBy(name => name).ToList().LastOrDefault();

            path = Path.Combine(path, newestJavaVersion);

            if (newestJavaVersion.StartsWith("jdk"))
            {
                path = Path.Combine(path, "jre");
            }

            path = Path.Combine(path, @"bin\server\jvm.dll");

            return path;
        }
    }
}
