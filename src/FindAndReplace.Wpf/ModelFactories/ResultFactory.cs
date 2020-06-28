using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FindAndReplace.Wpf.Models;

namespace FindAndReplace.Wpf.ModelFactories
{
    public static class ResultFactory
    {
        public static Result GetResult(int seed)
        {
            var generator = new Random();
            var r = new Result
            {
                ErrorMessage = $"Error message {seed}",
                FailedToOpen = seed % 3 == 0,
                FileEncoding = Encoding.UTF8,
                Filename = $"File{seed}.abc",
                FilePath = $@"C:\Path\To\File{seed}.abc",
                FileRelativePath = $@"..\File{seed}.abc",
                IsBinaryFile = seed % 4 == 0,
                IsSuccess = seed % 2 == 0,
                MatchCount = generator.Next(seed),
                Matches = MatchFactory.GetMatches(seed % 4).ToList(),
                PreviewText = "HAHA PREVIEW GO BRRRR"
            };
            return r;
        }

        public static IEnumerable<Result> GetResults(int count)
        {
            for (var index = 0; index < count; index++)
                yield return GetResult(index + 1);
        }

    }
}
