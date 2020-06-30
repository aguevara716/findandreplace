using System.Collections.Generic;
using FindAndReplace.Wpf.Models;

namespace FindAndReplace.Wpf.Services
{
    public interface IProcessStatusMapper
    {
        ProcessStatus Map(Stats stats);
        IEnumerable<ProcessStatus> Map(IEnumerable<Stats> stats);
    }

    public class ProcessStatusMapper : IProcessStatusMapper
    {
        public ProcessStatus Map(Stats stats)
        {
            var processStatus = new ProcessStatus
            {
                BinaryFilesCount = stats.Files.Binary,
                EllapsedTime = stats.Time.Passed,
                FilesFailedToOpenCount = stats.Files.FailedToRead,
                FilesProcessedCount = stats.Files.Processed,
                FilesWithMatchesCount = stats.Files.WithMatches,
                FilesWithoutMatchesCount = stats.Files.WithoutMatches,
                MatchesCount = stats.Matches.Found,
                RemainingTime = stats.Time.Remaining,
                TotalFilesCount = stats.Files.Total
            };
            return processStatus;
        }

        public IEnumerable<ProcessStatus> Map(IEnumerable<Stats> stats)
        {
            foreach (var stat in stats)
                yield return Map(stat);
        }

    }
}
