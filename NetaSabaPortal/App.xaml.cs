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
using System.Management;
using System.Collections.ObjectModel;
using NetaSabaPortal.Repositories;
// using Dapperer.QueryBuilders.MsSql;
using WinRT;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using DapperAid;
using Dapper;
using System.Text.Json;
using System.Reflection;


//[assembly: AssemblyVersion("0.3.0")]
namespace NetaSabaPortal
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            SessionId = Guid.NewGuid();
            Svc = ConfigureServices();
            this.InitializeComponent();

            var ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            Version = ver;//$"{@var.Major}.{@var.MajorRevision}.{@var.Minor}";
        }

        public new static App Current => (App)Application.Current;

        public IServiceProvider Svc { get; }
        public Guid SessionId { get; }

        public Version Version { get; }

        public const string AppIdentifier = "NetaSabaPortal";
        public const string ConfigParentFolder = "Hollen.Tech";
       

        private static IServiceProvider ConfigureServices()
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string configDir;

            var preConfigurationBuilder = new ConfigurationBuilder();
            preConfigurationBuilder.SetBasePath(currentDir).AddJsonFile(AdvancedOptions.DefaultFileName);
            var preCfg = preConfigurationBuilder.Build();
            var preCfgAdv = preCfg.GetSection("advanced");
            bool isStoreInAppData = preCfgAdv.GetValue<bool>(nameof(AdvancedOptions.IsStoreInAppData));
            bool isForceInitEntities = preCfgAdv.GetValue<bool>(nameof(AdvancedOptions.IsForceInitEntities));


            if (isStoreInAppData)
            {
                configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.ConfigParentFolder, App.AppIdentifier);
            }
            else
            {
                configDir = currentDir;
            }


            string baseCfgFilename = "config.json";

            if (isStoreInAppData)
            {
                InitConfigIfNotExists(baseCfgFilename);
                InitConfigIfNotExists(PathOptions.DefaultFileName);
                InitConfigIfNotExists(EntitiesOptions.DefaultFileName, isForceInitEntities);
                InitConfigIfNotExists(AdvancedOptions.DefaultFileName);
                InitConfigIfNotExists(UiOptions.DefaultFileName);
                InitConfigIfNotExists(WatcherOptions.DefaultFileName);
            }

            bool isFileExists_Base = File.Exists(GetFullCombinedPath(configDir, baseCfgFilename));
            bool isFileExists_Path = File.Exists(GetFullCombinedPath(configDir, PathOptions.DefaultFileName));
            bool isFileExists_Advanced = File.Exists(GetFullCombinedPath(configDir, AdvancedOptions.DefaultFileName));
            bool isFileExists_Entities = File.Exists(GetFullCombinedPath(configDir, EntitiesOptions.DefaultFileName));
            bool isFileExists_Watcher = File.Exists(GetFullCombinedPath(configDir, WatcherOptions.DefaultFileName));

            var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(configDir)
            .AddJsonFile(baseCfgFilename)
            // Add options files
            .AddJsonFile(PathOptions.DefaultFileName, true)
            .AddJsonFile(EntitiesOptions.DefaultFileName, true)
            .AddJsonFile(AdvancedOptions.DefaultFileName, true)
            .AddJsonFile(UiOptions.DefaultFileName, true)
            .AddJsonFile(WatcherOptions.DefaultFileName, true)
            ;

            var cfg = configurationBuilder.Build();

            var services = new ServiceCollection();

            // services.AddOptions<PathOptions>().Configure(x => configuration.Bind("path", x));
            services.ConfigureWritable<PathOptions>(cfg.GetSection("path"), GetFullCombinedPath(configDir, isFileExists_Path ? PathOptions.DefaultFileName : baseCfgFilename));
            services.ConfigureWritable<EntitiesOptions>(cfg.GetSection("entities"), GetFullCombinedPath(configDir, isFileExists_Entities ? EntitiesOptions.DefaultFileName : baseCfgFilename));
            services.ConfigureWritable<UiOptions>(cfg.GetSection("ui"), GetFullCombinedPath(configDir, isFileExists_Entities ? UiOptions.DefaultFileName : baseCfgFilename));
            services.ConfigureWritable<WatcherOptions>(cfg.GetSection("watcher"), GetFullCombinedPath(configDir, isFileExists_Watcher ? WatcherOptions.DefaultFileName : baseCfgFilename));
            //services.ConfigureWritable<AdvancedOptions>(cfg.GetSection("advanced"), isFileExists_Advanced ? AdvancedOptions.DefaultFileName : baseCfgFilename);
            services.AddOptions<AdvancedOptions>().Configure(x => cfg.Bind("advanced", x));
            
            //string savedataPath = Path.GetFullPath(Path.Combine(configDir, DataOptions.DefaultFileName));

            //if (File.Exists(Path.GetFullPath(Path.Combine(DataOptions.DefaultFileName))) != true)
            //{
            //    using (var fs = File.Create(savedataPath))
            //    {
            //        var obj = new 
            //        {
            //            data = new DataOptions()
            //        };
            //        string text = System.Text.Json.JsonSerializer.Serialize(obj);
            //        fs.Write(System.Text.Encoding.UTF8.GetBytes(text));
            //        fs.Close();
            //    }
            //}

            services.AddSingleton<DapperAid.QueryBuilder>(new QueryBuilder.SQLite());
            services.AddSingleton<Services.IConnectionProvider, Services.SqliteConnectionProvider>();
            services.AddSingleton<Repositories.WatcherRepository>();


            //services.ConfigureWritable<DataOptions>(cfg.GetSection("data"), DataOptions.DefaultFileName);

            services.AddSingleton<MainWindowVM>();
            services.AddSingleton<EditWatcherItemDialogVM>();
            services.AddSingleton<MainWindow>();
            SqlMapper.AddTypeHandler(new Extensions.SqliteGuidTypeHandler());
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));

            //services.AddSingleton<Dapperer.IQueryBuilder, SqlQueryBuilder>();
            //services.AddSingleton<Dapperer.IDbFactory, Dapperer.DbFactories.SqlDbFactory>();
            //services.AddSingleton<Dapperer.IDappererSettings, Extensions.DappererSettings>();

            return services.BuildServiceProvider();
        }

        private static string GetFullCombinedPath(params string[] arrPath)
        {
            return Path.GetFullPath(Path.Combine(arrPath));
        }

        private static void InitConfigIfNotExists(string fileName, bool forceInit = false)
        {
            
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            string localAppDataPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.ConfigParentFolder, App.AppIdentifier, fileName));
            string exePath = Path.GetFullPath(Path.Combine(baseDir, fileName));

            if (forceInit || !File.Exists(localAppDataPath))
            {
                // 如果在 AppData 中找不到，則複製一份
                Directory.CreateDirectory(Path.GetDirectoryName(localAppDataPath));
                if (!File.Exists(exePath))
                {
                    return;
                }
                File.Copy(exePath, localAppDataPath, overwrite: true);
            }

            return;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var svcs = Svc.CreateScope().ServiceProvider;
            var advOpts = svcs.GetRequiredService<IOptions<AdvancedOptions>>();
            var uiOpts = svcs.GetRequiredService<IWritableOptions<UiOptions>>();
            

            // Select advOpts.Value.Langs to be the list of languages to be used. new List<System.Globalization.CultureInfo>() { ... }
            var culList = new List<System.Globalization.CultureInfo>();
            //var rlp = MainResxLocalizationProvider.Instance;
            var rlp = LocalizeDictionary.Instance.DefaultProvider as ResxLocalizationProvider;
            advOpts.Value.Langs.ForEach(x =>
            {
                var cul = System.Globalization.CultureInfo.GetCultureInfo(x);
                culList.Add(cul);
                // rlp.AvailableCultures.Add(cul);
            });
            
            rlp.SearchCultures = culList;
            rlp.UpdateCultureList("NetaSabaPortal", "Strings");
            LocalizeDictionary.Instance.DefaultProvider = rlp;            
            //var dappererSettings = svcs.GetRequiredService<Dapperer.IDappererSettings>();
            //dappererSettings = new Extensions.DappererSettings() { ConnectionString = advOpts.Value.ConnectionString };
            

            //rlp.AvailableCultures.Clear();


            // Somehow LocalizeDictionary is just not able to find zh-tw and zh-cn on its own,
            // so we have to add them manually, not just because of speed.

            //rlp.SearchCultures = culList;
            //string assembName = "NetaSabaPortal";// System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            //string baseLocName = "Strings";


            //rlp.UpdateCultureList(assembName, baseLocName);
            //foreach (var cul in culList)
            //{
            //    if (cul.Name == "zh-Hant-TW")
            //    {
            //        rlp.UpdateCultureList(assembName, $"{baseLocName}.zh-TW");
            //        continue;
            //    }
            //    if (cul.Name == "zh-Hans-CN")
            //    {
            //        rlp.UpdateCultureList(assembName, $"{baseLocName}.zh-CN");
            //        continue;
            //    }

            //    rlp.UpdateCultureList(assembName, $"{baseLocName}.{cul.Name}");
            //}



            if (!string.IsNullOrEmpty(uiOpts?.Value?.Language))
            {
                LocalizeDictionary.Instance.Culture = System.Globalization.CultureInfo.GetCultureInfo(uiOpts.Value.Language);
            }
            else
            {
                LocalizeDictionary.Instance.Culture = System.Globalization.CultureInfo.CurrentUICulture;
            }
            
            var mainWindow = Svc.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }

}
