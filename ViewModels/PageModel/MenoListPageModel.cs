using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using TaskTip.Services;
using TaskTip.Views;

namespace TaskTip.ViewModels.PageModel
{
    internal class MenoListPageModel : ObservableObject
    {

        private string currentUid;
        public string CurrentUid
        {
            get => currentUid;
            set => SetProperty(ref currentUid, value);
        }

        private string menoEditText;
        /// <summary>
        /// 记事文本编辑
        /// </summary>
        public string MenoEditText
        {
            get => menoEditText;
            set
            {
                SetProperty(ref menoEditText, value);
                if (MenoButtonList?.FirstOrDefault(x => x.Uid == CurrentUid) != null)
                    WriteMenoFile(
                        GlobalVariable.MenoFilePath + "\\" +
                        MenoButtonList?.FirstOrDefault(x => x.Uid == CurrentUid).Content +
                        GlobalVariable.EndFileFormat);
            }
        }


        private DateTime selectDateTimeNow;
        /// <summary>
        /// 选中时间区间
        /// </summary>
        public DateTime SelectDateTimeNow
        {
            get
            {
                return selectDateTimeNow;
            }
            set
            {
                SetProperty(ref selectDateTimeNow, value);
                SelectDateTimeRangeFile(selectDateTimeNow);
            }
        }

        private DateTime dataStartTime;
        /// <summary>
        /// 根据文件名日期去获取可选范围
        /// </summary>
        public DateTime DataStartTime
        {
            get => dataStartTime;
            set => SetProperty(ref dataStartTime, value);
        }

        private ObservableCollection<Button> menoButtonList;
        /// <summary>
        /// 日期文件切换按钮集合
        /// </summary>
        public ObservableCollection<Button> MenoButtonList
        {
            get => menoButtonList;
            set => SetProperty(ref menoButtonList, value);
        }

        /// <summary>
        /// 生成一个新的Button控件
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private Button AddMenoButtonControl(string text)
        {
            var btnControl = new Button();
            btnControl.HorizontalAlignment = HorizontalAlignment.Stretch;
            btnControl.Uid = text;
            FocusManager.AddGotFocusHandler(btnControl, GotFocusButton);
            btnControl.Content = text;
            btnControl.Command = new RelayCommand(SelectedButtonChanged);
            return btnControl;
        }

        /// <summary>
        /// 写入Meno文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="text">保存的文本信息</param>
        private void WriteMenoFile(string path)
        {
            try
            {
                var msg = new
                {
                    CurrentUid,
                    MenoEditText
                };
                //var msg = $"Uid:{currentUid}\n{nameof(MenoEditText)}:{MenoEditText}";
                File.WriteAllText(path, JsonConvert.SerializeObject(msg, Formatting.Indented), encoding: Encoding.UTF8);
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void SelectedButtonChanged()
        {
            var btnControl = menoButtonList?.FirstOrDefault(x => x.Uid == CurrentUid);
            var path = GlobalVariable.MenoFilePath + "\\" + btnControl.Content + GlobalVariable.EndFileFormat;

            if (File.Exists(path))
            {
                var text = File.ReadAllText(path);
                try
                {

                    JObject readJson = JsonConvert.DeserializeObject<JObject>(text);

                    if (readJson == null)
                    {
                        MessageBox.Show($"错误：{btnControl.Content}中出未识别出有效JSON格式数据");
                        return;
                    }

                    foreach (var property in readJson.Properties())
                    {
                        GetType().GetProperty(property.Name)
                            .SetValue(this, property.Value.ToString());
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show($"错误：{btnControl.Content}中出现为解析字段或不完整\n{e.Message}");
                }
            }
            else
            {
                MenoEditText = string.Empty;
            }
        }


        private void GotFocusButton(object sender, EventArgs e)
        {
            var btnControl = (Button)sender;
            CurrentUid = btnControl.Uid;
        }

        /// <summary>
        /// 悬浮窗点开后自动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoBuildDateButton()
        {
            if (menoButtonList.Count == 0 ||
                menoButtonList[menoButtonList.Count - 1].Content.ToString() != DateTime.Now.ToString("yyyy-MM-dd"))
            {
                menoButtonList.Add(AddMenoButtonControl(DateTime.Now.ToString("yyyy-MM-dd")));
                CurrentUid = menoButtonList[0].Content.ToString() ?? "";
            }
        }


        /// <summary>
        /// 加载Meno文件夹对应路径的全部文件并生成控件
        /// </summary>
        /// <param name="fileType"></param>
        private void LoadReadMenoFile(string dirPath)
        {

            if (string.IsNullOrEmpty(dirPath))
                return;

            var filePaths = Directory.GetFiles(dirPath);

            var tempMenoList = new ObservableCollection<Button>();

            foreach (var filePath in filePaths)
            {
                var startIndex = filePath.LastIndexOf('\\') + 1;
                var endIndex = filePath.LastIndexOf('.');
                var text = filePath.Substring(startIndex, endIndex - startIndex);

                if (!filePath.EndsWith(GlobalVariable.EndFileFormat))
                    continue;

                if (startIndex == -1 || endIndex == -1)
                    continue;

                if (File.Exists(filePath))
                {
                    var msg = File.ReadAllText(filePath);
                    try
                    {

                        JObject readJson = JsonConvert.DeserializeObject<JObject>(msg);

                        if (readJson == null)
                        {
                            MessageBox.Show($"错误：{text}中出未识别出有效JSON格式数据");
                        }

                        if (readJson.ContainsKey("MenoEditText") &&
                            string.IsNullOrEmpty(readJson.GetValue(nameof(MenoEditText))?.ToString()))
                        {
                            File.Delete(filePath);
                            continue;
                        }

                        tempMenoList.Add(AddMenoButtonControl(text));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"错误：{text}中出现为解析字段或不完整\n{e.Message}");
                    }
                }

            }

            if (tempMenoList.Count == 0)
                tempMenoList.Add(AddMenoButtonControl(DateTime.Now.ToString("yyyy-MM-dd")));

            MenoButtonList = new ObservableCollection<Button>(tempMenoList.OrderBy(x => DateTime.Parse(x.Content.ToString())));
        }

        /// <summary>
        /// 根据选中时间获取显示在List里的内容
        /// </summary>
        /// <param name="starTime"></param>
        private void SelectDateTimeRangeFile(DateTime starTime)
        {
            LoadReadMenoFile(GlobalVariable.MenoFilePath!);

            MenoButtonList = new ObservableCollection<Button>(MenoButtonList.Where(x => DateTime.Compare(starTime, DateTime.Parse(x.Content.ToString())) <= 0));
        }

        private void InitRegister()
        {
            WeakReferenceMessenger.Default.Register<string, string>(this, Const.CONST_MENO_RELOAD, (obj, msg) => LoadReadMenoFile(msg));
            WeakReferenceMessenger.Default.Register<object, string>(this, Const.CONST_OPEN_APPLICATTION,
                (obj, msg) => { AutoBuildDateButton(); });
        }

        public MenoListPageModel()
        {

            menoButtonList = new ObservableCollection<Button>();
            DataStartTime = DateTime.MinValue;
            SelectDateTimeNow = DateTime.Now;
            CurrentUid = "";

            LoadReadMenoFile(GlobalVariable.MenoFilePath!);

            InitRegister();
        }
    }
}
