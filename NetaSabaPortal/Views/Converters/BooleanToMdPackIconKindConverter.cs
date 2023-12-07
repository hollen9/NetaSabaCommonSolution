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
    public class BooleanToMdPackIconKindConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value is string str)
                {
                    if (value.ToString() == "True")
                    {
                        return MaterialDesignThemes.Wpf.PackIconKind.CheckboxMarkedCircle;
                    }
                    else if (value.ToString() == "False")
                    {
                        return MaterialDesignThemes.Wpf.PackIconKind.CheckboxBlankCircle;
                    }
                    else
                    {
                        return MaterialDesignThemes.Wpf.PackIconKind.CheckboxBlankCircle;
                    }
                }

                switch ((bool?)value)
                {
                    case true:
                        return MaterialDesignThemes.Wpf.PackIconKind.CheckboxMarkedCircle;
                    case false:
                        return MaterialDesignThemes.Wpf.PackIconKind.CheckboxBlankCircle;
                    case null:
                        return MaterialDesignThemes.Wpf.PackIconKind.CheckboxBlankCircle;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
