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
            switch ((bool)value)
            {
                case true:
                    //return Color.Green;
                    return new SolidColorBrush(Colors.LightGreen);
                case false:
                    //return Color.Red;
                    return new SolidColorBrush(Colors.LightSalmon);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
