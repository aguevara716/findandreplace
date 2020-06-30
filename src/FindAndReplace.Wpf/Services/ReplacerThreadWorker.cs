using System;
using System.ComponentModel;
using FindAndReplace.Wpf.Dialogs;
using FindAndReplace.Wpf.Models;

namespace FindAndReplace.Wpf.Services
{
    public interface IReplacerThreadWorker
    {
        void InvokeWorker(FolderParameters folderParameters, FindParameters findParameters, ReplaceParameters replaceParameters, Action<Replacer.ReplaceResultItem, Stats, Status> processResultAction);
        void InvokeWorker(Replacer replacer, Action<Replacer.ReplaceResultItem, Stats, Status> processResultAction);
        void CancelWorker();
    }

    public class ReplacerThreadWorker : IReplacerThreadWorker
    {
        // Dependencies
        private readonly IDialogService dialogService;
        private readonly IReplacerMapper replacerMapper;

        // Variables
        private BackgroundWorker replacerWorker;

        // Constructors
        public ReplacerThreadWorker(IDialogService ds,
                                    IReplacerMapper rm)
        {
            dialogService = ds;
            replacerMapper = rm;
        }

        // Event Handlers
        private void OnBackgroundWorkerDoWork(Replacer replacer, Action<Replacer.ReplaceResultItem, Stats, Status> processResultAction)
        {
            try
            {
                replacer.Replace();
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage(ex.Message, "Error");
                processResultAction.Invoke(new Replacer.ReplaceResultItem(),
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
        private BackgroundWorker BuildReplacerWorker(Action<Replacer.ReplaceResultItem, Stats, Status> processResultAction)
        {
            var bgw = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            bgw.DoWork += (s, e) =>
            {
                var replacer = e.Argument as Replacer;
                OnBackgroundWorkerDoWork(replacer, processResultAction);
            };

            bgw.ProgressChanged += (s, e) =>
            {
                var replacerEventArgs = e.UserState as ReplacerEventArgs;
                processResultAction.Invoke(replacerEventArgs.ResultItem,
                                           replacerEventArgs.Stats,
                                           replacerEventArgs.Status);
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
        public void InvokeWorker(FolderParameters folderParameters, FindParameters findParameters, ReplaceParameters replaceParameters, Action<Replacer.ReplaceResultItem, Stats, Status> processResultAction)
        {
            var replacer = replacerMapper.Map(folderParameters, findParameters, replaceParameters);
            InvokeWorker(replacer, processResultAction);
        }

        public void InvokeWorker(Replacer replacer, Action<Replacer.ReplaceResultItem, Stats, Status> processResultAction)
        {
            if (replacerWorker?.IsBusy ?? false)
                return;

            replacer.FileProcessed += (s, e) =>
            {
                var percentCompleted = e.Stats.Files.Total == 0
                    ? 0
                    : e.Stats.Files.Processed / e.Stats.Files.Total;
                replacerWorker.ReportProgress(percentCompleted, e);
            };

            replacerWorker = BuildReplacerWorker(processResultAction);
            replacerWorker.RunWorkerAsync(replacer);
        }

        public void CancelWorker()
        {
            if (replacerWorker == null || !replacerWorker.IsBusy)
                return;

            replacerWorker.CancelAsync();
            replacerWorker.Dispose();
        }

    }
}
