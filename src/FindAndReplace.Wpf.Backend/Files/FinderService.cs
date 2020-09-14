using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FindAndReplace.Wpf.Backend.Results;

namespace FindAndReplace.Wpf.Backend.Files
{
    public interface IFinderService
    {
        Task<MatchPreviewExtractionResult> FindTextInFileAsync(string filePath, 
                                                               string findText, 
                                                               bool isRegexSearch, 
                                                               bool isUsingEscapeCharacters, 
                                                               bool isCaseSensitive);
        IEnumerable<MatchPreviewExtractionResult> FindTextInFiles(IDictionary<string, string> fileContentDictionary, 
                                                                  bool isRegexSearch, 
                                                                  bool isUsingEscapeCharacters, 
                                                                  bool isCaseSensitive);
    }

    public class FinderService : IFinderService
    {
        // Dependencies
        private readonly IBinaryFileDetector _binaryFileDetector;
        private readonly IFileReader _fileReader;
        private readonly IMatchPreviewExtractor _matchPreviewExtractor;
        private readonly ITextMatcher _textMatcher;

        // Constructors
        public FinderService(IBinaryFileDetector binaryFileDetector,
                             IFileReader fileReader,
                             IMatchPreviewExtractor matchPreviewExtractor,
                             ITextMatcher textMatcher)
        {
            _binaryFileDetector = binaryFileDetector;
            _fileReader = fileReader;
            _matchPreviewExtractor = matchPreviewExtractor;
            _textMatcher = textMatcher;
        }

        // Private Methods
        private MatchPreviewExtractionResult BuildFailure<T>(BaseResult<T> result)
        {
            return MatchPreviewExtractionResult.CreateFailure<MatchPreviewExtractionResult>(result.Path,
                                                                                            result.ErrorMessage,
                                                                                            result.Exception);
        }

        // Public Methods
        public async Task<MatchPreviewExtractionResult> FindTextInFileAsync(string filePath,
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
                return MatchPreviewExtractionResult.CreateFailure<MatchPreviewExtractionResult>(filePath, "Binary file detected");

            var fileContentResult = await _fileReader.GetFileContentAsync(filePath);
            if (!fileContentResult.IsSuccessful)
                return BuildFailure(fileContentResult);

            var textMatcherResult = _textMatcher.FindTextInFile(filePath,
                                                                findText,
                                                                fileContentResult.Content,
                                                                isRegexSearch,
                                                                isUsingEscapeCharacters,
                                                                isCaseSensitive);
            if (!textMatcherResult.IsSuccessful)
                return BuildFailure(textMatcherResult);
            
            var matchPreviewExtractionResult = _matchPreviewExtractor.ExtractMatchPreviews(filePath,
                                                                                           fileContentResult.Content,
                                                                                           textMatcherResult.TextMatches);
            return matchPreviewExtractionResult;
        }

        public IEnumerable<MatchPreviewExtractionResult> FindTextInFiles(IDictionary<string, string> fileContentDictionary,
                                                                         bool isRegexSearch,
                                                                         bool isUsingEscapeCharacters,
                                                                         bool isCaseSensitive)
        {
            var textMatchResultsDictionary = new ConcurrentDictionary<string, MatchPreviewExtractionResult>();
            Parallel.ForEach(fileContentDictionary, async fileContentKvp =>
            {
                var filePath = fileContentKvp.Key;
                var content = fileContentKvp.Value;

                var matchPreviewExtractionResult = await FindTextInFileAsync(filePath, content, isRegexSearch, isUsingEscapeCharacters, isCaseSensitive);
                textMatchResultsDictionary.AddOrUpdate(filePath, matchPreviewExtractionResult, (fp, oldResult) => matchPreviewExtractionResult);
            });

            var textMatchResultsCollection = textMatchResultsDictionary.Values.ToList();
            return textMatchResultsCollection;
        }

    }
}
