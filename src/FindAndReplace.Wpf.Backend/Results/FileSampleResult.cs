namespace FindAndReplace.Wpf.Backend.Results
{
    public class FileSampleResult : BaseResult<byte[]>
    {
        public byte[] Sample { get { return Payload; } }
    }
}
