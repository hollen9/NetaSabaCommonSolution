using NetaSabaPortal.ViewModels;
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

namespace NetaSabaPortal.Views
{
    /// <summary>
    /// Interaction logic for UnarchiveTab.xaml
    /// </summary>
    public partial class UnarchiveTab : UserControl
    {
        public UnarchiveTab()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
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
