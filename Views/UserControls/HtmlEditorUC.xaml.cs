using DiffPlex.Chunkers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Xml.XPath;
using System.Xml;
using UserControl = System.Windows.Controls.UserControl;
using System.ComponentModel;
using System.Drawing;

namespace TaskTip.Views.UserControls
{
    /// <summary>
    /// HtmlEditorUCUC.xaml 的交互逻辑
    /// </summary>
    public partial class HtmlEditorUC : UserControl
    {
        private HtmlDocument html;

        public HtmlEditorUC()
        {
            InitializeComponent();
            InitController();
            InitContainer();
            InitStyle();
            InitEvents();
            InitTimer();
        }

        private void InitController()
        {
            FontFamilyList.ItemsSource = new List<string>(GetAllFont());
            FontSizeList.ItemsSource = new List<int>([6,10,15,20,25,30]);
        }
        private List<string> GetAllFont()
        {
            var MyFont = new InstalledFontCollection();
            var MyFontFamilies = MyFont.Families;
            List<string> list = new List<string>();
            int Count = MyFontFamilies.Length;
            for (int i = 0; i<Count; i++)
            {
                string FontName = MyFontFamilies[i].Name;
                list.Add(FontName);
            }

            return list;
        }
        #region 字段

        private HtmlDocument htmldoc;
        private Window hostedWindow;
        private DispatcherTimer timer;
        private Dictionary<string, ImageObject> imgDictionary;
        private string style;
        bool isDocReady;
        RoutedEventArgs DocumentStateChangedEventArgs = new RoutedEventArgs(HtmlEditorUC.DocumentStateChangedEvent);

        #endregion

        #region 构造

        #endregion

        #region 依赖项属性

        public static readonly RoutedEvent DocumentReadyEvent =
            EventManager.RegisterRoutedEvent("DocumentReady", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(HtmlEditorUC));

        /// <summary>
        /// Raise when the document is ready.
        /// </summary>
        public event RoutedEventHandler DocumentReady
        {
            add { base.AddHandler(DocumentReadyEvent, value); }
            remove { base.RemoveHandler(DocumentReadyEvent, value); }
        }

