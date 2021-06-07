using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LaoDuanWinCtrl
{
    /// <summary>
    /// AttachWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AttachWindow : Window
    {
        public IntPtr hWnd { get; set; }
        DispatcherTimer dispatcher;
        public AttachWindow()
        {
            InitializeComponent();
        }
        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;                             //最左坐标
            public int Top;                             //最上坐标
            public int Right;                           //最右坐标
            public int Bottom;                        //最下坐标
        }
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int nMaxCount);
        //是否最小化
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern int IsWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern int SetWindowText(IntPtr hWnd, string text);
        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);
        [DllImport("user32.dll")]
        public static extern bool IsZoomed(IntPtr hWnd);
        private bool isHide = false;
        private void Window_Update()
        {
            if (hWnd.ToInt32() == 0)
            {
                return;
            }
            if (IsWindow(hWnd) == 0)
            {
                dispatcher.Stop();
                this.Close();
            }
            if (IsIconic(hWnd) == true)
            {
                Opacity = 0;
            }
            else
            {
                Opacity = 1;
            }
            if (isHide) return;
            RECT rect = new RECT();
            GetWindowRect(hWnd, ref rect);
            this.Width = (rect.Right - rect.Left)*0.6;
            this.Left = rect.Left + (rect.Right - rect.Left) / 2 - (this.Width > MinWidth ? Width : MinWidth) / 2;
            this.Top = (rect.Top - this.Height) > 0 ? (rect.Top - this.Height) : 0;
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(isHide)
                DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int length = GetWindowTextLength(hWnd);
            StringBuilder windowName = new StringBuilder(length + 1);
            GetWindowText(hWnd, windowName, windowName.Capacity);
            title.Content = windowName;
            SetWindowText(hWnd, "<Controlled>");
            dispatcher = new DispatcherTimer();
            dispatcher.Tick += new EventHandler((object s, EventArgs ee) =>
            {
                Window_Update();
            });
            dispatcher.Interval = new TimeSpan(0, 0, 0, 0, 30);
            dispatcher.Start();
        }
        

        private void Min(object sender, RoutedEventArgs e)
        {
            ShowWindow(hWnd, 2);
        }

        private void Show(object sender, RoutedEventArgs e)
        {
            ShowWindow(hWnd, 1);
            MinWidth = 300;
            kill.Visibility = Visibility.Visible;
            hide.Visibility = Visibility.Visible;
            isHide = false;
        }

        private void Hide(object sender, RoutedEventArgs e)
        {
            ShowWindow(hWnd, 0);
            kill.Visibility = Visibility.Collapsed;
            hide.Visibility = Visibility.Collapsed;
            MinWidth = 150;
            Width = 150;
            isHide = true;
        }

        private void Kill(object sender, RoutedEventArgs e)
        {
            SendMessage(hWnd, 0x10, 0, 0);
        }
    }
    public class interfaceCtl : INotifyPropertyChanged
    {
        private bool btnEn;
        private int wid;
        private int width
        {
            get { return wid; }
            set
            {
                if (value != wid)
                {
                    wid = value;
                    Notify("width");
                }
            }
        }

        public bool btnEnable
        {
            get { return btnEn; }
            set
            {
                if (value != btnEn)
                {
                    btnEn = value;
                    Notify("btnEnable");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string propertyName)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
