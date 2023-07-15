using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using TaskTip.Models;
using TaskTip.Services;
using TaskTip.UserControls;
using MessageBox = HandyControl.Controls.MessageBox;

namespace TaskTip.ViewModels
{
    internal partial class MenuItemUserControlModel : ObservableObject
    {
        public static bool isLoad = false;


        #region 属性

        public string GUID
        {
            get => TreeInfo.GUID;
            set => SetProperty(ref TreeInfo.GUID, value);
        }

        public string Title
        {
            get => TreeInfo.Name;
            set => SetProperty(ref TreeInfo.Name, value);
        }

        public bool IsDirectory
        {
            get => TreeInfo.IsDirectory;
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

        public TreeInfo TreeInfo { get; set; }

        public Visibility DirVisibility
        {
            get => TreeInfo.IsDirectory ? Visibility.Visible : Visibility.Collapsed;
        }

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

        public Geometry CollapsedGeometry
        {
            get
            {
                PathFigure figure = new PathFigure();
                figure.StartPoint = new Point(-5, 28);
                figure.Segments.Add(new LineSegment(new Point(10, 14), true));
                figure.Segments.Add(new LineSegment(new Point(-5, 0), true));
                PathGeometry pathGeometry = new PathGeometry();
                pathGeometry.Figures.Add(figure);
                return pathGeometry;
            }
        }

        private Geometry VisibilityGeometry
        {
            get
            {
                PathFigure figure = new PathFigure();
                figure.StartPoint = new Point(0, 0);
                figure.Segments.Add(new LineSegment(new Point(15, 15), true));
                figure.Segments.Add(new LineSegment(new Point(30, 0), true));
                PathGeometry pathGeometry = new PathGeometry();
                pathGeometry.Figures.Add(figure);
                return pathGeometry;
            }
        }

        private Geometry _showButtonGeometry;

        public Geometry ShowButtonGeometry
        {
            get => _showButtonGeometry;
            set => SetProperty(ref _showButtonGeometry, value);
        }

        #endregion

        #region 指令


        [RelayCommand]
        public void ShowList()
        {
            ListVisibility = ListVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            ShowButtonGeometry = ListVisibility == Visibility.Visible ? VisibilityGeometry : CollapsedGeometry;
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
            TreeInfo.Menu.Directories.Add(dirInfo);
            MenuItems.Add(AddItem(dirInfo));
            MenuItemsChanged();
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
            TreeInfo.Menu.Files.Add(fileInfo);
            MenuItems.Add(AddItem(fileInfo));
            MenuItemsChanged();
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

            TreeInfo.Name = s.Text;
            EdiVisibility = Visibility.Collapsed;

            var control = GetThisControl(s.TemplatedParent);
            WeakReferenceMessenger.Default.Send(
                new CorrespondenceModel()
                {
                    GUID = control.Guid.Text,
                    Operation = "ToolTip",
                    Message = Title
                },
                Const.CONST_NOTIFY_RECORD_ITEM);
        }

        [RelayCommand]
        public void RemoveItem(object control)
        {
            var a = GetThisControl(control);
            if (a == null) return;

            WeakReferenceMessenger.Default.Send(new CorrespondenceModel() { GUID = a.Guid.Text, Operation = "DELETE" }, Const.CONST_NOTIFY_RECORD_ITEM);
        }


        #endregion

        private bool isClick = false;
        private DateTime oldDateTime = DateTime.Now;

        [RelayCommand]
        public void DoubleClick()
        {
            if ((DateTime.Now - oldDateTime).TotalMilliseconds < 1000 && isClick && !IsDirectory)
            {
                LoadFileRecord();
                isClick = false;
            }
            else
            {
                isClick = true;
                oldDateTime = DateTime.Now;
            }
        }

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

            if (parent == null) return null;

            return parent as MenuItemUserControl;
        }

        public void LoadFileRecord()
        {
            WeakReferenceMessenger.Default.Send(
                new TreeInfo()
                {
                    GUID = TreeInfo.GUID,
                    Name = TreeInfo.Name,
                    IsDirectory = TreeInfo.IsDirectory,
                    Menu = TreeInfo.Menu
                }, Const.CONST_LOAD_RECORD_FILE);
        }


