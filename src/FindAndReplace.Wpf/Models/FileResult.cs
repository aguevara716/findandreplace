using System.Collections.Generic;

namespace FindAndReplace.Wpf.Models
{
    public class FileResult
    {
        public string FullPath { get; set; }
        public string Filename { get; set; }
        public string RelativePath { get; set; }
        public string Extension { get; set; }
        public List<string> Previews { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
