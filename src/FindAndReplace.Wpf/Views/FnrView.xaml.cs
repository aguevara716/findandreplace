using System;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using FindAndReplace.Wpf.Extensions;
using FindAndReplace.Wpf.ViewModels;
using GalaSoft.MvvmLight.Command;

namespace FindAndReplace.Wpf.Views
{
    public partial class FnrView : Page
    {
        private readonly FnrViewModel viewModel;

        public FnrView()
        {
            InitializeComponent();
            viewModel = DataContext as FnrViewModel;

            Loaded += OnLoad;
            Unloaded += OnUnload;
        }

        private void OnLoad(object sender, System.Windows.RoutedEventArgs e)
        {
            rootDirectoryTextBox.Focus();
            viewModel?.LoadedCommand?.Execute();
        }

        private void OnUnload(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel?.UnloadedCommand?.Execute();
        }

        private void OnAddButtonClicked(object sender, RoutedEventArgs e)
        {
            if (sender == addExcludeDirectoryButton)
                addExcludeDirectoryTextBox.Focus();
            else if (sender == addExcludeFileButton)
                addExcludeFileTextBox.Focus();
            else if (sender == addIncludeFileButton)
                addIncludeFileTextBox.Focus();
        }

        private void OnSaveButtonClicked(object sender, RoutedEventArgs e)
        {
            ToggleButton addButton = null;
            TextBox textBox = null;

            if (sender == excludeDirectorySaveButton)
            {
                addButton = addExcludeDirectoryButton;
                textBox = addExcludeDirectoryTextBox;
                viewModel.AddExcludeDirectoryCommand.Execute(textBox.Text);
            }
            else if (sender == excludedFileSaveButton)
            {
                addButton = addExcludeFileButton;
                textBox = addExcludeFileTextBox;
                viewModel.AddExcludeFileCommand.Execute(textBox.Text);
            }
            else if (sender == includeFileSaveButton)
            {
                addButton = addIncludeFileButton;
                textBox = addIncludeFileTextBox;
                viewModel.AddIncludeFileCommand.Execute(textBox.Text);
            }

            if (addButton == null || textBox == null)
                return;

            addButton.IsChecked = false;
            textBox.Text = String.Empty;
        }

        private void OnKeyDown_AddTextBox(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            string inputText = null;
            RelayCommand<String> addCommand = null;
            Button saveButton = null;

            if (sender == addExcludeDirectoryTextBox)
            {
                inputText = addExcludeDirectoryTextBox.Text;
                addCommand = viewModel.AddExcludeDirectoryCommand;
                saveButton = excludeDirectorySaveButton;
            }
            else if (sender == addExcludeFileTextBox)
            {
                inputText = addExcludeFileTextBox.Text;
                addCommand = viewModel.AddExcludeFileCommand;
                saveButton = excludedFileSaveButton;
            }
            else if (sender == addIncludeFileTextBox)
            {
                inputText = addIncludeFileTextBox.Text;
                addCommand = viewModel.AddIncludeFileCommand;
                saveButton = includeFileSaveButton;
            }

            addCommand.Execute(inputText);
            OnSaveButtonClicked(saveButton, new RoutedEventArgs());
        }
    }
}
