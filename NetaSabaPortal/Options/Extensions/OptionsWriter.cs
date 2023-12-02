//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.FileProviders;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Text.Json.Nodes;
//using System.Threading.Tasks;

//namespace NetaSabaPortal.Options.Extensions;

//public class OptionsWriter : IOptionsWriter
//{
//    private readonly IHostingEnvironment environment;
//    private readonly IConfigurationRoot configuration;
//    private readonly string file;

//    public OptionsWriter(
//        IHostingEnvironment environment,
//        IConfigurationRoot configuration,
//        string file)
//    {
//        this.environment = environment;
//        this.configuration = configuration;
//        this.file = file;
//    }

//    //public void UpdateOptions(Action<JObject> callback, bool reload = true)
//    public void UpdateOptions(Action<JsonObject> callback, bool reload = true)
//    {
//        IFileProvider fileProvider = this.environment.ContentRootFileProvider;
//        IFileInfo fi = fileProvider.GetFileInfo(this.file);
//        //JObject config = fileProvider.ReadJsonFileAsObject(fi);
//        JsonObject config = fileProvider.ReadJsonFileAsObject(fi);
//        callback(config);
//        using (var stream = File.OpenWrite(fi.PhysicalPath))
//        {
//            stream.SetLength(0);
//            config.WriteTo(,stream);
//        }

//        this.configuration.Reload();
//    }
//}

//public interface IOptionsWriter
//{
//    public void UpdateOptions(Action<JObject> callback, bool reload = true);
//}
