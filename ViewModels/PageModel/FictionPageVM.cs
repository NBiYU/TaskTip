using System;
using TaskTip.Views.FictionPage;
        {
            get => _pageWidth;
            set
            {
                SetProperty(ref _pageWidth, value);
                TabItemWidth = value / 3 - 1;//��һ��ֹ���
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
        //        MessageBox.Show($"δ�ҵ�{pageName}����");
        //        return;
        //    }

        //    SelectPage = pages[index];

        //}


        public FictionPageVM()