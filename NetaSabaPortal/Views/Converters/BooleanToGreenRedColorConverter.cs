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
    public class BooleanToGreenRedColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush colorBrush;
            switch ((bool)value)
            {
                case true:
                    colorBrush = new SolidColorBrush(Colors.DarkGray);
                    //colorBrush = new SolidColorBrush(Colors.LightGreen);
                    return colorBrush;
                case false:
                    colorBrush = new SolidColorBrush(Colors.Crimson);
                    //colorBrush = new SolidColorBrush(Colors.LightSalmon);
                    return colorBrush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
