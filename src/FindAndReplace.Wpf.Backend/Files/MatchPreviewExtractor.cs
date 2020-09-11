using System;
using System.Collections.Generic;
using System.Linq;
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
                return MatchPreviewExtractionResult.CreateSuccess<MatchPreviewExtractionResult>(filePath, new List<PreviewText>());
            if (!textMatches.Any())
                return MatchPreviewExtractionResult.CreateSuccess<MatchPreviewExtractionResult>(filePath, new List<PreviewText>());

            var lines = fileContent.Split(Environment.NewLine, StringSplitOptions.None);

            try
            {
                var previews = new List<PreviewText>();
                foreach (var textMatch in textMatches)
                {
                    var startingLineIndex = FindMatchLineNumber(lines, textMatch.StartIndex);
                    var endingLineIndex = FindMatchLineNumber(lines, textMatch.StartIndex + textMatch.Length);

                    var startingPreviewLineIndex = Math.Max(startingLineIndex - PREVIEW_LINE_COUNT, 0);
                    var endingPreviewLineIndex = Math.Min(endingLineIndex + PREVIEW_LINE_COUNT, lines.Length);

                    var pt = new PreviewText
                    {
                        MatchEndingLineNumber = endingLineIndex,
                        MatchStartingLineNumber = startingLineIndex
                    };
                    for (var currentLineIndex = startingPreviewLineIndex; currentLineIndex <= endingPreviewLineIndex; currentLineIndex++)
                    {
                        pt.Content += $"{lines[currentLineIndex]}{Environment.NewLine}";
                    }
                    previews.Add(pt);
                }

                return MatchPreviewExtractionResult.CreateSuccess<MatchPreviewExtractionResult>(filePath, previews);
            }
            catch (Exception ex)
            {
                return MatchPreviewExtractionResult.CreateFailure<MatchPreviewExtractionResult>(filePath, ex);
            }
        }

        private int FindMatchLineNumber(string[] fileContentLines, int matchStartIndex)
        {
            var separatorLength = Environment.NewLine.Length;
            var lineIndex = 0;

            var charsCount = fileContentLines[lineIndex].Length + separatorLength;
            while (charsCount <= matchStartIndex)
            {
                lineIndex++;
                charsCount += fileContentLines[lineIndex].Length + separatorLength;
            }

            return lineIndex;
        }

    }
}
