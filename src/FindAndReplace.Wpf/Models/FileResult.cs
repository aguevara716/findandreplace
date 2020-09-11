using System.Collections.Generic;
using FindAndReplace.Wpf.Backend.Results;

namespace FindAndReplace.Wpf.Models
{
    public class FileResult
    {
        public string FullPath { get; set; }
        public string Filename { get; set; }
        public string RelativePath { get; set; }
        public string Extension { get; set; }
        public List<TextMatch> TextMatches { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
