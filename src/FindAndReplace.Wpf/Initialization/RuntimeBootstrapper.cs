namespace FindAndReplace.Wpf.Initialization
{
    public static class RuntimeBootstrapper
    {
        // Lifecycle Events
        public static void ExecuteInitializationSteps()
        {

        }

        public static void ExecuteStartupSteps()
        {
            IocRegistrar.RegisterDialogService();
            IocRegistrar.RegisterNavigationServices();
            IocRegistrar.RegisterServices();
            IocRegistrar.RegisterViewModels();
        }

        public static void ExecuteUserSessionEndingSteps()
        {

        }

        public static void ExecuteFocusSteps()
        {

        }

        public static void ExecuteUnfocusSteps()
        {

        }

        public static void ExecuteExitSteps()
        {

        }

        // Private Methods
    }
}
