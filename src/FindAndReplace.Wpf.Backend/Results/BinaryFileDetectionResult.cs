namespace FindAndReplace.Wpf.Backend.Results
{
    public class BinaryFileDetectionResult : BaseResult<bool>
    {
        public bool IsBinaryFile { get { return Payload; } }
    }
}
