using System.Collections.Generic;

namespace FindAndReplace.Wpf.Backend.Results
{
    public class TextMatcherResult : BaseResult<IList<TextMatch>>
    {
        public IList<TextMatch> TextMatches { get { return Payload; } }
    }
}
