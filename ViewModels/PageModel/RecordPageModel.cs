using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Newtonsoft.Json;

using NLog.Filters;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using TaskTip.Common;
using TaskTip.Common.Converter.Map;
using TaskTip.Common.Extends;
using TaskTip.Common.Helpers;
using TaskTip.Models.DataModel;
using TaskTip.Models.Entities;
using TaskTip.Models.Enums;
using TaskTip.Models.ViewDataModels;
using TaskTip.Services;
using TaskTip.Views.Windows.PopWindow;

using Image = System.Windows.Controls.Image;

namespace TaskTip.ViewModels
{
    public partial class RecordPageModel : ObservableRecipient
    {


        #region 属性
        [ObservableProperty]
        private ObservableCollection<TreeInfo> _treeInfos = new();
        [ObservableProperty]
        private TreeInfo _treeInfo;
        private bool _isFullScreen;
        private bool _isLoad;
        private Dictionary<string, string> RegexRule;
        private List<DataFormat> DataFormatList;
        private List<string> _clearPath = new();



        private string _textContent;
        public string TextContent
        {
            get => _textContent;
            set {
                if(value!=TextContent) EditTextChanged();
                SetProperty(ref _textContent, value);
             }
        }

        private Visibility _sidewaysVisibility;
        public Visibility SidewaysVisibility
        {
            get => _sidewaysVisibility;
            set => SetProperty(ref _sidewaysVisibility, value);
        }

        private bool _otherControlIsEnable;
        public bool OtherControlIsEnable
        {
            get => _otherControlIsEnable;
            set => SetProperty(ref _otherControlIsEnable, value);
        }

        private Visibility _textChangedVisibility;

        public Visibility TextChangedVisibility
        {
            get => _textChangedVisibility;
            set => SetProperty(ref _textChangedVisibility, value);
        }

        private string _currentTitle;
        public string CurrentTitle
        {
            get => _currentTitle;
            set => SetProperty(ref _currentTitle, value);
        }

        private Uri _fullScreenSource;

        public Uri FullScreenSource
        {
            get => _fullScreenSource;
            set => SetProperty(ref _fullScreenSource, value);
        }

        [ObservableProperty]
        public string _searchStr;
        [ObservableProperty]
        public string _searchResult;

        #endregion

        #region 指令处理函数

        [RelayCommand]
        public void ShowSideMenu()
        {
            SidewaysVisibility = SidewaysVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            OtherControlIsEnable = _isFullScreen || SidewaysVisibility != Visibility.Visible;
        }

        [RelayCommand]
        public void Save()
        {
            try
            {
                if (string.IsNullOrEmpty(TreeInfo.GUID))
                {
                    MessageBox.Show("保存失败，未选择文件");
                    return;
                }
                var targetTitle = string.IsNullOrEmpty(CurrentTitle) ? TreeInfo.Name : CurrentTitle;

                if (MessageBox.Show($"是否将“{targetTitle}”保存在原“{TreeInfo.Name}”，并重命名为“{targetTitle}”", "保存", MessageBoxButton.YesNo, MessageBoxImage.Question) !=
                    MessageBoxResult.Yes) return;

                TreeInfo.Name = targetTitle;

                var content = TextContent;
                var db = new SQLiteDB();
                db.ReplaceRecord(new RecordFileModel() { GUID = TreeInfo.GUID, Title = TreeInfo.Name, Text = content });

                LoadRecord(TreeInfo);
                TextChangedVisibility = Visibility.Collapsed;
            }
            catch (Exception e)
            {
                MessageBox.Show($"保存失败：{e}");
                return;
            }

        }
        [RelayCommand]
        public void EditTextChanged()
        {
            if (!_isLoad)
                TextChangedVisibility = Visibility.Visible;
        }

        [RelayCommand]
        public void FullScreenChanged()
        {
            _isFullScreen = !_isFullScreen;

            if (_isFullScreen)
            {
                FullScreenSource = ((BitmapImage)Application.Current.Resources["Fullscreen"]).UriSource; //new Uri("pack://application:,,,/Resources/fullscreen.png");
                WindowResource.EditFullScreenViewShow();
                WindowResource.TaskMenoViewClose();
            }
            else
            {
                FullScreenSource = ((BitmapImage)Application.Current.Resources["ArrowsFullscreen"]).UriSource; //new Uri("pack://application:,,,/Resources/arrows_fullscreen.png");
                WindowResource.EditFullScreenViewClose();
                WindowResource.TaskMenoViewShow();
            }
        }
        #endregion

        #region 功能函数

