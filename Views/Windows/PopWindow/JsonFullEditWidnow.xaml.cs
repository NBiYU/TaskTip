using Newtonsoft.Json;
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
using System.Windows.Shapes;
using TaskTip.ViewModels.ToolPageVM;

namespace TaskTip.Views.Windows.PopWindow
{
    /// <summary>
    /// JsonFullEditWidnow.xaml 的交互逻辑
    /// </summary>
    public partial class JsonFullEditWidnow : Window
    {
        public JsonFullEditWidnow()
        {
            InitializeComponent();
        }

        public JsonFullEditWidnow(JsonVM vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox control)
            {
                if (control.Text.Contains('\t'))
                {
                    int caretIndex = control.CaretIndex;
                    control.Text = control.Text.Replace("\t", "  ");
                    control.CaretIndex = e.Changes.First().Offset + 2;
                }
                if(control.DataContext is JsonVM vm)
                {
                    int caretIndex = control.CaretIndex;
                    vm.JsonStringToEntity().Wait();
                    control.CaretIndex = caretIndex;

                }
            }
        }
    }
}
