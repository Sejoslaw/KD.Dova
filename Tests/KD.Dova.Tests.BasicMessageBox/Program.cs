using KD.Dova.Core;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace KD.Dova.Tests.BasicMessageBox
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create Java Runtime Environment
            using (JavaRuntime jre = new JavaRuntime())
            {
                var options = new Dictionary<string, string>();
                options.Add("-Djava.class.path", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

                // Load Java Virtual Machine
                jre.Load(options);
                // Call sample Java static method.
                jre.GetType("JOptionPane").InvokeStaticMethod<object>("showMessageDialog", null, "Hello C# from Java <3");
            }
        }
    }
}
