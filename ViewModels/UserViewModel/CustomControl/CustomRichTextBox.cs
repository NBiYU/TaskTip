using System.Windows;using System.Windows.Controls;using System.Windows.Documents;namespace TaskTip.ViewModels.UserViewModel.CustomControl{    public static class CustomRichTextBox    {
        #region -----Document------        public static readonly DependencyProperty BindableDocumentProperty =
           DependencyProperty.RegisterAttached("BindableDocument", typeof(FlowDocument), typeof(CustomRichTextBox), new PropertyMetadata(null, OnBindableDocumentChanged));        public static FlowDocument GetBindableDocument(DependencyObject obj)        {            return (FlowDocument)obj.GetValue(BindableDocumentProperty);        }        public static void SetBindableDocument(DependencyObject obj, FlowDocument value)        {            obj.SetValue(BindableDocumentProperty, value);        }        private static void OnBindableDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)        {            var richTextBox = (RichTextBox)d;            richTextBox.Document = (FlowDocument)e.NewValue;        }





        #endregion
        #region -----CaretPosition-----                                                public static readonly DependencyProperty BindableCaretPositionProperty =
DependencyProperty.RegisterAttached("BindableCaretPosition", typeof(TextPointer), typeof(CustomRichTextBox), new PropertyMetadata(null, OnBindableCaretPositionChanged));        public static void SetBindableCaretPosition(DependencyObject obj, TextPointer value)        {            obj.SetValue(BindableCaretPositionProperty, value);        }        public static TextPointer GetBindableCaretPosition(DependencyObject obj)        {            return (TextPointer)obj.GetValue(BindableCaretPositionProperty);        }        private static void OnBindableCaretPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)        {            var richTextBox = (RichTextBox)d;
            //richTextBox.Selection = (TextPointer)e.NewValue;
        }


        #endregion    }}