        public static readonly RoutedEvent DocumentStateChangedEvent =
            EventManager.RegisterRoutedEvent("DocumentStateChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(HtmlEditorUC));

        /// <summary>
        /// Raise when the state of document is changed.
        /// </summary>
        public event RoutedEventHandler DocumentStateChanged
        {
            add { base.AddHandler(DocumentStateChangedEvent, value); }
            remove { base.RemoveHandler(DocumentStateChangedEvent, value); }
        }

        #endregion

        #region 事件
        private void InitEvents()
        {
            this.Loaded += new RoutedEventHandler(OnHtmlEditorLoaded);
            this.Unloaded += new RoutedEventHandler(OnHtmlEditorUnloaded);


            TogglePrint.Click += new RoutedEventHandler(OnPrintClick);
            ToggleCodeMode.Checked += new RoutedEventHandler(OnCodeModeChecked);
            ToggleCodeMode.Unchecked += new RoutedEventHandler(OnCodeModeUnchecked);

            ToggleFontColor.MouseEnter += (s, e) =>
            {
                ShowContextMenu(s as FrameworkElement, FontColorContextMenu);
            };
            ToggleLineColor.MouseEnter += (s, e) =>
            {
                ShowContextMenu(s as FrameworkElement, LineColorContextMenu);
            };
            ToggleFontColor.MouseDown += (s, e) => { CloseContextMenuAll(); };
            ToggleLineColor.MouseDown += (s, e) => { CloseContextMenuAll(); };

            FontColorContextMenu.Opened += new RoutedEventHandler(OnFontColorContextMenuOpened);
            FontColorContextMenu.Closed += (s, e) => { OnContextMenuClosed(); };
            LineColorContextMenu.Opened += new RoutedEventHandler(OnLineColorContextMenuOpened);
            LineColorContextMenu.Closed += (s, e) => { OnContextMenuClosed(); };

            ToggleFaceImage.MouseEnter += (s, e) =>
            {
                ShowContextMenu(s as FrameworkElement, FaceImageContextMenu);
            };
            ToggleFaceImage.MouseDown += (s, e) => { CloseContextMenuAll(); };
            FaceImageContextMenu.Opened += new RoutedEventHandler(OnFaceContextMenuOpened);
            FaceImageContextMenu.Closed += (s, e) => { OnContextMenuClosed(); };
            FaceImagePicker.SelectedFaceChanged += new EventHandler<PropertyChangedEventArgs<String>>(OnFacePickerSelectedChanged);

            FontColorPicker.SelectedColorChanged += new EventHandler<PropertyChangedEventArgs<Color>>(OnFontColorPickerSelectedColorChanged);
            LineColorPicker.SelectedColorChanged += new EventHandler<PropertyChangedEventArgs<Color>>(OnLineColorPickerSelectedColorChanged);
        }
        private void OnPrintClick(object sender, RoutedEventArgs e)
        {

        }
        private void OnCodeModeChecked(object sender, RoutedEventArgs e)
        {
            EditMode = EditMode.Source;
        }
        private void OnCodeModeUnchecked(object sender, RoutedEventArgs e)
        {
            EditMode = EditMode.Visual;
        }
        private void OnHtmlEditorLoaded(object sender, RoutedEventArgs e)
        {
            imgDictionary = new Dictionary<string, ImageObject>();
            this.hostedWindow = this.GetParentWindow();
            timer.Start();
        }
        private void OnHtmlEditorUnloaded(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }
        private void OnFontColorContextMenuOpened(object sender, RoutedEventArgs e)
        {
            FontColorPicker.Reset();
            ToggleFontColor.IsChecked = true;
        }
        private void OnLineColorContextMenuOpened(object sender, RoutedEventArgs e)
        {
            LineColorPicker.Reset();
            ToggleLineColor.IsChecked = true;
        }
        private void OnFaceContextMenuOpened(object sender, RoutedEventArgs e)
        {
            FaceImagePicker.Reset();
            ToggleFaceImage.IsChecked = true;
        }
        private void OnFontColorPickerSelectedColorChanged(object sender, PropertyChangedEventArgs<Color> e)
        {
            htmldoc.SetFontColor(e.Value);
        }
        private void OnFacePickerSelectedChanged(object sender, PropertyChangedEventArgs<String> e)
        {
            string path = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\Face\\";
            htmldoc.InsertHTML("<image src='" + path + "\\" + e.Value + "'/>");
        }
        private void OnLineColorPickerSelectedColorChanged(object sender, PropertyChangedEventArgs<Color> e)
        {
            htmldoc.SetLineColor(e.Value);
        }
        private void OnVisualEditorStatusTextChanged(object sender, EventArgs e)
        {
            if (Document == null) return;

            RaiseEvent(DocumentStateChangedEventArgs);
            if (Document.State == HtmlDocumentState.Complete)
            {
                if (isDocReady)
                {
                    Dispatcher.BeginInvoke(new Action(this.NotifyBindingContentChanged));
                }
                else
                {
                    isDocReady = true;
                    RaiseEvent(new RoutedEventArgs(HtmlEditorUC.DocumentReadyEvent));
                }
            }
        }
        private void OnVisualEditorDocumentNavigated(object sender, System.Windows.Forms.WebBrowserNavigatedEventArgs e)
        {
            VisualEditor.Document.ContextMenuShowing += this.OnDocumentContextMenuShowing;
            htmldoc = new HtmlDocument(VisualEditor.Document);
            //((IHTMLDocument2)VisualEditor.Document.DomDocument).designMode = "ON";
            //样式
            if (style != null && VisualEditor.Document != null)
            {
                HTMLDocument hdoc = (HTMLDocument)VisualEditor.Document.DomDocument;
                IHTMLStyleSheet hstyle = hdoc.createStyleSheet("", 0);
                hstyle.cssText = style;
            }
            //内容
            if (myBindingContent != null)
                VisualEditor.Document.Body.InnerHtml = myBindingContent;

            VisualEditor.Document.Body.SetAttribute("contenteditable", "true");
            VisualEditor.Document.Focus();
        }
        private void OnDocumentContextMenuShowing(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            EditingContextMenu.IsOpen = true;
            e.ReturnValue = false;
        }
        #endregion

        #region 定时器

        private void InitTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += new EventHandler(OnTimerTick);
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (htmldoc.State != HtmlDocumentState.Complete) return;

            ToggleBold.IsChecked = htmldoc.IsBold();
            ToggleItalic.IsChecked = htmldoc.IsItalic();
            ToggleUnderline.IsChecked = htmldoc.IsUnderline();
            ToggleSubscript.IsChecked = htmldoc.IsSubscript();
            ToggleSuperscript.IsChecked = htmldoc.IsSuperscript();
            ToggleBulletedList.IsChecked = htmldoc.IsBulletsList();
            ToggleNumberedList.IsChecked = htmldoc.IsNumberedList();
            ToggleJustifyLeft.IsChecked = htmldoc.IsJustifyLeft();
            ToggleJustifyRight.IsChecked = htmldoc.IsJustifyRight();
            ToggleJustifyCenter.IsChecked = htmldoc.IsJustifyCenter();
            ToggleJustifyFull.IsChecked = htmldoc.IsJustifyFull();

            FontFamilyList.SelectedItem = htmldoc.GetFontFamily();
            FontSizeList.SelectedItem = htmldoc.GetFontSize();
        }

