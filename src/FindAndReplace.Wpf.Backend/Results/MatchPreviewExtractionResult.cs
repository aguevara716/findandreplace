using System.Collections.Generic;

namespace FindAndReplace.Wpf.Backend.Results
{
    public class MatchPreviewExtractionResult : BaseResult<List<string>>
    {
        public List<string> Previews { get { return Payload; } }
    }

}
