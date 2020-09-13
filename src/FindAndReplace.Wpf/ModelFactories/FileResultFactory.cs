using System.Collections.Generic;
using System.Linq;
using FindAndReplace.Wpf.Models;

namespace FindAndReplace.Wpf.ModelFactories
{
    public static class FileResultFactory
    {
        private static readonly string[] EXTENSIONS = new string[]
        {
            ".txt", ".cs", ".pdf", ".docx"
        };

        private static IEnumerable<string> GetPreviews(int count, int seed)
        {
            for (var index = 0; index < count; index++)
                yield return $"Preview text #{index + 1} for item {seed}";
        }

        public static FileResult GetFileResult(int seed)
        {
            var extension = EXTENSIONS[seed % EXTENSIONS.Length];
            var filename = $"File-{seed}{extension}";
            var fr = new FileResult
            {
                ErrorMessage = $"Error message {seed}",
                Extension = extension,
                Filename = filename,
                FullPath = $@"C:\Root\Directory\Subdirectory\{filename}",
                HasError = seed % 2 == 0,
                Previews = GetPreviews(5, seed).ToList(),
                RelativePath = @"Directory\Subdirectory",
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
