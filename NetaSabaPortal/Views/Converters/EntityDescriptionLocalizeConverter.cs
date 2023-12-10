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
    public class EntityDescriptionLocalizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return string.Empty;
            EntityDefinition def = (EntityDefinition)value;
            if (def.EntityDescriptions == null)
            {
                return string.Empty;
            }
            var cul = LocalizeDictionary.CurrentCulture;
            string result = FindDescription(def.EntityDescriptions, cul);
            
            return result;
        }
        string FindDescription(Dictionary<string,string> entityDescriptions, CultureInfo cultureInfo)
        {
            if (entityDescriptions == null || entityDescriptions.Count == 0)
            {
                return string.Empty;
            }

            string cultureName = cultureInfo.Name;

            // 尋找精確匹配的文化
            if (entityDescriptions.TryGetValue(cultureName.ToLower(), out var description))
            {
                return description;
            }

            // 尋找包含主文化的文化
            if (cultureInfo.Parent != null && entityDescriptions.TryGetValue(cultureInfo.Parent.Name.ToLower(), out description))
            {
                return description;
            }

            // 回退到默認值
            if (entityDescriptions.TryGetValue("en", out description))
            {
                return description;
            }

            return entityDescriptions.FirstOrDefault().Value ?? string.Empty;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
