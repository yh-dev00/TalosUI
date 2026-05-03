using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace TalosCore
{
    /// <summary>
    /// Click-through WPF overlay used to highlight the currently inspected UIA element.
    /// </summary>
    public partial class HighlighterWindow : Window
    {
        private const int GWL_EXSTYLE = -20;
        private const int HWND_TOPMOST = -1;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOACTIVATE = 0x0010;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int x,
            int y,
            int cx,
            int cy,
            int uFlags);

        public HighlighterWindow()
        {
            InitializeComponent();
            ShowActivated = false;
            Topmost = true;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(
                hwnd,
                GWL_EXSTYLE,
                extendedStyle | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE);

            SetWindowPos(
                hwnd,
                new IntPtr(HWND_TOPMOST),
                0,
                0,
                0,
                0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }

        public void ShowHighlight(PersistedRectangle bounds)
        {
            if (bounds == null || bounds.IsEmpty)
            {
                HideHighlight();
                return;
            }

            IntPtr hwnd = EnsureWindowHandle();
            Rect logicalBounds = DevicePixelsToLogicalBounds(bounds);

            Left = logicalBounds.X;
            Top = logicalBounds.Y;
            Width = logicalBounds.Width;
            Height = logicalBounds.Height;

            if (!IsVisible)
            {
                Show();
            }

            SetWindowPos(
                hwnd,
                new IntPtr(HWND_TOPMOST),
                (int)Math.Round(bounds.X),
                (int)Math.Round(bounds.Y),
                (int)Math.Round(bounds.Width),
                (int)Math.Round(bounds.Height),
                SWP_NOACTIVATE);

            Topmost = false;
            Topmost = true;
        }

        public void HideHighlight()
        {
            if (IsVisible)
            {
                Hide();
            }
        }

        private IntPtr EnsureWindowHandle()
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            return helper.Handle == IntPtr.Zero ? helper.EnsureHandle() : helper.Handle;
        }

        private Rect DevicePixelsToLogicalBounds(PersistedRectangle bounds)
        {
            Matrix transform = Matrix.Identity;
            PresentationSource source = PresentationSource.FromVisual(this);

            if (source != null && source.CompositionTarget != null)
            {
                transform = source.CompositionTarget.TransformFromDevice;
            }

            Point topLeft = transform.Transform(new Point(bounds.X, bounds.Y));
            Point bottomRight = transform.Transform(new Point(bounds.X + bounds.Width, bounds.Y + bounds.Height));
            return new Rect(topLeft, bottomRight);
        }
    }
}
