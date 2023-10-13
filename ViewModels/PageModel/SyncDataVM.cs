﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using TaskTip.Common;
using TaskTip.Common.Extends;
using TaskTip.Common.ExecuteServices;
using TaskTip.Models;
using TaskTip.Services;
using TaskTip.ViewModels.UserViewModel;
using TaskTip.Views.UserControls;
using Exception = System.Exception;

namespace TaskTip.ViewModels.PageModel
{
    public partial class SyncDataVM : ObservableObject
    {

        #region 属性

        private bool _isCancel = true;
        private bool[] _actionCancel = new bool[10];

        private string _targetIP;

        public string TargetIP
        {
            get => _targetIP;
            set => SetProperty(ref _targetIP, value);
        }

        private bool _synchronizing;

        public bool Synchronizing
        {
            get => _synchronizing;
            set
            {
                SetProperty(ref _synchronizing, !value);
                SyncDescription = value ? "同步中" : "同步完成";
            }
        }

        private bool _isSearching;

        public bool IsSearching
        {
            get => _isSearching;
            set => SetProperty(ref _isSearching, !value);
        }

        private Visibility _networkVisibility;

        public Visibility NetworkVisibility
        {
            get => _networkVisibility;
            set => SetProperty(ref _networkVisibility, value);
        }


        private List<string> TempNetworkCollection = new();
        private ObservableCollection<string> _networkCollection;

        public ObservableCollection<string> NetworkCollection
        {
            get => _networkCollection;
            set => SetProperty(ref _networkCollection, value);
        }

        private List<TcpRequestData> TempSyncDetailCollection = new();
        private ObservableCollection<SyncDetailUC> _syncFileCollection;
        public ObservableCollection<SyncDetailUC> SyncFileCollection
        {
            get => _syncFileCollection;
            set => SetProperty(ref _syncFileCollection, value);
        }

        private string _syncDescription;

        public string SyncDescription
        {
            get => _syncDescription;
            set => SetProperty(ref _syncDescription, value);
        }

        private string _networkText;

        public string NetworkText
        {
            get => _networkText;
            set => SetProperty(ref _networkText, value);
        }

        private string _executeStatusDescription;

        public string ExecuteStatusDescription
        {
            get => _executeStatusDescription;
            set => SetProperty(ref _executeStatusDescription, value);
        }

        #region 搜索状态

        private Uri WaitSearch => ((BitmapImage)Application.Current.Resources["Search"]).UriSource;
        private Uri Searching => ((BitmapImage)Application.Current.Resources["Error"]).UriSource;

        #endregion

        private Uri _searchStatus;

        public Uri SearchStatus
        {
            get => _searchStatus;
            set => SetProperty(ref _searchStatus, value);
        }

        private bool _allSelect;

        public bool AllSelect
        {
            get => _allSelect;
            set
            {
                SetProperty(ref _allSelect, value);
                SyncAllSelected(value);
            }
        }

