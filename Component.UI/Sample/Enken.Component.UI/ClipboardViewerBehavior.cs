using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Interop;

namespace Enken.Component.UI
{
    class ClipboardViewerBehavior : Behavior<Window>, IDisposable
    {
        #region Win32 API
        static class NativeMethods
        {
            [DllImport("user32")]
            public static extern IntPtr SetClipboardViewer(IntPtr hWnd);

            [DllImport("user32")]
            public static extern bool ChangeClipboardChain(IntPtr hWnd, IntPtr hWndNext);

            [DllImport("user32")]
            public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        }
        const int WM_DRAWCLIPBOARD = 0x0308;
        const int WM_CHANGECBCHAIN = 0x030D;
        #endregion

        /// <summary>
        /// 拡張したWindowのハンドル
        /// </summary>
        IntPtr hWnd;

        /// <summary>
        /// クリップボードチェーンの次のハンドル
        /// </summary>
        IntPtr nexthWnd;

        #region === Dispose Pattern ===
        /// <summary>
        /// リソースが解放されているかどうかを示します。
        /// </summary>
        bool disposed;

        /// <summary>
        /// ClipboardViewerBehavior で使用されるすべてのリソースを解放します。
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// ClipboardViewerBehavior によって使用されているアンマネージ リソースを解放し、オプションでマネージ リソースも解放します。
        /// </summary>
        /// <param name="disposing">
        /// マネージ リソースとアンマネージ リソースの両方を解放する場合は true。
        /// アンマネージ リソースだけを解放する場合は false。
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            disposed = true;

            if (disposing)
            {
                //Release managed resources
            }

            //Release unmanged resources
            hWnd = IntPtr.Zero;
            nexthWnd = IntPtr.Zero;

        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~ClipboardViewerBehavior()
        {
            Dispose(false);
        }
        #endregion

        #region 添付プロパティ Text
        public static string GetText(DependencyObject obj)
        {
            return (string)obj.GetValue(TextProperty);
        }

        public static void SetText(DependencyObject obj, string value)
        {
            obj.SetValue(TextProperty, value);
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached("Text", typeof(string), typeof(ClipboardViewerBehavior), new PropertyMetadata(null));
        #endregion

        /// <summary>
        /// フックしたウィンドウプロシージャ
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_DRAWCLIPBOARD: //Clipboard content changed
                    if (Clipboard.ContainsText())
                    {
                        //添付プロパティのClipboardTextに値を設定
                        //ClipboardProperty.SetClipboardText(this.AssociatedObject, Clipboard.GetText());
                        SetText(this.AssociatedObject, Clipboard.GetText());
                    }
                    if (nexthWnd != IntPtr.Zero)
                    {
                        NativeMethods.SendMessage(nexthWnd, msg, wParam, lParam);
                    }
                    break;
                case WM_CHANGECBCHAIN: //Viewer removed
                    if (wParam == nexthWnd) //If Removed viewer is next one
                    {
                        nexthWnd = lParam;
                    }
                    else if (nexthWnd != IntPtr.Zero)
                    {
                        NativeMethods.SendMessage(nexthWnd, msg, wParam, lParam);
                    }
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }

        void Loaded(object sender, RoutedEventArgs e)
        {
            //Hook WndProc
            hWnd = new WindowInteropHelper(this.AssociatedObject).Handle;
            var source = HwndSource.FromHwnd(hWnd);
            source.AddHook(new HwndSourceHook(WndProc));

            //Register to ClipboardViewer
            Clipboard.Clear();
            nexthWnd = NativeMethods.SetClipboardViewer(hWnd);
        }

        void Closed(object sender, EventArgs e)
        {
            //Remove from clipboared chain
            NativeMethods.ChangeClipboardChain(hWnd, nexthWnd);
        }

        #region Override
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += Loaded;
            this.AssociatedObject.Closed += Closed;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.Loaded -= Loaded;
            this.AssociatedObject.Closed -= Closed;
        }
        #endregion
    }
}
