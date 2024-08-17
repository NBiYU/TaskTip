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

namespace TaskTip.Views.ToolPage
{
    /// <summary>
    /// JsonPage.xaml 的交互逻辑
    /// </summary>
    public partial class JsonPage : Page
    {
        public JsonPage()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(sender is TextBox control)
            {
                if (control.Text.Contains('\t'))
                {
                    int caretIndex = control.CaretIndex;
                    control.Text = control.Text.Replace("\t", "    ");
                    control.CaretIndex = e.Changes.First().Offset + 4;
                }
            }

        }
    }
}
