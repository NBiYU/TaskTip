using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TaskTip.Models
{
    public class JsonEntityModel
    {

        public bool IsArrayType { get; set; } = true;
        public string VariableName { get; set; } = string.Empty;
        public string VariableValue { get; set; } = string.Empty;
        public int VariableType { get; set; } = 0;
        public List<JsonEntityModel> SubsetList { get; set; } = new();

        public JsonEntityModel() { }
        public JsonEntityModel(JsonEntityModel model)
        {
            IsArrayType = model.IsArrayType;
            VariableName = model.VariableName;
            VariableType = model.VariableType;
            SubsetList = model.SubsetList;
        }
        public JsonEntityModel(JToken jToken, List<JsonEntityModel> subsetList)
        {
            IsArrayType = jToken.HasValues;
            VariableName = $"\"{GetLastPath(jToken.Path)}\"";
            VariableType = (int)jToken.Type;
            VariableValue = $"[{subsetList.Count}]";
            SubsetList = subsetList;
        }
        public JsonEntityModel(JToken jToken)
        {
            IsArrayType = jToken.HasValues;
            VariableName = $"\"{GetLastPath(jToken.Path)}\"";
            VariableType = (int)jToken.Type;
        }

        private string GetLastPath(string path)
        {
            var paths = path.Split('.');
            if (paths is { Length: > 0 })
            {
                if (paths[paths.Length - 1].Contains("["))
                {
                    return $"[{paths[paths.Length - 1].Split("[")[1]}";
                }
                return paths[paths.Length - 1];
            }
            return path;
        }
    }
}