        private bool _isCompleteSearch;
        public bool IsCompleteSearch
        {
            get => _isCompleteSearch;
            set
            {
                if (value)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _isCancel = true;
                        IsSearching = true;
                        NetworkVisibility = Visibility.Collapsed;
                        SearchStatus = WaitSearch;
                        NetworkText = $"已搜索到 {NetworkCollection.Count} 个IP";
                        NetworkCollection = new ObservableCollection<string>(NetworkCollection.OrderBy(x => x));
                        NetworkVisibility = Visibility.Collapsed;
                        TempNetworkCollection.Clear();
                    });
                }
            }
        }

        private bool _isCompleteReceive;
        public bool IsCompleteReceive
        {
            get => _isCompleteReceive;
            set
            {
                if (value)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        SyncFileCollection.Clear();
                        TempSyncDetailCollection.ForEach(x => SyncFileCollection.Add(AddSyncDetailUc(x)));
                        NetworkVisibility = Visibility.Collapsed;
                        SyncDescription = $"当前可同步 {SyncFileCollection.Count} 个内容";
                        NetworkText = "获取完成";
                        TempSyncDetailCollection.Clear();
                    });
                }
            }
        }

        private bool _isCompleteSync;

        public bool IsCompleteSync
        {
            get => _isCompleteSync;
            set
            {
                if (value)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Synchronizing = false;
                        MessageBox.Show("已同步完成");
                    });
                }
            }
        }

        private string _key;
        public string Key
        {
            get => _key;
            set
            {
                SetProperty(ref _key, value);
                GlobalVariable.SaveConfig("LocalKey", value);
            }
        }

        #endregion

        #region 指令
        [RelayCommand]
        public Task SearchNetwork(object sender)
        {
            if (!_isCancel)
            {
                IsCompleteSearch = true;
                return Task.CompletedTask;
            }

            if (sender is string segment)
            {
                NetworkVisibility = Visibility.Visible;
                NetworkText = "搜索中";
                SearchStatus = Searching;
                NetworkCollection.Clear();
                _isCancel = false;
                Task.Run(() =>
                {
                    Parallel.For(1, 256,
                    i =>
                    {
                        if (_isCancel) return;

                        var ping = new Ping();
                        var ip = $"{segment}.{i}";
                        PingReply reply = ping.Send(ip);

                        if (reply.Status == IPStatus.Success)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                NetworkCollection.Add(ip);
                            });
                        }

                    });
                    IsCompleteSearch = true;
                });
            }
            return Task.CompletedTask;
        }

        [RelayCommand]
        public async Task GetSyncFileList(object sender)
        {
            try
            {
                sender = !string.IsNullOrEmpty(TargetIP) ? TargetIP : sender;

                if (sender is string ip)
                {
                    if (!CheckIP(ip))
                    {
                        NetworkText = $"{ip} 不合法";
                        return;
                    }
                    TargetIP = ip;

                    var client = new TcpRequestService(ip);
                    await client.SendAsync(new TcpDataModel()
                    {
                        Key = GlobalVariable.LocalKey,
                        IsKeep = false,
                        CommandType = SyncCommand.SyncRequest,
                        RequestData = new TcpRequestData()
                    });
                    _isCancel = true;
                    NetworkText = $"已向 {sender} 发送请求";
                    NetworkVisibility = Visibility.Visible;
                    AllSelect = false;
                    SyncDescription = $"等待 {sender} 推送完成";

                    await ActionTimer(async () =>
                    {
                        var clientAsync = new TcpRequestService(ip);
                        await clientAsync.SendAsync(new TcpDataModel()
                        {
                            Key = GlobalVariable.LocalKey,
                            IsKeep = false,
                            CommandType = SyncCommand.SyncRequest,
                            RequestData = new TcpRequestData()
                        });

                    }, 0, 3000);

                    await ActionTimer(() =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            SyncDescription = "未收到任何推送";
                            NetworkVisibility = Visibility.Collapsed;
                        });
                    }, 1);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"【请求同步】请检查同步网络地址是否正确  或  主机是否开启程序，异常：{e}");
            }

        }

        private int Count = 0;

        [RelayCommand]
        public async Task ExecuteSync()
        {
            try
            {
                var selected = SyncFileCollection
                    .Where(x => (x.DataContext as SyncDetailUCM)?.IsSync == true && (x.DataContext as SyncDetailUCM)?.IsEnable == true)
                    .ToList();
                Synchronizing = true;
                var syncVMs = new List<SyncDetailUCM>();
                selected.ForEach(x => syncVMs.Add(x.DataContext as SyncDetailUCM));
                syncVMs.RemoveAll(x => x == null);

                if (syncVMs.Count == 0)
                {
                    MessageBox.Show("当前无可同步项");
                    return;
                }


                syncVMs.OrderBy(x => (int)x.TcpData.OperationType);
                foreach (var vm in syncVMs)
                {
                    await vm?.ExecuteSync(TargetIP);
                }

                IsCompleteSync = true;

            }
            catch (Exception e)
            {
                GlobalVariable.LogHelper.Error($"【同步】执行同步时遇到了异常：{e}");
            }

            return;
        }

        [RelayCommand]
        public async Task NetworkSelected(object sender)
        {
            if (sender is string ip)
            {
                NetworkText = $"当前选中   {sender}";
            }
        }

        [RelayCommand]
        public void NetworkSegmentLoad()
        {
            InitProperty();
            var networks = NetworkInterface.GetAllNetworkInterfaces().ToList();
            networks.RemoveAll(x => x.OperationalStatus != OperationalStatus.Up);

            foreach (var network in networks)
            {
                var ips = network.GetIPProperties().UnicastAddresses;
                foreach (var ip in ips)
                {
                    if (IPAddress.Parse(ip.Address.ToString()).AddressFamily == AddressFamily.InterNetwork)
                    {
                        NetworkCollection.Add(string.Join(".", ip.Address.ToString().Split(".")[..3]));
                    }
                }
            }

        }

        [RelayCommand]
        public void CopyKey(object sender)
        {
            if (sender is string key)
            {
                Clipboard.SetText(key);
            }
        }

        #endregion

        #region  功能函数

        private bool CheckIP(string ip)
        {
            if (!Regex.IsMatch(ip, @"^([0-9]{1,3}\.){3}[0-9]{1,3}$")) return false;

            var nums = ip.Split(".");
            foreach (var num in nums)
            {
                if (int.Parse(num) > 256) return false;
            }

            return true;
        }

        public void SyncAllSelected(object sender)
        {
            if (sender is bool isAllSelected)
            {
                foreach (var syncFile in SyncFileCollection)
                {
                    if (syncFile.DataContext is not SyncDetailUCM model) continue;
                    if (model.IsEnable == false) continue;

                    model.IsSync = isAllSelected;

                }
            }
        }

        private Task ActionTimer(Action act, int idx, int second = 10, int sleep = 1000)
        {
            _actionCancel[idx] = false;
            Task.Run(() => {
                var count = 0;
                while (count < second)
                {
                    if (_actionCancel[idx]) return;
                    count++;
                    System.Threading.Thread.Sleep(sleep);
                }
                act();
            });
            return Task.CompletedTask;
        }

        private void SyncDataReceiver(CorrespondenceModel corr)
        {
            try
            {
                if (corr.Message is TcpRequestData item)
                {
                    switch (item.SyncCategory)
                    {
                        case SyncFileCategory.Record:
                        case SyncFileCategory.TaskPlan:
                            if (corr.Operation == OperationRequestType.Completed)
                            {
                                IsCompleteReceive = true;
                                return;
                            }

                            SyncData(item);
                            break;
                        case SyncFileCategory.RecordFile:
                            SyncFile(item);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                GlobalVariable.LogHelper.Error($"【数据同步】【推送】【{corr.Operation.GetDesc()}】【{((TcpRequestData)corr.Message).SyncCategory.GetDesc()}】出现异常：{e}");
            }

        }

        private void SyncData(TcpRequestData data)
        {
            _actionCancel[0] = true;
            _actionCancel[1] = true;
            TempSyncDetailCollection.Add(data);
        }

        private void SyncFile(TcpRequestData data)
        {

            RecordFileModel fileModel = JsonConvert.DeserializeObject<RecordFileModel>(JsonConvert.SerializeObject(data.FileData));
            var path = Path.Combine(GlobalVariable.RecordFilePath, data.GUID) + GlobalVariable.EndFileFormat;
            var xamlPath = Path.Combine(GlobalVariable.RecordFilePath ,"Xaml" ,$"{data.GUID}{GlobalVariable.EndFileFormat}");

            File.WriteAllText(path, JsonConvert.SerializeObject(new RecordFileModel() { Title = fileModel.Title }));
            using (FileStream fileStream = new FileStream(xamlPath, FileMode.Create))
            {
                XamlWriter.Save(XamlReader.Parse(fileModel.Text), fileStream);
            }
        }


        private SyncDetailUC AddSyncDetailUc(TcpRequestData data)
        {
            var control = new SyncDetailUC
            {
                DataContext = new SyncDetailUCM(data)
            };
            return control;
        }
        #endregion

        #region  初始化

        private void InitRegister()
        {
            WeakReferenceMessenger.Default.Register<CorrespondenceModel, string>(this, Const.CONST_SYNC_RECEIVE,
                (obj, msg) => { SyncDataReceiver(msg); });
        }

        private void InitProperty()
        {
            NetworkVisibility = Visibility.Collapsed;
            _isCompleteSearch = false;
            _isCompleteReceive = false;
            _isCompleteSync = false;
            IsSearching = false;
            Synchronizing = false;
            SyncDescription = "未选择同步网络";
            ExecuteStatusDescription = "开始同步";
            SyncFileCollection = new();
            NetworkCollection = new();
            SearchStatus = WaitSearch;
            Key = GlobalVariable.LocalKey;
        }

        public SyncDataVM()
        {
            InitProperty();
            InitRegister();
        }
        #endregion

    }
}
