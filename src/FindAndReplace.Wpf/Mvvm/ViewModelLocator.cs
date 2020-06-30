using FindAndReplace.Wpf.Initialization;
using FindAndReplace.Wpf.Ioc;
using FindAndReplace.Wpf.ViewModels;
using GalaSoft.MvvmLight;

namespace FindAndReplace.Wpf.Mvvm
{
    public class ViewModelLocator
    {
        public ExceptionViewModel ExceptionViewModel { get { return IocWrapper.GetWithoutCaching<ExceptionViewModel>(); } }
        public FnrViewModel FnrViewModel { get { return IocWrapper.GetWithoutCaching<FnrViewModel>(); } }
        public MainViewModel MainViewModel { get { return IocWrapper.GetWithoutCaching<MainViewModel>(); } }

        public ViewModelLocator()
        {
            if (ViewModelBase.IsInDesignModeStatic)
                DesignTimeBootstrapper.ExecuteInitializationSteps();
        }

    }
}
