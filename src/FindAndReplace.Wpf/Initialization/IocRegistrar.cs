using FindAndReplace.Wpf.DesignViewModels;
using FindAndReplace.Wpf.Dialogs;
using FindAndReplace.Wpf.Ioc;
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
            IocWrapper.Register<IFinderMapper, FinderMapper>();
            IocWrapper.Register<IFinderThreadWorker, FinderThreadWorker>();
            IocWrapper.Register<IProcessStatusMapper, ProcessStatusMapper>();
            IocWrapper.Register<IReplacerMapper, ReplacerMapper>();
            IocWrapper.Register<IReplacerThreadWorker, ReplacerThreadWorker>();
            IocWrapper.Register<IResultMapper, ResultMapper>();
        }

        public static void RegisterViewModels()
        {
            IocWrapper.Register<FnrViewModel>();
            IocWrapper.Register<ExceptionViewModel>();
            IocWrapper.Register<MainViewModel>();
        }

    }
}
