using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TaskTip.Common.Converter.Version;
using TaskTip.Common.Extends;
using TaskTip.Models.ViewDataModels;

using TaskTip.Services;

namespace TaskTip.Views.Windows
{
    /// <summary>
    /// ResourceUpdateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ResourceUpdateWindow : Window
    {
        private CancellationTokenSource _cts;
        private int _totalLength;
        public ResourceUpdateWindow()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _cts.Cancel();
        }

        private void Loaded_Start(object sender, RoutedEventArgs e)
        {
            _cts = new();
            var progress = new Progress<int>(value =>
            {
                ProgressValue.Value = ((double)value + 1) / (double)_totalLength * 100;
            });
            var total = new Progress<int>(value =>
            {
                _totalLength = value;
            });
            var toSQLite = new File2SQLiteDBConverter(total,progress, _cts.Token);
            App.Current.Dispatcher.BeginInvoke( async() => {
                StepTip.Text = $"资源更新中";
                await FlowToHtml(progress,total,_cts.Token);

                StepTip.Text = "资源转储中";
                toSQLite.MenusConvert(GlobalVariable.MenuTreeConfigPath);
                toSQLite.TaskFileConvert(GlobalVariable.TaskFilePath);
                toSQLite.WorkInfoConvert(GlobalVariable.WorkTimeRecordPath);
                toSQLite.RecordContentConvert(GlobalVariable.RecordFilePath);
                StepTip.Text = "资源转储完成";
                GlobalVariable.SaveConfig(nameof(GlobalVariable.Version), GlobalVariable.NewVersion);
                Close();
            });
        }
        private async Task<bool> FlowToHtml(IProgress<int> total, IProgress<int> progress, CancellationToken token)
        {
            var path = GlobalVariable.RecordFilePath;
            if (!Directory.Exists(path)) return true;
            var files = Directory.GetFiles(path);
            if (files.Length == 0) return true;
            total.Report(files.Length);
            for (var i = 0; i < files.Length; i++)
            {
                try
                {
                    token.ThrowIfCancellationRequested();
                    var content = await File.ReadAllTextAsync(files[i], token);
                    if (content.IsNullOrEmpty()) continue;
                    var obj = JsonConvert.DeserializeObject<RecordFileModel>(content);
                    if (obj?.Text.IsNullOrEmpty() == true) continue;
                    var converter = new FlowHTMLConverter(total,progress,token);
                    obj.Text = converter.ConvertFlowDocumentToHtml(obj.Text);
                    await File.WriteAllTextAsync(files[i], JsonConvert.SerializeObject(obj), token);
                }
                catch (Exception ex) { }

                progress.Report(i);
            }

            GlobalVariable.SaveConfig(nameof(GlobalVariable.RecordVersion), GlobalVariable.RecordMaxVersion);

            return true;
        }
    }
}
