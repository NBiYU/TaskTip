using System;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Data;
using Newtonsoft.Json;
using TaskTip.Models;
using TaskTip.Services;

namespace TaskTip.ViewModels
{
    internal partial class RecordPageModel : ObservableObject
    {

        #region 属性

        private TreeInfo TreeInfo;
        private bool _isLoad;

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

        private string _currentText;
        public string CurrentText
        {
            get => _currentText;
            set => SetProperty(ref _currentText, value);
        }

        #endregion


        [RelayCommand]
        public void ShowSideMenu()
        {
            SidewaysVisibility = SidewaysVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            OtherControlIsEnable = SidewaysVisibility != Visibility.Visible;
        }

        [RelayCommand]
        public void Save()
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

            var path = Path.Combine(GlobalVariable.RecordFilePath, TreeInfo.GUID) + GlobalVariable.EndFileFormat;
            var content = JsonConvert.SerializeObject(new RecordFileModel() { Title = targetTitle, Text = CurrentText });
            File.WriteAllText(path, content);

            WeakReferenceMessenger.Default.Send(TreeInfo, Const.CONST_RECORD_SAVE_TITLE);

            TextChangedVisibility = Visibility.Collapsed;
        }

        [RelayCommand]
        public void EditTextChanged()
        {
            if (!_isLoad)
                TextChangedVisibility = Visibility.Visible;
        }


        #region 功能函数

        private void LoadRecord(TreeInfo tree)
        {
            _isLoad = true;
            if (TextChangedVisibility == Visibility.Visible)
            {
                if (MessageBox.Show("当前文件未保存，是否放弃修改直接加载新文件", $"加载{tree.Name}", MessageBoxButton.YesNo,
                        MessageBoxImage.Question) != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            TreeInfo = tree;

            SidewaysVisibility = Visibility.Collapsed;
            OtherControlIsEnable = true;

            var path = Path.Combine(GlobalVariable.RecordFilePath, tree.GUID) + GlobalVariable.EndFileFormat;
            if (!Directory.Exists(GlobalVariable.RecordFilePath))
                Directory.CreateDirectory(GlobalVariable.RecordFilePath);
            if (!File.Exists(path))
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(new RecordFileModel()));
            }

            var fileText = File.ReadAllText(path);
            var content = JsonConvert.DeserializeObject<RecordFileModel>(fileText);
            if (content == null)
            {
                MessageBox.Show($"{tree.Name}解析失败");
                return;
            }

            CurrentTitle = TreeInfo.Name;
            CurrentText = content.Text;
            _isLoad = false;
        }

        #endregion

        #region 初始化


        private void InitProperty()
        {
            TreeInfo = new TreeInfo() { IsDirectory = false };
            TextChangedVisibility = Visibility.Collapsed;
        }

        private void InitRegister()
        {
            WeakReferenceMessenger.Default.Register<TreeInfo, string>(this, Const.CONST_LOAD_RECORD_FILE,
                (obj, msg) => { LoadRecord(msg); });
        }

        public RecordPageModel()
        {
            InitRegister();
            InitProperty();
        }

        #endregion
    }
}
