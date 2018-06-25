using KD.Dova.Api;
using KD.Dova.Core;
using System;

namespace KD.Dova.Tests.GetJavaVersionFromJava
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

                // This is equal with -> String myValue = System.getProperty("java.version")
                // Splitted if for easier debugging.
                JClass javaType = jre.GetClass("java.lang.System");
                // Invokes static method "getProperty" with parameter "java.version" which returns "java.lang.String" and casts it to generic parameter "string".
                string javaVersion = javaType.InvokeStaticMethod<string>("getProperty", "java.lang.String", "java.version");

                Console.WriteLine($"Currently used Java version: { javaVersion }");
            }
        }
    }
}
