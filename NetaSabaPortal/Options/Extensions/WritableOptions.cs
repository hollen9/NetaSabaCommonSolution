//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace NetaSabaPortal.Options.Extensions;

//public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
//{
//    private readonly string sectionName;
//    private readonly IOptionsWriter writer;
//    private readonly IOptionsMonitor<T> options;

//    public WritableOptions(
//        string sectionName,
//        IOptionsWriter writer,
//        IOptionsMonitor<T> options)
//    {
//        this.sectionName = sectionName;
//        this.writer = writer;
//        this.options = options;
//    }

//    public T Value => this.options.CurrentValue;

//    public void Update(Action<T> applyChanges)
//    {
//        this.writer.UpdateOptions(opt =>
//        {
//            JsonElement section; //JObject section;
//            T sectionObject = opt.TryGetValue(this.sectionName, out section) ?
//                JsonSerializer.Deserialize<T>(section.ToString()) : // JsonConvert.DeserializeObject<T>(section.ToString()) :
//                this.options.CurrentValue ?? new T();

//            applyChanges(sectionObject);

//            string json = JsonSerializer.Serialize(sectionObject); //JsonConvert.SerializeObject(sectionObject);
//            opt[this.sectionName] = System.Text.Json.Nodes.JsonObject.Parse(json); //JObject.Parse(json);
//        });
//    }
//}
