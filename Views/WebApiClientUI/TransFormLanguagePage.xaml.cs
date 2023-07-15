using System.Windows.Controls;
using System.Windows.Input;

namespace TaskTip.Pages.WebApiClientUI
{
    /// <summary>
    /// TransFormLanguagePage.xaml 的交互逻辑
    /// </summary>
    public partial class TransFormLanguagePage : Page
    {
        public TransFormLanguagePage()
        {
            InitializeComponent();
        }

        private void Input_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is TextBox control)
                {
                    control.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
            }
        }
    }
}
