using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NetaSabaPortal.Options;
using NetaSabaPortal.ViewModels;
using NetaSabaPortal.Views;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using System.Xaml;

namespace NetaSabaPortal
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private static IServiceProvider ConfigureServices()
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;

            var configuration = new ConfigurationBuilder()
            .SetBasePath(currentDir)
            .AddJsonFile("config.jsonc")
            //.AddJsonFile("config_secret.jsonc")
            .Build();
//
            //configuration.Bind("directories", new DirectoryOptions());

            var services = new ServiceCollection();

            //services.AddOptions<DirectoryOptions>().Bind(configuration.GetSection("revive"));
            
            // services.AddOptions<DirectoryOptions>().Configure(x=> configuration.Bind("directories"));
            services.AddOptions<DirectoryOptions>().Configure(x => configuration.Bind("directories", x));
            services.AddTransient<MainWindowVM>();
            services.AddSingleton<MainWindow>();

            return services.BuildServiceProvider();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }

}
