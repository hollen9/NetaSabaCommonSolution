using MaterialDesignExtensions.Controls;
using Microsoft.Extensions.DependencyInjection;
using NetaSabaPortal.ViewModels;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow// : MaterialWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var services = App.Current.Svc;
            var viewmodel = services.GetService<NetaSabaPortal.ViewModels.MainWindowVM>();
            DataContext = viewmodel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindowVM vm = (ViewModels.MainWindowVM)DataContext;
            vm.AutoCommand.Execute("steam");

            //int selectedIdx = 0;
            //for (int i = vm.EntitiesDefinitions.Count - 1; i >= 0; i--)
            //{
            //    Models.EntityDefinition entity = vm.EntitiesDefinitions[i];
            //    if (entity.IsDefault ?? false)
            //    {
            //        selectedIdx = i;
            //    }
            //}
            tabUnarchive.DataContext = vm;            
        }
    }
}