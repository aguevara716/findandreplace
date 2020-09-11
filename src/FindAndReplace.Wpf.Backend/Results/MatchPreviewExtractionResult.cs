using System.Collections.Generic;

namespace FindAndReplace.Wpf.Backend.Results
{
    public class MatchPreviewExtractionResult : BaseResult<List<PreviewText>>
    {
        public List<PreviewText> PreviewText { get { return Payload; } }
    }

}
