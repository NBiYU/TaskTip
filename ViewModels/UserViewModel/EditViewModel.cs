using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Windows.Controls.Image;
using System.ComponentModel;

namespace TaskTip.ViewModels.UserViewModel
{
    internal partial class EditViewModel : ObservableObject
    {
        #region 属性

        private TextSelection _currentSelection;

        public TextSelection CurrentSelection
        {
            get => _currentSelection;
            set => SetProperty(ref _currentSelection, value);
        }

        private FlowDocument _richContent;

        public FlowDocument RichContent
        {
            get => _richContent;
            set => SetProperty(ref _richContent, value);
        }

        private TextPointer _currentPointer;

        public TextPointer CurrentPointer
        {
            get => _currentPointer;
            set => SetProperty(ref _currentPointer, value);
        }

        #endregion

        [RelayCommand]
        public void CurrentSelectionChanged(object sender)
        {
            if (sender is RichTextBox box)
            {
                CurrentSelection = box.Selection;
            }
        }

        [RelayCommand]
        public void PasteImage()
        {
            var clipboardData = Clipboard.GetDataObject();


            if (clipboardData == null) return;

            if (clipboardData.GetDataPresent(DataFormats.Bitmap))
            {
                BitmapSource bitmapSource = clipboardData.GetData(DataFormats.Bitmap) as BitmapSource;
                if (bitmapSource != null && bitmapSource.Format == PixelFormats.Bgra32)
                {
                    // 创建一个新的InlineUIContainer，并将图像添加到其中
                    InlineUIContainer container = new InlineUIContainer(new Image { Source = bitmapSource });

                    // 将图像插入到RichTextDocument中
                    CurrentPointer.Paragraph?.Inlines.Add(container);
                }
            }
            else if (clipboardData.GetDataPresent(DataFormats.StringFormat))
            {
                string text = clipboardData.GetData(DataFormats.StringFormat) as string;
                if (text != null)
                {
                    CurrentPointer.Paragraph?.Inlines.Add(new Run(text));
                }
            }
        }

        public EditViewModel()
        {
            RichContent = new FlowDocument();
            CurrentPointer = RichContent.ContentEnd;
        }
    }
}
