using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SSOAuth.Helper;
using SSOAuth.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuth.Extensions
{
    public static class ConfigurationExtension
    {

        public static AppSettings LoadConfiguration(this IConfiguration config)
        {
            var file = System.IO.File.ReadAllText(Constants._configPath);
            SSOAuth.Settings.AppSettings appSetting = JsonConvert.DeserializeObject<SSOAuth.Settings.AppSettings>(file);
            return appSetting;
        }

        private static void getPropertiesAndValues(string json, ref IConfiguration ret)
        {

            JObject jObject = JObject.Parse(json);
            foreach (JProperty i in jObject.Properties())
            {
                var name = i.Name;
                var value = i.Value;

                if (!i.Value.HasValues)
                    ret[name] = value.ToString();

                Console.WriteLine();
                if (i.HasValues && i.Value.HasValues)
                    getPropertiesAndValues(i.First.ToString(), ref ret);
            }

        }
    }
}
