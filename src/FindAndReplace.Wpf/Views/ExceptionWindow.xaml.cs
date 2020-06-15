using System.Windows;
using FindAndReplace.Wpf.Extensions;
using FindAndReplace.Wpf.ViewModels;

namespace FindAndReplace.Wpf.Views
{
    public partial class ExceptionWindow : Window
    {
        private readonly ExceptionViewModel viewModel;

        public ExceptionWindow()
        {
            InitializeComponent();
            viewModel = DataContext as ExceptionViewModel;

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            viewModel?.LoadedCommand?.Execute();
        }

    }
}
