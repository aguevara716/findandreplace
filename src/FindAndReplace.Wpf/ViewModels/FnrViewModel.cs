using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using System.Windows.Threading;
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
        private readonly IReplacerMapper replacerMapper;

        // Delegates
        private delegate void SetFindResultCallback(Finder.FindResultItem resultItem, Stats stats, Status status);
        private delegate void SetReplaceResultCallback(Replacer.ReplaceResultItem resultItem, Stats stats, Status status);

        // Variables
        private bool isRunning;
        private Thread finderThread;

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
                            IReplacerMapper rm)
        {
            dialogService = ds;
            finderMapper = fm;
            replacerMapper = rm;

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

        private void OnFinderFileProcessed(object sender, FinderEventArgs e)
        {
            SetFindResultCallback findResultCallback = ShowFindResult;
            findResultCallback.Invoke(e.ResultItem, e.Stats, e.Status);
        }

        private void ShowFindResult(Finder.FindResultItem resultItem, Stats stats, Status status)
        {
            var result = new Result
            {
                ErrorMessage = resultItem.ErrorMessage,
                FailedToOpen = false,
                FileEncoding = resultItem.FileEncoding ?? Encoding.Default,
                Filename = resultItem.FileName,
                FilePath = resultItem.FilePath,
                FileRelativePath = resultItem.FileRelativePath,
                IsBinaryFile = resultItem.IsBinaryFile,
                IsSuccess = resultItem.IsSuccess,
                MatchCount = resultItem.NumMatches,
                Matches = resultItem.Matches?.Select(lm => new Match
                {
                    Index = lm.Index,
                    Length = lm.Length
                }).ToList()
            };
            //Results.Add(result);

            ProcessStatus = new ProcessStatus
            {
                BinaryFilesCount = stats.Files.Binary,
                EllapsedTime = stats.Time.Passed,
                FilesFailedToOpenCount = stats.Files.FailedToRead,
                FilesProcessedCount = stats.Files.Processed,
                FilesWithMatchesCount = stats.Files.WithMatches,
                FilesWithoutMatchesCount = stats.Files.WithoutMatches,
                MatchesCount = stats.Matches.Found,
                TotalFilesCount = stats.Files.Total
            };
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

            finderThread = new Thread(DoFinderWork)
            {
                IsBackground = true
            };
            finderThread.Start();
        }

        private async void ReplaceExecuted()
        {
            UpdateIsRunning(true);
            await Task.Delay(5000);
            UpdateIsRunning(false);
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
