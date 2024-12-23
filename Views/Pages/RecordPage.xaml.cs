using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TaskTip.ViewModels;

namespace TaskTip.Pages
{
    /// <summary>
    /// RecordPage.xaml µÄ½»»¥Âß¼­
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
        }

    }
}
