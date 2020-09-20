namespace FindAndReplace.Wpf.Backend.Results
{
    public class TextReplacementResult : BaseResult<string>
    {
        public string UpdatedContent { get { return Payload; } }
    }
}
