using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TaskTip.Views.UserControls
{
    /// <summary>
    /// KeyInputUC.xaml µÄ½»»¥Âß¼­
    /// </summary>
    public partial class KeyInputUC : UserControl
    {
        public static readonly DependencyProperty InputKeysProperty = DependencyProperty.Register(nameof(InputKeys), typeof(int[]),typeof(KeyInputUC),new PropertyMetadata(new int[2]));

        public int[] InputKeys
        {
            get => (int[])GetValue(InputKeysProperty);
            set
            {
                var combineKey = string.Empty;
                if (value[0] != 0) combineKey = ((ModifierKeys)value[0]).ToString() + " + ";
                if (value[1] != 0) combineKey += ((Key)value[1]).ToString();
                CombineKey.Text = combineKey;
                SetValue(InputKeysProperty, value);
                InputKeysChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public delegate void EventHandler(object sender,EventArgs e);
        public event EventHandler InputKeysChanged;

        public KeyInputUC(string hotKeyName)
        {
            InitializeComponent();
            HotKeyName.Text = hotKeyName;
        }

        public KeyInputUC()
        {
            InitializeComponent();
        }

        private void InputKeys_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var keys = new int[] { 0, 0 };
            if (Keyboard.Modifiers != ModifierKeys.None)
            {
                keys[0] = (int)Keyboard.Modifiers;
            } 

            if (e.Key != Key.None)
            {
                keys[1] = (int)e.Key;
            }

            InputKeys = keys;
            e.Handled = true;
        }
    }
}
