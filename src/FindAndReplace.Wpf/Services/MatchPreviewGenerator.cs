using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FindAndReplace.Wpf.Services
{
    public interface IMatchPreviewGenerator
    {
        string GetMatchPreview(ResultItem resultItem);
    }

    public class MatchPreviewGenerator : IMatchPreviewGenerator
    {
        private string GetFileContents(string filePath, Encoding encoding)
        {
            using var reader = new StreamReader(filePath, encoding);
            var fileContent = reader.ReadToEnd();
            return fileContent;
        }

        private List<Int32> GetMatchPreviewLineNumbers(string fileContent, List<LiteMatch> matches)
        {
            var lineNumbers = Utils.GetLineNumbersForMatchesPreview(fileContent, matches);
            var matchLineNumbers = lineNumbers.Select(ln => ln.LineNumber).Distinct().OrderBy(i => i).ToList();
            return matchLineNumbers;
        }

        private string BuildPreviewText(string fileContent, List<Int32> lineNumbersWithMathces)
        {
            var filePreviewTextBuilder = new StringBuilder();
            var lines = fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            var previousLineIndex = 0;
            foreach(var lineNumberWithMatch in lineNumbersWithMathces)
            {
                if (previousLineIndex != 0 && lineNumberWithMatch - previousLineIndex > 1)
                {
                    filePreviewTextBuilder.AppendLine(String.Empty);
                    filePreviewTextBuilder.AppendLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                    filePreviewTextBuilder.AppendLine(String.Empty);
                }

                filePreviewTextBuilder.AppendLine(lines[lineNumberWithMatch]);
                previousLineIndex = lineNumberWithMatch;
            }

            return filePreviewTextBuilder.ToString();
        }

        public string GetMatchPreview(ResultItem resultItem)
        {
            var shouldGeneratePreview = resultItem.IsSuccess && resultItem.NumMatches > 0;
            if (!shouldGeneratePreview)
                return String.Empty;

            var fileContent = GetFileContents(resultItem.FilePath, resultItem.FileEncoding);
            var matchLineNumbers = GetMatchPreviewLineNumbers(fileContent, resultItem.Matches);
            var previewText = BuildPreviewText(fileContent, matchLineNumbers);
            return previewText;
        }

    }
}
