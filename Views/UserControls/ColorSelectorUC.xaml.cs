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
using HandyControl.Data;

namespace TaskTip.Views.UserControls
{
    /// <summary>
    /// ColorSelectorUC.xaml µÄ½»»¥Âß¼­
    /// </summary>
    public partial class ColorSelectorUC : UserControl
    {
        public ColorSelectorUC()
        {
            InitializeComponent();

            ColorSelectPicker.Confirmed += (o, e) => { ColorSelectPickerPopup.IsOpen = false; };
            ColorSelectPicker.Canceled += (o, e) => { ColorSelectPickerPopup.IsOpen = false; };
        }

        private void ColorSelectPicker_OnSelectedColorChanged(object? sender, FunctionEventArgs<Color> e)
        {
            ColorSelectorButton.Background = new SolidColorBrush(e.Info);
        }

        private void ColorSelectPicker_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
