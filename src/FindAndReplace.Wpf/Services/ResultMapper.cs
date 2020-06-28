using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FindAndReplace.Wpf.Models;

namespace FindAndReplace.Wpf.Services
{
    public interface IResultMapper
    {
        Result Map(Finder.FindResultItem findResultItem);
        Result Map(Replacer.ReplaceResultItem replaceResultItem);

        IEnumerable<Result> Map(IEnumerable<Finder.FindResultItem> findResultItems);
        IEnumerable<Result> Map(IEnumerable<Replacer.ReplaceResultItem> replaceResultItems);
    }

    public class ResultMapper : IResultMapper
    {
        private readonly IMatchPreviewGenerator matchPreviewGenerator;

        public ResultMapper(IMatchPreviewGenerator mpg)
        {
            matchPreviewGenerator = mpg;
        }

        private Result MapInternal(ResultItem resultItem)
        {
            var previewText = matchPreviewGenerator.GetMatchPreview(resultItem);
            var result = new Result
            {
                ErrorMessage = resultItem.ErrorMessage,
                FailedToOpen = resultItem.FailedToOpen,
                FileEncoding = resultItem.FileEncoding ?? Encoding.Default,
                Filename = resultItem.FileName,
                FilePath = resultItem.FilePath,
                FileRelativePath = resultItem.FileRelativePath.Replace(resultItem.FileName, String.Empty),
                IsBinaryFile = resultItem.IsBinaryFile,
                IsSuccess = resultItem.IsSuccess,
                MatchCount = resultItem.NumMatches,
                Matches = resultItem.Matches?.Select(lm => new Match
                {
                    Index = lm.Index,
                    Length = lm.Length
                }).ToList(),
                PreviewText = previewText
            };
            return result;
        }

        public Result Map(Finder.FindResultItem findResultItem)
        {
            var result = MapInternal(findResultItem);
            return result;
        }

        public Result Map(Replacer.ReplaceResultItem replaceResultItem)
        {
            var result = MapInternal(replaceResultItem);
            //result.FailedToWrite = replaceResultItem.FailedToWrite;
            return result;
        }

        public IEnumerable<Result> Map(IEnumerable<Finder.FindResultItem> findResultItems)
        {
            foreach (var findResultItem in findResultItems)
                yield return Map(findResultItem);
        }

        public IEnumerable<Result> Map(IEnumerable<Replacer.ReplaceResultItem> replaceResultItems)
        {
            foreach (var replaceResultItem in replaceResultItems)
                yield return Map(replaceResultItem);
        }

    }
}
