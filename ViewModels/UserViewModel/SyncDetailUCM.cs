using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using TaskTip.Common;
using TaskTip.Common.Extends;
using TaskTip.Common.ExecuteServices;
using TaskTip.Services;
using TaskTip.Enums;
using TaskTip.Models.DataModel;

namespace TaskTip.ViewModels.UserViewModel
{
    internal class SyncDetailUCM : ObservableObject
    {
        #region 成员

        public TcpRequestData TcpData;

        #region 同步状态图片

        public static Uri NonSync => ((BitmapImage)Application.Current.Resources["Minimize"]).UriSource;
        public static Uri WaitSync => ((BitmapImage)Application.Current.Resources["WaitDownload"]).UriSource;
        public static Uri Synchronizing => ((BitmapImage)Application.Current.Resources["Downloading"]).UriSource;
        public static Uri SyncSuccess => ((BitmapImage)Application.Current.Resources["Success"]).UriSource;
        public static Uri SyncError => ((BitmapImage)Application.Current.Resources["Error"]).UriSource;

        #endregion


        private bool _isSync;

        public bool IsSync
        {
            get => _isSync;
            set
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetProperty(ref _isSync, value);
                    SyncStatus = value ? WaitSync : NonSync;
                });
            }
        }

        private string _category;

        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        private string _fileName;

        public string FileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }

        private string _operation;

        public string Operation
        {
            get => _operation;
            set => SetProperty(ref _operation, value);
        }

        private Uri _syncStatus;

        public Uri SyncStatus
        {
            get => _syncStatus;
            set
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetProperty(ref _syncStatus, value);
                });

            }
        }

        private bool _isEnable;

        public bool IsEnable
        {
            get => _isEnable;
            set
            {
                SetProperty(ref _isEnable, value);
                SyncStatus = !value ? SyncStatus : Synchronizing;
            }
        }

        #endregion


        #region 功能函数

        public async Task ExecuteSync(string ip)
        {
            try
            {
                SyncStatus = Synchronizing;

                dynamic reqCategory = TcpData.SyncCategory switch
                {
                    SyncFileCategory.TaskPlan => JsonConvert.DeserializeObject<TaskFileModel>(TcpData.FileData.ToString()),
                    SyncFileCategory.Record => JsonConvert.DeserializeObject<TreeInfo>(TcpData.FileData.ToString()),
                };

                if (TcpData.SyncCategory == SyncFileCategory.Record && !reqCategory.IsDirectory)
                {
                    var client = new TcpRequestService(ip);
                    await client.SendAsync(new TcpDataModel()
                    {
                        Key = GlobalVariable.LocalKey,
                        CommandType = SyncCommand.FileRequest,
                        IsKeep = false,
                        RequestData = new TcpRequestData()
                        {
                            GUID = reqCategory.GUID,
                            SyncCategory = SyncFileCategory.RecordFile
                        }
                    });
                }

                var sendToken = TcpData.SyncCategory switch
                {
                    SyncFileCategory.TaskPlan => Const.CONST_LISTITEM_CHANGED,
                    SyncFileCategory.Record => Const.CONST_NOTIFY_RECORD_ITEM,
                };

                WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { GUID = TcpData.GUID, Operation = TcpData.OperationType, Message = reqCategory }, sendToken);
                SyncStatus = SyncSuccess;
                IsEnable = false;

            }
            catch (Exception exception)
            {
                GlobalVariable.LogHelper.Error($"【数据同步】【同步到本地】【{TcpData.OperationType.GetDesc()}】【{TcpData.SyncCategory.GetDesc()}】出现异常：{exception}");
                SyncStatus = SyncError;
            }
        }

        #endregion

        #region 初始化

        private void Init()
        {
            IsEnable = true;
            SyncStatus = NonSync;

            if (TcpData.FileData == null)
                return;
            dynamic reqCategory = TcpData.SyncCategory switch
            {
                SyncFileCategory.TaskPlan => JsonConvert.DeserializeObject<TaskFileModel>(TcpData.FileData.ToString()),
                SyncFileCategory.Record => JsonConvert.DeserializeObject<TreeInfo>(TcpData.FileData.ToString()),
            };



            Category = TcpData.SyncCategory.GetDesc();
            FileName = TcpData.SyncCategory switch
            {
                SyncFileCategory.TaskPlan => reqCategory.EditTextTitle ?? "",
                SyncFileCategory.Record => reqCategory.Name,
            };
            Operation = TcpData.OperationType.GetDesc();

        }

        public SyncDetailUCM()
        {
            TcpData = new TcpRequestData();
            Init();
        }

        public SyncDetailUCM(TcpRequestData data)
        {
            TcpData = data;

            Init();
        }

        #endregion
    }
}
