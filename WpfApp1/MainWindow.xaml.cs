using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;

        [StructLayout(LayoutKind.Sequential)]
        private struct Margins
        {
            public int LeftWidth;
            public int RightWidth;
            public int TopHeight;
            public int BottomHeight;
        }

#pragma warning disable SYSLIB0014 // DllImport 旧写法警告
        [DllImport("dwmapi.dll", EntryPoint = "DwmSetWindowAttribute")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, IntPtr pvAttr, uint cbAttr);

        [DllImport("dwmapi.dll", EntryPoint = "DwmExtendFrameIntoClientArea")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref Margins pMarInset);
#pragma warning restore SYSLIB0014

        public MainWindow()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
                Background = Brushes.Transparent;

            SourceInitialized += OnSourceInitialized;
        }

        private unsafe void OnSourceInitialized(object? sender, EventArgs e)
        {
            var hwndSource = (System.Windows.Interop.HwndSource)PresentationSource.FromVisual(this)!;
            hwndSource.CompositionTarget.BackgroundColor = Colors.Transparent;

            var margins = new Margins { LeftWidth = -1, TopHeight = -1, RightWidth = -1, BottomHeight = -1 };
            _ = DwmExtendFrameIntoClientArea(hwndSource.Handle, ref margins);

            int* backdropPtr = stackalloc int[1];
            backdropPtr[0] = 2;
            _ = DwmSetWindowAttribute(hwndSource.Handle, DWMWA_SYSTEMBACKDROP_TYPE, new IntPtr(backdropPtr), sizeof(int));
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                ToggleMaximize();
            else if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaximize();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ToggleMaximize()
        {
            var iconMax = FindName("IconMaximize") as Path;
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                iconMax?.SetResourceReference(Path.DataProperty, "CropSquare");
            }
            else
            {
                WindowState = WindowState.Maximized;
                iconMax?.SetResourceReference(Path.DataProperty, "ChromeRestore");
            }
        }
    }
}
