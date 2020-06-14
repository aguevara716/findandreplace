using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace FindAndReplace.Wpf.ViewModels
{
    public class FnrViewModel : ViewModelBase
    {
        // Variables

        // Dependencies

        // Binding Variables
        public RelayCommand LoadedCommand { get; private set; }
        public RelayCommand UnloadedCommand { get; private set; }
        public RelayCommand SelecRoottDirectoryCommand { get; private set; }
        public RelayCommand FindCommand { get; private set; }
        public RelayCommand ReplaceCommand { get; private set; }
        public RelayCommand GenerateCommandLineCommand { get; private set; }
        public RelayCommand SwapCommand { get; private set; }
        public RelayCommand OpenFileCommand { get; private set; }

        // Commands

        // Constructors
        public FnrViewModel()
        {
            if (IsInDesignMode)
                return;

            LoadedCommand = new RelayCommand(LoadedExecuted);
            UnloadedCommand = new RelayCommand(UnloadedExecuted);
        }

        // Private Methods

        // Commands CanExecute

        // Commands Executed
        private void LoadedExecuted()
        {

        }

        private void UnloadedExecuted()
        {

        }

    }
}
