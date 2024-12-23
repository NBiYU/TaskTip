using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TaskTip.Base;
using TaskTip.Common;
using TaskTip.Common.Extends;
using TaskTip.Enums;
using TaskTip.Models;
using TaskTip.Models.DataModel;
using TaskTip.Services;
using TaskTip.UserControls;
using TaskTip.Views.Windows;
using MessageBox = HandyControl.Controls.MessageBox;

namespace TaskTip.ViewModels
{
    internal partial class MenuItemUserControlModel : BaseVM
    {
        public static bool isLoad = false;

        private static bool _isNodeMove = false;
        private bool isCheckChanged = false;
        private string windowGuid = Guid.NewGuid().ToString();

        #region 属性



        public string GUID
        {
            get => TreeInfo.GUID;
            set => SetProperty(ref TreeInfo.GUID, value);
        }

        public string Title
        {
            get => TreeInfo.Name;
            set
            {
                SetProperty(ref TreeInfo.Name, string.Empty);
                SetProperty(ref TreeInfo.Name, value);
            }
        }

        public bool IsDirectory
        {
            get => TreeInfo.IsDirectory;
        }

        private Visibility _nodeMoveVisibility;
        public Visibility NodeMoveVisibility
        {
            get => _nodeMoveVisibility;
            set => SetProperty(ref _nodeMoveVisibility, value);
        }

        private Visibility _iconVisibility;
        public Visibility IconVisibility
        {
            get => _iconVisibility;
            set => SetProperty(ref _iconVisibility, value);
        }

        private bool _isNode;
        public bool IsNode
        {
            get => _isNode;
            set => SetProperty(ref _isNode, value);
        }

        private Visibility _ediVisibility;
        public Visibility EdiVisibility
        {
            get => _ediVisibility;
            set => SetProperty(ref _ediVisibility, value);
        }

        private bool _moveCheck;
        public bool MoveCheck
        {
            get => _moveCheck;
            set => SetProperty(ref _moveCheck, value);
        }

        public TreeInfo TreeInfo { get; set; }

        private Visibility _listVisibility;
        public Visibility ListVisibility
        {
            get => _listVisibility;
            set => SetProperty(ref _listVisibility, value);
        }

        private ObservableCollection<MenuItemUserControl> _menuItems = new();
        public ObservableCollection<MenuItemUserControl> MenuItems
        {
            get => _menuItems;
            set => SetProperty(ref _menuItems, value);

        }

        private ObservableCollection<string> _dirItems = new();
        public ObservableCollection<string> DirItems { get => _dirItems; set => SetProperty(ref _dirItems, value); }

        private ObservableCollection<string> _fileItems = new();
        public ObservableCollection<string> FileItems { get => _fileItems; set => SetProperty(ref _fileItems, value); }

        #region 类型图标

        public Uri CollapsedGeometry
        {
            get => ((BitmapImage)Application.Current.Resources["Folder"]).UriSource;
        }

        private Uri OpenedGeometry
        {
            get => ((BitmapImage)Application.Current.Resources["OpenFolder"]).UriSource;
        }

        public Uri FileGeometry
        {
            get => ((BitmapImage)Application.Current.Resources["File"]).UriSource;
        }

        private Uri _showButtonGeometry;

        public Uri ShowButtonGeometry
        {
            get => _showButtonGeometry;
            set => SetProperty(ref _showButtonGeometry, value);
        }
        #endregion


        #endregion

        #region 指令


        [RelayCommand]
        public void ShowList()
        {
            ListVisibility = ListVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            ShowButtonGeometry = IsDirectory ? ListVisibility == Visibility.Visible ? OpenedGeometry : CollapsedGeometry : FileGeometry;
        }

        #region 右键菜单

