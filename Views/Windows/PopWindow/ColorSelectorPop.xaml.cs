using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TaskTip.Views.Windows.PopVM
{
    /// <summary>
    /// ColorSelectorPop.xaml µÄ½»»¥Âß¼­
    /// </summary>
    public partial class ColorSelectorPop : Window
    {
        public ColorSelectorPop()
        {
            InitializeComponent();

            this.Width = ColorPicker.Width;
            this.Height = ColorPicker.Height;

            ColorPicker.Confirmed += (o, c) =>
            {
                this.DialogResult = true;
            };
            ColorPicker.Canceled += (o, e) =>
            {
                this.DialogResult = false;
            };
        }

        private void ColorSelectorPop_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
