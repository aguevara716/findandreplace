using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using FindAndReplace.Wpf.Dialogs;
using FindAndReplace.Wpf.Models;
using FindAndReplace.Wpf.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace FindAndReplace.Wpf.ViewModels
{
    public class FnrViewModel : ViewModelBase
    {
        // Dependencies
        private readonly IDialogService _dialogService;
        private readonly ISettingsService _settingsService;

        // Variables
        private bool isRunning;

        // Binding Variables
        private FolderParameters folderParameters;
        public FolderParameters FolderParameters
        {
            get { return folderParameters; }
            set { Set(nameof(FolderParameters), ref folderParameters, value); }
        }

        private FindParameters findParameters;
        public FindParameters FindParameters
        {
            get { return findParameters; }
            set { Set(nameof(FindParameters), ref findParameters, value); }
        }

        private ReplaceParameters replaceParameters;
        public ReplaceParameters ReplaceParameters
        {
            get { return replaceParameters; }
            set { Set(nameof(ReplaceParameters), ref replaceParameters, value); }
        }

        private ProcessStatus processStatus;
        public ProcessStatus ProcessStatus
        {
            get { return processStatus; }
            set { Set(nameof(ProcessStatus), ref processStatus, value); }
        }

        private ObservableCollection<Result> results;
        public ObservableCollection<Result> Results
        {
            get { return results; }
            set { Set(nameof(Results), ref results, value); }
        }

        private List<String> encodings;
        public List<String> Encodings
        {
            get { return encodings; }
            set { Set(nameof(Encodings), ref encodings, value); }
        }

        private string status;
        public string Status
        {
            get { return status; }
            set { Set(nameof(Status), ref status, value); }
        }

        // Commands
        public RelayCommand LoadedCommand { get; private set; }
        public RelayCommand UnloadedCommand { get; private set; }
        public RelayCommand SelectRootDirectoryCommand { get; private set; }
        public RelayCommand FindCommand { get; private set; }
        public RelayCommand ReplaceCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }
        public RelayCommand SwapCommand { get; private set; }
        public RelayCommand<Result> OpenFileCommand { get; private set; }
        public RelayCommand<Result> OpenFolderCommand { get; private set; }
        public RelayCommand<String> AddExcludeDirectoryCommand { get; private set; }
        public RelayCommand<String> RemoveExcludeDirectoryCommand { get; private set; }
        public RelayCommand<String> AddExcludeFileCommand { get; private set; }
        public RelayCommand<String> RemoveExcludeFileCommand { get; private set; }
        public RelayCommand<String> AddIncludeFileCommand { get; private set; }
        public RelayCommand<String> RemoveIncludeFileCommand { get; private set; }

        // Constructors
        public FnrViewModel(IDialogService dialogService,
                            ISettingsService settingsService)
        {
            _dialogService = dialogService;
            _settingsService = settingsService;

            InitializeVariables();

            if (IsInDesignMode)
                return;

            LoadedCommand = new RelayCommand(LoadedExecuted);
            UnloadedCommand = new RelayCommand(UnloadedExecuted);
            SelectRootDirectoryCommand = new RelayCommand(SelectRootDirectoryExecuted);
            FindCommand = new RelayCommand(FindExecuted, FindOrReplaceCanExecute);
            ReplaceCommand = new RelayCommand(ReplaceExecuted, FindOrReplaceCanExecute);
            CancelCommand = new RelayCommand(CancelExecuted, CancelCanExecute);
            SwapCommand = new RelayCommand(SwapExecuted);
            OpenFileCommand = new RelayCommand<Result>(OpenFileExecuted);
            OpenFolderCommand = new RelayCommand<Result>(OpenFolderExecuted);
            AddExcludeDirectoryCommand = new RelayCommand<String>(AddExcludeDirectoryExecuted);
            RemoveExcludeDirectoryCommand = new RelayCommand<String>(RemoveExcludeDirectoryExecuted);
            AddExcludeFileCommand = new RelayCommand<String>(AddExcludeFileExecuted);
            RemoveExcludeFileCommand = new RelayCommand<String>(RemoveExcludeFileExecuted);
            AddIncludeFileCommand = new RelayCommand<String>(AddIncludeFileExecuted);
            RemoveIncludeFileCommand = new RelayCommand<String>(RemoveIncludeFileExecuted);
        }

        // Private Methods
        private void InitializeVariables()
        {
            FolderParameters = new FolderParameters();
            FindParameters = new FindParameters();
            ReplaceParameters = new ReplaceParameters();
            ProcessStatus = new ProcessStatus();
            Results = new ObservableCollection<Result>();
            Encodings = new List<string>();
            Status = String.Empty;

            isRunning = false;
        }

        private void UpdateIsRunning(bool isRunning)
        {
            if (isRunning)
                Status = "Getting file list...";

            this.isRunning = isRunning;
            FindCommand.RaiseCanExecuteChanged();
            ReplaceCommand.RaiseCanExecuteChanged();
            CancelCommand.RaiseCanExecuteChanged();
        }

        private void UpdateSettings()
        {
            _settingsService.SaveSettings(FolderParameters, FindParameters, ReplaceParameters);
        }

        // Commands CanExecute
        private bool FindOrReplaceCanExecute()
        {
            return //!String.IsNullOrEmpty(FolderParameters.RootDirectory) && 
                   !isRunning;
        }

        private bool CancelCanExecute()
        {
            return isRunning;
        }

        // Commands Executed
        private void LoadedExecuted()
        {
            var localEncodings = new List<String>
            {
                "Auto Detect"
            };
            localEncodings.AddRange(Encoding.GetEncodings().Select(ei => ei.Name.ToUpper()).OrderBy(e => e));

            Encodings = new List<string>(localEncodings);
            FindParameters.Encoding = Encodings.First();

            var settingsTuple = _settingsService.LoadSettings();
            if (settingsTuple == null)
                return;
            FolderParameters = settingsTuple.FolderParameters;
            FindParameters = settingsTuple.FindParameters;
            ReplaceParameters = settingsTuple.ReplaceParameters;
        }

        private void UnloadedExecuted()
        {

        }

        private void SelectRootDirectoryExecuted()
        {
            var selectedPath = _dialogService.ShowFolderPickerDialog("Select root directory", FolderParameters.RootDirectory, false);
            if (String.IsNullOrEmpty(selectedPath))
                return;

            FolderParameters.RootDirectory = selectedPath;
        }

        private void FindExecuted()
        {
            UpdateSettings();
        }

        private void ReplaceExecuted()
        {
            UpdateSettings();
        }

        private void CancelExecuted()
        {

        }

        private void SwapExecuted()
        {
            var originalFindString = FindParameters.FindString;

            FindParameters.FindString = ReplaceParameters.ReplaceString;
            ReplaceParameters.ReplaceString = originalFindString;
        }

        private void OpenFileExecuted(Result resultItem)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = resultItem.FilePath,
                UseShellExecute = true
            };
            Process.Start(startInfo);
        }

        private void OpenFolderExecuted(Result resultItem)
        {
            var folder = Path.GetDirectoryName(resultItem.FilePath);
            var startInfo = new ProcessStartInfo
            {
                FileName = folder,
                UseShellExecute = true
            };
            Process.Start(startInfo);
        }

        private void AddExcludeDirectoryExecuted(string newExcludeDirectory)
        {
            if (String.IsNullOrEmpty(newExcludeDirectory))
                return;
            if (FolderParameters.ExcludeDirectories.Contains(newExcludeDirectory))
                return;

            FolderParameters.ExcludeDirectories.Add(newExcludeDirectory);
            FolderParameters.ExcludeDirectories = new ObservableCollection<String>(FolderParameters.ExcludeDirectories.OrderBy(s => s));
        }

        private void RemoveExcludeDirectoryExecuted(string excludedDirectory)
        {
            FolderParameters.ExcludeDirectories.Remove(excludedDirectory);
        }

        private void AddExcludeFileExecuted(string newExcludeFile)
        {
            if (String.IsNullOrEmpty(newExcludeFile))
                return;
            if (FolderParameters.ExcludeFiles.Contains(newExcludeFile))
                return;

            FolderParameters.ExcludeFiles.Add(newExcludeFile);
            FolderParameters.ExcludeFiles = new ObservableCollection<String>(FolderParameters.ExcludeFiles.OrderBy(s => s));
        }

        private void RemoveExcludeFileExecuted(string excludedFile)
        {
            FolderParameters.ExcludeFiles.Remove(excludedFile);
        }

        private void AddIncludeFileExecuted(string newIncludeFile)
        {
            if (String.IsNullOrEmpty(newIncludeFile))
                return;
            if (FolderParameters.IncludeFiles.Contains(newIncludeFile))
                return;

            FolderParameters.IncludeFiles.Add(newIncludeFile);
            FolderParameters.IncludeFiles = new ObservableCollection<String>(FolderParameters.IncludeFiles.OrderBy(s => s));
        }

        private void RemoveIncludeFileExecuted(string includeFile)
        {
            FolderParameters.IncludeFiles.Remove(includeFile);
        }

    }
}
