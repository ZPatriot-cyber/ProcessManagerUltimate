using System;
using System.Globalization;
using System.Windows.Data;

namespace ProcessManagerUltimate.Converters
{
    public class BytesToMBConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long bytes)
            {
                double mb = bytes / (1024.0 * 1024.0);
                return mb.ToString("N2") + " MB";
            }

            return "0 MB";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}