        [RelayCommand]
        public void AddDirectory()
        {
            var dirInfo = new TreeInfo()
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "新建文件夹",
                IsDirectory = true,
                Menu = new MenuTreeModel() { Directories = new List<TreeInfo>(), Files = new List<TreeInfo>() }
            };
            WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { GUID = GUID, Operation = OperationRequestType.Add, Message = dirInfo }, Const.CONST_NOTIFY_RECORD_ITEM);

        }

        [RelayCommand]
        public void AddFile()
        {
            var fileInfo = new TreeInfo()
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "新建文件",
                IsDirectory = false,
                Menu = new MenuTreeModel() { Directories = new List<TreeInfo>(), Files = new List<TreeInfo>() }
            };
            //TreeInfo.Menu.Files.Add(fileInfo);
            //MenuItems.Add(AddItem(fileInfo));

            WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { GUID = GUID, Operation = OperationRequestType.Add, Message = fileInfo }, Const.CONST_NOTIFY_RECORD_ITEM);
        }

        [RelayCommand]
        public void Remark()
        {
            EdiVisibility = Visibility.Visible;
        }

        [RelayCommand]
        public void TitleChanged(object sender)
        {
            if (sender is not System.Windows.Controls.TextBox s) return;

            //TreeInfo.Name = s.Text;
            EdiVisibility = Visibility.Collapsed;

            var control = GetThisControl(s.TemplatedParent);
            WeakReferenceMessenger.Default.Send(
                new CorrespondenceModel()
                {
                    GUID = control.TextGuid.Text,
                    Operation = OperationRequestType.Update,
                    Message = TreeInfo
                },
                Const.CONST_NOTIFY_RECORD_ITEM);
        }

        [RelayCommand]
        public void NodeMove(string guid)
        {
            if (IsNode && !_isNodeMove)
            {
                WeakReferenceMessenger.Default.Send(TreeInfo, Const.CONST_NODE_MOVE);
            }
        }

        [RelayCommand]
        public void RemoveItem(object control)
        {
            var a = GetThisControl(control);
            if (a == null) return;

            WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { GUID = a.TextGuid.Text, Operation = OperationRequestType.Delete }, Const.CONST_NOTIFY_RECORD_ITEM);
        }

        private string gg = Guid.NewGuid().ToString();
        [RelayCommand]
        public void CheckChanged(string guid)
        {
            //是个文件夹，已勾选
            if (!(IsDirectory && NodeMoveVisibility == Visibility.Visible && MoveCheck)) return;

            if (isCheckChanged)
            {
                isCheckChanged = false;
                return;
            }

            if (TreeInfo.GUID == guid)
            {
                isCheckChanged = true;
                WeakReferenceMessenger.Default.Send(guid, Const.CONST_MOVE_CHECK_CHANGED);
            }
            else
            {
                MoveCheck = false;
            }
        }

        #endregion

        private bool isClick = false;
        private DateTime oldDateTime = DateTime.Now;


        #endregion

        #region 功能函数

        private MenuItemUserControl GetThisControl(object control)
        {
            if (control is not DependencyObject menuItem) return null;
            var parent = VisualTreeHelper.GetParent(menuItem);
            while (parent != null && parent is not MenuItemUserControl)
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as MenuItemUserControl;
        }

        public void LoadFileRecord()
        {
            if (TreeInfo.IsDirectory)
            {
                ShowList();
                return;
            }

            WeakReferenceMessenger.Default.Send(
                new TreeInfo()
                {
                    GUID = TreeInfo.GUID,
                    Name = TreeInfo.Name,
                    IsDirectory = TreeInfo.IsDirectory,
                    Menu = TreeInfo.Menu
                }, Const.CONST_LOAD_RECORD_FILE);
        }

        #region 新增控件
        private MenuItemUserControl AddItem(TreeInfo info)
        {
            var control = new MenuItemUserControl();
            control.DataContext = new MenuItemUserControlModel(info);
            //controlModel.TreeInfo = info;
            //controlModel.LoadContent();
            return control;
        }

        private MenuItemUserControl AddItem(TreeInfo info, Visibility nodeMoveVisibility)
        {
            var control = new MenuItemUserControl();
            var model = new MenuItemUserControlModel(info);
            model.NodeMoveVisibility = nodeMoveVisibility;
            control.DataContext = model;

            //controlModel.TreeInfo = info;
            //controlModel.LoadContent();
            return control;
        }

        private MenuItemUserControl AddItem(string guid)
        {
            var control = new MenuItemUserControl();
            var controlModel = control.DataContext as MenuItemUserControlModel;
            controlModel.GUID = guid;
            return control;
        }
        #endregion


        public void LoadContent()
        {
            var menuItems = new ObservableCollection<MenuItemUserControl>();
            foreach (var dirItem in TreeInfo.Menu.Directories)
            {
                menuItems.Add(AddItem(dirItem));
            }

            foreach (var fileItem in TreeInfo.Menu.Files)
            {
                menuItems.Add(AddItem(fileItem));
            }

            MenuItems = menuItems;

            MenuItemsChanged();
        }

        /// <param name="tree"></param>
        //private void TitleChanged(TreeInfo tree)
        //{
        //    if (TreeInfo.GUID == tree.GUID)  
        //    {
        //        Title = tree.Name;
        //        WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { 
        //            GUID = tree.GUID,
        //            Operation = OperationRequestType.Update,
        //            Message = tree
        //        } , Const.CONST_NOTIFY_RECORD_ITEM);
        //    }
        //}

        #region Pop事件处理

        private void ControlLogout(MenuItemUserControl control)
        {
            var model = control.DataContext as MenuItemUserControlModel;
            model?.Unregister();

            if (control.MenuListItem.Items.Count == 0) return;

            foreach (MenuItemUserControl item in control.MenuListItem.Items)
            {
                ControlLogout(item);
            }
        }

        private void MoveHandler(TreeInfo tree)
        {
            if (IsNode) return;

            _isNodeMove = true;
            var dirTree = FindAllDirItem(TreeInfo);


            var layout = new MenuItemUserControl();
            layout.DataContext = new MenuItemUserControlModel(dirTree);
            if (layout.DataContext is MenuItemUserControlModel vm)
            {
                vm.IsNode = false;
            }
            var window = new PopView
            {
                TipMessage =
                {
                    Text = tree.Name
                },
                Layout =
                {
                    Content = layout
                },
            };
            window.PopConfirm += (o, args) =>
            {
                if (MessageBox.Show($"是否移动“{((PopView)o).TipMessage.Text}”文件", "文件移动", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    window.DialogResult = true;
                }
            };
            window.PopOnClose += (o, args) => {
                if (window.Layout.Content is MenuItemUserControl control)
                {
                    ControlLogout(control);
                }
            };

            var result = window.ShowDialog();
            _isNodeMove = false;
            if (result == true)
            {
                var item = ((ScrollViewer)window.Layout).Content as MenuItemUserControl;
                var checkedGuid = FindCheckedItem(item);
                window.Close();

                if (string.IsNullOrEmpty(checkedGuid))
                {
                    ExecuteLogger("未选中目录，默认放弃", true, LogType.Warning);
                    return;
                }
                if (tree.GUID == checkedGuid)
                {
                    MessageBox.Show("移动内容已在当前选择文件夹中");
                    return;
                }
                WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { GUID = tree.GUID, Operation = OperationRequestType.DeleteNotWithFile }, Const.CONST_NOTIFY_RECORD_ITEM);
                WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { GUID = checkedGuid, Operation = OperationRequestType.Add, Message = tree }, Const.CONST_NOTIFY_RECORD_ITEM);
                //WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { GUID = tree.GUID + "Move" + checkedGuid, Operation = OperationRequestType.Move, Message = tree }, Const.CONST_NOTIFY_RECORD_ITEM);
            }

        }


        private TreeInfo FindAllDirItem(TreeInfo tree)
        {
            var dirTree = new TreeInfo()
            {
                GUID = tree.GUID,
                Name = tree.Name,
                IsDirectory = tree.IsDirectory,
                Menu = new MenuTreeModel()
                {
                    Directories = new List<TreeInfo>(),
                    Files = new List<TreeInfo>()
                }
            };
            if (tree.Menu.Directories.Count == 0) return dirTree;

            foreach (var item in tree.Menu.Directories)
            {
                dirTree.Menu.Directories.Add(FindAllDirItem(item));
            }

            return dirTree;
        }

        private string FindCheckedItem(MenuItemUserControl control)
        {
            var model = control.DataContext as MenuItemUserControlModel;
            if (model == null) return string.Empty;
            var checkedGuid = string.Empty;
            if (model.MoveCheck)
            {
                return model.GUID;
            }
            else
            {
                foreach (MenuItemUserControl child in control.MenuListItem.Items)
                {
                    checkedGuid = FindCheckedItem(child);
                    if (!string.IsNullOrEmpty(checkedGuid)) return checkedGuid;
                }
            }
            return string.Empty;
        }

        #endregion

        private void MenuItemsChanged()
        {
            if (TreeInfo.Menu == null)
                return;
            DirItems = new ObservableCollection<string>(TreeInfo.Menu.Directories.Select(x => x.Name).ToList());
            FileItems = new ObservableCollection<string>(TreeInfo.Menu.Files.Select(x => x.Name).ToList());
            MenuItems = new ObservableCollection<MenuItemUserControl>(MenuItems.OrderBy(x => x.TextGuid.IsEnabled == false));

            WeakReferenceMessenger.Default.Send(string.Empty, Const.CONST_RECORD_SAVE_CONFIG);
        }

        private void SaveTreeConfig()
        {
            //节点为子节点或者是文件移动时跳出
            if (IsNode || NodeMoveVisibility == Visibility.Visible) return;

            var path = GlobalVariable.MenuTreeConfigPath;
            var content = JsonConvert.SerializeObject(TreeInfo, Formatting.Indented);
            var existsContent = string.Empty;


            if (File.Exists(path)) existsContent = File.ReadAllText(path);

            if (!existsContent.Equals(content))
            {
                File.WriteAllText(path, content);
            }

        }

        private void Reload(string path)
        {
            if (IsNode) return;

            isLoad = false;
            LoadFile(path);
        }

        private void ParentChanged(CorrespondenceModel corr)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {

                switch (corr.Operation)
                {
                    case OperationRequestType.Move:
                        var startGuid = corr.GUID.Split("Move")[0];
                        var endGuid = corr.GUID.Split("Move")[1];

                        WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { GUID = startGuid, Operation = OperationRequestType.Delete }, Const.CONST_NOTIFY_RECORD_ITEM);
                        WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { GUID = endGuid, Operation = OperationRequestType.Add, Message = corr.Message }, Const.CONST_NOTIFY_RECORD_ITEM);
                        return;
                    case OperationRequestType.Delete:
                    case OperationRequestType.DeleteNotWithFile:
                        if (_isNodeMove) break;

                        var deleteItem = MenuItems.FirstOrDefault(x => x.TextGuid.Text == corr.GUID);
                        if (deleteItem == null) return;

                        MenuItems.Remove(deleteItem);
                        TreeInfo.Menu.Directories.RemoveAll(x => x.GUID == corr.GUID);
                        TreeInfo.Menu.Files.RemoveAll(x => x.GUID == corr.GUID);
                        if (corr.Operation == OperationRequestType.DeleteNotWithFile)
                        {
                            var path = Path.Combine(GlobalVariable.RecordFilePath,
                                $"{deleteItem.TextGuid.Text}{GlobalVariable.EndFileFormat}");
                            var xamlPath = Path.Combine(GlobalVariable.RecordFilePath, "Xaml",
                                $"{deleteItem.TextGuid.Text}{GlobalVariable.EndFileFormat}");

                            if (File.Exists(path)) File.Delete(path);
                            if (File.Exists(xamlPath)) File.Delete(xamlPath);
                        }

                        OperationRecord.OperationRecordWrite(new TcpRequestData() { GUID = corr.GUID, OperationType = corr.Operation, SyncCategory = SyncFileCategory.Record, FileData = (deleteItem.DataContext as MenuItemUserControlModel)?.TreeInfo });
                        break;
                    case OperationRequestType.Update:
                        var toolTipItem = MenuItems.FirstOrDefault(x => x.TextGuid.Text == corr.GUID);
                        if (toolTipItem == null) return;

                        var updateTree = corr.Message as TreeInfo;
                        if (updateTree == null) return;

                        var model = toolTipItem.DataContext as MenuItemUserControlModel;
                        model.Title = updateTree.Name;

                        if (updateTree.IsDirectory)
                        {
                            var dir = TreeInfo.Menu?.Directories.FirstOrDefault(x => x.GUID == corr.GUID);
                            dir.Name = updateTree.Name;
                        }
                        else
                        {
                            var file = TreeInfo.Menu?.Files.FirstOrDefault(x => x.GUID == corr.GUID);
                            file.Name = updateTree.Name;
                        }
                        OperationRecord.OperationRecordWrite(new TcpRequestData() { GUID = corr.GUID, OperationType = corr.Operation, SyncCategory = SyncFileCategory.Record, FileData = corr.Message });
                        break;

                    case OperationRequestType.Add:
                        if (GUID != corr.GUID || !IsDirectory) return;

                        var tree = corr.Message as TreeInfo;

                        if (TreeInfo.Menu.Directories.FirstOrDefault(x => x.GUID == tree.GUID) != null) return;

                        MenuItems.Add(AddItem(tree, NodeMoveVisibility));
                        if (tree.IsDirectory)
                        {
                            TreeInfo.Menu.Directories.Add(tree);
                        }
                        else
                        {
                            TreeInfo.Menu.Files.Add(tree);
                        }
                        OperationRecord.OperationRecordWrite(new TcpRequestData() { GUID = corr.GUID, OperationType = corr.Operation, SyncCategory = SyncFileCategory.Record, FileData = corr.Message });
                        break;
                }
                MenuItemsChanged();
            });
        }

        #endregion

        #region 初始化

        public void Unregister()
        {
            WeakReferenceMessenger.Default.Unregister<string, string>(this, Const.CONST_MOVE_CHECK_CHANGED);
            WeakReferenceMessenger.Default.Unregister<TreeInfo, string>(this, Const.CONST_NODE_MOVE);
            WeakReferenceMessenger.Default.Unregister<string, string>(this, Const.CONST_RECORD_RELOAD);
            WeakReferenceMessenger.Default.Unregister<CorrespondenceModel, string>(this, Const.CONST_NOTIFY_RECORD_ITEM);
            //WeakReferenceMessenger.Default.Unregister<TreeInfo, string>(this, Const.CONST_RECORD_SAVE_TITLE);
            WeakReferenceMessenger.Default.Unregister<string, string>(this, Const.CONST_RECORD_SAVE_CONFIG);
        }

        private void InitRegister()
        {
            //文件移动选中事件
            WeakReferenceMessenger.Default.Register<string, string>(this, Const.CONST_MOVE_CHECK_CHANGED, (obj, msg) =>
            {
                CheckChanged(msg);
            });
            //文件移动处理
            WeakReferenceMessenger.Default.Register<TreeInfo, string>(this, Const.CONST_NODE_MOVE, (obj, msg) =>
            {
                MoveHandler(msg);
            });
            //重新加载
            WeakReferenceMessenger.Default.Register<string, string>(this, Const.CONST_RECORD_RELOAD, (obj, msg) =>
            {
                Reload(msg);
            });
            //父元素内容变更
            WeakReferenceMessenger.Default.Register<CorrespondenceModel, string>(this, Const.CONST_NOTIFY_RECORD_ITEM, (obj, msg) =>
            {
                ParentChanged(msg);
            });
            // 保存树结构
            WeakReferenceMessenger.Default.Register<string, string>(this, Const.CONST_RECORD_SAVE_CONFIG, (obj, msg) =>
            {
                SaveTreeConfig();
            });
        }

        private void LoadFile(string path)
        {
            try
            {
                isLoad = true;
                var rootJson = JsonConvert.SerializeObject(new TreeInfo()
                {
                    GUID = "RootDocument",
                    Name = "根目录",
                    IsDirectory = true,
                    Menu = new MenuTreeModel()
                    {
                        Directories = new List<TreeInfo>(),
                        Files = new List<TreeInfo>()
                    }
                }, Formatting.Indented);
                if (!File.Exists(path))
                {
                    if (!Directory.Exists(Path.GetDirectoryName(path)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                    }
                    File.WriteAllText(GlobalVariable.MenuTreeConfigPath, rootJson);
                }
                var json = File.ReadAllText(path);
                rootJson = string.IsNullOrEmpty(json) ? rootJson : json;
                var content = JsonConvert.DeserializeObject<TreeInfo>(rootJson);
                TreeInfo = content;
                IsNode = false;
                ShowButtonGeometry = CollapsedGeometry;
                LoadContent();
            }
            catch (Exception e)
            {
                ExecuteLogger($"【{this}】配置文件读取异常：{e.Message}", true, LogType.Error);
            }
        }

        private void InitItem(TreeInfo info)
        {
            IsNode = true;
            MoveCheck = false;
            VMName = SyncFileCategory.Record.GetDesc();
            NodeMoveVisibility = _isNodeMove ? Visibility.Visible : Visibility.Collapsed;
            EdiVisibility = Visibility.Collapsed;
            ListVisibility = Visibility.Collapsed;
            ShowButtonGeometry = IsDirectory ? ListVisibility == Visibility.Visible ? OpenedGeometry : CollapsedGeometry : FileGeometry;
            InitRegister();
        }

        public MenuItemUserControlModel(TreeInfo info)
        {
            TreeInfo = info;
            InitItem(info);
            LoadContent();
        }

        public MenuItemUserControlModel()
        {
            TreeInfo = new TreeInfo();
            InitItem(TreeInfo);
            if (!isLoad)
            {
                LoadFile(GlobalVariable.MenuTreeConfigPath);
            }
        }

        ~MenuItemUserControlModel()
        {
            Unregister();
            ExecuteLogger($"【{this}】【注销信使】【{windowGuid}】 {GUID} 已全部注销");
        }

        #endregion
    }


}
