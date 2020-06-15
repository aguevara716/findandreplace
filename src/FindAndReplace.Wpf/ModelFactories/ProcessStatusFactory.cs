using System;
using System.Collections.Generic;
using FindAndReplace.Wpf.Models;

namespace FindAndReplace.Wpf.ModelFactories
{
    public static class ProcessStatusFactory
    {
        public static ProcessStatus GetProcessStatus(int seed)
        {
            var generator = new Random();
            var ps = new ProcessStatus
            {
                BinaryFilesCount = generator.Next(seed),
                EllapsedTime = TimeSpan.FromSeconds(generator.Next(seed)),
                FilesFailedToOpenCount = generator.Next(seed),
                FilesProcessedCount = generator.Next(seed),
                FilesWithMatchesCount = generator.Next(seed),
                FilesWithoutMatchesCount = generator.Next(seed),
                MatchesCount = generator.Next(seed),
                TotalFilesCount = generator.Next(seed) 
            };
            return ps;
        }

        public static IEnumerable<ProcessStatus> GetProcessStatuses(int count)
        {
            for (var index = 0; index < count; index++)
                yield return GetProcessStatus(index + 1);
        }
    }
}
