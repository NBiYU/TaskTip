﻿using System;
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

namespace TaskTip.Views
{
    /// <summary>
    /// FloatingTitleView.xaml 的交互逻辑
    /// </summary>
    public  class FloatingTitleView : Window
    {
        public bool IsClosed { get; set; } = false;
        //public FloatingTitleView()
        //{
        //    InitializeComponent();
        //}
        private void FloatingTitleView_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsClosed = true;
        }

    }
}
