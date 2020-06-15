using GalaSoft.MvvmLight;

namespace FindAndReplace.Wpf.Models
{
    public class FolderParameters : ObservableObject
    {
        private string excludeDirectories;
        public string ExcludeDirectories
        {
            get { return excludeDirectories; }
            set { Set(nameof(ExcludeDirectories), ref excludeDirectories, value); }
        }

        private string excludeMask;
        public string ExcludeMask
        {
            get { return excludeMask; }
            set { Set(nameof(ExcludeMask), ref excludeMask, value); }
        }

        private string fileMask;
        public string FileMask
        {
            get { return fileMask; }
            set { Set(nameof(FileMask), ref fileMask, value); }
        }

        private bool isRecursive;
        public bool IsRecursive
        {
            get { return isRecursive; }
            set { Set(nameof(IsRecursive), ref isRecursive, value); }
        }

        private string rootDirectory;
        public string RootDirectory
        {
            get { return rootDirectory; }
            set { Set(nameof(RootDirectory), ref rootDirectory, value); }
        }

    }
}
