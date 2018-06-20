using KD.Dova.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace KD.Dova.Tests.BasicTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Java Runtime...");

            using (JavaRuntime jre = new JavaRuntime())
            {
                Console.WriteLine("Loading library...");

                Console.WriteLine("Adding custom options to Java Runtime...");
                var options = new Dictionary<string, string>();
                options.Add("-Djava.class.path", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

                jre.Load(@"C:\Program Files\Java\jdk1.8.0_172\jre\bin\server\jvm.dll", options);

                Console.WriteLine("Java Runtime initialized.");
            }
        }
    }
}
