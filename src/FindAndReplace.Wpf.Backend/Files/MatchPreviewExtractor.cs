using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FindAndReplace.Wpf.Backend.Extensions;
using FindAndReplace.Wpf.Backend.Results;

namespace FindAndReplace.Wpf.Backend.Files
{
    public interface IMatchPreviewExtractor
    {
        MatchPreviewExtractionResult ExtractMatchPreviews(string filePath, string fileContent, IList<TextMatch> textMatches);
    }

    public class MatchPreviewExtractor : IMatchPreviewExtractor
    {
        private const int PREVIEW_LINE_COUNT = 2;

        public MatchPreviewExtractionResult ExtractMatchPreviews(string filePath, string fileContent, IList<TextMatch> textMatches)
        {
            if (string.IsNullOrEmpty(filePath))
                return MatchPreviewExtractionResult.CreateFailure<MatchPreviewExtractionResult>(filePath, "File path must be provided");
            if (fileContent == null)
                return MatchPreviewExtractionResult.CreateFailure<MatchPreviewExtractionResult>(filePath, "File content must be provided");
            if (textMatches == null)
                return MatchPreviewExtractionResult.CreateFailure<MatchPreviewExtractionResult>(filePath, "Matches must be provided");

            if (fileContent.IsEmpty())
                return MatchPreviewExtractionResult.CreateSuccess<MatchPreviewExtractionResult>(filePath, new List<string>());
            if (!textMatches.Any())
                return MatchPreviewExtractionResult.CreateSuccess<MatchPreviewExtractionResult>(filePath, new List<string>());

            var fileContentLines = fileContent.SplitOnNewline();

            try
            {
                var previews = new List<string>();
                foreach (var textMatch in textMatches)
                {
                    var startingLineIndex = FindMatchLineNumber(fileContentLines, textMatch.StartIndex);
                    var endingLineIndex = FindMatchLineNumber(fileContentLines, textMatch.StartIndex + textMatch.Length);

                    // these 2 lines will avoid an index out of bounds exception
                    var startingPreviewLineIndex = Math.Max(startingLineIndex - PREVIEW_LINE_COUNT, 0);
                    var endingPreviewLineIndex = Math.Min(endingLineIndex + PREVIEW_LINE_COUNT, fileContentLines.Length - 1);

                    var lineNumbersLength = endingPreviewLineIndex.ToString().Length;
                    var contentStringBuilder = new StringBuilder();
                    for (var currentLineIndex = startingPreviewLineIndex; currentLineIndex <= endingPreviewLineIndex; currentLineIndex++)
                    {
                        var lineNumberString = (currentLineIndex + 1).ToString().PadLeft(lineNumbersLength);
                        contentStringBuilder.AppendLine($"{lineNumberString} {fileContentLines[currentLineIndex]}");
                    }
                    previews.Add(contentStringBuilder.ToString());
                }

                return MatchPreviewExtractionResult.CreateSuccess<MatchPreviewExtractionResult>(filePath, previews);
            }
            catch (Exception ex)
            {
                return MatchPreviewExtractionResult.CreateFailure<MatchPreviewExtractionResult>(filePath, ex);
            }
        }

        private int FindMatchLineNumber(string[] fileContentLines, int targetedCharacterIndex)
        {
            var newlineCharacterLength = Environment.NewLine.Length;
            var currentLineIndex = 0;
            var consumedCharacterCount = fileContentLines[currentLineIndex].Length + newlineCharacterLength;

            while (consumedCharacterCount <= targetedCharacterIndex)
            {
                currentLineIndex++;
                consumedCharacterCount += fileContentLines[currentLineIndex].Length + newlineCharacterLength;
            }

            return currentLineIndex;
        }

    }
}
