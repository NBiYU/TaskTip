using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using Button = System.Windows.Controls.Button;

namespace TaskTip.ViewModels.PageModel
{
    internal class NoteViewModel : ObservableObject
    {

        #region 属性

        public ObservableCollection<Button> NoteTitleList { get; set; }

        private double editViewerHeight;
        public double EditViewerHeight
        {
            get => editViewerHeight;
            set => SetProperty(ref editViewerHeight, value);
        }
        private double effectViewerHeight;
        public double EffectViewerHeight
        {
            get => effectViewerHeight;
            set => SetProperty(ref effectViewerHeight, value);
        }

        private FlowDocument effectFlowDocument;
        public FlowDocument EffectFlowDocument
        {
            get => effectFlowDocument;
            set => SetProperty(ref effectFlowDocument, value);
        }

        private string editText;
        public string EditText
        {
            get => editText;
            set
            {
                SetProperty(ref editText, value);
                EffectFlowDocument = EffectFlowDocumentChanged(value);
            }
        }

        private string effectsource;
        public string EffectSource
        {
            get => effectsource;
            set => SetProperty(ref effectsource, value);
        }

        #endregion

        #region  控件指令

        public RelayCommand AddNoteCommand { get; set; }
        public RelayCommand TopButtonCommand { get; set; }
        public RelayCommand BottomCommand { get; set; }
        public RelayCommand RestoreCommand { get; set; }

        #endregion

        #region 控件指令处理函数

        private void AddNoteHandle()
        {
            NoteTitleList.Add(AddNoteItem(Guid.NewGuid().ToString()));
        }

        private void TopButtonHandle()
        {
            EditViewerHeight -= 20;
            EffectViewerHeight += 20;
        }

        private void BottomButtonHandle()
        {
            EditViewerHeight += 20;
            EffectViewerHeight -= 20;
        }

        private void RestoreButtonHandle()
        {
            EditViewerHeight = 280;
            EffectViewerHeight = 280;
        }

        #endregion

        #region 公共处理函数

        private FlowDocument EffectFlowDocumentChanged(string text)
        {
            FlowDocument document = new FlowDocument();



            return document;
        }



        #endregion

        /// <summary>
        /// 单击预览
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotedSelect(object sender, RoutedEventArgs e)
        {
            Button currentButton = (Button)sender;
            //MessageBox.Show("单击");
            Console.Write("hsakd");
        }

        /// <summary>
        /// 双击编辑
        /// </summary>
        private void NotedDoubleClick(object sender, RoutedEventArgs e)
        {

        }

        private Button AddNoteItem(string title)
        {
            Button noteButton = new Button();
            noteButton.Uid = Guid.NewGuid().ToString();
            noteButton.BorderThickness = new Thickness(0);
            noteButton.Content = title;
            noteButton.Click += NotedSelect;//单击预览
            noteButton.MouseDoubleClick += NotedDoubleClick;//双击编辑

            return noteButton;
        }

        #region 功能函数


        #endregion

        public NoteViewModel()
        {
            NoteTitleList = new ObservableCollection<Button>();
            EffectFlowDocument = new FlowDocument();

            EditViewerHeight = 280;
            EffectViewerHeight = 280;

            AddNoteCommand = new RelayCommand(AddNoteHandle);
            TopButtonCommand = new RelayCommand(TopButtonHandle);
            BottomCommand = new RelayCommand(BottomButtonHandle);
            RestoreCommand = new RelayCommand(RestoreButtonHandle);
        }
    }
}
