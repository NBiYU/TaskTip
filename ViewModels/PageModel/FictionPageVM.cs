using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskTip.Services;
using TaskTip.Views.FictionPage;
using MessageBox = HandyControl.Controls.MessageBox;

namespace TaskTip.ViewModels.PageModel
{
    internal partial class FictionPageVM:ObservableObject
    {

        private double _pageWidth;
        public double PageWidth
        {
            get => _pageWidth;
            set
            {
                SetProperty(ref _pageWidth, value);
                TabItemWidth = value / 3 - 1;//减一防止溢出
            }
        }

        private double _tabItemWidth;
        public double TabItemWidth
        {
            get => _tabItemWidth;
            set => SetProperty(ref _tabItemWidth, value);
        }


        //private List<Page> pages;

        //private Page _selectPage;

        //public Page SelectPage
        //{
        //    get=>_selectPage;
        //    set=>SetProperty(ref _selectPage,value);
        //}

        //[RelayCommand]
        //public void SelectPageChanged(string pageName)
        //{
        //    var index = pageName switch
        //    {
        //        "MyFictions" => 0,
        //        "FictionCategory" => 1,
        //        "FictionSearch" => 2,
        //        "FictionAccount" => 3,
        //        _ => 99
        //    };

        //    if (index == 99)
        //    {
        //        MessageBox.Show($"未找到{pageName}界面");
        //        return;
        //    }

        //    SelectPage = pages[index];

        //}


        public FictionPageVM()
        {
            PageWidth = GlobalVariable.TaskMenoView.Width;
            //pages = new List<Page>()
            //{
            //    new MyFictionsPage(),
            //    new FictionCategoryPage(),
            //    new FictionSearchPage(),
            //    new FictionAccountPage()
            //};
            //SelectPageChanged("MyFictions");
        }
    }
}
