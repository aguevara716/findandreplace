using System;
using System.Windows;
using System.Windows.Controls;

namespace FindAndReplace.Wpf.Dialogs
{
    public partial class InputDialog : Window
    {
        public string Response { get; private set; }

        public InputDialog(string title, string message)
            : this(title, message, String.Empty)
        {

        }

        public InputDialog(string title, string message, string input)
        {
            InitializeComponent();

            Title = title;
            promptMessageTextBlock.Text = message;
            inputTextBox.Text = input;
        }

        private void OnOkClicked(object sender, RoutedEventArgs e)
        {
            Response = inputTextBox.Text;
            DialogResult = true;
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            promptMessageTextBlock.Focus();
        }

        private void OnInputChanged(object sender, TextChangedEventArgs e)
        {
            if (okButton == null)
                return;
            okButton.IsEnabled = !String.IsNullOrEmpty(inputTextBox.Text);
        }
    }
}
