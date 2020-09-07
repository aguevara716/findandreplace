using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FindAndReplace.Wpf.Backend.Results;

namespace FindAndReplace.Wpf.Backend.Files
{
    public interface IFinderService
    {
        Task<TextMatcherResult> FindTextInFileAsync(string filePath, string findText, bool isRegexSearch, bool isUsingEscapeCharacters, bool isCaseSensitive);
        IEnumerable<TextMatcherResult> FindTextInFiles(IDictionary<string, string> fileContentDictionary, bool isRegexSearch, bool isUsingEscapeCharacters, bool isCaseSensitive);
    }

    public class FinderService : IFinderService
    {
        // Dependencies
        private readonly IBinaryFileDetector _binaryFileDetector;
        private readonly IEncodingDetector _encodingDetector;
        private readonly IFileReader _fileReader;
        private readonly ITextMatcher _textMatcher;

        // Constructors
        public FinderService(IBinaryFileDetector binaryFileDetector,
                             IEncodingDetector encodingDetector,
                             IFileReader fileReader,
                             ITextMatcher textMatcher)
        {
            _binaryFileDetector = binaryFileDetector;
            _encodingDetector = encodingDetector;
            _fileReader = fileReader;
            _textMatcher = textMatcher;
        }

        private TextMatcherResult BuildFailure<T>(BaseResult<T> result)
        {
            return TextMatcherResult.CreateFailure<TextMatcherResult>(result.Path,
                                                                      result.ErrorMessage,
                                                                      result.Exception);
        }

        // Public Methods
        public async Task<TextMatcherResult> FindTextInFileAsync(string filePath,
                                                                 string findText,
                                                                 bool isRegexSearch,
                                                                 bool isUsingEscapeCharacters,
                                                                 bool isCaseSensitive)
        {
            var fileSampleResult = _fileReader.GetFileSampleData(filePath);
            if (!fileSampleResult.IsSuccessful)
                return BuildFailure(fileSampleResult);

            var binaryFileDetectionResult = _binaryFileDetector.CheckIsBinaryFile(filePath, fileSampleResult.Sample);
            if (!binaryFileDetectionResult.IsSuccessful)
                return BuildFailure(binaryFileDetectionResult);
            if (binaryFileDetectionResult.IsBinaryFile)
                return TextMatcherResult.CreateFailure<TextMatcherResult>(filePath, "Binary file detected");

            var encodingDetectionResult = _encodingDetector.DetectFileEncoding(filePath, fileSampleResult.Sample);
            if (!encodingDetectionResult.IsSuccessful)
                return BuildFailure(encodingDetectionResult);

            var fileContentResult = await _fileReader.GetFileContentAsync(filePath);
            if (!fileContentResult.IsSuccessful)
                return BuildFailure(fileContentResult);

            var textMatcherResult = _textMatcher.FindTextInFile(filePath,
                                                                findText,
                                                                fileContentResult.Content,
                                                                isRegexSearch,
                                                                isUsingEscapeCharacters,
                                                                isCaseSensitive);
            return textMatcherResult;
        }

        public IEnumerable<TextMatcherResult> FindTextInFiles(IDictionary<string, string> fileContentDictionary,
                                                              bool isRegexSearch,
                                                              bool isUsingEscapeCharacters,
                                                              bool isCaseSensitive)
        {
            var textMatchResultsDictionary = new ConcurrentDictionary<string, TextMatcherResult>();
            Parallel.ForEach(fileContentDictionary, async fileContentKvp =>
            {
                var filePath = fileContentKvp.Key;
                var content = fileContentKvp.Value;

                var textMatchResult = await FindTextInFileAsync(filePath, content, isRegexSearch, isUsingEscapeCharacters, isCaseSensitive);
                textMatchResultsDictionary.AddOrUpdate(filePath, textMatchResult, (fp, oldResult) => textMatchResult);
            });

            var textMatchResultsCollection = textMatchResultsDictionary.Values.ToList();
            return textMatchResultsCollection;
        }

    }
}
