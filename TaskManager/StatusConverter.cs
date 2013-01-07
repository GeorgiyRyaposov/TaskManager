using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace TaskManager
{
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
            return "Статус не найден";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
