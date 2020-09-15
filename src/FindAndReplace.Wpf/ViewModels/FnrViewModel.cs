using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FindAndReplace.Wpf.Backend.Collections;
using FindAndReplace.Wpf.Backend.Extensions;
using FindAndReplace.Wpf.Backend.Files;
using FindAndReplace.Wpf.Backend.Filesystem;
using FindAndReplace.Wpf.Backend.Results;
using FindAndReplace.Wpf.Dialogs;
using FindAndReplace.Wpf.Mappers;
using FindAndReplace.Wpf.Models;
using FindAndReplace.Wpf.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace FindAndReplace.Wpf.ViewModels
{
    public class FnrViewModel : ViewModelBase
    {
        // Dependencies
        private readonly IClipboardDataService _clipboardDataService;
        private readonly IDialogService _dialogService;
        private readonly IFileDiscoverer _fileDiscoverer;
        private readonly IFileResultMapper _fileResultMapper;
        private readonly IFinderService _finderService;
        private readonly ISettingsService _settingsService;

        // Variables
        private bool isRunning;
        private bool isCancelRequested;

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

        private ObservableCollection<FileResult> results;
        public ObservableCollection<FileResult> Results
        {
            get { return results; }
            set { Set(nameof(Results), ref results, value); }
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
        public RelayCommand<FileResult> OpenFileCommand { get; private set; }
        public RelayCommand<FileResult> OpenFolderCommand { get; private set; }
        public RelayCommand<FileResult> CopyPathCommand { get; private set; }
        public RelayCommand<String> AddExcludeDirectoryCommand { get; private set; }
        public RelayCommand<String> RemoveExcludeDirectoryCommand { get; private set; }
        public RelayCommand<String> AddExcludeFileCommand { get; private set; }
        public RelayCommand<String> RemoveExcludeFileCommand { get; private set; }
        public RelayCommand<String> AddIncludeFileCommand { get; private set; }
        public RelayCommand<String> RemoveIncludeFileCommand { get; private set; }
        public RelayCommand<String> CopyPreviewCommand { get; private set; }

        // Constructors
        public FnrViewModel(IClipboardDataService clipboardDataService,
                            IDialogService dialogService,
                            IFileDiscoverer fileDiscoverer,
                            IFileResultMapper fileResultMapper,
                            IFinderService finderService,
                            ISettingsService settingsService)
        {
            _clipboardDataService = clipboardDataService;
            _dialogService = dialogService;
            _fileDiscoverer = fileDiscoverer;
            _fileResultMapper = fileResultMapper;
            _finderService = finderService;
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
            OpenFileCommand = new RelayCommand<FileResult>(OpenFileExecuted);
            OpenFolderCommand = new RelayCommand<FileResult>(OpenFolderExecuted);
            CopyPathCommand = new RelayCommand<FileResult>(CopyPathExecuted);
            AddExcludeDirectoryCommand = new RelayCommand<String>(AddExcludeDirectoryExecuted);
            RemoveExcludeDirectoryCommand = new RelayCommand<String>(RemoveExcludeDirectoryExecuted);
            AddExcludeFileCommand = new RelayCommand<String>(AddExcludeFileExecuted);
            RemoveExcludeFileCommand = new RelayCommand<String>(RemoveExcludeFileExecuted);
            AddIncludeFileCommand = new RelayCommand<String>(AddIncludeFileExecuted);
            RemoveIncludeFileCommand = new RelayCommand<String>(RemoveIncludeFileExecuted);
            CopyPreviewCommand = new RelayCommand<string>(CopyPreviewExecuted);
        }

        // Private Methods
        private void InitializeVariables()
        {
            FolderParameters = new FolderParameters();
            FindParameters = new FindParameters();
            ReplaceParameters = new ReplaceParameters();
            ProcessStatus = new ProcessStatus();
            Results = new ObservableCollection<FileResult>();
            Status = String.Empty;

            isRunning = false;
            isCancelRequested = false;
        }

        private void UpdateIsRunning(bool isRunning)
        {
            if (isRunning)
                Status = "Getting file list...";

            this.isRunning = isRunning;
            isCancelRequested = false;
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

        private async void FindExecuted()
        {
            UpdateSettings();
            UpdateIsRunning(true);

            ProcessStatus = new ProcessStatus();
            Results.Clear();
            Status = "Searching for files";

            var startTime = DateTime.Now;
            var fileDiscoveryResult = await _fileDiscoverer.DiscoverFilesAsync(FolderParameters.RootDirectory,
                                                                               FolderParameters.IncludeFiles.ToList(),
                                                                               FolderParameters.ExcludeDirectories.ToList(),
                                                                               FolderParameters.ExcludeFiles.ToList(),
                                                                               FolderParameters.IsRecursive);
            ProcessStatus.EllapsedTime = DateTime.Now - startTime;
            Status = $"Found {fileDiscoveryResult.Files?.Count ?? 0:N0} files in {ProcessStatus.EllapsedTime.TotalMilliseconds:N0} ms";
            if (!fileDiscoveryResult.IsSuccessful)
            {
                UpdateIsRunning(false);
                var errorText = fileDiscoveryResult.GetErrorText();
                _dialogService.ShowMessage(errorText, "Error Finding Files");
                return;
            }

            ProcessStatus.TotalFilesCount = fileDiscoveryResult.Files.Count;

            Results = new ObservableCollection<FileResult>();
            foreach (var filePath in fileDiscoveryResult.Files)
            {
                if (isCancelRequested)
                {
                    Status = "Cancelled";
                    break;
                }
                Status = $"Scanning file \"{filePath}\"";

                MatchPreviewExtractionResult matchPreviewExtractionResult;
                if (!FindParameters.IsSearchingFilenameOnly)
                    matchPreviewExtractionResult = await _finderService.FindTextInFileAsync(filePath, FindParameters.FindString, FindParameters.IsRegex, FindParameters.IsUsingEscapeCharacters, FindParameters.IsCaseSensitive);
                else
                    matchPreviewExtractionResult = MatchPreviewExtractionResult.CreateSuccess<MatchPreviewExtractionResult>(filePath, new List<string>());

                var fileResult = _fileResultMapper.Map(FolderParameters.RootDirectory, filePath);
                fileResult.ErrorMessage = matchPreviewExtractionResult.GetErrorText();
                fileResult.HasError = !String.IsNullOrEmpty(fileResult.ErrorMessage);
                fileResult.Previews = matchPreviewExtractionResult.Previews?.ToList();

                var shouldAddResult = fileResult.HasError ||
                                      FindParameters.IsSearchingFilenameOnly ||
                                      (!FindParameters.IsOnlyShowingFilesWithoutMatches && (fileResult.Previews?.Any() ?? false)) ||
                                      (FindParameters.IsOnlyShowingFilesWithoutMatches && (!fileResult.Previews?.Any() ?? true));
                if (shouldAddResult)
                    Results.Add(fileResult);
                
                ProcessStatus.FilesProcessedCount++;
                ProcessStatus.FilesWithMatchesCount += matchPreviewExtractionResult.IsSuccessful && matchPreviewExtractionResult.Previews.Any() ? 1 : 0;
                ProcessStatus.FilesWithoutMatchesCount += !matchPreviewExtractionResult.IsSuccessful || !matchPreviewExtractionResult.Previews.Any() ? 1 : 0;
                ProcessStatus.MatchesCount += matchPreviewExtractionResult.Previews?.Count ?? 0;
                ProcessStatus.EllapsedTime = DateTime.Now - startTime;
            }

            if (!isCancelRequested)
                Status = "Finished";
            UpdateIsRunning(false);
        }

        private void ReplaceExecuted()
        {
            UpdateSettings();
            _dialogService.ShowMessage("This doesn't actually do anything yet", "Not Implemented");
        }

        private void CancelExecuted()
        {
            isCancelRequested = true;
        }

        private void SwapExecuted()
        {
            var originalFindString = FindParameters.FindString;

            FindParameters.FindString = ReplaceParameters.ReplaceString;
            ReplaceParameters.ReplaceString = originalFindString;
        }

        private void OpenFileExecuted(FileResult fileResult)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileResult.FullPath,
                UseShellExecute = true
            };
            Process.Start(startInfo);
        }

        private void OpenFolderExecuted(FileResult fileResult)
        {
            var folder = Path.GetDirectoryName(fileResult.FullPath);
            var startInfo = new ProcessStartInfo
            {
                FileName = folder,
                UseShellExecute = true
            };
            Process.Start(startInfo);
        }

        private void CopyPathExecuted(FileResult fileResult)
        {
            _clipboardDataService.SetText(fileResult.FullPath);
        }

        private void AddExcludeDirectoryExecuted(string newExcludeDirectory)
        {
            if (String.IsNullOrEmpty(newExcludeDirectory))
                return;

            var newExcludeDirectories = newExcludeDirectory.SplitAndTrim(",");
            FolderParameters.ExcludeDirectories.AddRange(newExcludeDirectories);
            FolderParameters.ExcludeDirectories = new ObservableHashSet<String>(FolderParameters.ExcludeDirectories.OrderBy(s => s));
        }

        private void RemoveExcludeDirectoryExecuted(string excludedDirectory)
        {
            FolderParameters.ExcludeDirectories.Remove(excludedDirectory);
        }

        private void AddExcludeFileExecuted(string newExcludeFile)
        {
            if (String.IsNullOrEmpty(newExcludeFile))
                return;

            var newExcludeFiles = newExcludeFile.SplitAndTrim(",");
            FolderParameters.ExcludeFiles.AddRange(newExcludeFiles);
            FolderParameters.ExcludeFiles = new ObservableHashSet<String>(FolderParameters.ExcludeFiles.OrderBy(s => s));
        }

        private void RemoveExcludeFileExecuted(string excludedFile)
        {
            FolderParameters.ExcludeFiles.Remove(excludedFile);
        }

        private void AddIncludeFileExecuted(string newIncludeFile)
        {
            if (String.IsNullOrEmpty(newIncludeFile))
                return;

            var newIncludeFiles = newIncludeFile.SplitAndTrim(",");
            FolderParameters.IncludeFiles.AddRange(newIncludeFiles);
            FolderParameters.IncludeFiles = new ObservableHashSet<String>(FolderParameters.IncludeFiles.OrderBy(s => s));
        }

        private void RemoveIncludeFileExecuted(string includeFile)
        {
            FolderParameters.IncludeFiles.Remove(includeFile);
        }

        private void CopyPreviewExecuted(string previewText)
        {
            _clipboardDataService.SetText(previewText);
        }

    }
}
