using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TaskTip.ViewModels;

namespace TaskTip.Pages
{
    /// <summary>
    /// RecordPage.xaml �Ľ����߼�
    /// </summary>
    public partial class RecordPage : Page
    {
        public RecordPage()
        {
            MenuItemUserControlModel.isLoad = false;
            InitializeComponent();

        }


        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = (TextBox)sender;
                textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }

            var viewModel = (RecordPageModel)RichTextGrid.DataContext;

            viewModel.OnKey(sender, e);
        }

        private void RichText_KeyDown(object sender, KeyEventArgs e)
        {
            var viewModel = (RecordPageModel)RichTextGrid.DataContext;

            viewModel.OnKey(sender, e);
        }
    }
}
