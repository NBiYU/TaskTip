using CommunityToolkit.Mvvm.ComponentModel;
using TaskTip.ViewModels.WindowModel;

namespace TaskTip.ViewModels.UserViewModel
{
    class FileListItemVM:ObservableRecipient
    {
        public string Title { get; set; }
        public string Guid { get; set; }
        public CompareStatus ComplateStatus { get; set; }
    }
}
