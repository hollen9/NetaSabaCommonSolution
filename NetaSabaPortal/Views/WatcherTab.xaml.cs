using NetaSabaPortal.Models;
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
    /// Interaction logic for WatcherTab.xaml
    /// </summary>
    public partial class WatcherTab : UserControl
    {
        public WatcherTab()
        {
            InitializeComponent();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is not MainWindowVM vm)
            {
                return;
            }
            if (e.Source is not ListViewItem lvItem)
            {
                return;
            }
            if (lvItem.Content is not WatcherItem wItem)
            {
                return;
            }
            vm.ModifyWatchItemCmd.Execute(wItem);
        }
    }
}
