using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ProcessManagerUltimate.Converters
{
    public class PriorityToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProcessPriorityClass p)
            {
                if (p == ProcessPriorityClass.RealTime)
                    return Brushes.Red;
                if (p == ProcessPriorityClass.High)
                    return Brushes.Orange;
                if (p == ProcessPriorityClass.AboveNormal)
                    return Brushes.Yellow;
                if (p == ProcessPriorityClass.Normal)
                    return Brushes.Transparent;
                if (p == ProcessPriorityClass.BelowNormal)
                    return Brushes.LightBlue;
                if (p == ProcessPriorityClass.Idle)
                    return Brushes.LightGray;
            }

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}