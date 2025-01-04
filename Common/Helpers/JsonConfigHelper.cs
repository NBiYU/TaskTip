using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTip.Common
{
    public static class JsonConfigHelper
    {
        public static bool SaveConfig(object obj, string name) 
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(),$"{name}.json");
            if(obj == null) return false;
            try
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(obj, Formatting.Indented));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public  static T ReadConfig<T>(string name)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"{name}.json");
            if (File.Exists(path))
            {
                var content = File.ReadAllText(path);
                var obj = JsonConvert.DeserializeObject<T>(content);
                
                return obj;
            }
            return default;
        }
    }
}
