using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FindAndReplace.Wpf.Backend.Extensions;
using FindAndReplace.Wpf.Backend.Filesystem;
using FindAndReplace.Wpf.Models;

namespace FindAndReplace.Wpf.Mappers
{
    public interface IFileResultMapper
    {
        FileResult Map(string rootDirectory, string filePath);
        IList<FileResult> Map(string rootDirectory, IList<string> filePaths);
    }

    public class FileResultMapper : IFileResultMapper
    {
        private readonly IRelativePathExtractor _relativePathExtractor;

        public FileResultMapper(IRelativePathExtractor relativePathExtractor)
        {
            _relativePathExtractor = relativePathExtractor;
        }

        public FileResult Map(string rootDirectory, string filePath)
        {
            if (string.IsNullOrEmpty(rootDirectory))
                throw new ArgumentNullException(nameof(rootDirectory));
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            var fr = new FileResult
            {
                Extension = Path.GetExtension(filePath),
                Filename = Path.GetFileName(filePath),
                FullPath = filePath,
                RelativePath = _relativePathExtractor.GetRelativePathWithoutFilename(rootDirectory, filePath)
            };
            return fr;
        }

        public IList<FileResult> Map(string rootDirectory, IList<string> filePaths)
        {
            if (string.IsNullOrEmpty(rootDirectory))
                throw new ArgumentNullException(nameof(rootDirectory));
            if (filePaths.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(filePaths));

            var fileResultsDictionary = new ConcurrentDictionary<FileResult, bool>();
            Parallel.ForEach(filePaths, filePath =>
            {
                var fileResult = Map(rootDirectory, filePath);
                fileResultsDictionary.AddOrUpdate(fileResult, true, (s, b) => true);
            });

            var fileResultsCollection = fileResultsDictionary.Keys.OrderBy(fr => fr.FullPath).ToList();
            return fileResultsCollection;
        }

    }
}
