using FindAndReplace.Wpf.Initialization;
using FindAndReplace.Wpf.Ioc;
using FindAndReplace.Wpf.ViewModels;
using GalaSoft.MvvmLight;

namespace FindAndReplace.Wpf.Mvvm
{
    public class ViewModelLocator
    {
        public ExceptionViewModel ExceptionViewModel { get { return IocWrapper.Get<ExceptionViewModel>(); } }
        public FnrViewModel FnrViewModel { get { return IocWrapper.Get<FnrViewModel>(); } }
        public MainViewModel MainViewModel { get { return IocWrapper.Get<MainViewModel>(); } }

        public ViewModelLocator()
        {
            if (ViewModelBase.IsInDesignModeStatic)
                DesignTimeBootstrapper.ExecuteInitializationSteps();
        }

    }
}
