using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace FindAndReplace.Wpf.Converters
{
    public class NegateBooleanConverter : MarkupExtension, IValueConverter
    {
        private static NegateBooleanConverter _converter = null;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_converter == null)
                _converter = new NegateBooleanConverter();
            return _converter;
        }

        private bool Convert(object value)
        {
            if (value == null || !(value is bool))
                return false;

            var boolValue = System.Convert.ToBoolean(value);
            return !boolValue;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value);
        }

    }
}
