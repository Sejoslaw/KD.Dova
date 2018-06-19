using System.Collections.Generic;

namespace KD.Dova.Generator.Definitions
{
    internal interface IFileConvertable
    {
        string Name { get; set; }

        List<string> ToFileDefinition();
    }
}
