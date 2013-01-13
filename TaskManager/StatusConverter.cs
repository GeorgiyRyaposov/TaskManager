using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TaskManager
{
    //Converts StatusID to StatusName
    class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                using (TaskManagerEntities taskManagerEntities = new TaskManagerEntities())
                {
                    var status = (from item in taskManagerEntities.Status
                                 where item.ID == (short)value
                                 select item.Name).First();

                    return status.ToString(CultureInfo.InvariantCulture);
                }
            }
            return Properties.Resources.StatusNotFound;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
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
                        return new SolidColorBrush(Colors.Red);
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
