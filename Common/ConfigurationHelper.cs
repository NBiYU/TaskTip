using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TaskTip.Common
{
    public class ConfigurationHelper : IConfigurationRoot
    {
        private IConfiguration configuration;
        private string appsettingPath;

        public ConfigurationHelper()
        {
            ConfigurationBuild(Directory.GetCurrentDirectory(), "appsettings.json");
        }

        public ConfigurationHelper(string path, string jsonName)
        {
            ConfigurationBuild(path, jsonName);
        }


        private void ConfigurationBuild(string path, string jsonName)
        {
            jsonName = jsonName.ToLower().EndsWith(".json") ? jsonName : jsonName + ".json";
            appsettingPath = Path.Combine(path, jsonName);
            configuration = new ConfigurationBuilder()
                .AddJsonFile(jsonName, optional: false, reloadOnChange: true)
                .Build();
        }

        private void Save(string key, object value)
        {
            var keys = key.Split(":").ToList();

            var obj = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(appsettingPath));

            var numList = keys.Where(x => Regex.IsMatch(x, "^[0-9]+(\\.[0-9]+){0,1}$")).ToList();
            foreach (var num in numList)
            {
                var idx = keys.IndexOf(num) - 1;
                keys.Remove(num);
                keys[idx] += $"[{num}]";
            }
            var token = "$." + string.Join(".", keys);
            var property = obj.SelectToken(token);


            property.Replace(SetValue(value));

            File.WriteAllText(appsettingPath, JsonConvert.SerializeObject(obj, Formatting.Indented));

        }

        public T TryGetValue<T>(string key)
        {
            var keys = key.Split(":");
            //if (keys.FirstOrDefault(x => Regex.IsMatch(x, "^[0-9]+(\\.[0-9]+){0,1}$")) != null)
            //{
                var jObj = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(appsettingPath));

                if (jObj == null) throw new Exception($"Not Found JSON File in {appsettingPath}");

                return GetCustomClass<T>(jObj, keys);
            //}
            //return (T)(object)configuration[key];
        }


        private T? GetCustomClass<T>(JObject obj,params string[] keys)
        {
            JToken currentToken = obj;

            foreach (var tokenName in keys)
            {
                if (currentToken is JObject)
                {
                    currentToken = ((JObject)currentToken)[tokenName];
                }
                else if (currentToken is JArray)
                {
                    if (int.TryParse(tokenName, out var index))
                    {
                        currentToken = ((JArray)currentToken)[index];
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid array index.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Invalid JSON structure.");
                }
            }

            return  currentToken.ToObject<T>();
        }
        

        #region 解析为字典

        //private dynamic SetValue(object value)
        //{
        //    var type = value.GetType();

        //    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        //    {
        //        var result = new List<Dictionary<string, object>>(); //数组
        //        var properties = value.GetType().GetProperties();
        //        var length = (int)properties[1].GetValue(value);
        //        for (int i = 0; i < length; i++)
        //        {
        //            var array = new Dictionary<string, object>();
        //            var indexers = properties[2].GetValue(value, new object?[] { i }); //获取数组中元素
        //            var indexType = indexers.GetType();
        //            var indexProperties = indexType.GetProperties();
        //            foreach (var prop in indexProperties)
        //            {
        //                array.TryAdd(prop.Name, SetValue(prop.GetValue(indexers)));
        //            }
        //            result.Add(array);
        //        }

        //        return result;
        //    }

        //    return value;
        //}

        #endregion

        #region 解析为JSON原结构

        private dynamic SetValue(object value)
        {
            var type = value.GetType();

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var result = new JArray(); //数组
                var properties = value.GetType().GetProperties();
                var length = (int)properties[1].GetValue(value);
                for (int i = 0; i < length; i++)
                {
                    var indexers = properties[2].GetValue(value, new object?[] { i }); //获取数组中元素
                    result.Add(SetValue(indexers));
                }

                return result;
            }  //解析数组

            if (type is { IsClass: true, IsValueType: false } && type.Name != nameof(String))
            {
                var keyAndValue = new JObject();
                var indexProperties = type.GetProperties();
                foreach (var prop in indexProperties)
                {
                    keyAndValue.Add(prop.Name, SetValue(prop.GetValue(value)));
                }
                return keyAndValue;
            }  //解析非基元类型对象

            return value; //基元对象直接返回
        }

        #endregion


        public IConfigurationSection GetSection(string key)
        {
            return configuration.GetSection(key);
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            return configuration.GetChildren();
        }

        public IChangeToken GetReloadToken()
        {
            return configuration.GetReloadToken();
        }


        public object? this[string key,Type type]
        {
            set
            {
                configuration[key] = value.ToString();
                Save(key, value);
            }
        }

        public string? this[string key]
        {
            get => configuration[key];
            set => configuration[key] = value;
        }

        public void Reload()
        {

        }

        public IEnumerable<IConfigurationProvider> Providers { get; }
    }
}
