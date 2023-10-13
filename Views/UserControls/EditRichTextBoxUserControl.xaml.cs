using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskTip.ViewModels.UserViewModel;

namespace TaskTip.Views.UserControls
{
    /// <summary>
    /// EditRichTextBoxUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class EditRichTextBoxUserControl : UserControl
    {
        public EditRichTextBoxUserControl()
        {
            InitializeComponent();
        }

        private void RichTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // 处理粘贴操作（Ctrl+V）
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.V)
            {

                // 获取ViewModel实例
                var viewModel = (EditViewModel)RichTextGrid.DataContext;

                // 调用ViewModel中的方法来处理粘贴图片操作
                viewModel.PasteImage();

                // 阻止事件继续传播，以避免将文本“V”插入RichTextBox中
                e.Handled = true;
            }
        }
    }
}
