using System.Collections.Generic;
using System.Linq;

namespace FindAndReplace
{
    public partial class Finder
	{
        public class FindResult
		{
			public List<FindResultItem> Items { get; set; }
			public Stats Stats { get; set; }

			public List<FindResultItem> ItemsWithMatches
			{
				get 
				{ 
					return Items.Where(r => r.NumMatches > 0).ToList(); 
				}
			}

		}
	}
}
