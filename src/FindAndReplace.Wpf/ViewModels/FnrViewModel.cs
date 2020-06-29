using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private readonly IDialogService dialogService;
        private readonly IFinderThreadWorker finderThreadWorker;
        private readonly IProcessStatusMapper processStatusMapper;
        private readonly IReplacerMapper replacerMapper;
        private readonly IReplacerThreadWorker replacerThreadWorker;
        private readonly IResultMapper resultMapper;
        private readonly ISettingsService settingsService;

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
        public RelayCommand GenerateCommandLineCommand { get; private set; }
        public RelayCommand SwapCommand { get; private set; }
        public RelayCommand<String> OpenFileCommand { get; private set; }

        // Constructors
        public FnrViewModel(IDialogService ds,
                            IFinderThreadWorker ftw,
                            IProcessStatusMapper psm,
                            IReplacerMapper rm,
                            IReplacerThreadWorker rtw,
                            IResultMapper resm,
                            ISettingsService ss)
        {
            dialogService = ds;
            finderThreadWorker = ftw;
            processStatusMapper = psm;
            replacerMapper = rm;
            replacerThreadWorker = rtw;
            resultMapper = resm;
            settingsService = ss;

            InitializeVariables();

            if (IsInDesignMode)
                return;

            LoadedCommand = new RelayCommand(LoadedExecuted);
            UnloadedCommand = new RelayCommand(UnloadedExecuted);
            SelectRootDirectoryCommand = new RelayCommand(SelectRootDirectoryExecuted);
            FindCommand = new RelayCommand(FindExecuted, FindOrReplaceCanExecute);
            ReplaceCommand = new RelayCommand(ReplaceExecuted, FindOrReplaceCanExecute);
            CancelCommand = new RelayCommand(CancelExecuted, CancelCanExecute);
            GenerateCommandLineCommand = new RelayCommand(GenerateCommandLineExecuted);
            SwapCommand = new RelayCommand(SwapExecuted);
            OpenFileCommand = new RelayCommand<String>(OpenFileExecuted);
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

        private void ShowFindResult(Finder.FindResultItem resultItem, Stats stats, Status status)
        {
            ProcessStatus = processStatusMapper.Map(stats);

            if (!resultItem.IncludeInResultsList)
                return;
            var result = resultMapper.Map(resultItem);
            Results.Add(result);

            UpdateStatus(stats.Files.Processed, stats.Files.Total, resultItem.FileRelativePath, status);
        }

        private void ShowReplaceResult(Replacer.ReplaceResultItem resultItem, Stats stats, Status status)
        {
            ProcessStatus = processStatusMapper.Map(stats);

            if (!resultItem.IncludeInResultsList)
                return;
            var result = resultMapper.Map(resultItem);
            Results.Add(result);

            UpdateStatus(stats.Files.Processed, stats.Files.Total, resultItem.FileRelativePath, status);
        }

        private void UpdateStatus(int processed, int total, string lastFile, Status status)
        {
            switch (status)
            {
                case FindAndReplace.Status.Processing:
                    Status = $"Processing {processed} of {total} files. Last file: {lastFile}";
                    break;
                case FindAndReplace.Status.Completed:
                    Status = $"Processed {total} files.";
                    break;
                case FindAndReplace.Status.Cancelled:
                    Status = "Operation was cancelled";
                    break;
            }
        }

        private void UpdateSettings()
        {
            settingsService.SaveSettings(FolderParameters, FindParameters, ReplaceParameters);
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

            var settingsTuple = settingsService.LoadSettings();
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
            var selectedPath = dialogService.ShowFolderPickerDialog("Select root directory", FolderParameters.RootDirectory, false);
            if (String.IsNullOrEmpty(selectedPath))
                return;

            FolderParameters.RootDirectory = selectedPath;
        }

        private void FindExecuted()
        {
            UpdateIsRunning(true);
            UpdateSettings();
            ProcessStatus = new ProcessStatus();
            Results.Clear();

            finderThreadWorker.InvokeWorker(FolderParameters, FindParameters, ShowFindResult);

            UpdateIsRunning(false);
        }

        private void ReplaceExecuted()
        {
            if (String.IsNullOrEmpty(ReplaceParameters.ReplaceString))
            {
                var response = dialogService.ShowYesNo("Are you sure you would like to replace with an empty string?", "Replace Confirmation");
                if (!response)
                    return;
            }

            UpdateIsRunning(true);
            UpdateSettings();
            ProcessStatus = new ProcessStatus();
            Results.Clear();

            replacerThreadWorker.InvokeWorker(FolderParameters, FindParameters, ReplaceParameters, ShowReplaceResult);

            UpdateIsRunning(false);
        }

        private void CancelExecuted()
        {
            // TODO specify which process is executing
            finderThreadWorker.CancelWorker();
            replacerThreadWorker.CancelWorker();

            UpdateIsRunning(false);
        }

        private void GenerateCommandLineExecuted()
        {
            var replacer = replacerMapper.Map(FolderParameters, FindParameters, ReplaceParameters);
            var exePath = "fnr.exe";
            var commandLineArgs = replacer.GenCommandLine(findParameters.IsShowingEncoding);
            dialogService.ShowMessage($"{exePath} {commandLineArgs}", "Command Line");
        }

        private void SwapExecuted()
        {
            var originalFindString = FindParameters.FindString;

            FindParameters.FindString = ReplaceParameters.ReplaceString;
            ReplaceParameters.ReplaceString = originalFindString;
        }

        private void OpenFileExecuted(string filePath)
        {
            Process.Start(filePath);
        }

    }
}