        #endregion

        #region 初始化样式
        private void InitStyle()
        {
            List<FontFamily> families = new List<FontFamily>();
            FontFamily srcfamily = new FontFamily("Times New Roman");
            int srcsize = 10;

            try
            {
                using (XmlReader reader = XmlTextReader.Create(Glo.EditorConfigPath))
                {
                    XPathDocument xmlDoc = new XPathDocument(reader);
                    XPathNavigator navDoc = xmlDoc.CreateNavigator();
                    XPathNodeIterator it;
                    it = navDoc.Select(VisualFontFamiliesPath);
                    while (it.MoveNext())
                    {
                        FontFamily ff = new FontFamily(it.Current.Value);
                        families.Add(ff);
                    }
                    it = navDoc.Select(SourceFontFamilyPath);
                    while (it.MoveNext())
                    {
                        srcfamily = new FontFamily(it.Current.Value);
                        break;
                    }
                    it = navDoc.Select(SourceFontSizePath);
                    while (it.MoveNext())
                    {
                        srcsize = it.Current.ValueAsInt;
                        break;
                    }
                }
                FontFamilyList.ItemsSource = new ReadOnlyCollection<FontFamily>(families);
                FontFamilyList.SelectionChanged += new SelectionChangedEventHandler(OnFontFamilyChanged);
            }
            catch (Exception)
            {

            }
            FontSizeList.ItemsSource = GetDefaultFontSizes();
            FontSizeList.SelectionChanged += new SelectionChangedEventHandler(OnFontSizeChanged);
            TxtEditor.FontFamily = srcfamily;
            TxtEditor.FontSize = srcsize;
        }
        private ReadOnlyCollection<FontSize> GetDefaultFontSizes()
        {
            List<FontSize> ls = new List<FontSize>()
            {
                Msl.HtmlEditorUC.FontSize.XXSmall,
                Msl.HtmlEditorUC.FontSize.XSmall,
                Msl.HtmlEditorUC.FontSize.Small,
                Msl.HtmlEditorUC.FontSize.Middle,
                Msl.HtmlEditorUC.FontSize.Large,
                Msl.HtmlEditorUC.FontSize.XLarge,
                Msl.HtmlEditorUC.FontSize.XXLarge
            };
            return new ReadOnlyCollection<FontSize>(ls);
        }
        private void OnFontFamilyChanged(object sender, SelectionChangedEventArgs e)
        {
            if (htmldoc != null)
            {
                FontFamily selectionFontFamily = htmldoc.GetFontFamily();
                FontFamily selectedFontFamily = (FontFamily)FontFamilyList.SelectedValue;
                if (selectedFontFamily != selectionFontFamily) htmldoc.SetFontFamily(selectedFontFamily);
            }
        }
        private void OnFontSizeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (htmldoc != null)
            {
                FontSize selectionFontSize = htmldoc.GetFontSize();
                FontSize selectedFontSize = (FontSize)FontSizeList.SelectedValue;
                if (selectedFontSize != selectionFontSize) htmldoc.SetFontSize(selectedFontSize);
            }
        }
        private void LoadStylesheet()
        {
            try
            {
                using (StreamReader reader = new StreamReader(Glo.StylesheetPath))
                {
                    style = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
            }
        }

        private static readonly string VisualFontFamiliesPath = @"/htmleditor/visualmode/fontfamilies/add/@value";
        private static readonly string SourceFontFamilyPath = @"/htmleditor/sourcemode/fontfamily/@value";
        private static readonly string SourceFontSizePath = @"/htmleditor/sourcemode/fontsize/@value";

        #endregion

        #region 属性

        #region EditMode Dependency Property

        private EditMode mode;

        /// <summary>
        /// Get or set the edit mode of editor.
        /// This is a dependency property.
        /// </summary>
        public EditMode EditMode
        {
            get { return (EditMode)GetValue(EditModeProperty); }
            set { SetValue(EditModeProperty, value); }
        }

        public static readonly DependencyProperty EditModeProperty =
            DependencyProperty.Register("EditMode", typeof(EditMode), typeof(HtmlEditorUC),
                new FrameworkPropertyMetadata(EditMode.Visual, new PropertyChangedCallback(OnEditModeChanged)));

        private static void OnEditModeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            HtmlEditorUC editor = (HtmlEditorUC)sender;
            if ((EditMode)e.NewValue == EditMode.Visual) editor.SetVisualMode();
            else editor.SetSourceMode();
        }

