using System;
using FindAndReplace.Wpf.Backend.Collections;
using GalaSoft.MvvmLight;

namespace FindAndReplace.Wpf.Models
{
    public class FolderParameters : ObservableObject
    {
        private ObservableHashSet<String> excludeDirectories;
        public ObservableHashSet<String> ExcludeDirectories
        {
            get { return excludeDirectories; }
            set { Set(nameof(ExcludeDirectories), ref excludeDirectories, value); }
        }

        public string ExcludeDirectoriesString
        {
            get { return String.Join(",", ExcludeDirectories); }
        }

        private ObservableHashSet<String> excludeFiles;
        public ObservableHashSet<String> ExcludeFiles
        {
            get { return excludeFiles; }
            set { Set(nameof(ExcludeFiles), ref excludeFiles, value); }
        }

        public string ExcludeFilesString
        {
            get { return String.Join(",", ExcludeFiles); }
        }

        private ObservableHashSet<String> includeFiles;
        public ObservableHashSet<String> IncludeFiles
        {
            get { return includeFiles; }
            set { Set(nameof(IncludeFiles), ref includeFiles, value); }
        }

        public string IncludeFilesString
        {
            get { return String.Join(",", IncludeFiles); }
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
