using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace ProcessManagerUltimate.Converters
{
    public class PriorityToRussianConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProcessPriorityClass priority)
            {
                switch (priority)
                {
                    case ProcessPriorityClass.Idle:
                        return "Низкий";
                    case ProcessPriorityClass.BelowNormal:
                        return "Ниже среднего";
                    case ProcessPriorityClass.Normal:
                        return "Средний";
                    case ProcessPriorityClass.AboveNormal:
                        return "Выше среднего";
                    case ProcessPriorityClass.High:
                        return "Высокий";
                    case ProcessPriorityClass.RealTime:
                        return "Реального времени";
                    default:
                        return priority.ToString();
                }
            }
            return value?.ToString() ?? "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}