        /// <summary>
        /// Set the editor to visual mode.
        /// </summary>
        private void SetVisualMode()
        {
            if (mode != EditMode.Visual)
            {
                BrowserHost.Visibility = Visibility.Visible;
                TxtEditor.Visibility = Visibility.Hidden;

                FontFamilyList.IsEnabled = true;
                FontSizeList.IsEnabled = true;
                ToggleFontColor.IsEnabled = true;
                ToggleLineColor.IsEnabled = true;

                VisualEditor.Document.Body.InnerHtml = GetEditContent();
                mode = EditMode.Visual;
            }
        }

        /// <summary>
        /// Set the editor to source mode.
        /// </summary>
        private void SetSourceMode()
        {
            if (mode != EditMode.Source)
            {
                BrowserHost.Visibility = Visibility.Hidden;
                TxtEditor.Visibility = Visibility.Visible;

                FontFamilyList.IsEnabled = false;
                FontSizeList.IsEnabled = false;
                ToggleFontColor.IsEnabled = false;
                ToggleLineColor.IsEnabled = false;

                TxtEditor.Text = GetEditContent();
                mode = EditMode.Source;
            }
        }

        #endregion

        #region BindingContent Dependency Property

        private string myBindingContent = string.Empty;

        public string BindingContent
        {
            get { return (string)GetValue(BindingContentProperty); }
            set { SetValue(BindingContentProperty, value); }
        }

