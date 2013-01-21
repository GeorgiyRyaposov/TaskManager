using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace TaskManager
{
    //Converts StatusID to StatusName
    class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                switch ((short)value)
                {
                    case (short)StatusEnum.Assigned:
                        return Properties.Resources.Status_Assigned;
                    case (short)StatusEnum.Complete:
                        return Properties.Resources.Status_Complete;
                    case (short)StatusEnum.InProgress:
                        return Properties.Resources.Status_InProgress;
                    case (short)StatusEnum.Stopped:
                        return Properties.Resources.Status_Stopped;
                }
            }
            return Properties.Resources.StatusNotFound;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

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
                    case 1:
                        return new SolidColorBrush(Colors.DeepSkyBlue);
                    case 2:
                        return new SolidColorBrush(Colors.Yellow);
                    case 3:
                        return new SolidColorBrush(Colors.Gray);
                    case 4:
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
