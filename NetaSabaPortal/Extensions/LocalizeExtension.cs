using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WPFLocalizeExtension.Extensions;

namespace NetaSabaPortal.Extensions
{
    public static class LocalizeExtension
    {
        private const string ResourceKey = "Strings";
        public static T GetText<T>(string key)
        {
            return LocExtension.GetLocalizedValue<T>(Assembly.GetCallingAssembly().GetName().Name + ":"+ ResourceKey + ":" + key);
        }
    }
}
