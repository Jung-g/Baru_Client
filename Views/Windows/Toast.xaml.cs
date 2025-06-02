using System;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Interop;
using Timer = System.Timers.Timer;

namespace Baru_Client.Views.Windows
{
    public partial class Toast : Window
    {
        public static Toast Instance { get; } = new Toast();


        private static Toast? _currentToast;  // 현재 떠 있는 Toast를 추적

        private Timer? _autoCloseTimer;

        private Toast()
        {
            InitializeComponent();
            Loaded += Toast_Loaded;
        }

        public static void ShowToast(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                // 기존 알림이 있으면 닫기
                if (_currentToast != null)
                {
                    _currentToast.Close();
                    _currentToast = null;
                }

                // 새 알림 생성
                var toast = new Toast();
                toast.Message = message;
                toast.Show();

                _currentToast = toast;
            });
        }

        private void Toast_Loaded(object? sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;

            //int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);

            var workingArea = SystemParameters.WorkArea;
            this.Left = workingArea.Right - this.ActualWidth - 10;
            this.Top = workingArea.Bottom - this.ActualHeight - 10;

            // 5분 뒤에 자동 닫힘
            _autoCloseTimer = new Timer(300000);
            _autoCloseTimer.Elapsed += AutoCloseTimer_Elapsed;
            _autoCloseTimer.AutoReset = false;
            _autoCloseTimer.Start();
        }

        private void AutoCloseTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            _autoCloseTimer?.Stop();
            _autoCloseTimer?.Dispose();
            _autoCloseTimer = null;

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (this.IsVisible)
                {
                    this.Close();
                    if (_currentToast == this)
                    {
                        _currentToast = null;
                    }
                }
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (_currentToast == this)
            {
                _currentToast = null;
            }

            _autoCloseTimer?.Stop();
            _autoCloseTimer?.Dispose();
            _autoCloseTimer = null;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public string Message
        {
            get => MessageTextBlock.Text;
            set => MessageTextBlock.Text = value;
        }
        private const int GWL_EXSTYLE = -20;
       
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int nIndex, int dwNewLong);
    }
}
