using KD.Dova.Api;
using System.Collections.Generic;

namespace KD.Dova.Utils
{
    /// <summary>
    /// Contains mapping for Java types.
    /// </summary>
    internal static class JavaMapper
    {
        private const string JAVA_ARRAY_FIELD_CHAR = "[";

        private static List<JavaType> JAVA_TYPES { get; set; }

        static JavaMapper()
        {
            JAVA_TYPES = new List<JavaType>();

            // Add basic Java types mappings
            JAVA_TYPES.Add(new JavaType("void", "V", typeof(void)));
            JAVA_TYPES.Add(new JavaType("boolean", "Z", typeof(bool)));
            JAVA_TYPES.Add(new JavaType("byte", "B", typeof(byte)));
            JAVA_TYPES.Add(new JavaType("char", "C", typeof(char)));
            JAVA_TYPES.Add(new JavaType("short", "S", typeof(short)));
            JAVA_TYPES.Add(new JavaType("int", "I", typeof(int)));
            JAVA_TYPES.Add(new JavaType("long", "J", typeof(long)));
            JAVA_TYPES.Add(new JavaType("float", "F", typeof(float)));
            JAVA_TYPES.Add(new JavaType("double", "D", typeof(double)));
            JAVA_TYPES.Add(new JavaType("string", "Ljava/lang/String;", typeof(string)));
            JAVA_TYPES.Add(new JavaType("object", "Ljava/lang/Object;", typeof(object)));
            JAVA_TYPES.Add(new JavaType("object", "Ljava/lang/Object;", typeof(JObject))); // We need to double register this because JObject is a wrapper for Java object.
        }

        /// <summary>
        /// Returns currently registered Java JNI types.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<JavaType> GetRegisteredJavaTypes()
        {
            var copy = new JavaType[JAVA_TYPES.Count];
            JAVA_TYPES.CopyTo(copy);
            return copy;
        }
    }
}
