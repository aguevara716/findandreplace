using System.Collections.Generic;
using FindAndReplace.Wpf.Models;

namespace FindAndReplace.Wpf.ModelFactories
{
    public static class FileResultFactory
    {
        private static readonly string[] EXTENSIONS = new string[]
        {
            ".txt", ".cs", ".pdf", ".docx"
        };

        public static FileResult GetFileResult(int seed)
        {
            var extension = EXTENSIONS[seed % EXTENSIONS.Length];
            var filename = $"File-{seed}{extension}";
            var fr = new FileResult
            {
                Extension = extension,
                Filename = filename,
                FullPath = $@"C:\Root\Directory\Subdirectory\{filename}",
                RelativePath = @"Directory\Subdirectory"
            };
            return fr;
        }

        public static IEnumerable<FileResult> GetFileResults(int count)
        {
            for (var index = 0; index < count; index++)
                yield return GetFileResult(index + 1);
        }
    }
}
