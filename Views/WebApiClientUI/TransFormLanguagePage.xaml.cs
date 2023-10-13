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

namespace TaskTip.Pages.WebApiClientUI
{
    /// <summary>
    /// TransFormLanguagePage.xaml µÄ½»»¥Âß¼­
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
