using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using TaskTip.Common.Converter;
using TaskTip.Common.Converter.Version;
using TaskTip.Common.Extends;
using TaskTip.Models.Entities;
using TaskTip.Services;

namespace TaskTip.ViewModels.WindowModel
{
    public partial class ResourceLoadingVM : ObservableObject
    {
        [ObservableProperty]
        public Func<IProgress<int>, IProgress<int>, CancellationToken, Task<bool>> _workFunc;
        [ObservableProperty]
        public string _commadText;
        private bool _startOrCancel;
        public bool StartOrCancel
        {
            get => _startOrCancel;
            set {
                SetProperty(ref _startOrCancel, value);
                CommadText = value ? "取消" : "关闭";
            } 
        }

        [RelayCommand]
        public void Cancel(object obj)
        {
            StartOrCancel = false;
            if(obj is Window w)
            {
                w.Close();
            }
        }
        [RelayCommand]
        public void LoadedStart()
        {
            StartOrCancel = true;
        }

        private async Task<bool> FlowToHtml(IProgress<int> total, IProgress<int> progress, CancellationToken token)
        {
            var path = GlobalVariable.RecordFilePath;
            var files = Directory.GetFiles(path);
            total.Report(files.Length);
            for(var i=0;i < files.Length; i++)
            {
                try
                {
                    token.ThrowIfCancellationRequested();
                    var content = await File.ReadAllTextAsync(files[i], token);
                    if (content.IsNullOrEmpty()) continue;
                    var obj = JsonConvert.DeserializeObject<RecordFileModel>(content);
                    if (obj?.Text.IsNullOrEmpty() == true) continue;
                    var converter = new FlowHTMLConverter();
                    obj.Text = converter.ConvertFlowDocumentToHtml(obj.Text);
                    await File.WriteAllTextAsync(files[i], JsonConvert.SerializeObject(obj), token);
                }
                catch (Exception ex) { }
            
                progress.Report(i);
            }

            GlobalVariable.SaveConfig(nameof(GlobalVariable.RecordVersion), GlobalVariable.RecordMaxVersion);
            StartOrCancel = false;
            return true;
        } 

        public ResourceLoadingVM()
        {
            WorkFunc = new Func<IProgress<int>, IProgress<int>, CancellationToken, Task<bool>>(FlowToHtml);
            StartOrCancel = false;
        }
    }
}
