namespace FindAndReplace.Wpf.Backend.Results
{
    public class FileWriterResult : BaseResult<bool>
    {
        public bool IsFileUpdated { get { return Payload; } }
    }
}
