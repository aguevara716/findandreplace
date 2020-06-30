using System.Windows;
using Ookii.Dialogs.Wpf;

namespace FindAndReplace.Wpf.Dialogs
{
    public interface IDialogService
    {
        // Folder
        string ShowFolderPickerDialog(string title, string rootFolder, bool showNewFolderButton);

        // Message box
        void ShowMessage(string message, string title);
        bool ShowYesNo(string message, string title);
        bool? ShowYesNoCancel(string message, string title);
    }

    public class DialogService : IDialogService
    {
        // Folder
        public string ShowFolderPickerDialog(string title, string rootFolder, bool showNewFolderButton)
        {
            var folderBrowserDialog = new VistaFolderBrowserDialog
            {
                Description = title,
                SelectedPath = rootFolder,
                ShowNewFolderButton = showNewFolderButton,
                UseDescriptionForTitle = true
            };

            var result = folderBrowserDialog.ShowDialog();
            if (!result.HasValue || !result.Value)
                return null;

            return folderBrowserDialog.SelectedPath;
        }

        // Message box
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
