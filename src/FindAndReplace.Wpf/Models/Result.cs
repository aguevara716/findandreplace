using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;

namespace FindAndReplace.Wpf.Models
{
    public class Result : ObservableObject
    {
        private string filename;
        public string Filename
        {
            get { return filename; }
            set { Set(nameof(Filename), ref filename, value); }
        }

        private string filePath;
        public string FilePath
        { 
            get { return filePath; }
            set { Set(nameof(FilePath), ref filePath, value); }
        }

        private string fileRelativePath;
        public string FileRelativePath
        {
            get { return fileRelativePath; }
            set { Set(nameof(FileRelativePath), ref fileRelativePath, value); }
        }
        
        private Encoding fileEncoding;
        public Encoding FileEncoding
        {
            get { return fileEncoding; }
            set { Set(nameof(FileEncoding), ref fileEncoding, value); }
        }
        
        private int matchCount;
        public int MatchCount
        {
            get { return matchCount; }
            set { Set(nameof(MatchCount), ref matchCount, value); }
        }
        
        private List<Match> matches;
        public List<Match> Matches
        {
            get { return matches; }
            set { Set(nameof(Matches), ref matches, value); }
        }
        
        private bool isSuccess;
        public bool IsSuccess
        {
            get { return isSuccess; }
            set { Set(nameof(IsSuccess), ref isSuccess, value); }
        }
        
        private bool isBinaryFile;
        public bool IsBinaryFile
        {
            get { return isBinaryFile; }
            set { Set(nameof(IsBinaryFile), ref isBinaryFile, value); }
        }
        
        private bool failedToOpen;
        public bool FailedToOpen
        {
            get { return failedToOpen; }
            set { Set(nameof(FailedToOpen), ref failedToOpen, value); }
        }
        
        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { Set(nameof(ErrorMessage), ref errorMessage, value); }
        }

        private string previewText;
        public string PreviewText
        {
            get { return previewText; }
            set { Set(nameof(PreviewText), ref previewText, value); }
        }

    }
}
