using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using FindAndReplace.Wpf.Extensions;
using FindAndReplace.Wpf.Initialization;
using FindAndReplace.Wpf.Ioc;
using FindAndReplace.Wpf.Models;
using FindAndReplace.Wpf.Mvvm;
using FindAndReplace.Wpf.Navigation;
using FindAndReplace.Wpf.Views;
using GalaSoft.MvvmLight;

namespace FindAndReplace.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Variables
        public static App Instance { get; private set; }
        public ViewModelLocator ViewModelLocator { get; }

        // Constructors
        public App()
        {
            Instance = App.Current as App;
            ViewModelLocator = Resources["Locator"] as ViewModelLocator;

            if (ViewModelBase.IsInDesignModeStatic)
                DesignTimeBootstrapper.ExecuteInitializationSteps();
            else
                RuntimeBootstrapper.ExecuteInitializationSteps();
        }

        // Lifecycle Events
        protected override void OnStartup(StartupEventArgs e)
        {
            WireUpEvents();

            // app launches
            RuntimeBootstrapper.ExecuteStartupSteps();
        }

        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            // fired when windows session ends (logging off or shutting down)
            RuntimeBootstrapper.ExecuteUserSessionEndingSteps();
        }

        protected override void OnActivated(EventArgs e)
        {
            // app goes to the foreground
            RuntimeBootstrapper.ExecuteFocusSteps();
        }

        protected override void OnDeactivated(EventArgs e)
        {
            // app goes to the background
            RuntimeBootstrapper.ExecuteUnfocusSteps();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // app is closed
            RuntimeBootstrapper.ExecuteExitSteps();
        }

        private void OnAppUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            OnUnhandledException(e.Exception);
            e.Handled = true;
        }

        private void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            OnUnhandledException(exception);
        }

        // Private Methods
        private void WireUpEvents()
        {
            DispatcherUnhandledException += OnAppUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
        }

        private void OnUnhandledException(Exception exception)
        {
            var windowNavService = IocWrapper.Get<IWindowNavigationService>();
            var exList = exception.Flatten();
            var exModels = exList.Select(e => new ExceptionModel(e));
            windowNavService.OpenWindow(typeof(ExceptionWindow), exModels, false, false);
        }

    }
}
