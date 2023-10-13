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
    /// EditRichTextBoxUserControl.xaml �Ľ����߼�
    /// </summary>
    public partial class EditRichTextBoxUserControl : UserControl
    {
        public EditRichTextBoxUserControl()
        {
            InitializeComponent();
        }

        private void RichTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // ����ճ��������Ctrl+V��
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.V)
            {

                // ��ȡViewModelʵ��
                var viewModel = (EditViewModel)RichTextGrid.DataContext;

                // ����ViewModel�еķ���������ճ��ͼƬ����
                viewModel.PasteImage();

                // ��ֹ�¼������������Ա��⽫�ı���V������RichTextBox��
                e.Handled = true;
            }
        }
    }
}
