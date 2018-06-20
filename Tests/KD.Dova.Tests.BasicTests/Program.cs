using KD.Dova.Core;
using System;

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

                jre.Load(@"C:\Program Files\Java\jdk1.8.0_172\jre\bin\server\jvm.dll");

                Console.WriteLine("Java Runtime initialized.");
            }
        }
    }
}
