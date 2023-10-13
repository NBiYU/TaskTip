using Quartz.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace TaskTip.Common
{
    public sealed class HotKeyManager:IDisposable
    {
        private bool disposed = false;

        private const int WM_HOTKEY = 0x0312;

        // 定义Windows API函数
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // 定义全局快捷键的标识符
        private int hotkeyId;
        private IntPtr hwnd;


        public HotKeyManager(Window window, Key key, ModifierKeys modifiers=ModifierKeys.None, EventHandler action=null)
        {
            hwnd = new WindowInteropHelper(window).Handle;
            hotkeyId = GetHashCode();
            RegisterHotKey(hwnd, hotkeyId, (uint)modifiers, (uint)KeyInterop.VirtualKeyFromKey(key));
            if (action != null)
            {
                HotKeyPressed += action;
            }
            ComponentDispatcher.ThreadPreprocessMessage += HotKeyHandler;
        }

        public event EventHandler HotKeyPressed;

        private void HotKeyHandler(ref MSG msg, ref bool handled)
        {
            if (msg.message == WM_HOTKEY && (int)msg.wParam == hotkeyId)
            {
                HotKeyPressed?.Invoke(this, EventArgs.Empty);
                handled = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // 释放托管资源
                }
                
                // 取消注册全局快捷键
                UnregisterHotKey(hwnd, hotkeyId);
                ComponentDispatcher.ThreadPreprocessMessage -= HotKeyHandler;

                disposed = true;
            }
        }
    }
}
