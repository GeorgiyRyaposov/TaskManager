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
                switch ((short)value)
                {
                    case (short)StatusEnum.Assigned:
                        return new SolidColorBrush(Colors.DeepSkyBlue);
                    case (short)StatusEnum.InProgress:
                        return new SolidColorBrush(Colors.Yellow);
                    case (short)StatusEnum.Stopped:
                        return new SolidColorBrush(Colors.Gray);
                    case (short)StatusEnum.Complete:
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
