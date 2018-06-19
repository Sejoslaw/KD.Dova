using System;

namespace KD.Dova.Generator.Definitions
{
    internal class SingleImportDefinition : FunctionDefinition
    {
        public string ToAttribute()
        {
            if (this.IsLinux()) // Linux
            {
                return ""; // TODO: Prepare header for Linux.
            }
            else // Windows
            {
                return $"[DllImport(\"jvm.dll\", CallingConvention = CallingConvention.Winapi)]";
            }
        }

        private bool IsLinux()
        {
            int p = (int)Environment.OSVersion.Platform;
            return (p == 4) || (p == 6) || (p == 128);
        }
    }
}
