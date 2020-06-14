using System.Windows;

namespace FindAndReplace.Wpf.Dialogs
{
    public interface IDialogService
    {
        void ShowMessage(string message, string title);
        bool ShowYesNo(string message, string title);
        bool? ShowYesNoCancel(string message, string title);
    }

    public class DialogService : IDialogService
    {
        public void ShowMessage(string message, string title)
        {
            MessageBox.Show(message, title);
        }

        public bool ShowYesNo(string message, string title)
        {
            var response = MessageBox.Show(message, title, MessageBoxButton.YesNo);
            return response == MessageBoxResult.Yes;
        }

        public bool? ShowYesNoCancel(string message, string title)
        {
            var response = MessageBox.Show(message, title, MessageBoxButton.YesNoCancel);
            switch (response)
            {
                case MessageBoxResult.Yes:
                    return true;
                case MessageBoxResult.No:
                    return false;
                default:
                    return null;
            }
        }

    }
}
