using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;

namespace SmartXmlSql.DBTables
{
    internal class HumpUtil
    {
        public string Check(Object msg)
        {
            string json = JsonConvert.SerializeObject(msg, Formatting.Indented);
            Console.WriteLine(json);

            json = JsonConvert.SerializeObject(msg, Formatting.Indented, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            Console.WriteLine(json);

            //json = JsonConvert.SerializeObject(new MessageBody() { MyIDNum = 1, IsCheck = false }, Formatting.Indented, new JsonSerializerSettings()
            //{
            //    ContractResolver = new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() }
            //});
            return "";
        }
    }
}
