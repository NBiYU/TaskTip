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

using TaskTip.Models.Entities;

namespace TaskTip.Pages
{
    /// <summary>
    /// TaskListPage.xaml µÄ½»»¥Âß¼­
    /// </summary>
    public partial class TaskListPage : Page
    {
        public TaskListPage()
        {
            InitializeComponent();
        }

        private void ListBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if(sender is ListBox box && e.OriginalSource is Control originalControl && originalControl.DataContext is TaskFileModel originalModel)
            {
                foreach(var item in box.Items)
                {
                    var model = (TaskFileModel)item;
                    if(model.GUID!=originalModel.GUID && model.IsFocus)
                    {
                        model.IsFocus = false;
                    }else if(model.GUID == originalModel.GUID)
                    {
                        model.IsFocus = true;
                    }
                }
            }
        }
    }
}
