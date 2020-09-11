namespace FindAndReplace.Wpf.Backend.Results
{
    public class FileContentResult : BaseResult<string>
    {
        public string Content { get { return Payload; } }
    }
}
