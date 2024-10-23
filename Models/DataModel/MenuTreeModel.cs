using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTip.Services;

namespace TaskTip.Models.DataModel
{
    public class MenuTreeModel
    {
        public List<TreeInfo> Directories;
        public List<TreeInfo> Files;
    }

    public class TreeInfo
    {
        public string GUID;
        public string Name;
        public bool IsDirectory;
        public MenuTreeModel Menu;


        public override string ToString()
        {
            var path = Path.Combine(GlobalVariable.RecordFilePath, $"{GUID}{GlobalVariable.EndFileFormat}");
            if (File.Exists(path))
            {
                var obj = JsonConvert.DeserializeObject<RecordFileModel>(File.ReadAllText(path));
                if (obj != null)
                {
                    return obj.Title;
                }
            }
            return "";
        }
    }
}
