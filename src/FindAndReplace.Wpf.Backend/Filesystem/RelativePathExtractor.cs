using System;
using System.IO;

namespace FindAndReplace.Wpf.Backend.Filesystem
{
    public interface IRelativePathExtractor
    {
        string GetRelativePathWithoutFilename(string rootDirectory, string fullPath);
    }

    public class RelativePathExtractor : IRelativePathExtractor
    {
        public string GetRelativePathWithoutFilename(string rootDirectory, string fullPath)
        {
            if (rootDirectory == null)
                throw new ArgumentNullException(nameof(rootDirectory));
            if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentNullException(nameof(fullPath), $"{nameof(fullPath)} cannot be null or empty");

            if (rootDirectory == string.Empty)
                return fullPath;

            var rootDirectoryEndsWithDirectorySeparatorChar = rootDirectory.EndsWith(Path.DirectorySeparatorChar);

            var substringStartIndex = rootDirectory.Length;
            if (!rootDirectoryEndsWithDirectorySeparatorChar)
                substringStartIndex++;

            var substringLength = fullPath.LastIndexOf(Path.DirectorySeparatorChar) - rootDirectory.Length;
            if (rootDirectoryEndsWithDirectorySeparatorChar)
                substringLength++;

            var relativePath = fullPath.Substring(substringStartIndex, substringLength);
            return relativePath;
        }

    }
}
