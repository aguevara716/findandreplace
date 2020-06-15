using GalaSoft.MvvmLight;

namespace FindAndReplace.Wpf.Models
{
    public class ReplaceParameters : ObservableObject
    {
        private string replaceString;
        public string ReplaceString
        {
            get { return replaceString; }
            set { Set(nameof(ReplaceString), ref replaceString, value); }
        }

    }
}
