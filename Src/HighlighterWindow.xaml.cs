using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace TalosCore
{
    /// <summary>
    /// Interaction logic for HighlighterWindow.xaml
    /// This window acts as the visual red box that follows the mouse/UIA element.
    /// </summary>
    public partial class HighlighterWindow : Window
    {
        // Constants for Win32 API to set window to 'Transparent' (click-through)
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public HighlighterWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Get the handle for this window
            var hwnd = new WindowInteropHelper(this).Handle;

            // Use Win32 API to make the window transparent to mouse clicks.
            // This ensures that TalosUI doesn't 'inspect itself' when the red box appears.
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }
    }
}