﻿using System.Collections.Generic;

namespace KD.Dova.Generator.Definitions
{
    internal abstract class FileConvertable : IFileConvertable
    {
        public string Name { get; set; }

        public virtual List<string> ToFileDefinition()
        {
            List<string> fileLines = new List<string>();

            // File definition
            fileLines.Add("// This file has been machine generated.");
            fileLines.Add("// Generated by: KD.Dova.Generator");
            fileLines.Add("// For more information go to: https://github.com/Sejoslaw/KD.Dova");
            fileLines.Add("");
            fileLines.Add("");
            fileLines.Add("using System;");

            this.AddLibraries(fileLines);

            fileLines.Add("");
            fileLines.Add("namespace KD.Dova.Proxy.Natives");
            fileLines.Add("{");

            this.AddMainContent(fileLines);

            fileLines.Add("}");

            return fileLines;
        }

        internal abstract void AddLibraries(List<string> fileLines);
        internal abstract void AddMainContent(List<string> fileLines);
    }
}
