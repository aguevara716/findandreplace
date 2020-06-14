using FindAndReplace.Wpf.Navigation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace FindAndReplace.Wpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // Variables
        private readonly IFrameNavigationService frameNavigationService;
        
        // Commands
        public RelayCommand LoadedCommand { get; private set; }
        public RelayCommand UnloadedCommand { get; private set; }

        // Constructors
        public MainViewModel(IFrameNavigationService fns)
        {
            frameNavigationService = fns;

            if (IsInDesignMode)
                return;

            LoadedCommand = new RelayCommand(LoadedExecuted);
            UnloadedCommand = new RelayCommand(UnloadedExecuted);
        }

        // Commands Executed
        private void LoadedExecuted()
        {
            frameNavigationService.NavigateTo(nameof(FnrViewModel));
        }

        private void UnloadedExecuted()
        {

        }

    }
}
