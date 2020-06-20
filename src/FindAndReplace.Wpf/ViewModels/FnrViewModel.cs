using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
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
        private readonly IFinderMapper finderMapper;
        private readonly IProcessStatusMapper processStatusMapper;
        private readonly IReplacerMapper replacerMapper;
        private readonly IResultMapper resultMapper;

        // Delegates
        private delegate void SetFindResultCallback(Finder.FindResultItem resultItem, Stats stats, Status status);
        private delegate void SetReplaceResultCallback(Replacer.ReplaceResultItem resultItem, Stats stats, Status status);

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
                            IFinderMapper fm,
                            IProcessStatusMapper psm,
                            IReplacerMapper rm,
                            IResultMapper resm)
        {
            dialogService = ds;
            finderMapper = fm;
            processStatusMapper = psm;
            replacerMapper = rm;
            resultMapper = resm;

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

            isRunning = false;
        }

        private void UpdateIsRunning(bool isRunning)
        {
            this.isRunning = isRunning;
            FindCommand.RaiseCanExecuteChanged();
            ReplaceCommand.RaiseCanExecuteChanged();
            CancelCommand.RaiseCanExecuteChanged();
        }

        private void DoFinderWork()
        {
            var finder = finderMapper.Map(FolderParameters, FindParameters);
            try
            {
                finder.FileProcessed += OnFinderFileProcessed;
                finder.Find();
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage(ex.Message,
                                          "Error");
                var finderEventArgs = new FinderEventArgs
                (
                    new Finder.FindResultItem(),
                    new Stats(),
                    Status.Cancelled,
                    finder.IsSilent
                );
                OnFinderFileProcessed(this, finderEventArgs);
            }
            finally
            {
                UpdateIsRunning(false);
            }
        }

        private void OnFinderFileProcessed(object sender, FinderEventArgs e)
        {
            SetFindResultCallback findResultCallback = ShowFindResult;
            findResultCallback.Invoke(e.ResultItem, e.Stats, e.Status);
        }

        private void ShowFindResult(Finder.FindResultItem resultItem, Stats stats, Status status)
        {
            ProcessStatus = processStatusMapper.Map(stats);

            if (!resultItem.IncludeInResultsList)
                return;
            var result = resultMapper.Map(resultItem);
            Results.Add(result);
        }

        private void DoReplaceWork()
        {
            var replacer = replacerMapper.Map(FolderParameters, FindParameters, ReplaceParameters);
            try
            {
                replacer.FileProcessed += OnReplacerFileProcessed;
                replacer.Replace();
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage(ex.Message, "Error");
                var replacerEventArgs = new ReplacerEventArgs
                (
                    new Replacer.ReplaceResultItem(),
                    new Stats(),
                    Status.Cancelled,
                    replacer.IsSilent
                );
                OnReplacerFileProcessed(this, replacerEventArgs);
            }
            finally
            {
                UpdateIsRunning(false);
            }
        }

        private void OnReplacerFileProcessed(object sender, ReplacerEventArgs e)
        {
            var replaceResultCallback = new SetReplaceResultCallback(ShowReplaceResult);
            replaceResultCallback.Invoke(e.ResultItem, e.Stats, e.Status);
        }

        private void ShowReplaceResult(Replacer.ReplaceResultItem resultItem, Stats stats, Status status)
        {
            ProcessStatus = processStatusMapper.Map(stats);

            if (!resultItem.IncludeInResultsList)
                return;
            var result = resultMapper.Map(resultItem);
            Results.Add(result);
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
            ProcessStatus = new ProcessStatus();
            Results.Clear();

            var finderThread = new Thread(DoFinderWork)
            {
                IsBackground = true
            };
            finderThread.Start();
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
            ProcessStatus = new ProcessStatus();
            Results.Clear();

            var replacerThread = new Thread(DoReplaceWork)
            {
                IsBackground = true
            };
            replacerThread.Start();
        }

        private void CancelExecuted()
        {
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
