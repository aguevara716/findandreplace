using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace FindAndReplace.Wpf.Navigation
{
    public interface IFrameNavigationService
    {
        string CurrentPageKey { get; set; }

        T GetPageParameter<T>() where T : class;
        bool IsKeyRegistered(string pageKey);

        bool CanGoBack();
        bool CanGoForward();

        void GoBack();
        void GoForward();

        void NavigateTo(Type pageType);
        void NavigateTo(string pageKey);
        void NavigateTo(Type pageType, object parameter);
        void NavigateTo(string pageKey, object parameter);

        void Configure(Type viewModelType, string pageUri);
        void Configure(Type viewModelType, Uri pageUri);
        void Configure(string key, string pageUri);
        void Configure(string key, Uri pageUri);
    }

    public class FrameNavigationService : IFrameNavigationService, INotifyPropertyChanged
    {
        // Events
        public event PropertyChangedEventHandler PropertyChanged;

        // Variables
        private readonly Func<Frame> FRAME_GETTER;
        private readonly Dictionary<String, Uri> PAGES_BY_KEY;
        private Frame targetFrame;
        private object pageParameter;

        // Binding Variables
        private string currentPageKey;
        public string CurrentPageKey
        {
            get { return currentPageKey; }
            set
            {
                if (currentPageKey == value)
                    return;
                currentPageKey = value;
                RaisePropertyChanged(nameof(CurrentPageKey));
            }
        }

        // Constructors
        public FrameNavigationService(Func<Frame> frameGetter)
        {
            FRAME_GETTER = frameGetter;
            PAGES_BY_KEY = new Dictionary<string, Uri>();
        }

        // Private Methods
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Frame GetTargetFrame()
        {
            if (targetFrame == null)
                targetFrame = FRAME_GETTER.Invoke();
            return targetFrame;
        }

        private void UpdateCurrentPageKey()
        {
            lock(PAGES_BY_KEY)
            {
                var frame = GetTargetFrame();
                if (frame.Source == null)
                    CurrentPageKey = null;
                else
                {
                    var key = PAGES_BY_KEY.First(p => p.Value == frame.Source).Key;
                    CurrentPageKey = key;
                }
            }
        }

        // Public Methods
        public T GetPageParameter<T>()
            where T : class
        {
            var parameter = pageParameter as T;
            if (parameter == null && pageParameter != null)
                throw new InvalidCastException($"{nameof(pageParameter)} is a {pageParameter.GetType().Name}, not a {typeof(T).Name}");

            return parameter;
        }

        public bool IsKeyRegistered(string pageKey)
        {
            return PAGES_BY_KEY.ContainsKey(pageKey);
        }

        public bool CanGoBack()
        {
            return GetTargetFrame().CanGoBack;
        }

        public bool CanGoForward()
        {
            return GetTargetFrame().CanGoForward;
        }

        public void GoBack()
        {
            if (!CanGoBack())
                return;

            GetTargetFrame().GoBack();
            UpdateCurrentPageKey();
        }

        public void GoForward()
        {
            if (!CanGoForward())
                return;

            GetTargetFrame().GoForward();
            UpdateCurrentPageKey();
        }

        public void NavigateTo(Type pageType)
        {
            NavigateTo(pageType.Name, null);
        }

        public void NavigateTo(string pageKey)
        {
            NavigateTo(pageKey, null);
        }

        public void NavigateTo(Type pageType, object parameter)
        {
            NavigateTo(pageType.Name, parameter);
        }

        public void NavigateTo(string pageKey, object parameter)
        {
            lock (PAGES_BY_KEY)
            {
                if (!PAGES_BY_KEY.ContainsKey(pageKey))
                    throw new ArgumentException($"No such page: {pageKey}. Did you forget to call the {nameof(Configure)} method?", nameof(pageKey));

                GetTargetFrame().Navigate(PAGES_BY_KEY[pageKey]);
                UpdateCurrentPageKey();
                pageParameter = parameter;
            }
        }

        public void Configure(Type viewModelType, string pageUri)
        {
            Configure(viewModelType.Name, new Uri(pageUri, UriKind.Relative));
        }

        public void Configure(Type viewModelType, Uri pageUri)
        {
            Configure(viewModelType.Name, pageUri);
        }

        public void Configure(string key, string pageUri)
        {
            Configure(key, new Uri(pageUri, UriKind.Relative));
        }

        public void Configure(string key, Uri pageUri)
        {
            lock (PAGES_BY_KEY)
            {
                if (PAGES_BY_KEY.ContainsKey(key))
                    throw new ArgumentException($"This key has already been used: {key}", nameof(key));
                if (PAGES_BY_KEY.ContainsValue(pageUri))
                    throw new ArgumentException($"This type has already been configured with key {PAGES_BY_KEY.First(p => p.Value == pageUri).Key}", nameof(pageUri));
                PAGES_BY_KEY.Add(key, pageUri);
            }
        }

    }
}
