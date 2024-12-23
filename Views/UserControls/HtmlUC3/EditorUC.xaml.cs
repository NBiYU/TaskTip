using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using TaskTip.Common.Extends;
using TaskTip.Models.DataModel;
using TaskTip.Views.Windows.PopWindow;


namespace TaskTip.Views.UserControls.HtmlUC3
{
    /// <summary>
    /// EditorUC.xaml 的交互逻辑
    /// </summary>
    public partial class EditorUC : System.Windows.Controls.UserControl
    {
        private bool _isUpdatingFromViewModel;
        private bool _isUpdatingFromEditor;
        private const string InitialHtml = @"<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <title>HTML Editor</title>
    <style>
        html, body {
            margin: 0;
            padding: 0;
            height: 100%;
            font-family: Arial, sans-serif;
        }

        #editor-container {
            height: 100%;
            display: flex;
            flex-direction: column;
        }

        #editor {
            flex: 1; /* 填满剩余空间 */
            overflow: auto; /* 确保内容溢出时可滚动 */
            border: 1px solid #ccc;
            padding: 10px;
            box-sizing: border-box;
        }
    </style>
</head>
<body>
    <div id='editor-container'>
        <div id='editor' contenteditable='true'></div>
    </div>
    <script>
        function setFontSizeStyle(size) {
            const selection = window.getSelection();
            if (selection.rangeCount > 0) {
                const range = selection.getRangeAt(0);
                let selectedElement = range.commonAncestorContainer;

                if (selectedElement.nodeType !== 1) {
                    selectedElement = selectedElement.parentElement;
                }
                selectedElement.style.fontSize = size;
            }
        }

        function insertHr() {
            const hr = document.createElement('hr');
            const selection = window.getSelection();
            if (selection.rangeCount > 0) {
                const range = selection.getRangeAt(0);
                range.insertNode(hr);
                range.setStartAfter(hr);
                range.collapse(true);
                selection.removeAllRanges();
                selection.addRange(range);
            } else {
                editor.appendChild(hr);
            }
        }
        function setOrRemoveLink() {
            const result = removeLink();
            if (!result) {
                setLink();
            }
        }
        function setLink() {
            const selection = window.getSelection();
            if (selection.rangeCount > 0) {
                const range = selection.getRangeAt(0);

                const anchor = document.createElement('a');
                anchor.href = selection.toString();;
                anchor.textContent = selection.toString();

                range.deleteContents();
                range.insertNode(anchor);

                range.setStartAfter(anchor);
                range.setEndAfter(anchor);
                selection.removeAllRanges();
            }
        }
        function removeLink() {
            const selection = window.getSelection();
            if (selection.rangeCount > 0) {
                const range = selection.getRangeAt(0);
                let parent = range.commonAncestorContainer;

                if (parent.nodeType === 3) {
                    parent = parent.parentNode;
                }

                if (parent.tagName === ""A"") {
                    const span = document.createElement(""span"");
                    span.textContent = parent.textContent;

                    parent.replaceWith(span);
                    console.log(""Link removed."");
                    return true;
                }
            }
            return false;
        }

        function sendMessage(type, data) {
            window.chrome.webview.postMessage({ type, data });
        }
        const editor = document.getElementById('editor');

        editor.addEventListener('input', () => {
            const content = editor.innerHTML;
            sendMessage('contentChanged', content);
        });
        editor.removeEventListener('click', handleClick);

        function handleClick(event) {
            const target = event.target;

            if (!editor.contains(target)) return;

            if (target.tagName === 'A') {
                event.preventDefault();
                const href = target.getAttribute('href');
                window.chrome.webview.postMessage({ type: 'linkClicked', data: href });
                return;
            }

            if (target.closest('a')) {
                event.preventDefault();
                const href = target.closest('a').getAttribute('href');
                window.chrome.webview.postMessage({ type: 'linkClicked', data: href });
            }
        }

        // 确保只绑定一次事件监听器
        editor.addEventListener('click', handleClick);
        document.addEventListener('selectionchange', () => {
            const selection = window.getSelection();
            let isLink = false;
            if (selection.rangeCount > 0) {
                
                const range = selection.getRangeAt(0);
                let selectedElement = range.commonAncestorContainer;
                // 获取选中文本的背景颜色
                if (selectedElement.nodeType !== 1) {
                    selectedElement = selectedElement.parentElement;
                }
                if (selectedElement.tagName === 'A' || (selectedElement.parentElement && selectedElement.parentElement.tagName === 'A')) {
                    isLink = true;
                }
                const computedStyle = window.getComputedStyle(selectedElement);
                    
                sendMessage('styleChanged', {
                    bold: document.queryCommandState('bold'),
                    italic: document.queryCommandState('italic'),
                    underline: document.queryCommandState('underline'),
                    strikeThrough: document.queryCommandState('strikeThrough'),
                    isLink: isLink,
                    foreColor: document.queryCommandValue('foreColor'), // 返回文本颜色
                    hiliteColor: computedStyle.backgroundColor, // 使用 getComputedStyle 获取背景颜色
                    textAlign: computedStyle.textAlign,
                    fontFamily: computedStyle.fontFamily,
                    fontSize:computedStyle.fontSize,
                });
            } else {
                sendMessage('styleChanged', {});
            }
        });

        window.chrome.webview.addEventListener('message', event => {
            const message = event.data;
            if (message.type === 'setContent') {
                editor.innerHTML = message.data;
            }
        });
    </script>
