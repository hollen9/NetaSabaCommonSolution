using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nogic.WritableOptions;

using NetaSabaPortal.Options;
using NetaSabaPortal.ViewModels;
using NetaSabaPortal.Views;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using System.Xaml;
using WPFLocalizeExtension.Engine;
using WPFLocalizeExtension.Providers;
using Microsoft.VisualBasic.FileIO;



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

            bool isFileExists_Path = File.Exists(Path.Combine(currentDir, PathOptions.DefaultFileName));
            bool isFileExists_Advanced = File.Exists(Path.Combine(currentDir, AdvancedOptions.DefaultFileName));
            bool isFileExists_Entities = File.Exists(Path.Combine(currentDir, EntitiesOptions.DefaultFileName));

            string baseCfgFilename = "config.jsonc";

            var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(currentDir)
            .AddJsonFile(baseCfgFilename)
            .AddJsonFile(PathOptions.DefaultFileName, true)
            .AddJsonFile(EntitiesOptions.DefaultFileName, true)
            .AddJsonFile(AdvancedOptions.DefaultFileName, true)
            .AddJsonFile(UiOptions.DefaultFileName, true)
            ;

            var cfg = configurationBuilder.Build();

            var svcs = new ServiceCollection();

            // services.AddOptions<PathOptions>().Configure(x => configuration.Bind("path", x));
            svcs.ConfigureWritable<PathOptions>(cfg.GetSection("path"), isFileExists_Path ? PathOptions.DefaultFileName : baseCfgFilename);
            svcs.ConfigureWritable<AdvancedOptions>(cfg.GetSection("advanced"), isFileExists_Advanced ? AdvancedOptions.DefaultFileName : baseCfgFilename);
            svcs.ConfigureWritable<EntitiesOptions>(cfg.GetSection("entities"), isFileExists_Entities ? EntitiesOptions.DefaultFileName : baseCfgFilename);
            svcs.ConfigureWritable<UiOptions>(cfg.GetSection("ui"), isFileExists_Entities ? UiOptions.DefaultFileName : baseCfgFilename);
            
            svcs.AddSingleton<MainWindowVM>();
            svcs.AddSingleton<MainWindow>();

            return svcs.BuildServiceProvider();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var svcs = Services.CreateScope().ServiceProvider;
            var advOpts = svcs.GetRequiredService<IWritableOptions<AdvancedOptions>>();
            var uiOpts = svcs.GetRequiredService<IWritableOptions<UiOptions>>();


            // Select advOpts.Value.Langs to be the list of languages to be used. new List<System.Globalization.CultureInfo>() { ... }
            var culList = new List<System.Globalization.CultureInfo>();
            advOpts.Value.Langs.ForEach(x => culList.Add(System.Globalization.CultureInfo.GetCultureInfo(x)));

            // Somehow LocalizeDictionary is just not able to find zh-tw and zh-cn on its own,
            // so we have to add them manually, not just because of speed.
            (LocalizeDictionary.Instance.DefaultProvider as ResxLocalizationProvider).SearchCultures = culList;
            
            if (!string.IsNullOrEmpty(uiOpts?.Value?.Language))
            {
                LocalizeDictionary.Instance.Culture = System.Globalization.CultureInfo.GetCultureInfo(uiOpts.Value.Language);
            }
            
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }

}
