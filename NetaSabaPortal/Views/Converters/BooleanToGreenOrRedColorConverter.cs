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
    public class BooleanToGreenOrRedColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush colorBrush;

            if (value is string str)
            {
                if (value.ToString() == "True")
                {
                    colorBrush = new SolidColorBrush(Colors.Green);
                    return colorBrush;
                }
                else if (value.ToString() == "False")
                {
                    colorBrush = new SolidColorBrush(Colors.DarkRed);
                    return colorBrush;
                }
                else
                {
                    colorBrush = new SolidColorBrush(Colors.DarkRed);
                    return colorBrush;
                }
            }

            switch ((bool?)value)
            {
                case true:
                    colorBrush = new SolidColorBrush(Colors.Green);
                    //colorBrush = new SolidColorBrush(Colors.LightGreen);
                    return colorBrush;
                case null:
                case false:
                    colorBrush = new SolidColorBrush(Colors.DarkRed);
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
