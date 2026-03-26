using System;
using System.Globalization;
using System.Windows.Data;

namespace ProcessManagerUltimate.Converters
{
    public class CoreRangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int coreCount && coreCount > 0)
                return $"Ядра (0-{coreCount - 1})";
            return "Ядра";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}