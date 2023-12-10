﻿using Microsoft.Extensions.Configuration;
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



namespace NetaSabaPortal
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //LocalizeDictionary.Instance.PropertyChanged += (sender, e) =>
            //{
            //    if (e.PropertyName == "Culture")
            //    {
            //        App.Current.
            //    }
            //};
            SessionId = Guid.NewGuid();
            Svc = ConfigureServices();
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Svc { get; }
        public Guid SessionId { get; }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private static IServiceProvider ConfigureServices()
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;

            bool isFileExists_Path = File.Exists(Path.Combine(currentDir, PathOptions.DefaultFileName));
            bool isFileExists_Advanced = File.Exists(Path.Combine(currentDir, AdvancedOptions.DefaultFileName));
            bool isFileExists_Entities = File.Exists(Path.Combine(currentDir, EntitiesOptions.DefaultFileName));
            bool isFileExists_Watcher = File.Exists(Path.Combine(currentDir, WatcherOptions.DefaultFileName));

            string baseCfgFilename = "config.json";

            var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(currentDir)
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
            services.ConfigureWritable<PathOptions>(cfg.GetSection("path"), isFileExists_Path ? PathOptions.DefaultFileName : baseCfgFilename);            
            services.ConfigureWritable<EntitiesOptions>(cfg.GetSection("entities"), isFileExists_Entities ? EntitiesOptions.DefaultFileName : baseCfgFilename);
            services.ConfigureWritable<UiOptions>(cfg.GetSection("ui"), isFileExists_Entities ? UiOptions.DefaultFileName : baseCfgFilename);
            services.ConfigureWritable<WatcherOptions>(cfg.GetSection("watcher"), isFileExists_Watcher ? WatcherOptions.DefaultFileName : baseCfgFilename);
            //services.ConfigureWritable<AdvancedOptions>(cfg.GetSection("advanced"), isFileExists_Advanced ? AdvancedOptions.DefaultFileName : baseCfgFilename);
            services.AddOptions<AdvancedOptions>().Configure(x => cfg.Bind("advanced", x));
            
            string savedataPath = Path.GetFullPath(Path.Combine(currentDir, DataOptions.DefaultFileName));
            if (File.Exists(Path.GetFullPath(Path.Combine(DataOptions.DefaultFileName))) != true)
            {
                using (var fs = File.Create(savedataPath))
                {
                    var obj = new 
                    {
                        data = new DataOptions()
                    };
                    string text = System.Text.Json.JsonSerializer.Serialize(obj);
                    fs.Write(System.Text.Encoding.UTF8.GetBytes(text));
                    fs.Close();
                }
            }

            services.AddSingleton<DapperAid.QueryBuilder>(new QueryBuilder.SQLite());
            services.AddSingleton<Services.IConnectionProvider, Services.SqliteConnectionProvider>();
            services.AddSingleton<Repositories.WatcherRepository>();


            services.ConfigureWritable<DataOptions>(cfg.GetSection("data"), DataOptions.DefaultFileName);

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
            LocalizeDictionary.Instance.DefaultProvider = rlp;

            //var dappererSettings = svcs.GetRequiredService<Dapperer.IDappererSettings>();
            //dappererSettings = new Extensions.DappererSettings() { ConnectionString = advOpts.Value.ConnectionString };
            var test = svcs.GetRequiredService<WatcherRepository>();

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
            
            var mainWindow = Svc.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }

}
