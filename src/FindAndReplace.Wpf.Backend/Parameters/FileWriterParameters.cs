namespace FindAndReplace.Wpf.Backend.Parameters
{
    public class FileWriterParameters
    {
        public string FilePath { get; set; }
        public bool IsKeepingOriginalModificationDate { get; set; }
        public string NewFileContent { get; set; }
    }
}
