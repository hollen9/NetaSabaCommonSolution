using NetaSabaPortal.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

// using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using WPFLocalizeExtension.Engine;

namespace NetaSabaPortal.Views.Converters
{
    public class EntityNameLocalizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return string.Empty;
            EntityDefinition def = (EntityDefinition)value;
            if (def.EntityNames == null)
            {
                return def.EntityName;
            }
            var cul = LocalizeDictionary.CurrentCulture;
            string result = FindName(def, cul);
            
            return result;
        }
        string FindName(EntityDefinition entityDef, CultureInfo cultureInfo)
        {
            var names = entityDef.EntityNames;
            if (names == null || names.Count == 0)
            {
                return string.Empty;
            }

            string cultureName = cultureInfo.Name;

            // 尋找精確匹配的文化
            if (names.TryGetValue(cultureName.ToLower(), out var description))
            {
                return description;
            }

            // 尋找包含主文化的文化
            if (cultureInfo.Parent != null && names.TryGetValue(cultureInfo.Parent.Name.ToLower(), out description))
            {
                return description;
            }

            // 回退到默認值
            
            return entityDef.EntityName;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
