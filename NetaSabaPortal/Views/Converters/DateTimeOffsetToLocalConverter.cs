using System;
using System.Collections.Generic;
// using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace NetaSabaPortal.Views.Converters
{
    public class DateTimeOffsetToLocalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTimeOffset? dateTime = (DateTimeOffset?)value;
            if (dateTime == null)
            {
                return string.Empty;
            }
            return dateTime.Value.ToLocalTime().ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
