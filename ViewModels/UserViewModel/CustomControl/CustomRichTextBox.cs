using System.Windows;
        #region -----Document------
           DependencyProperty.RegisterAttached("BindableDocument", typeof(FlowDocument), typeof(CustomRichTextBox), new PropertyMetadata(null, OnBindableDocumentChanged));





        #endregion
        #region -----CaretPosition-----
DependencyProperty.RegisterAttached("BindableCaretPosition", typeof(TextPointer), typeof(CustomRichTextBox), new PropertyMetadata(null, OnBindableCaretPositionChanged));
            //richTextBox.Selection = (TextPointer)e.NewValue;
        }


        #endregion