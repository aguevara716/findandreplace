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
                yield return $"({index + 1} Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. In hendrerit gravida rutrum quisque non tellus orci ac auctor. Quam id leo in vitae turpis massa sed elementum tempus. Integer feugiat scelerisque varius morbi. Malesuada fames ac turpis egestas maecenas pharetra convallis posuere morbi.\n\nAccumsan tortor posuere ac ut consequat semper viverra nam libero. Tortor consequat id porta nibh venenatis cras. Ac turpis egestas maecenas pharetra convallis posuere. Augue eget arcu dictum varius duis at. Diam in arcu cursus euismod quis viverra nibh. Sed turpis tincidunt id aliquet risus feugiat in ante. Et netus et malesuada fames ac turpis egestas.\n\nScelerisque mauris pellentesque pulvinar pellentesque habitant morbi tristique senectus et. Sit amet nisl purus in mollis. Feugiat pretium nibh ipsum consequat.\n\nPurus sit amet volutpat consequat mauris. Orci phasellus egestas tellus rutrum.";
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
