using System;

namespace KD.Dova.Commons
{
    /// <summary>
    /// Shortform of "Operating System".
    /// Contains definitions on which OS we are currently working.
    /// 
    /// TODO: Add support for more operating systems and add generators for native calls in NativeImportGenerator.
    /// </summary>
    public static class OS
    {
        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        public static bool IsWindows
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 0) || (p == 1) || (p == 2) || (p == 3);
            }
        }
    }
}
