using System;
using System.ComponentModel;
using FindAndReplace.Wpf.Dialogs;
using FindAndReplace.Wpf.Models;

namespace FindAndReplace.Wpf.Services
{
    public interface IFinderThreadWorker
    {
        void InvokeWorker(FolderParameters folderParameters, FindParameters findParameters, Action<Finder.FindResultItem, Stats, Status> processResultAction);
        void InvokeWorker(Finder finder, Action<Finder.FindResultItem, Stats, Status> processResultAction);
        void CancelWorker();
    }

    public class FinderThreadWorker : IFinderThreadWorker
    {
        // Dependencies
        private readonly IDialogService dialogService;
        private readonly IFinderMapper finderMapper;

        // Variables
        private BackgroundWorker finderWorker;

        // Constructors
        public FinderThreadWorker(IDialogService ds,
                                  IFinderMapper fm)
        {
            dialogService = ds;
            finderMapper = fm;
        }

        // Event Handlers
        private void OnBackgroundworkerDoWork(Finder finder, Action<Finder.FindResultItem, Stats, Status> processResultAction)
        {
            try
            {
                finder.Find();
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage(ex.Message, "Error");
                processResultAction.Invoke(new Finder.FindResultItem(),
                                           new Stats(),
                                           Status.Cancelled);
            }
        }

        private void OnBackgroundWorkerCompleted()
        {

        }

        private void OnBackgroundWorkerDisposed()
        {

        }

        // Private Methods
        private BackgroundWorker BuildFinderWorker(Action<Finder.FindResultItem, Stats, Status> processResultAction)
        {
            var bgw = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            bgw.DoWork += (s, e) =>
            {
                var finder = e.Argument as Finder;
                OnBackgroundworkerDoWork(finder, processResultAction);
            };

            bgw.ProgressChanged += (s, e) =>
            {
                var finderEventArgs = e.UserState as FinderEventArgs;
                processResultAction.Invoke(finderEventArgs.ResultItem,
                                           finderEventArgs.Stats,
                                           finderEventArgs.Status);
            };

            bgw.RunWorkerCompleted += (s, e) =>
            {
                OnBackgroundWorkerCompleted();
            };

            bgw.Disposed += (s, e) =>
            {
                OnBackgroundWorkerDisposed();
            };

            return bgw;
        }

        // Public Methods
        public void InvokeWorker(FolderParameters folderParameters, FindParameters findParameters, Action<Finder.FindResultItem, Stats, Status> processResultAction)
        {
            var finder = finderMapper.Map(folderParameters, findParameters);
            InvokeWorker(finder, processResultAction);
        }

        public void InvokeWorker(Finder finder, Action<Finder.FindResultItem, Stats, Status> processResultAction)
        {
            if (finderWorker?.IsBusy ?? false)
                return;

            finder.FileProcessed += (s, e) =>
            {
                var percentCompleted = e.Stats.Files.Total == 0 
                    ? 0 
                    : e.Stats.Files.Processed / e.Stats.Files.Total;
                finderWorker.ReportProgress(percentCompleted, e);
            };

            finderWorker = BuildFinderWorker(processResultAction);
            finderWorker.RunWorkerAsync(finder);
        }

        public void CancelWorker()
        {
            if (finderWorker == null || !finderWorker.IsBusy)
                return;

            finderWorker.CancelAsync();
            finderWorker.Dispose();
        }

    }
}
