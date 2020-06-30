using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace FindAndReplace.Wpf.Models
{
    public class FolderParameters : ObservableObject
    {
        private ObservableCollection<String> excludeDirectories;
        public ObservableCollection<String> ExcludeDirectories
        {
            get { return excludeDirectories; }
            set { Set(nameof(ExcludeDirectories), ref excludeDirectories, value); }
        }

        public string ExcludeDirectoriesString
        {
            get { return String.Join(", ", ExcludeDirectories); }
        }

        private ObservableCollection<String> excludeFiles;
        public ObservableCollection<String> ExcludeFiles
        {
            get { return excludeFiles; }
            set { Set(nameof(ExcludeFiles), ref excludeFiles, value); }
        }

        public string ExcludeFilesString
        {
            get { return String.Join(", ", ExcludeFiles); }
        }

        private ObservableCollection<String> includeFiles;
        public ObservableCollection<String> IncludeFiles
        {
            get { return includeFiles; }
            set { Set(nameof(IncludeFiles), ref includeFiles, value); }
        }

        public string IncludeFilesString
        {
            get { return String.Join(", ", IncludeFiles); }
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
