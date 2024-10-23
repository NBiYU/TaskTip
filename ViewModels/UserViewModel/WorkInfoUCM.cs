using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Printing;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TaskTip.Models.DataModel;
using TaskTip.Services;
using TaskTip.Views;
using TaskTip.Views.UserControls;

namespace TaskTip.ViewModels.UserViewModel
{
    public partial class WorkInfoUCM : ObservableRecipient
    {
        #region 属性
        private string _recordDate;
        public string RecordDate
        {
            get => _recordDate;
            set => SetProperty(ref _recordDate, value);
        }

        private string _workTime;
        public string WorkTime { get => _workTime; set => SetProperty(ref _workTime, value); }

        private string _startTime;
        public string StartTime { get => _startTime;set=>SetProperty(ref _startTime, value); }

        private string _endTime;
        public string EndTime { get => _endTime; set => SetProperty(ref _endTime, value); }

        private Visibility _showVisibility;
        public Visibility ShowVisibility
        {
            get=> _showVisibility;
            set=>SetProperty(ref _showVisibility, value);
        }

        private Uri _showUri;
        public Uri ShowUri
        {
            get => _showUri;
            set=>SetProperty(ref _showUri, value);
        }

        #endregion

        #region 指令

        [RelayCommand]
        public void Save()
        {
            RecordDate = DateTime.Parse(StartTime).ToString("yyyy-MM-dd");
            WorkTime = ComplateWorkTime(DateTime.Parse(StartTime), DateTime.Parse(EndTime));

            SaveRecord();

        }

        [RelayCommand]
        public void Show()
        {
           ShowVisibility = ShowVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
           ShowUri = ShowVisibility == Visibility.Visible ? ((BitmapImage)Application.Current.Resources["Visibility"]).UriSource : ((BitmapImage)Application.Current.Resources["Collapsed"]).UriSource;
        }

        [RelayCommand]
        public void ShowSatrtDateSelect()
        {
            SelectDateTime("开始时间");
        }
                
        [RelayCommand]
        public void ShowEndDateSelect()
        {
            SelectDateTime("结束时间");
        }

        #endregion


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

        private void SelectDateTime(string title)
        {
            var datetime = string.Empty;
            if (DateTimeGetView.IsClosed)
            {
                var taskTime = new DateTimeGetView();
                taskTime.TitleName.Text = title;
                taskTime.HideCancekPlan();
                taskTime.CalendarWithClock.Confirmed += () => {
                    datetime = taskTime.CalendarWithClock.SelectedDateTime.ToString();
                    switch (title) {
                        case "开始时间":
                            StartTime = DateTime.Parse(datetime).ToString("yyyy-MM-dd HH:mm"); break;
                        case "结束时间":
                            EndTime = DateTime.Parse(datetime).ToString("yyyy-MM-dd HH:mm"); break;
                        default:
                            MessageBox.Show($"时间选择：未知分支 {title}");
                            break;
                    }
                    taskTime.Close();
                };
                taskTime.NoneTime.Click += (o, e) =>
                {
                    datetime = DateTime.MinValue.ToString();
                };
                taskTime.Show();
            }

        }

        private void SaveRecord()
        {

            var existsRecordList = new List<WorkInfoModel>();
            if (File.Exists(GlobalVariable.WorkTimeRecordPath))
            {
                var existsRecordText = File.ReadAllText(GlobalVariable.WorkTimeRecordPath);
                if (!string.IsNullOrEmpty(existsRecordText))
                {
                    var temp = JsonConvert.DeserializeObject<List<WorkInfoModel>>(existsRecordText);
                    if (temp is { Count : >0 })
                    {
                        existsRecordList = temp;
                        existsRecordList.RemoveAll(x => x.RecordDate == RecordDate);
                    }
                }
            }

            existsRecordList.Add(new WorkInfoModel { RecordDate = RecordDate, WorkTime = WorkTime, StartTime = StartTime, EndTime = EndTime });

            File.WriteAllText(GlobalVariable.WorkTimeRecordPath,JsonConvert.SerializeObject(existsRecordList,Formatting.Indented));
        }

        #endregion

        #region 初始化

        private void InitProperty(WorkInfoModel workInfo)
        {
            RecordDate = workInfo.RecordDate;
            WorkTime = workInfo.WorkTime;
            StartTime = workInfo.StartTime;
            EndTime = workInfo.EndTime;


            ShowUri = ((BitmapImage)Application.Current.Resources["Collapsed"]).UriSource;
            ShowVisibility = Visibility.Collapsed;
        }

        public WorkInfoUCM()
        {
            InitProperty(new WorkInfoModel() { WorkTime = "0", StartTime = $"{DateTime.Now:yyyy-MM-dd} {GlobalVariable.WorkStartTime}" });
        }

        public WorkInfoUCM(WorkInfoModel workInfo)
        {
            InitProperty(workInfo);
        }

        #endregion
    }
}
