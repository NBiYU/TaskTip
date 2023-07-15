using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Windows.Input;
using TaskTip.Models;
using TaskTip.UserControls;
using TaskTip.ViewModels;

namespace TaskTip.Pages
{
    /// <summary>
    /// RecordPage.xaml 的交互逻辑
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
