using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaskTip.Models.DataModel;

namespace TaskTip.Models.Entities
{
    public class MenuTreeEntity
    {
        public string GUID;
        public string Name;
        public bool IsDirectory;
        public string ParentGuid;
    }
}
