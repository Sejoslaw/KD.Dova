using KD.Dova.Api;
using KD.Dova.Core;

namespace KD.Dova.Tests.HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create Java Runtime
            using (JavaRuntime jre = new JavaRuntime())
            {
                // Load Java Virtual Machine
                jre.Load();

                // Get the "System" type, than get the static field "out" and than invoke method "println" with parameter.
                jre.GetType("java.lang.System").GetStaticFieldValue<JObject>("out", "java.io.PrintStream").Invoke<object>("println", "Hello from Java World !!!");
            }
        }
    }
}