</body>
</html>";

        public EditorUC()
        {
            InitializeComponent();
            InitToolBar();
            InitWebView2();
        }

        private async void InitWebView2()
        {
            try
            {
                await WebView.EnsureCoreWebView2Async();
                WebView.CoreWebView2.NavigateToString(InitialHtml);

                // 启动内容同步
                WebView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebView initialization failed: {ex}");
            }
        }

        #region WebView2

        #region 接受WebView2的数据推送
        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var message = JsonConvert.DeserializeObject<WebMessage>(e.WebMessageAsJson);
                if (message != null)
                {
                    switch (message.type)
                    {
                        case "contentChanged":
                            UpdateContentFromEditor(message.data?.ToString() ?? string.Empty);
                            break;
                        case "styleChanged":
                            UpdateStyleInfoFromEditor(message.data);
                            break;
                        case "linkClicked":
                            LinkClicked((string)message.data);
                            break;
                        case "shortcut":
                            // Handle setContent if needed
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }
        private void UpdateContentFromEditor(string newContent)
        {
            if (_isUpdatingFromViewModel) return;

            if (EditorContent != newContent)
            {
                _isUpdatingFromEditor = true;
                EditorContent = newContent;
                _isUpdatingFromEditor = false;
            }
        }
        private void UpdateStyleInfoFromEditor(object styleData)
        {
            var styleInfo = JsonConvert.DeserializeObject<StyleInfoFromCSS>(styleData.ToString() ?? "");
            if (styleInfo == null) return;
            ToggleButtonSwitch(BoldButton,styleInfo.bold);
            ToggleButtonSwitch(ItalicButton, styleInfo.italic);
            ToggleButtonSwitch(UnderlineButton, styleInfo.underline);
            ToggleButtonSwitch(EditDelLineButton, styleInfo.strikeThrough);
            ToggleButtonSwitch(TextAlignleft, styleInfo.textAlign == "left");
            ToggleButtonSwitch(TextAligncenter, styleInfo.textAlign == "center");
            ToggleButtonSwitch(TextAlignright, styleInfo.textAlign == "right");
            ToggleButtonSwitch(LinkButton, styleInfo.isLink);

            FontSizeTextBox.Text = styleInfo.fontSize.Replace("px","");

            if (!styleInfo.foreColor.IsNullOrEmpty()) TextColorButton.Foreground = new SolidColorBrush(ColorFromString(styleInfo.foreColor));
            if (!styleInfo.hiliteColor.IsNullOrEmpty()) BackgroundColorButton.Background = new SolidColorBrush(ColorFromString(styleInfo.hiliteColor));
            if (!styleInfo.fontFamily.IsNullOrEmpty())
            {
                var fontFamily = styleInfo.fontFamily.Split(',')[0];
                FontComboBox.Text = fontFamily;
                FontComboBox.SelectedItem = fontFamily;
            }
            

        }
        private void LinkClicked(string url)
        {
            var webUrlRegex = new Regex("^(http?|https?|ftp):\\/\\/[^\\s/$.?#].[^\\s]*$");
            var windowsFileRegex = new Regex(@"^[a-zA-Z]:\\(?:[^<>:""/\\|?*\r\n]+\\)*[^<>:""/\\|?*\r\n]+\.[^<>:""/\\|?*\r\n]+$", RegexOptions.Compiled);
            var windowsFolderRegex = new Regex(@"^[a-zA-Z]:\\(?:[^<>:""/\\|?*\r\n]+\\)*[^<>:""/\\|?*\r\n]*\\?$", RegexOptions.Compiled);
            if (webUrlRegex.IsMatch(url)) OpenUrl(url);
            else if (windowsFileRegex.IsMatch(url)) OpenFile(url); 
            else if (windowsFolderRegex.IsMatch(url)) OpenFolder(url);
        }

        #endregion

        #region EditorContentProperty
        public static readonly DependencyProperty EditorContentProperty =
            DependencyProperty.Register(nameof(EditorContent), typeof(string), typeof(EditorUC),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnContentChanged));
        public string EditorContent
        {
            get => (string)GetValue(EditorContentProperty);
            set => SetValue(EditorContentProperty, value);
        }
        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as EditorUC;
            control?.UpdateEditorContent((string)e.NewValue);
        }
        private void UpdateEditorContent(string content)
        {
            if (WebView.CoreWebView2 == null) return;
            if (_isUpdatingFromEditor) return;
            _isUpdatingFromViewModel = true;

            var sanitizedContent = content?.Replace("'", "\\'").Replace(Environment.NewLine, "\\n") ?? string.Empty;
            WebView.ExecuteScriptAsync($"document.getElementById('editor').innerHTML = '{sanitizedContent}'")
                .ContinueWith(_ => _isUpdatingFromViewModel = false);
        }
        #endregion

        #region StyleInfoProperty
        public static readonly DependencyProperty StyleInfoProperty =
            DependencyProperty.Register(nameof(StyleInfo), typeof(string), typeof(EditorUC),
                new FrameworkPropertyMetadata(string.Empty));

        public string StyleInfo
        {
            get => (string)GetValue(StyleInfoProperty);
            set => SetValue(StyleInfoProperty, value);
        }

        #endregion

        #endregion

        #region ToolBar

        private void InitToolBar()
        {
            InstalledFontCollection fonts = new InstalledFontCollection();
            var fontFamilies = new List<string>();
            foreach (var font in fonts.Families)
            {
                fontFamilies.Add(font.Name);
            }
            FontComboBox.ItemsSource = fontFamilies;
        }

        #region 样式处理
        private async void InsertImageButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp",
                Title = "选择图片"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = new Uri(openFileDialog.FileName).AbsoluteUri; // 转换为 URI 格式
                await WebView.ExecuteScriptAsync($"document.execCommand('insertImage', false, '{filePath}')");
            }
        }
        private async void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            await WebView.ExecuteScriptAsync("document.execCommand('italic')");
        }
        private async void EditDelLineButton_Click(object sender, RoutedEventArgs e)
        {
            await WebView.ExecuteScriptAsync("document.execCommand('strikeThrough')");
        }
        private async void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            await WebView.ExecuteScriptAsync("document.execCommand('bold')");
        }
        // 下划线
        private async void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            await WebView.ExecuteScriptAsync("document.execCommand('underline')");
        }

        // 左对齐
        private async void AlignLeftButton_Click(object sender, RoutedEventArgs e)
        {
            await WebView.ExecuteScriptAsync("document.execCommand('justifyLeft')");
        }

        // 居中
        private async void AlignCenterButton_Click(object sender, RoutedEventArgs e)
        {
            await WebView.ExecuteScriptAsync("document.execCommand('justifyCenter')");
        }

        // 右对齐
        private async void AlignRightButton_Click(object sender, RoutedEventArgs e)
        {
            await WebView.ExecuteScriptAsync("document.execCommand('justifyRight')");
        }

        // 插入超链接
        private async void InsertLinkButton_Click(object sender, RoutedEventArgs e)
        {
            await WebView.ExecuteScriptAsync($"setOrRemoveLink()");
        }

        // 插入表格
        private async void InsertTableButton_Click(object sender, RoutedEventArgs e)
        {
            if(sender is System.Windows.Controls.Button button)
            {
                var model = new PopInteractiveModel
                {
                    Title = (string)button.ToolTip,
                    InteractiveItemModels =
                    [
                        new(){ Tip = "行数",ControlType=ControlTypeEnum.Input,Data = 3},
                        new(){ Tip = "列数",ControlType=ControlTypeEnum.Input,Data = 3},
                        new(){ Tip = "宽度",ControlType=ControlTypeEnum.Input,Data = 340},
                        new(){ Tip = "高度",ControlType=ControlTypeEnum.Input,Data = 100},
                        new(){ Tip = "间隔",ControlType=ControlTypeEnum.Input,Data = 0},
                        new(){ Tip = "边距",ControlType=ControlTypeEnum.Input,Data = 0},
                        new(){ Tip = "边框",ControlType=ControlTypeEnum.Input,Data = 1},
                        new(){ Tip = "标题",ControlType=ControlTypeEnum.Input,Data = "Test"},
                        new(){ Tip = "表头位置",ControlType=ControlTypeEnum.Dropdown,Data=new List<string>{ 
                            "无","首行","首列"} },
                        new(){ Tip = "对齐方式",ControlType=ControlTypeEnum.Dropdown,Data = new List<string>{
                                "左对齐","居中","右对齐"
                            }
                        }
                    ]
                };
                var dialog = new PopInteractiveView(model);
                var result = dialog.ShowDialog();
                if (result == true)
                {
                    var tableHtml = GetTableHtml(new TableStyleObject(
                        Convert.ToInt32(model.InteractiveItemModels[0].Data),
                        Convert.ToInt32(model.InteractiveItemModels[1].Data),
                        Convert.ToInt32(model.InteractiveItemModels[2].Data),
                        Convert.ToInt32(model.InteractiveItemModels[3].Data),
                        Convert.ToInt32(model.InteractiveItemModels[4].Data),
                        Convert.ToInt32(model.InteractiveItemModels[5].Data),
                        Convert.ToInt32(model.InteractiveItemModels[6].Data),
                        (string)model.InteractiveItemModels[7].Data,
                        (string)model.InteractiveItemModels[8].SelectData 
                            switch { 
                                "首行"=> TitleHeaderPostionEnum.FirstRow,
                                "首列" => TitleHeaderPostionEnum.FirstCol, 
                                _ => TitleHeaderPostionEnum.None
                            },
                        (string)model.InteractiveItemModels[9].SelectData
                            switch { 
                                "左对齐" => TextAlignEnum.Left, 
                                "居中" => TextAlignEnum.Center , 
                                "右对齐" => TextAlignEnum.Right,
                                _=> TextAlignEnum.Left 
                            }
                    ));
                    //string tableHtml = "<table border='1' style='width: 100%;'><tr><th>Header 1</th><th>Header 2</th></tr><tr><td>Row 1</td><td>Row 2</td></tr></table>";
                    await WebView.ExecuteScriptAsync($"document.execCommand('insertHTML', false, `{tableHtml}`)");
                }

            }

        }

        // 动态选择文本颜色
        private async void TextColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedColor = $"#{colorDialog.Color.R:X2}{colorDialog.Color.G:X2}{colorDialog.Color.B:X2}";
                await WebView.ExecuteScriptAsync($"document.execCommand('foreColor', false, '{selectedColor}')");
            }
        }

        // 动态选择背景颜色
        private async void BackgroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedColor = $"#{colorDialog.Color.R:X2}{colorDialog.Color.G:X2}{colorDialog.Color.B:X2}";
                await WebView.ExecuteScriptAsync($"document.execCommand('hiliteColor', false, '{selectedColor}')");
            }
        }

        private async void EditIndentLeft_Click(object sender, RoutedEventArgs e)
        {
            await WebView.ExecuteScriptAsync("document.execCommand('indent')");
        }

        private async void EditIndentRight_Click(object sender, RoutedEventArgs e)
        {
            await WebView.ExecuteScriptAsync("document.execCommand('outdent')");
        }
        private async void InsertLineButton_Click(object sender, RoutedEventArgs e)
        {
            await WebView.ExecuteScriptAsync("insertHr()");
        }

        private async void FontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.ComboBox control && control.SelectedItem != null)
            {
                await WebView.ExecuteScriptAsync($"document.execCommand('fontName', false, '{control.SelectedItem}')");
            }
        }
        private async void FontSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox control)
            {
                if (int.TryParse(control.Text,out _)){
                    await WebView.ExecuteScriptAsync($"setFontSizeStyle('{control.Text}px')");
                }

            }

        }
        private static Color ColorFromString(string rgbStr)
        {
            Regex regex = new Regex(@"(?<=\().*?(?=\))");
            var match = regex.Match(rgbStr);
            string[] colorValues = match.Value.Split(',');

            // 将字符串转换为整数并创建 Color 对象
            if (colorValues.Length > 3)
            {
                byte r = byte.Parse(colorValues[0].Trim());
                byte g = byte.Parse(colorValues[1].Trim());
                byte b = byte.Parse(colorValues[2].Trim());
                byte a = byte.Parse(colorValues[3].Trim());
                return Color.FromArgb(a, r, g, b);
            }
            else
            {
                byte r = byte.Parse(colorValues[0].Trim());
                byte g = byte.Parse(colorValues[1].Trim());
                byte b = byte.Parse(colorValues[2].Trim());
                return Color.FromRgb(r, g, b);
            }

        }

        private static string GetTableHtml(TableStyleObject tableStyle)
        {
            var sb = new StringBuilder();

            // 表格样式
            sb.Append($"<table style='border-collapse: collapse; width: {tableStyle.Width}px; height: {tableStyle.Height}px; margin: {tableStyle.Margins}px; border: {tableStyle.Frame}px solid black;'>");

            // 判断是否需要生成标题行
            if (!string.IsNullOrEmpty(tableStyle.Title))
            {
                sb.Append("<thead>");
                sb.Append("<tr>");
                sb.AppendFormat(
                    "<th colspan='{0}' style='border: {1}px solid black; text-align: center;'>{2}</th>",
                    tableStyle.Cols,
                    tableStyle.Frame,
                    tableStyle.Title);
                sb.Append("</tr>");
                sb.Append("</thead>");
            }

            sb.Append("<tbody>");
            var heightUnit = tableStyle.Height / tableStyle.Rows;
            var widthUnit = tableStyle.Width / tableStyle.Cols;
            // 生成表格内容
            for (int i = 0; i < tableStyle.Rows; i++)
            {
                sb.Append("<tr>");
                for (int j = 0; j < tableStyle.Cols; j++)
                {
                    // 判断是否需要生成标题列
                    if (tableStyle.TitlePostionEnum == TitleHeaderPostionEnum.FirstCol && j == 0)
                    {
                        sb.AppendFormat("<th style='border: {0}px solid black;vertical-align: middle; text-align: {1};width: {2}px; height: {3}px'>{2}</th>",
                            tableStyle.Frame,
                            tableStyle.TextAlign.ToString().ToLower(),
                            widthUnit,heightUnit);
                    }
                    else
                    {
                        sb.AppendFormat("<td style='border: {0}px solid black;vertical-align: middle; text-align: {1};width: {2}px; height: {3}px'></td>",
                            tableStyle.Frame,
                            tableStyle.TextAlign.ToString().ToLower(),
                            widthUnit, heightUnit);
                    }
                }
                sb.Append("</tr>");
            }

            sb.Append("</tbody>");
            sb.Append("</table>");

            return sb.ToString();
        }
        #endregion

        #region 样式选中处理

        private void ToggleButtonSwitch(System.Windows.Controls.Button control,bool isSelect)
        {
            control.BorderThickness = new Thickness(isSelect ? 1 : 0);
        }

        #endregion

        #endregion

        #region 链接点击处理

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"无法打开网址：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void OpenFolder(string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    System.Windows.MessageBox.Show($" {folderPath} 文件夹不存在");
                    return;
                }
                Process.Start("explorer.exe", folderPath);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"无法打开文件夹：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenFile(string filePath)
        {
            try
            {
                // 使用 /select 参数打开文件所在的文件夹并选中文件
                if (!File.Exists(filePath))
                {
                    System.Windows.MessageBox.Show($" {filePath} 文件不存在");
                    return;
                }
                Process.Start("explorer.exe", $"/select,\"{filePath}\"");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"无法打开文件：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            // 确保事件来源是 ScrollViewer
            if (sender is ScrollViewer scrollViewer && !FontComboBox.IsDropDownOpen)
            {
                // 改变 ScrollViewer 的水平偏移量
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - e.Delta);

                // 标记事件为已处理，防止默认的垂直滚动行为
                e.Handled = true;
            }
        }

        private record WebMessage(string type, object data);
        private record StyleInfoFromCSS (bool bold,bool italic,bool underline,bool strikeThrough,bool isLink,string foreColor,string hiliteColor,string textAlign,string fontFamily,string fontSize);
        public enum TitleHeaderPostionEnum
        {
            None,
            FirstRow,
            FirstCol,
        }
        public enum TextAlignEnum
        {
            Left,
            Center,
            Right
        }
        private record TableStyleObject(int Rows, int Cols, int Width, int Height, int Gap,int Margins,int Frame, string Title, TitleHeaderPostionEnum TitlePostionEnum, TextAlignEnum TextAlign);
    }
}

