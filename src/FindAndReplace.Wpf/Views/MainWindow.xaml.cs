using System.Windows;
using FindAndReplace.Wpf.ViewModels;

namespace FindAndReplace.Wpf.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = DataContext as MainViewModel;

            Loaded += OnLoad;
            Unloaded += OnUnloaded;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            viewModel?.LoadedCommand?.Execute(null);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            viewModel?.UnloadedCommand?.Execute(null);
        }

    }
}
