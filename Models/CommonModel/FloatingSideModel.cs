using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTip.Models.CommonModel
{
    public class FloatingSideModel
    {
        public string Key { get; set; }
        public string IamgeUri { get; set; }
        public RelayCommand<object> Command { get; set; }
    }
}
