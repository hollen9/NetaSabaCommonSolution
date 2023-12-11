using Microsoft.Extensions.DependencyInjection;
using NetaSabaPortal.Models;
using NetaSabaPortal.ViewModels;
using NetaSabaPortal.Views.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFLocalizeExtension.Engine;

namespace NetaSabaPortal.Views
{
    /// <summary>
    /// Interaction logic for UnarchiveTab.xaml
    /// </summary>
    public partial class UnarchiveTab : UserControl
    {
        public UnarchiveTab()
        {
            var vm = App.Current.Svc.GetRequiredService<MainWindowVM>();

            LocalizeDictionary.Instance.PropertyChanged += (s, e) => 
            {
                if (e.PropertyName == "Culture")
                {
                    EntityNameLocalizeConverter converter = new EntityNameLocalizeConverter();
                    string res = converter.Convert((EntityDefinition) cbEntities.SelectedItem, typeof(string), null, LocalizeDictionary.Instance.Culture).ToString();
                    cbEntities.Text = res;
                }
            };

            InitializeComponent();
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext == null)
            {
                return;
            }

            MainWindowVM vm = (ViewModels.MainWindowVM) DataContext;
            int selectedIdx = 0;
            for (int i = vm.EntitiesDefinitions.Count - 1; i >= 0; i--)
            {
                Models.EntityDefinition entity = vm.EntitiesDefinitions[i];
                if (entity.IsDefault ?? false)
                {
                    selectedIdx = i;
                }
            }
            cbEntities.SelectedIndex = selectedIdx;
        }
    }
}
