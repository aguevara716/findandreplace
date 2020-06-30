using GalaSoft.MvvmLight;

namespace FindAndReplace.Wpf.Models
{
    public class FindParameters : ObservableObject
    {
        private string encoding;
        public string Encoding
        {
            get { return encoding; }
            set { Set(nameof(Encoding), ref encoding, value); }
        }

        private string findString;
        public string FindString
        {
            get { return findString; }
            set { Set(nameof(FindString), ref findString, value); }
        }

        private bool isCaseSensitive;
        public bool IsCaseSensitive
        {
            get { return isCaseSensitive; }
            set { Set(nameof(IsCaseSensitive), ref isCaseSensitive, value); }
        }

        private bool isIncludingFilesWithoutMatches;
        public bool IsIncludingFilesWithoutMatches
        {
            get { return isIncludingFilesWithoutMatches; }
            set { Set(nameof(IsIncludingFilesWithoutMatches), ref isIncludingFilesWithoutMatches, value); }
        }

        private bool isRegex;
        public bool IsRegex
        {
            get { return isRegex; }
            set { Set(nameof(IsRegex), ref isRegex, value); }
        }

        private bool isRetainingModifiedDate;
        public bool IsRetainingModifiedDate
        {
            get { return isRetainingModifiedDate; }
            set { Set(nameof(IsRetainingModifiedDate), ref isRetainingModifiedDate, value); }
        }

        private bool isShowingEncoding;
        public bool IsShowingEncoding
        {
            get { return isShowingEncoding; }
            set { Set(nameof(IsShowingEncoding), ref isShowingEncoding, value); }
        }

        private bool isSkippingBinaryDetection;
        public bool IsSkippingBinaryDetection
        {
            get { return isSkippingBinaryDetection; }
            set { Set(nameof(IsSkippingBinaryDetection), ref isSkippingBinaryDetection, value); }
        }

        private bool isUsingEscapeCharacters;
        public bool IsUsingEscapeCharacters
        {
            get { return isUsingEscapeCharacters; }
            set { Set(nameof(IsUsingEscapeCharacters), ref isUsingEscapeCharacters, value); }
        }

    }
}
