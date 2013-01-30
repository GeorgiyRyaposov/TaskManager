using System;
using System.Windows.Data;
using System.Windows.Media;

namespace TaskManager
{
    public enum StatusEnum:short
    {
        Assigned = 1,
        InProgress = 2,
        Stopped = 3,
        Complete = 4
    }

    //Converts StatusID to Color
    class StatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {   
            if (value != null)
            {
                switch ((StatusEnum)value)
                {
                    case StatusEnum.Assigned:
                        return new SolidColorBrush(Colors.DeepSkyBlue);
                    case StatusEnum.InProgress:
                        return new SolidColorBrush(Colors.Yellow);
                    case StatusEnum.Stopped:
                        return new SolidColorBrush(Colors.Gray);
                    case StatusEnum.Complete:
                        return new SolidColorBrush(Colors.LimeGreen);
                    default:
                        return new SolidColorBrush(Colors.Black);
                }
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
