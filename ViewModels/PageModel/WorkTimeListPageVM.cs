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
using System.Windows.Media.Imaging;
using System.Windows;

using TaskTip.Common;
using TaskTip.Common.Converter.Map;
using TaskTip.Common.Helpers;
using TaskTip.Models.Entities;
using TaskTip.Pages;
using TaskTip.Services;
using TaskTip.ViewModels.UserViewModel;
using TaskTip.Views.UserControls;
using TaskTip.Views.Windows.PopWindow;
using TaskTip.Models.ViewDataModels;

namespace TaskTip.ViewModels.PageModel
{
    public partial class WorkTimeListPageVM:ObservableRecipient
    {
        #region 属性
        private ObservableCollection<WorkInfoModel> _workTimeList=new();
        public ObservableCollection<WorkInfoModel> WorkTimeList
        {
            get => _workTimeList;
            set => SetProperty(ref _workTimeList, value);
        }
        #endregion

        #region 指令

        [RelayCommand]
        public void AddWorkInfoListItem()
        {
            WorkTimeList.Add(new WorkInfoModel());
        }
        #endregion

        #region ListItem

        [RelayCommand]
        public void Save(WorkInfoModel model)
        {
            model.RecordDate = DateTime.Parse(model.StartTime).ToString("yyyy-MM-dd");
            if (WorkTimeList.Any(x=>x.RecordDate == model.RecordDate))
            {
                var result = MessageBox.Show($"{model.RecordDate}记录已存在，是否覆盖？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }
            model.WorkTime = ComplateWorkTime(DateTime.Parse(model.StartTime), DateTime.Parse(model.EndTime));
            SaveRecord(model);
        }

        [RelayCommand]
        public void Show(WorkInfoModel model)
        {
            model.ShowVisibility = model.ShowVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        [RelayCommand]
        public void ShowSatrtDateSelect(WorkInfoModel model)
        {
            SelectDateTime("开始时间", model);
        }

        [RelayCommand]
        public void ShowEndDateSelect(WorkInfoModel model)
        {
            SelectDateTime("结束时间", model);
        }
        [RelayCommand]
        public void DelRecord(WorkInfoModel model)
        {
            var db = new SQLiteDB();
            db.DeleteWorkRecordByRecordDate(model.RecordDate);
            WorkTimeList.Remove(model);
        }
        #region 功能函数

        private string ComplateWorkTime(DateTime startTime, DateTime endTime)
        {
            var startNum = GetTimeNum(startTime);
            var endNum = GetTimeNum(endTime);
            DateTime defaultStartTime;
            DateTime defaultEndTime;

            if (!(DateTime.TryParse(GlobalVariable.WorkStartTime, out defaultStartTime) && DateTime.TryParse(GlobalVariable.WorkFinishTime, out defaultEndTime)))
                return "请正确设置上班时间和下班时间";

            var defaultStartNum = GetTimeNum(defaultStartTime);
            var defaultFinishNum = GetTimeNum(defaultEndTime);


            if (startNum < defaultStartNum) startNum = defaultStartNum;
            if (endNum < defaultStartNum) endNum += 24F;

            var totalNum = endNum - startNum - GlobalVariable.SiestaTime;

            if (totalNum >= (defaultFinishNum - defaultStartNum - GlobalVariable.SiestaTime + GlobalVariable.AgainWorkGapTime)) totalNum -= GlobalVariable.AgainWorkGapTime;

            return totalNum > 0 ? totalNum.ToString() : "计算异常，请检查输入数值";
        }

        private double GetTimeNum(DateTime dateTimeString)
        {
            var strs = dateTimeString.ToString("HH:mm").Split(':');
            return double.Parse(strs[0]) + (int.Parse(strs[1]) >= 30 ? 0.5F : 0);
        }

        private void SelectDateTime(string title, WorkInfoModel model)
        {
            var oldRecordDate = string.IsNullOrEmpty(model.RecordDate) ? DateTime.Now : DateTime.Parse(model.RecordDate);
            var win = new ClockSelectorPop(oldRecordDate);
            win.Confirmed += (o, e) =>
            {
                var date = (DateTime)o;
                switch (title)
                {
                    case "开始时间":
                        model.StartTime = date.ToString("yyyy-MM-dd HH:mm"); break;
                    case "结束时间":
                        model.EndTime = date.ToString("yyyy-MM-dd HH:mm"); break;
                    default:
                        MessageBox.Show($"时间选择：未知分支 {title}");
                        break;
                }
            };
            win.ShowDialog();
        }

        private void SaveRecord(WorkInfoModel model)
        {

            #region SQLiteDB

            var db = new SQLiteDB();

            db.InsertWorkRecord(model);

            #endregion
        }

        #endregion
        #endregion

        #region 初始化
        public WorkTimeListPageVM() {
            WorkTimeList = [.. ReadWorkTimeListData()];
        }

        private IEnumerable<WorkInfoModel> ReadWorkTimeListData()
        {
            var db = new SQLiteDB();
            var models = db.GetAllWorkRecord().Select(x => x.Entity2WorkModel());
            return models;
        }

        #endregion

    }
}
