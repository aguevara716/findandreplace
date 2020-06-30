using GalaSoft.MvvmLight;

namespace FindAndReplace.Wpf.Models
{
    public class Match : ObservableObject
    {
        private int index;
        public int Index
        {
            get { return index; }
            set { Set(nameof(Index), ref index, value); }
        }

        private int length;
        public int Length
        {
            get { return length; }
            set { Set(nameof(Length), ref length, value); }
        }
    }
}
