using FindAndReplace.Wpf.Backend.Files;
using FindAndReplace.Wpf.Backend.Filesystem;
using FindAndReplace.Wpf.DesignViewModels;
using FindAndReplace.Wpf.Dialogs;
using FindAndReplace.Wpf.Ioc;
using FindAndReplace.Wpf.Mappers;
using FindAndReplace.Wpf.Navigation;
using FindAndReplace.Wpf.Services;
using FindAndReplace.Wpf.ViewModels;
using FindAndReplace.Wpf.Views;

namespace FindAndReplace.Wpf.Initialization
{
    public static class IocRegistrar
    {
        public static void RegisterDesignViewModels()
        {
            IocWrapper.Register<DesignFnrViewModel>();
            IocWrapper.Register<DesignExceptionViewModel>();
            IocWrapper.Register<DesignMainViewModel>();
        }

        public static void RegisterDialogService()
        {
            IocWrapper.Register<IDialogService, DialogService>();
        }

        public static void RegisterFileServices()
        {
            IocWrapper.Register<IBinaryFileDetector, BinaryFileDetector>();
            IocWrapper.Register<IFileReader, FileReader>();
            IocWrapper.Register<IFileWriter, FileWriter>();
            IocWrapper.Register<IFinderService, FinderService>();
            IocWrapper.Register<IMatchPreviewExtractor, MatchPreviewExtractor>();
            IocWrapper.Register<ITextMatcher, TextMatcher>();
            IocWrapper.Register<ITextReplacer, TextReplacer>();
        }

        public static void RegisterFilesystemServices()
        {
            IocWrapper.Register<IFileDiscoverer, FileDiscoverer>();
            IocWrapper.Register<IFileFilterer, FileFilterer>();
            IocWrapper.Register<IFileRetriever, FileRetriever>();
            IocWrapper.Register<IRelativePathExtractor, RelativePathExtractor>();
        }

        public static void RegisterMappers()
        {
            IocWrapper.Register<IFileResultMapper, FileResultMapper>();
        }

        public static void RegisterNavigationServices()
        {
            IocWrapper.Register<IFrameNavigationService>(() =>
            {
                var fns = new FrameNavigationService(() => (App.Instance.MainWindow as MainWindow).MainFrame);

                fns.Configure(typeof(FnrViewModel), "Views/FnrView.xaml");

                return fns;
            });

            IocWrapper.Register<IWindowNavigationService>(() =>
            {
                var wns = new WindowNavigationService(() => App.Instance.MainWindow);

                wns.Configure<ExceptionWindow>();

                return wns;
            });
        }

        public static void RegisterServices()
        {
            IocWrapper.Register<IClipboardDataService, ClipboardDataService>();
            IocWrapper.Register<IExceptionFormatter, ExceptionFormatter>();
            IocWrapper.Register<IMatchPreviewGenerator, MatchPreviewGenerator>();
            IocWrapper.Register<ISettingsService, SettingsService>();
        }

        public static void RegisterViewModels()
        {
            IocWrapper.Register<FnrViewModel>();
            IocWrapper.Register<ExceptionViewModel>();
            IocWrapper.Register<MainViewModel>();
        }

    }
}
