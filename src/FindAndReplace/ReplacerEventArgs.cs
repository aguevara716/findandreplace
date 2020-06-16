using System;

namespace FindAndReplace
{
    public class ReplacerEventArgs : EventArgs
	{
		public Replacer.ReplaceResultItem ResultItem { get; set; }
		public Stats Stats { get; set; }
		public Status Status { get; set; }
		public bool IsSilent { get; set; }

		public ReplacerEventArgs(Replacer.ReplaceResultItem resultItem, Stats stats, Status status, bool isSilent = false)
		{
			ResultItem = resultItem;
			Stats = stats;
			Status = status;
			IsSilent = isSilent;
		}
	}
}
