using System;
using GalaSoft.MvvmLight;

namespace FindAndReplace.Wpf.Models
{
    public class ProcessStatus : ObservableObject
    {
        private int totalFilesCount;
        public int TotalFilesCount
        {
            get { return totalFilesCount; }
            set { Set(nameof(TotalFilesCount), ref totalFilesCount, value); }
        }

        private int filesProcessedCount;
        public int FilesProcessedCount
        {
            get { return filesProcessedCount; }
            set { Set(nameof(FilesProcessedCount), ref filesProcessedCount, value); }
        }

        private int binaryFilesCount;
        public int BinaryFilesCount
        {
            get { return binaryFilesCount; }
            set { Set(nameof(BinaryFilesCount), ref binaryFilesCount, value); }
        }

        private int filesWithMatchesCount;
        public int FilesWithMatchesCount
        {
            get { return filesWithMatchesCount; }
            set { Set(nameof(FilesWithMatchesCount), ref filesWithMatchesCount, value); }
        }

        private int filesWithoutMatchesCount;
        public int FilesWithoutMatchesCount
        {
            get { return filesWithoutMatchesCount; }
            set { Set(nameof(FilesWithoutMatchesCount), ref filesWithoutMatchesCount, value); }
        }

        private int filesFailedToOpenCount;
        public int FilesFailedToOpenCount
        {
            get { return filesFailedToOpenCount; }
            set { Set(nameof(FilesFailedToOpenCount), ref filesFailedToOpenCount, value); }
        }

        private int matchesCount;
        public int MatchesCount
        {
            get { return matchesCount; }
            set { Set(nameof(MatchesCount), ref matchesCount, value); }
        }

        private TimeSpan ellapsedTime;
        public TimeSpan EllapsedTime
        {
            get { return ellapsedTime; }
            set { Set(nameof(EllapsedTime), ref ellapsedTime, value); }
        }

    }
}
