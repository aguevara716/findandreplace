using System;
using System.Windows;

namespace FindAndReplace.Wpf.Services
{
    public interface IClipboardDataService
    {
        // Text
        string GetText();
        void SetText(string text);
    }

    public class ClipboardDataService : IClipboardDataService
    {
        // Constructors
        public ClipboardDataService()
        {

        }

        // Text
        public string GetText()
        {
            if (!Clipboard.ContainsText())
                return String.Empty;

            return Clipboard.GetText();
        }

        public void SetText(string text)
        {
            Clipboard.SetText(text);
        }

    }
}
