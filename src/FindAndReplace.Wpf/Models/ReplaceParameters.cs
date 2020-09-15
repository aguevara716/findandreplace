using GalaSoft.MvvmLight;

namespace FindAndReplace.Wpf.Models
{
    public class ReplaceParameters : ObservableObject
    {
        private bool isRetainingModifiedDate;
        public bool IsRetainingModifiedDate
        {
            get { return isRetainingModifiedDate; }
            set { Set(nameof(IsRetainingModifiedDate), ref isRetainingModifiedDate, value); }
        }

        private string replaceString;
        public string ReplaceString
        {
            get { return replaceString; }
            set { Set(nameof(ReplaceString), ref replaceString, value); }
        }

    }
}