        private MenuItemUserControl AddItem(TreeInfo info)
        {
            var control = new MenuItemUserControl();
            control.RecordGrid.DataContext = new MenuItemUserControlModel(info);
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
        private void TitleChanged(TreeInfo tree)
        {
            if (TreeInfo.GUID == tree.GUID)
            {
                Title = tree.Name;
            }
        }

        private void MenuItemsChanged()
        {
            DirItems = new ObservableCollection<string>(TreeInfo.Menu?.Directories.Select(x => x.Name).ToList());
            FileItems = new ObservableCollection<string>(TreeInfo.Menu?.Files.Select(x => x.Name).ToList());
            MenuItems = new ObservableCollection<MenuItemUserControl>(MenuItems.OrderBy(x => x.Guid.IsEnabled == false));

            WeakReferenceMessenger.Default.Send(string.Empty, Const.CONST_RECORD_SAVE_CONFIG);

        }

        private void SaveTreeConfig()
        {
            if (IsNode) return;

            var path = GlobalVariable.MenuTreeConfigPath;
            var content = JsonConvert.SerializeObject(TreeInfo);
            var existsContent = string.Empty;


            if (File.Exists(path)) existsContent = File.ReadAllText(path);

            if (!existsContent.Equals(content))
                File.WriteAllText(path, content);
        }


        private void ParentChanged(CorrespondenceModel corr)
        {
            switch (corr.Operation)
            {
                case "DELETE":
                    var deleteItem = MenuItems.FirstOrDefault(x => x.Guid.Text == corr.GUID);
                    if (deleteItem == null) return;

                    MenuItems.Remove(deleteItem);
                    TreeInfo.Menu.Directories.RemoveAll(x => x.GUID == corr.GUID);
                    TreeInfo.Menu.Files.RemoveAll(x => x.GUID == corr.GUID);

                    var path = Path.Combine(GlobalVariable.RecordFilePath,
                        $"{{deleteItem.Guid}}{GlobalVariable.EndFileFormat}");

                    if (File.Exists(path)) File.Delete(path);

                    break;
                case "ToolTip":
                    var toolTipItem = MenuItems.FirstOrDefault(x => x.Guid.Text == corr.GUID);
                    if (toolTipItem == null) return;

                    var dir = TreeInfo.Menu?.Directories.FirstOrDefault(x => x.GUID == corr.GUID);
                    var file = TreeInfo.Menu?.Files.FirstOrDefault(x => x.GUID == corr.GUID);
                    if (dir != null)
                    {
                        dir.Name = corr.Message.ToString();
                    }
                    if (file != null)
                    {
                        file.Name = corr.Message.ToString();
                    }
                    break;

            }
            MenuItemsChanged();
        }

        #endregion

        #region 初始化


        private void InitRegister()
        {
            WeakReferenceMessenger.Default.Register<CorrespondenceModel, string>(this, Const.CONST_NOTIFY_RECORD_ITEM,
                (obj, msg) => { ParentChanged(msg); });
            WeakReferenceMessenger.Default.Register<TreeInfo, string>(this, Const.CONST_RECORD_SAVE_TITLE,
                (obj, msg) => { TitleChanged(msg); });
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
                if (!File.Exists(path))
                    File.WriteAllText(path,
                        JsonConvert.SerializeObject(new TreeInfo()
                        {
                            GUID = Guid.NewGuid().ToString(),
                            Name = "根目录",
                            IsDirectory = true,
                            Menu = new MenuTreeModel()
                            {
                                Directories = new List<TreeInfo>(),
                                Files = new List<TreeInfo>()
                            }
                        }));

                var json = File.ReadAllText(path);
                var content = JsonConvert.DeserializeObject<TreeInfo>(json);
                TreeInfo = content;
                IsNode = false;
                LoadContent();
            }
            catch (Exception e)
            {
                MessageBox.Show($"【记录】配置文件读取异常：{e}");
            }
        }

        private void InitItem(TreeInfo info)
        {
            IsNode = true;
            EdiVisibility = Visibility.Collapsed;
            ListVisibility = Visibility.Collapsed;
            ShowButtonGeometry = ListVisibility == Visibility.Visible ? ShowButtonGeometry : CollapsedGeometry;
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

        #endregion
    }
}
