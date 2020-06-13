using System;

namespace FindAndReplace
{
    public class FinderEventArgs : EventArgs
	{
		public Finder.FindResultItem ResultItem { get; set; }
		public Stats Stats { get; set; }
		public Status Status { get; set; }
		public bool IsSilent { get; set; }

		public FinderEventArgs(Finder.FindResultItem resultItem, Stats stats, Status status)
			: this(resultItem, stats, status, false)
		{

		}

        public FinderEventArgs(Finder.FindResultItem resultItem,
							   Stats stats,
							   Status status,
							   bool isSilent)
        {
			ResultItem = resultItem;
			Stats = stats;
			Status = status;
			IsSilent = isSilent;
        }

	}
}
