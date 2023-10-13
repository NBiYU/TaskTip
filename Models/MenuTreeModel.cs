using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTip.Models
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
    }
}
