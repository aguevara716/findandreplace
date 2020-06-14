using System.Windows.Controls;
using FindAndReplace.Wpf.ViewModels;

namespace FindAndReplace.Wpf.Views
{
    /// <summary>
    /// Interaction logic for FnrView.xaml
    /// </summary>
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
            viewModel?.LoadedCommand?.Execute(null);
        }

        private void OnUnload(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel?.UnloadedCommand?.Execute(null);
        }

    }
}
