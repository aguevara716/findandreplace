using System.Collections.Generic;

namespace FindAndReplace
{

    public partial class Replacer
	{
        public class ReplaceResult
		{
			public List<ReplaceResultItem> ResultItems { get; set; }

			public Stats Stats { get; set; }
		}
	}
}
