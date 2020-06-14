using System.Windows;

namespace FindAndReplace.Wpf.Initialization
{
    public interface IMainWindowRetriever
    {
        Window GetMainWindow();
    }

    public class MainWindowRetriever : IMainWindowRetriever
    {
        public Window GetMainWindow()
        {
            return App.Instance.MainWindow;
        }

    }
}
