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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var services = App.Current.Services;
            var viewmodel = services.GetService<NetaSabaPortal.ViewModels.MainWindowVM>();
            DataContext = viewmodel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindowVM vm = (ViewModels.MainWindowVM)DataContext;
            vm.AutoCommand.Execute("steam");
        }
    }
}