using GalaSoft.MvvmLight;

namespace FindAndReplace.Wpf.Models
{
    public class FindParameters : ObservableObject
    {
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

        private bool isOnlyShowingFilesWithoutMatches;
        public bool IsOnlyShowingFilesWithoutMatches
        {
            get { return isOnlyShowingFilesWithoutMatches; }
            set { Set(nameof(IsOnlyShowingFilesWithoutMatches), ref isOnlyShowingFilesWithoutMatches, value); }
        }

        private bool isRegex;
        public bool IsRegex
        {
            get { return isRegex; }
            set { Set(nameof(IsRegex), ref isRegex, value); }
        }

        private bool isSearchingFilenameOnly;
        public bool IsSearchingFilenameOnly 
        {
            get { return isSearchingFilenameOnly; }
            set { Set(nameof(IsSearchingFilenameOnly), ref isSearchingFilenameOnly, value); }
        }

        private bool isShowingErrors;
        public bool IsShowingErrors
        {
            get { return isShowingErrors; }
            set { Set(nameof(IsShowingErrors), ref isShowingErrors, value); }
        }

        private bool isUsingEscapeCharacters;
        public bool IsUsingEscapeCharacters
        {
            get { return isUsingEscapeCharacters; }
            set { Set(nameof(IsUsingEscapeCharacters), ref isUsingEscapeCharacters, value); }
        }
    }
}
