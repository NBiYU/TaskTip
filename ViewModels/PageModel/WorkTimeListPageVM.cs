using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTip.Common;
using TaskTip.Models.DataModel;
using TaskTip.Services;
using TaskTip.ViewModels.UserViewModel;
using TaskTip.Views.UserControls;

namespace TaskTip.ViewModels.PageModel
{
    public partial class WorkTimeListPageVM:ObservableRecipient
    {
        #region 属性
        private ObservableCollection<WorkInfoUC> _workTimeList=new();
        public ObservableCollection<WorkInfoUC> WorkTimeList
        {
            get => _workTimeList;
            set => SetProperty(ref _workTimeList, value);
        }
        #endregion

        #region 指令

        [RelayCommand]
        public void AddWorkInfoListItem()
        {
            WorkTimeList.Add(AddWorkInfoUC());
        }

        #endregion

        #region 功能函数


        private WorkInfoUC AddWorkInfoUC()
        {
            var control = new WorkInfoUC();
            return control;
        }

        private WorkInfoUC AddWorkInfoUC(WorkInfoModel item)
        {
            var control = new WorkInfoUC();
            control.DataContext = new WorkInfoUCM(item);
            return control;
        }


        #endregion

        #region 初始化





        public WorkTimeListPageVM() {
            WorkTimeList = new ObservableCollection<WorkInfoUC>(ReadWorkTimeListData());
        }

        private List<WorkInfoUC> ReadWorkTimeListData()
        {
            if (File.Exists(GlobalVariable.WorkTimeRecordPath))
            {
                var dataText = File.ReadAllText(GlobalVariable.WorkTimeRecordPath);
                var dataList = JsonConvert.DeserializeObject<List<WorkInfoModel>>(dataText);
                if (!(dataList is { Count: 0 }))
                {
                    var ucList = new List<WorkInfoUC>();
                    foreach (var item in dataList)
                    {
                        ucList.Add(AddWorkInfoUC(item));
                    }
                    return ucList;
                }
            }
            
            return new List<WorkInfoUC>();
        }

        #endregion

    }
}
