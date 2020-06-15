using GalaSoft.MvvmLight.Command;

namespace FindAndReplace.Wpf.Extensions
{
    public static class RelayCommandExtensions
    {
        public static void Execute(this RelayCommand relayCommand)
        {
            relayCommand.Execute(null);
        }

        public static bool CanExecute(this RelayCommand relayCommand)
        {
            return relayCommand.CanExecute(null);
        }

    }

    public static class RelayCommandGenericExtensions
    {
        public static void Execute<T>(this RelayCommand<T> relayCommand, T parameter)
        {
            relayCommand.Execute(parameter);
        }

        public static bool CanExecute<T>(this RelayCommand<T> relayCommand, T parameter)
        {
            return relayCommand.CanExecute(parameter);
        }

    }
}
