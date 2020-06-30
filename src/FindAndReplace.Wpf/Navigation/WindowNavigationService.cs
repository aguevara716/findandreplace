using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace FindAndReplace.Wpf.Navigation
{
    public interface IWindowNavigationService
    {
        T GetWindowParameter<T>() where T : class;
        bool IsKeyRegistered(string windowKey);

        void OpenWindow(Type vmType, bool isTopMost, bool isDialog);
        void OpenWindow(Type vmType, object parameter, bool isTopMost, bool isDialog);

        void OpenWindow(string key, bool isTopMost, bool isDialog);
        void OpenWindow(string key, object parameter, bool isTopMost, bool isDialog);
    }


    public class WindowNavigationService : IWindowNavigationService
    {
        // Variables
        private readonly Func<Window> MAIN_WINDOW_GETTER;
        private readonly Dictionary<String, Type> WINDOWS_BY_KEY;
        private Window mainWindow;
        private object windowParameter;

        // Constructors
        public WindowNavigationService(Func<Window> mainWindowGetter)
        {
            MAIN_WINDOW_GETTER = mainWindowGetter;
            WINDOWS_BY_KEY = new Dictionary<String, Type>();
        }

        // Private methods
        private Window GetMainWindow()
        {
            if (mainWindow == null && MAIN_WINDOW_GETTER != null)
                mainWindow = MAIN_WINDOW_GETTER();
            return mainWindow;
        }

        // Public methods
        public T GetWindowParameter<T>()
            where T : class
        {
            var parameter = windowParameter as T;
            if (parameter == null && windowParameter != null)
                throw new InvalidCastException($"{nameof(windowParameter)} is a {windowParameter.GetType().Name}, not a {typeof(T).Name}");

            return parameter;
        }

        public bool IsKeyRegistered(string windowKey)
        {
            return WINDOWS_BY_KEY.ContainsKey(windowKey);
        }

        public void OpenWindow(Type vmType, bool isTopMost, bool isDialog)
        {
            OpenWindow(vmType.Name, null, isTopMost, isDialog);
        }

        public void OpenWindow(Type vmType, object parameter, bool isTopMost, bool isDialog)
        {
            OpenWindow(vmType.Name, parameter, isTopMost, isDialog);
        }

        public void OpenWindow(string key, bool isTopMost, bool isDialog)
        {
            OpenWindow(key, null, isTopMost, isDialog);
        }

        public void OpenWindow(string key, object parameter, bool isTopMost, bool isDialog)
        {
            lock (WINDOWS_BY_KEY)
            {
                if (!WINDOWS_BY_KEY.ContainsKey(key))
                    throw new ArgumentException($"No such window: {key}. Did you forget to call the Configure method?", nameof(key));

                windowParameter = parameter;

                var type = WINDOWS_BY_KEY[key];
                var window = type.GetConstructor(Type.EmptyTypes).Invoke(null) as Window;
                window.Topmost = isTopMost;
                window.Owner = GetMainWindow();

                if (isDialog)
                    window.ShowDialog();
                else
                    window.Show();
            }
        }

        public void Configure<T>() where T : Window
        {
            Configure(typeof(T), typeof(T).Name);
        }

        public void Configure<T>(string key) where T : Window
        {
            Configure(typeof(T), key);
        }

        public void Configure(Type windowType)
        {
            Configure(windowType, windowType.Name);
        }

        public void Configure(Type windowType, string key)
        {
            lock (WINDOWS_BY_KEY)
            {
                if (WINDOWS_BY_KEY.ContainsKey(key))
                    throw new ArgumentException($"This key has already been used: \"{key}\"", nameof(key));
                if (WINDOWS_BY_KEY.ContainsValue(windowType))
                    throw new ArgumentException($"This type has already been configured with key \"{WINDOWS_BY_KEY.First(t => t.Value == windowType).Key}\"", nameof(windowType));

                WINDOWS_BY_KEY.Add(key, windowType);
            }
        }
    }
}
