using System;

namespace KD.Dova.Generator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("You must specify path to \"jni.h\" file and path to output directory.");
                return;
            }

            GeneratorFactory.Instance.JniFilePath = args[0];
            Console.WriteLine($"Input file: { GeneratorFactory.Instance.JniFilePath }");

            GeneratorFactory.Instance.OutputDirectory = args[1];
            Console.WriteLine($"Output directory: { GeneratorFactory.Instance.OutputDirectory }");

            Console.WriteLine("Starting generators...");

            GeneratorFactory.Instance.StartGenerators();

            Console.WriteLine("Generators finished.");
        }
    }
}
