//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NetaSabaPortal.Options.Extensions;

//public static class ServicesCollectionExtensions
//{
//    public static void ConfigureWritable<T>(
//        this IServiceCollection services,
//        IConfigurationRoot configuration,
//        string sectionName,
//        string file) where T : class, new()
//    {
//        services.Configure<T>(x=> configuration.GetSection(sectionName));

//        services.AddTransient<IWritableOptions<T>>(provider =>
//        {
//            var environment = provider.GetService<IHostingEnvironment>();
//            var options = provider.GetService<IOptionsMonitor<T>>();
//            IOptionsWriter writer = new OptionsWriter(environment, configuration, file);
//            return new WritableOptions<T>(sectionName, writer, options);
//        });
//    }
//}
