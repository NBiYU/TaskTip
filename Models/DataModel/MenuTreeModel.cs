using CommunityToolkit.Mvvm.ComponentModel;

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TaskTip.Models.DataModel
{
    public partial class TreeInfo: ObservableObject
    {
        public string GUID;
        [ObservableProperty]
        private string _name;
        [ObservableProperty]
        private bool _isDirectory;
        [ObservableProperty]
        private ObservableCollection<TreeInfo> _childMenus = new ObservableCollection<TreeInfo>();


        public override string ToString()
        {
            return Name;
        }
    }
}
