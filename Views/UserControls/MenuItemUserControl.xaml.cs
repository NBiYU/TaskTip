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
using CommunityToolkit.Mvvm.Messaging;
using TaskTip.Services;
using TaskTip.ViewModels;

namespace TaskTip.UserControls
{
    /// <summary>
    /// MenuItemUserControl.xaml µÄ½»»¥Âß¼­
    /// </summary>
    public partial class MenuItemUserControl : UserControl
    {

        public MenuItemUserControl()
        {
            InitializeComponent();
        }

        private void Title_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = (TextBox)sender;
                textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }

        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
