using Baru_Client.Services;
using Baru_Client.ViewModels.Pages;
using Baru_Client.ViewModels.Windows;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace Baru_Client.Views.Windows
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginWindow(LoginViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = _viewModel;

            _viewModel.LoginSucceeded += OnLoginSucceeded;
            _viewModel.LoginFailed += OnLoginFailed;
        }

        private void OnLoginSucceeded()
        {
            Dispatcher.Invoke(() =>
            {
                var appHostService = App.Services.GetRequiredService<ApplicationHostService>();
                appHostService.HandleActivationAsync();  // MainWindow 실행
                Close();  // 로그인 창 종료
            });
        }

        private void OnLoginFailed()
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show(this, "로그인 실패. 아이디 또는 비밀번호를 확인하세요.", "로그인", MessageBoxButton.OK, MessageBoxImage.Warning);
            });
        }

        private void SignupButton_Click(object sender, RoutedEventArgs e)
        {
            var signupWindow = App.Services.GetRequiredService<SignupWindow>();
            signupWindow.Show();
            this.Hide(); // 현재 창을 숨기고 회원가입 창 열기
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null && sender is PasswordBox box)
            {
                _viewModel.Password = box.Password;
            }
        }
    }
}