        /// <summary>
        /// 内容加载
        /// </summary>
        /// <param name="tree"></param>
        private void LoadRecord(TreeInfo tree)
        {
            try
            {
                _isLoad = true;
                if (TextChangedVisibility == Visibility.Visible && TreeInfo != tree)
                {
                    if (MessageBox.Show("当前文件未保存，是否放弃修改直接加载新文件", $"加载{tree.Name}", MessageBoxButton.YesNo,
                            MessageBoxImage.Question) != MessageBoxResult.Yes)
                    {
                        return;
                    }
                }

                TreeInfo = tree;

                SidewaysVisibility = _isFullScreen ? Visibility.Visible : Visibility.Collapsed;
                OtherControlIsEnable = true;

                var content = string.Empty;
                var db = new SQLiteDB();
                var obj = db.GetRecordByGuid(tree.GUID)?.Enity2RecordFileModel();
                if(obj != null)
                {
                    content = obj.Text;
                }
                TextContent = content;
                CurrentTitle = TreeInfo.Name;

                _isLoad = false;

            }
            catch (Exception e)
            {
                MessageBox.Show($"加载文件内容异常：{e.Message}");
            }

        }
        #endregion
        #region TreeView

        [RelayCommand]
        public void Rename(TreeInfo info)
        {
            if(info == null) return;
            var result = GetNameInteraction($"重新命名：“{info.Name}”", info.Name);
            if (result.IsNullOrEmpty()) return;
            info.Name = result;
            var db = new SQLiteDB();
            db.UpdateMenu(info);
        }
        [RelayCommand]
        public void AddFolder(TreeInfo info)
        {
            if (info == null) return;
            var result = GetNameInteraction($"新文件夹命名：", string.Empty);
            if (result.IsNullOrEmpty()) return;
            var newInfo = new TreeInfo() { 
                GUID = Guid.NewGuid().ToString(),
                IsDirectory = true,
                Name = result
            };
            var db = new SQLiteDB();
            db.InsertMenuByParentGuid(info.GUID, newInfo);
            info.ChildMenus.Add(newInfo);
        }
        [RelayCommand]
        public void AddFile(TreeInfo info)
        {
            if (info == null) return;
            var result = GetNameInteraction($"新文件命名：", string.Empty);
            if (result.IsNullOrEmpty()) return;
            var newInfo = new TreeInfo()
            {
                GUID = Guid.NewGuid().ToString(),
                IsDirectory = false,
                Name = result
            };
            var db = new SQLiteDB();
            db.InsertMenuByParentGuid(info.GUID, newInfo);
            info.ChildMenus.Add(newInfo);
        }
        [RelayCommand]
        public void DeleteItem(TreeInfo info)
        {
            if (info == null) return;
            if (MessageBox.Show($"是否删除“{info.Name}”", "删除", MessageBoxButton.YesNo, MessageBoxImage.Question) !=
                MessageBoxResult.Yes) return;
            var db = new SQLiteDB();
            db.DeleteMenuByGuid(info.GUID);
        }
        [RelayCommand]
        public void SwitchContent(TreeInfo info)
        {
            if (info.IsDirectory) return;
            LoadRecord(info);
        }
        private string GetNameInteraction(string title,object oldData)
        {
            var winModel = new PopInteractiveModel
            {
                Title = title,
                InteractiveItemModels = new List<PopInteractiveItemModel>(){
                    new()
                    {
                        Data = oldData,
                        ControlType = ControlTypeEnum.Input
                    }
                }
            };
            var win = new PopInteractiveView(winModel);
            var result = win.ShowDialog();
            if (result == true)
            {
                return winModel.InteractiveItemModels.First().Data.ToString();
            }
            return string.Empty;
        }
        #endregion

        #region 初始化


        private void InitProperty()
        {

            #region 规则定义

            RegexRule = new Dictionary<string, string>()
            {
                { "Link", @"[a-zA-z]+://[^\s]*" },
                {"Email",@"[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?"},
                {"DateTime",@"([0-9]{3}[1-9]|[0-9]{2}[1-9][0-9]{1}|[0-9]{1}[1-9][0-9]{2}|[1-9][0-9]{3})-(((0[13578]|1[02])-(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)-(0[1-9]|[12][0-9]|30))|(02-(0[1-9]|[1][0-9]|2[0-8])))"},
                { "PNG" , @"^..+\.PNG$"},{ "JPG",@"^..+\.JPG$"}
            };

            #endregion

            var totalNum = 23;
            DataFormatList = new List<DataFormat>();
            for (int i = 0; i < totalNum; i++)
            {
                DataFormatList.Add(DataFormats.GetDataFormat(i));
            }

            TextChangedVisibility = Visibility.Collapsed;
            _isFullScreen = false;
            FullScreenSource = _isFullScreen
                    ? ((BitmapImage)Application.Current.Resources["Fullscreen"]).UriSource //"pack://application:,,,/Resources/fullscreen.png"
                    : ((BitmapImage)Application.Current.Resources["ArrowsFullscreen"]).UriSource;//"pack://application:,,,/Resources/arrows_fullscreen.png"
            ;
        }
        
        private void InitRegister()
        {
        }
        private void InitRecordMenu()
        {
            var db = new SQLiteDB();
            TreeInfos.Add(db.GetAllMenus()?.Entity2TreeInfo() ?? GetDefaultTree());
        }

        private TreeInfo GetDefaultTree()
        {
            return new TreeInfo()
            {
                GUID = "RootDocument",
                Name = "根目录",
                IsDirectory = true
            };
        }

        public RecordPageModel()
        {
            InitRegister();
            InitProperty();
            InitRecordMenu();
        }

        #endregion
    }
}