        public static readonly DependencyProperty BindingContentProperty =
            DependencyProperty.Register("BindingContent", typeof(string), typeof(HtmlEditorUC),
                new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnBindingContentChanged)));

        private static void OnBindingContentChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            HtmlEditorUC editor = (HtmlEditorUC)sender;
            editor.myBindingContent = (string)e.NewValue;
            editor.ContentHtml = editor.myBindingContent;
        }

        private void NotifyBindingContentChanged()
        {
            if (myBindingContent != this.ContentHtml)
            {
                BindingContent = this.ContentHtml;
            }
        }

        #endregion

        /// <summary>
        /// 获取字数统计
        /// </summary>
        public int WordCount
        {
            get
            {
                if (ToggleCodeMode.IsChecked.Value)
                {
                    WordCounter counter = WordCounter.Create();
                    return counter.Count(this.TxtEditor.Text);
                }
                else if (htmldoc != null && htmldoc.Content != null)
                {
                    WordCounter counter = WordCounter.Create();
                    return counter.Count(htmldoc.Text);
                }
                return 0;
            }
        }

        /// <summary>
        /// 获取或设置编辑器中的HTML内容。
        /// </summary>
        public string ContentHtml
        {
            get
            {
                if (ToggleCodeMode.IsChecked == true)
                    VisualEditor.Document.Body.InnerHtml = this.TxtEditor.Text;
                return VisualEditor.Document.Body.InnerHtml;
            }
            set
            {
                value = (value != null ? value : string.Empty);
                BindingContent = value;
                if (VisualEditor.Document != null && VisualEditor.Document.Body != null)
                    VisualEditor.Document.Body.InnerHtml = value;

                if (ToggleCodeMode.IsChecked == true)
                    TxtEditor.AppendText(VisualEditor.Document.Body.InnerHtml);
            }
        }

        /// <summary>
        /// 获取编辑器中的文本内容。
        /// </summary>
        public string ContentText
        {
            get
            {
                if (ToggleCodeMode.IsChecked == true)
                    VisualEditor.Document.Body.InnerHtml = this.TxtEditor.Text;
                return VisualEditor.Document.Body.InnerText;
            }
        }

        /// <summary>
        /// 获取HTML文档对象。
        /// </summary>
        public HtmlDocument Document
        {
            get { return htmldoc; }
        }

        /// <summary>
        /// 获取一个值，撤销命令是否可执行。
        /// </summary>
        public bool CanUndo
        {
            get
            {
                return
                    mode == EditMode.Visual &&
                    htmldoc != null &&
                    htmldoc.QueryCommandEnabled("Undo");
            }
        }

        /// <summary>
        /// 获取一个值，指示重做命令是否可执行。
        /// </summary>
        public bool CanRedo
        {
            get
            {
                return
                    mode == EditMode.Visual &&
                    htmldoc != null &&
                    htmldoc.QueryCommandEnabled("Redo");
            }
        }

        /// <summary>
        /// 获取一个值，指示剪切命令是否可执行。
        /// </summary>
        public bool CanCut
        {
            get
            {
                return
                    mode == EditMode.Visual &&
                    htmldoc != null &&
                    htmldoc.QueryCommandEnabled("Cut");
            }
        }

        /// <summary>
        /// 获取一个值，指示复制命令是否可执行。
        /// </summary>
        public bool CanCopy
        {
            get
            {
                return
                    mode == EditMode.Visual &&
                    htmldoc != null &&
                    htmldoc.QueryCommandEnabled("Copy");
            }
        }

        /// <summary>
        /// 获取一个值，指示粘贴命令是否可执行。
        /// </summary>
        public bool CanPaste
        {
            get
            {
                return
                    mode == EditMode.Visual &&
                    htmldoc != null &&
                    htmldoc.QueryCommandEnabled("Paste");
            }
        }

        /// <summary>
        /// 获取一个值，指示删除命令是否可执行。
        /// </summary>
        public bool CanDelete
        {
            get
            {
                return
                    mode == EditMode.Visual &&
                    htmldoc != null &&
                    htmldoc.QueryCommandEnabled("Delete");
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 执行撤销命令。
        /// Execute the undo command.
        /// </summary>
        public void Undo()
        {
            if (htmldoc != null)
                htmldoc.ExecuteCommand("Undo", false, null);
        }

        /// <summary>
        /// 执行重做命令。
        /// Execute the redo command.
        /// </summary>
        public void Redo()
        {
            if (htmldoc != null)
                htmldoc.ExecuteCommand("Redo", false, null);
        }

        /// <summary>
        /// 执行剪切命令。
        /// Execute the cut command.
        /// </summary>
        public void Cut()
        {
            if (htmldoc != null)
                htmldoc.ExecuteCommand("Cut", false, null);
        }

        /// <summary>
        /// 执行复制命令。
        /// Execute the copy command.
        /// </summary>
        public void Copy()
        {
            if (htmldoc != null)
                htmldoc.ExecuteCommand("Copy", false, null);
        }
        /// <summary>
        /// 执行粘贴命令。
        /// Execute the paste command.
        /// </summary>
        public void Paste()
        {
            if (htmldoc != null)
                htmldoc.ExecuteCommand("Paste", false, null);
        }

        /// <summary>
        /// 执行删除命令。
        /// Execute the delete command.
        /// </summary>
        public void Delete()
        {
            if (htmldoc != null)
                htmldoc.ExecuteCommand("Delete", false, null);
        }

        /// <summary>
        /// 执行全选命令。
        /// Execute the select all command.
        /// </summary>
        public void SelectAll()
        {
            if (htmldoc != null)
                htmldoc.ExecuteCommand("SelectAll", false, null);
        }
        /// <summary>
        /// 显示 ContextMenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cmId"></param>
        private void ShowContextMenu(FrameworkElement fxElement, ContextMenu cmId)
        {
            if (fxElement != null && cmId != null)
            {
                cmId.PlacementTarget = fxElement;
                cmId.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                cmId.IsOpen = true;
            }
        }
        private void CloseContextMenuAll()
        {
            this.FaceImageContextMenu.IsOpen = false;
            this.FontColorContextMenu.IsOpen = false;
            this.LineColorContextMenu.IsOpen = false;
        }
        private void OnContextMenuClosed()
        {
            ToggleFaceImage.IsChecked = false;
            ToggleFontColor.IsChecked = false;
            ToggleLineColor.IsChecked = false;
        }
        private void InitContainer()
        {
            LoadStylesheet();
            VisualEditor.Navigated += this.OnVisualEditorDocumentNavigated;
            VisualEditor.StatusTextChanged += this.OnVisualEditorStatusTextChanged;
            VisualEditor.DocumentText = String.Empty;
        }
        /// <summary>
        /// Get the content from editor.
        /// </summary>
        private string GetEditContent()
        {
            switch (mode)
            {
                case EditMode.Visual:
                    return VisualEditor.Document.Body.InnerHtml;
                default:
                    return this.TxtEditor.Text;
            }
        }

        #endregion

        #region 命令

        private void UndoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Undo();
        }

        private void UndoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanUndo;
        }

        private void RedoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Redo();
        }

        private void RedoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanRedo;
        }

        private void CutExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Cut();
        }

        private void CutCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanCut;
        }

        private void CopyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Copy();
        }

        private void CopyCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanCopy;
        }

        private void PasteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Paste();
        }

        private void PasteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanPaste;
        }

        private void DeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Delete();
        }

        private void DeleteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanDelete;
        }

        private void SelectAllExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SelectAll();
        }

        private void BoldExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.Bold();
        }

        private void ItalicExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.Italic();
        }
        private void UnderlineExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.Underline();
        }

        private void SubscriptExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.Subscript();
        }

        private void SubscriptCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (mode == EditMode.Visual && htmldoc != null && htmldoc.CanSubscript());
        }

        private void SuperscriptExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.Superscript();
        }

        private void SuperscriptCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (mode == EditMode.Visual && htmldoc != null && htmldoc.CanSuperscript());
        }

        private void ClearStyleExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.ClearStyle();
        }

        private void IndentExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.Indent();
        }

        private void OutdentExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.Outdent();
        }

        private void BubbledListExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.BulletsList();
        }

        private void NumericListExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.NumberedList();
        }

        private void JustifyLeftExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.JustifyLeft();
        }
        private void PrintExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.Print();
        }
        private void JustifyRightExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.JustifyRight();
        }

        private void JustifyCenterExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.JustifyCenter();
        }

        private void JustifyFullExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.JustifyFull();
        }

        private void EditCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (htmldoc != null && mode == EditMode.Visual);
        }

        /// <summary>
        /// 插入超链接事件
        /// </summary>
        private void InsertHyperlinkExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null)
            {
                HyperlinkDialog d = new HyperlinkDialog();
                d.Owner = this.hostedWindow;
                d.Model = new HyperlinkObject { URL = "http://", Text = htmldoc.Selection.Text };
                if (d.ShowDialog() == true)
                {
                    htmldoc.InsertHyperlick(d.Model);
                }
            }
        }

        /// <summary>
        /// 删除线事件
        /// </summary>
        public void StrikethroughExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null)
            {
                string txt = htmldoc.Selection.Text;
                htmldoc.InsertText("<s>" + txt + "</s>");
            }
        }

        /// <summary>
        /// 插入图像事件
        /// </summary>
        private void InsertImageExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null)
            {
                ImageDialog d = new ImageDialog();
                d.Owner = this.hostedWindow;
                if (d.ShowDialog() == true)
                {
                    htmldoc.InsertImage(d.Model);
                    imgDictionary[d.Model.ImageUrl] = d.Model;
                }
            }
        }
        /// <summary>
        /// 插入附件事件
        /// </summary>
        //private void InsertAttachmentExecuted(object sender, ExecutedRoutedEventArgs e)
        //{
        //    if (htmldoc != null)
        //    {
        //        AttachDialog d = new AttachDialog();
        //        d.Owner = this.hostedWindow;
        //        if (d.ShowDialog() == true)
        //        {
        //            //htmldoc.InsertImage(d.Model);
        //            //imageDic[d.Model.ImageUrl] = d.Model;
        //        }
        //    }
        //}
        /// <summary>
        /// 插入表格事件
        /// </summary>
        private void InsertTableExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null)
            {
                TableDialog d = new TableDialog();
                d.Owner = this.hostedWindow;
                if (d.ShowDialog() == true)
                {
                    htmldoc.InsertTable(d.Model);
                }
            }
        }
        /// <summary>插入横线</summary>
        public void InsertHorizontalLineExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null)
                htmldoc.InsertText("<hr style='page -break-after:always;' />");
        }
        /// <summary>导出为文本文件</summary>
        public void ExportTxtExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (ContentText == null || ContentText.Length == 0)
            {
                MessageBox.Show("没有文本！", "警告", MessageBoxButton.OK);
                return;
            }

            System.Windows.Forms.SaveFileDialog sd = new System.Windows.Forms.SaveFileDialog();
            sd.Filter = "(*.txt)|*.txt|" + "(*.*)|*.*";
            sd.FileName = "文件" + DateTime.Now.ToString("yyyyMMddHHmm") + ".txt";

            if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(sd.FileName, true); //写文件

                sw.Write(ContentText);
                sw.Close();
            }
        }

        private void InsertCodeBlockExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }

        #endregion
    }

    internal class ImageObject
    {
    }
}
