using System.Collections.Generic;
using System.Linq;
using FindAndReplace.Wpf.Models;
using FindAndReplace.Wpf.Navigation;
using FindAndReplace.Wpf.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace FindAndReplace.Wpf.ViewModels
{
    public class ExceptionViewModel : ViewModelBase
    {
        private readonly IExceptionFormatter exceptionFormatter;
        private readonly IClipboardDataService clipboardDataService;
        private readonly IWindowNavigationService windowNavigationService;

        private List<ExceptionModel> exceptions;
        public List<ExceptionModel> Exceptions
        {
            get { return exceptions; }
            set { Set(nameof(Exceptions), ref exceptions, value); }
        }

        public RelayCommand LoadedCommand { get; private set; }
        public RelayCommand CopyAllCommand { get; private set; }
        public RelayCommand<ExceptionModel> CopyExceptionCommand { get; private set; }

        public ExceptionViewModel(IExceptionFormatter ef, IClipboardDataService cds, IWindowNavigationService wns)
        {
            exceptionFormatter = ef;
            clipboardDataService = cds;
            windowNavigationService = wns;
            Exceptions = new List<ExceptionModel>();

            if (IsInDesignMode)
                return;

            LoadedCommand = new RelayCommand(LoadedExecuted);
            CopyAllCommand = new RelayCommand(CopyAllExecuted, CopyAllCanExecute);
            CopyExceptionCommand = new RelayCommand<ExceptionModel>(CopyExceptionExecuted);
        }

        private void CopyExceptions(params ExceptionModel[] exceptions)
        {
            var exText = exceptionFormatter.FormatException(exceptions);
            clipboardDataService.SetText(exText);
        }

        private bool CopyAllCanExecute()
        {
            return Exceptions.Any();
        }

        private void LoadedExecuted()
        {
            var exList = windowNavigationService.GetWindowParameter<IEnumerable<ExceptionModel>>();
            Exceptions = exList.ToList();
        }

        private void CopyAllExecuted()
        {
            CopyExceptions(Exceptions.ToArray());
        }

        private void CopyExceptionExecuted(ExceptionModel exception)
        {
            CopyExceptions(exception);
        }
    }
}
