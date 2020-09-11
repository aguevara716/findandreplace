using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FindAndReplace.Wpf.Backend.Tests.Files
{
    public class FilePathGenerator
    {
        // Get Real Files
        public static IEnumerable<string> GetRealFiles()
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            return GetRealFiles(rootDirectory);
        }

        public static IEnumerable<string> GetRealFiles(string rootDirectory)
        {
            return GetRealFiles(rootDirectory, "*");
        }

        public static IEnumerable<string> GetRealFiles(string rootDirectory, string searchPattern)
        {
            var files = Directory.GetFiles(rootDirectory, searchPattern);
            return files;
        }

        // Generate Fake File Paths
        private static readonly string[] EXTENSIONS = new string[]
        {
            ".cs", 
            ".exe",
            ".pdf",
            ".txt",
            ".zip"
        };

        public static IEnumerable<string> GenerateFilePaths(int count)
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            return GenerateFilePaths(rootDirectory, count);
        }

        public static IEnumerable<string> GenerateFilePaths(string rootDirectory, int count)
        {
            var paths = Enumerable.Range(0, count).AsParallel().Select(index =>
            {
                var extension = EXTENSIONS[index % EXTENSIONS.Length];
                var path = Path.Combine(rootDirectory, $"File-{index}{extension}");
                return path;
            }).OrderBy(s => s);
            return paths;
        }

        public static IEnumerable<string> GenerateRecursiveFilePaths(int count, params string[] directories)
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            var generatedPaths = new HashSet<string>();
            foreach (var directory in directories)
            {
                rootDirectory = Path.Combine(rootDirectory, directory);
                var paths = GenerateFilePaths(rootDirectory, count);
                generatedPaths.Union(paths);
            }
            return generatedPaths;
        }

        // Generate Real Files
        public static void GenerateFiles(IEnumerable<string> paths)
        {
            Parallel.ForEach(paths, path =>
            {
                File.Create(path);
            });
        }
    }
}
