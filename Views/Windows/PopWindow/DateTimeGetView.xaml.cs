using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;using System.Windows;using System.Windows.Controls;using System.Windows.Data;using System.Windows.Documents;using System.Windows.Input;using System.Windows.Media;using System.Windows.Media.Imaging;using System.Windows.Shapes;namespace TaskTip.Views{    /// <summary>    /// DateTimeGetView.xaml �Ľ����߼�    /// </summary>    public partial class DateTimeGetView : Window    {        public static bool IsClosed { get; set; } = true;        public DateTimeGetView()        {            InitializeComponent();        }        protected override void OnClosed(EventArgs e)        {            base.OnClosed(e);            IsClosed = true;        }        public void HideCancekPlan()
        {
            this.NoneTime.Visibility = Visibility.Collapsed;
        }
        private void Close_OnClick(object sender, RoutedEventArgs e)        {            Close();        }    }}