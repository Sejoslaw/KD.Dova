using KD.Dova.Proxy.Natives;

namespace KD.Dova.Utils
{
    /// <summary>
    /// Used to convert values from / to Java values. 
    /// </summary>
    public static class JavaConverter
    {
        public static bool ToBool(byte b)
        {
            return b == JNIConstants.JNI_TRUE ? true : false;
        }

        public static byte ToByte(bool b)
        {
            return b ? (byte)JNIConstants.JNI_TRUE : (byte)JNIConstants.JNI_FALSE;
        }
    }
}
