using System;
using System.ComponentModel;
using System.Windows;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using LaoDuanWinCtrl;

namespace Handle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private interfaceCtl btnCtl;

        public MainWindow()
        {
            InitializeComponent();
            btnCtl = new interfaceCtl() { btnEnable = false };
            this.DataContext = btnCtl;
        }

        private IntPtr hWnd = (IntPtr)0;

        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "WindowFromPoint")]
        public static extern int WindowFromPoint(
            int xPoint,
            int yPoint
        );

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        // private void UpdatePos(object sender, RoutedEventArgs e)
        // {
        //     XYpos.Content = Mouse.GetPosition(this).X.ToString() + " "+Mouse.GetPosition(this).Y.ToString();
        // }

        private void Find(object sender, RoutedEventArgs e)
        {
            hWnd = FindWindow(null, TextBox0.Text);
            if (hWnd.ToInt32() != 0)
            {
                int length = GetWindowTextLength(hWnd);
                StringBuilder windowName = new StringBuilder(length + 1);
                GetWindowText(hWnd, windowName, windowName.Capacity);
                this.InfoText.Content = "获取到窗口句柄 : [" + hWnd.ToString() + "] (" + windowName.ToString() + ")";
                btnCtl.btnEnable = true;

                AttachWindow subWin = new AttachWindow();
                subWin.hWnd = hWnd;
                subWin.Show();
            }
            else
            {
                this.InfoText.Content = "没有找到窗口!";
                btnCtl.btnEnable = false;
            }
        }

        private void Min(object sender, RoutedEventArgs e)
        {
            ShowWindow(hWnd, 2);
        }

        private void Show(object sender, RoutedEventArgs e)
        {
            ShowWindow(hWnd, 1);
        }

        private void Hide(object sender, RoutedEventArgs e)
        {
            ShowWindow(hWnd, 0);
        }

        private void Kill(object sender, RoutedEventArgs e)
        {
            SendMessage(hWnd, 0x10, 0, 0);
        }
    }

    public class interfaceCtl : INotifyPropertyChanged
    {
        private bool btnEn;

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