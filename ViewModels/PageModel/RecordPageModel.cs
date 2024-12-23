using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
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
using TaskTip.Common.Extends;
using TaskTip.Enums;
using TaskTip.Models.DataModel;
using TaskTip.Services;

using Image = System.Windows.Controls.Image;

namespace TaskTip.ViewModels
{
    public partial class RecordPageModel : ObservableObject
    {
        #region 属性

        private TreeInfo TreeInfo;
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

        private FlowDocument _richContent;

        public FlowDocument RichContent
        {
            get => _richContent;
            set => SetProperty(ref _richContent, value);
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

                var path = System.IO.Path.Combine(GlobalVariable.RecordFilePath, TreeInfo.GUID) + GlobalVariable.EndFileFormat;

                var content = TextContent;
                WriteJsonContent(path, new RecordFileModel() { GUID = TreeInfo.GUID, Title = TreeInfo.Name, Text = content });

                LoadRecord(TreeInfo);

                WeakReferenceMessenger.Default.Send(new CorrespondenceModel()
                {
                    GUID = TreeInfo.GUID,
                    Operation = OperationRequestType.Update,
                    Message = TreeInfo
                }, Const.CONST_NOTIFY_RECORD_ITEM);
                TextChangedVisibility = Visibility.Collapsed;
            }
            catch (Exception e)
            {
                MessageBox.Show($"保存失败：{e}");
                return;
            }

        }
        public void CopyToDocument(FlowDocument from, FlowDocument to)
        {
            TextRange range = new TextRange(from.ContentStart, from.ContentEnd);
            MemoryStream stream = new MemoryStream();
            System.Windows.Markup.XamlWriter.Save(range, stream);
            range.Save(stream, DataFormats.XamlPackage);
            TextRange range2 = new TextRange(to.ContentEnd, to.ContentEnd);
            range2.Load(stream, DataFormats.XamlPackage);
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
                GlobalVariable.EditFullScreenViewShow();
                GlobalVariable.TaskMenoViewClose();
            }
            else
            {
                FullScreenSource = ((BitmapImage)Application.Current.Resources["ArrowsFullscreen"]).UriSource; //new Uri("pack://application:,,,/Resources/arrows_fullscreen.png");
                GlobalVariable.EditFullScreenViewClose();
                GlobalVariable.TaskMenoViewShow();
            }
        }
        #endregion

        #region 功能函数

        /// <summary>
        /// 快捷键定义
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnKey(object sender, KeyEventArgs e)
        {
            try
            {
                switch (Keyboard.Modifiers)
                {
                    case ModifierKeys.Control when e.Key == Key.V:
                        {
                            // 粘贴
                            if (sender is RichTextBox richBox)
                            {
                                Paste(richBox);
                                e.Handled = true;
                            }

                            break;
                        }
                    case ModifierKeys.Control when e.Key == Key.S:
                        {
                            //快捷键保存
                            Save();
                            e.Handled = true;
                            break;
                        }
                    case ModifierKeys.None:
                    case ModifierKeys.Alt:
                    case ModifierKeys.Shift:
                    case ModifierKeys.Windows:
                    default:
                        {
                            if (e.Key == Key.Back)
                            {
                                // 内容删除时将本地缓存的图片一并删除
                                if (sender is RichTextBox richBox)
                                {
                                    var deleteImage = FindItemInRichTextBox<Image>(richBox.Document.ContentStart).FirstOrDefault();
                                    if (deleteImage == null) return;
                                    _clearPath.Add(deleteImage.Uid);
                                }
                            }

                            break;
                        }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show($"处理快捷键异常：{exception}");
            }

        }

        public void Paste(RichTextBox richBox)
        {
            var paragraph = new Paragraph();
            var position = richBox.Selection.Start.Paragraph;
            var clipboardData = Clipboard.GetDataObject();

            if (clipboardData == null) return;

            try
            {
                var dataFormat = DataFormatList.FirstOrDefault(x => clipboardData.GetDataPresent(x.Name));

                if (dataFormat == null)
                {
                    MessageBox.Show("粘贴内容类型不存在，无法解析");
                    return;
                }

                if (dataFormat.Name == DataFormats.Bitmap)
                {
                    BitmapSource bitmapSource = clipboardData.GetData(DataFormats.Bitmap) as BitmapSource;
                    if (bitmapSource != null && bitmapSource.Format == PixelFormats.Bgra32)
                    {
                        var url = SaveImage(bitmapSource, SaveImageType.PNG);
                        paragraph.Inlines.Add(AddImage(url));
                    }
                }
                else if (dataFormat.Name == DataFormats.FileDrop)
                {
                    var paths = Clipboard.GetFileDropList();
                    var pathString = string.Empty;
                    foreach (var path in paths)
                    {
                        switch (CheckFileCategory(path))
                        {
                            case "PNG":
                            case "JPG":
                                paragraph.Inlines.Add(AddImage(path));
                                break;
                            default:
                                paragraph.Inlines.Add(AddHyperlink(path, LinkType.File));
                                break;
                        }
                    }
                }
                else
                {
                    richBox.Paste();
                    richBox.Document = TrimTransparent(richBox.Document);
                    return;
                }

                if (position?.Inlines.FirstOrDefault() == null)
                {
                    RichContent.Blocks.Add(paragraph);
                    return;
                }

                RichContent.Blocks.InsertAfter(position, paragraph);
            }
            catch (Exception e)
            {
                MessageBox.Show($"粘贴内容出现异常:{e.Message}");
                return;
            }
        }

        /// <summary>
        /// 检查文本是否符合已有规则
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string CheckFileCategory(string str)
        {
            var p = str.ToUpper();
            foreach (var category in RegexRule.Where(category => Regex.IsMatch(p, category.Value)))
            {
                return category.Key;
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取元素类型在选定位置往外找的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pointer"></param>
        /// <returns></returns>
        private List<T> FindItemInRichTextBox<T>(TextPointer pointer)
        {
            List<T> targetList = new List<T>();

            while (pointer != null)
            {
                if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart
                    && pointer.GetAdjacentElement(LogicalDirection.Forward) is Inline inline)
                {
                    if (inline is InlineUIContainer uiContainer && uiContainer.Child is T item)
                    {
                        targetList.Add(item);
                    }
                }

                pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
            }

            return targetList;
        }

        private void ReplaceItemInRichTextBox<T>(UIElement control, TextPointer pointer)
        {
            while (pointer != null)
            {
                if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart
                    && pointer.GetAdjacentElement(LogicalDirection.Forward) is Inline inline)
                {
                    if (inline is InlineUIContainer uiContainer && uiContainer.Child is T item)
                    {
                        if (uiContainer.Child.Uid == control.Uid)
                            uiContainer.Child = control;
                    }
                }

                pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
            }
        }

        /// <summary>
        /// 获取图片唯一标识
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private string GetImageIdentifier(BitmapSource source)
        {
            return source.GetHashCode().ToString();
        }

        #region 文件处理
        /// <summary>
        /// 将普通文本转换为XAML格式，兼容旧版存储格式
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string ConvertPlainTextToXaml(string text)
        {
            return
                $"<FlowDocument PagePadding=\"5,0,5,0\" AllowDrop=\"True\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Paragraph><Run Foreground=\"#FF000000\" xml:lang=\"zh-cn\">{text}</Run></Paragraph></FlowDocument>";
        }
        /// <summary>
        /// 将内容中的图片存储
        /// </summary>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string SaveImage(BitmapSource source, SaveImageType type)
        {
            dynamic encoder = type switch
            {
                SaveImageType.PNG => new JpegBitmapEncoder(),
                SaveImageType.JPEG => new PngBitmapEncoder(),
                _ => new PngBitmapEncoder(),
            };

            encoder.Frames.Add(BitmapFrame.Create(source));

            var hash = GetImageIdentifier(source); // 需要更新为根据识别图片获取唯一值
            var filePath = Path.Combine(GlobalVariable.RecordFilePath, "SaveImage", TreeInfo.GUID,
                $"{hash}.{type.GetDesc()}");

            if (!Directory.Exists(Path.GetDirectoryName(filePath))) Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            if (File.Exists(filePath)) return filePath;

            using (var file = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(file);
                return filePath;
            }
        }

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

                var path = System.IO.Path.Combine(GlobalVariable.RecordFilePath, tree.GUID) + GlobalVariable.EndFileFormat;
                var content = string.Empty;
                if (File.Exists(path))
                {
                    var json = ReadJsonContent(path);
                    content = json.Text;
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
        /// <summary>
        /// 写入JSON内容
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        private void WriteJsonContent(string path, object content)
        {
            var jsonStr = JsonConvert.SerializeObject(content, Formatting.Indented);
            File.WriteAllText(path, jsonStr);

        }
        /// <summary>
        /// 读取JSON内容
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private RecordFileModel ReadJsonContent(string path)
        {

            var file = File.ReadAllText(path);

            if (string.IsNullOrEmpty(file))
                throw new Exception($"{path}的内容为空！");

            return JsonConvert.DeserializeObject<RecordFileModel>(file) ?? new RecordFileModel();

        }

        private FlowDocument TrimTransparent(FlowDocument content)
        {
            foreach (Block block in content.Blocks)
            {
                TextRange blockRange = new TextRange(block.ContentStart, block.ContentEnd);

                // 获取块的字体颜色属性
                object foreground = blockRange.GetPropertyValue(TextElement.ForegroundProperty);

                if (foreground is SolidColorBrush solidColorBrush)
                {
                    if (solidColorBrush.Color.A == 0)
                    {
                        // 将透明字体颜色替换为其他颜色
                        solidColorBrush.Color = Colors.Black; // 替换为所需的颜色
                    }
                }
            }

            return content;
        }

        #endregion

        #endregion

        #region  RichTextBox新增控件
        /// <summary>
        /// 添加链接类型内容
        /// </summary>
        /// <param name="path"></param>
        /// <param name="linkType"></param>
        /// <returns></returns>
        private Hyperlink AddHyperlink(string path, LinkType linkType)
        {

            var (linkDescription, link) = (string.Empty, string.Empty);
            switch (linkType)
            {
                case LinkType.File:
                    (linkDescription, link) = FileType(path);
                    break;
                case LinkType.Directory:
                    (linkDescription, link) = DirType(path);
                    break;
                case LinkType.WebLink:
                    (linkDescription, link) = WebLinkType(path);
                    break;
                default:
                    MessageBox.Show($"未知类型{linkType}");
                    break;
            }
            var hyperlink = new Hyperlink(new Run(linkDescription))
            {
                Foreground = new LinearGradientBrush(Colors.Blue, Colors.Blue, 0.5),
                Tag = ((int)linkType).ToString(),
                ToolTip = link,
            };
            hyperlink.MouseLeftButtonUp += LinkOnClick;

            return hyperlink;
        }
        /// <summary>
        /// 添加图片类型内容（写入缓存版）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private Image? AddImage(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    var binaryReader = new BinaryReader(File.Open(path, FileMode.Open));
                    var fileInfo = new FileInfo(path);
                    var bytes = binaryReader.ReadBytes((int)fileInfo.Length);
                    binaryReader.Close();

                    var bmTemp = new Bitmap(new MemoryStream(bytes));

                    var bmNew = new Bitmap(bmTemp.Width, bmTemp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    bmNew.SetResolution(96, 96);
                    //绘制图片
                    using (var g = Graphics.FromImage(bmNew))
                    {
                        g.Clear(System.Drawing.Color.Transparent);
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(bmTemp, new Rectangle(0, 0, bmNew.Width, bmNew.Height), 0, 0, bmTemp.Width, bmTemp.Height, GraphicsUnit.Pixel);
                        g.Dispose();
                    }

                    var bitmapImage = new BitmapImage();


                    //载入内存
                    using (var ms = new MemoryStream())
                    {
                        bmNew.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = ms;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();

                        ms.Dispose();
                    }

                    return AddImage(bitmapImage, path);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
            //return AddImage(new BitmapImage(new Uri(path)),path);
        }
        /// <summary>
        /// 根据源直接获取图片
        /// </summary>
        /// <param name="source"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private Image AddImage(BitmapSource source, string path)
        {

            return new Image()
            {
                Uid = path,
                Source = source,

            };
        }

        #endregion

        #region LinkControl

        /// <summary>
        /// 链接点击事件（未实现可交互控件）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LinkOnClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Hyperlink hyperlink)
            {
                MessageBox.Show("异常路由，当前非链接");
                return;
            }

            if (string.IsNullOrEmpty(hyperlink.ToolTip.ToString()))
            {
                MessageBox.Show("路径为空,打开失败");
                return;
            }

            try
            {
                var linkType = (LinkType)int.Parse(hyperlink.Tag.ToString());
                switch (linkType)
                {
                    case LinkType.File:
                    case LinkType.Directory:
                        Process.Start("explorer.exe", hyperlink.ToolTip.ToString());
                        break;
                    case LinkType.WebLink:
                        Process.Start(hyperlink.ToolTip.ToString());
                        break;
                    default:
                        MessageBox.Show($"未知类型{linkType}");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开路径异常：{ex}");
                return;
            }
            e.Handled = true;
        }
        #region 类型处理
        private (string, string) FileType(string path)
        {
            if (!File.Exists(path))
            {
                return ("该文件不存在", string.Empty);
            }

            var fileName = Path.GetFileName(path);
            var dirPath = Path.GetDirectoryName(path);
            return (fileName, dirPath);
        }

        private (string, string) DirType(string path)
        {
            if (!Directory.Exists(path))
            {
                return ("该文件夹不存在", string.Empty);
            }

            var temp = Path.GetDirectoryName(path).Split("\\");
            var dirName = Path.GetDirectoryName(path);
            var dirPath = temp[temp.Length - 1];
            return (dirName, dirPath);
        }

        private (string, string) WebLinkType(string path)
        {
            return (path, path);
        }


        #endregion

        #endregion

        #region 初始化


        private void InitProperty()
        {
            TreeInfo = new TreeInfo() { IsDirectory = false };

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
