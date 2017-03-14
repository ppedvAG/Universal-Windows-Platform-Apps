using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace ContactsUWP.Common.Converter
{
    class BooleanToPinSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool b = (bool)value;

            if (b)
                return new SymbolIcon(Symbol.UnPin);
            else
                return new SymbolIcon(Symbol.Pin);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            SymbolIcon icon = (SymbolIcon)value;

            if (icon.Symbol == Symbol.UnPin)
                return true;
            else
                return false;
        }
    }
}
