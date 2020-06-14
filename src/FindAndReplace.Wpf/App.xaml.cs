using System;
using System.Windows;
using FindAndReplace.Wpf.Initialization;

namespace FindAndReplace.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Variables

        // Constructors
        public App()
        {
            RuntimeBootstrapper.ExecuteInitializationSteps();
        }

        // Lifecycle Events
        protected override void OnStartup(StartupEventArgs e)
        {
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

    }
}
