using Microsoft.Extensions.DependencyInjection;
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

namespace NetaSabaPortal.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for EditWatcherItemDialog.xaml
    /// </summary>
    public partial class EditWatcherItemDialog : UserControl
    {
        
        public EditWatcherItemDialog()
        {
            var services = App.Current.Services;
            var viewmodel = services.GetService<NetaSabaPortal.ViewModels.EditWatcherItemDialogVM>();
            DataContext = viewmodel;

            //DataContext = new EditWatcherItemDialogVM();
            InitializeComponent();
        }
        //public EditWatcherItemDialog(string name)
        //{
        //    InitializeComponent();
        //}
    }
}
