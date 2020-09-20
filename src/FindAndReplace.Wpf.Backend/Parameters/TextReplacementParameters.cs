namespace FindAndReplace.Wpf.Backend.Parameters
{
    public class TextReplacementParameters
    {
        public string FileContent { get; set; }
        public string FilePath { get; set; }
        public string FindText { get; set; }
        public bool IsCaseSensitive { get; set; }
        public bool IsRegexSearch { get; set; }
        public bool IsUsingEscapeCharacters { get; set; }
        public string ReplaceText { get; set; }
    }
}
