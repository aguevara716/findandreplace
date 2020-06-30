using System.Windows.Controls;
using FindAndReplace.Wpf.Extensions;
using FindAndReplace.Wpf.ViewModels;

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

    }
}
