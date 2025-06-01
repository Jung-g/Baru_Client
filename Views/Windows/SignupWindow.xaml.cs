using Baru_Client.ViewModels.Pages;
using Baru_Client.ViewModels.Windows;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Baru_Client.Views.Windows
{
    public partial class SignupWindow : Window
    {
        private readonly SignupViewModel _viewModel;
        private readonly LoginWindow _loginWindow;

        public SignupWindow(SignupViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = _viewModel;

            _viewModel.SignSucceeded += OnSignupSucceeded;
            _viewModel.SignFailed += OnSignupFailed;

            // DI로 등록된 LoginWindow 참조 (복귀용)
            _loginWindow = App.Services.GetRequiredService<LoginWindow>();
        }

        private void OnSignupSucceeded()
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show("회원가입 완료. 로그인 창으로 돌아갑니다.", "회원가입", MessageBoxButton.OK, MessageBoxImage.Information);
                _loginWindow.Show();
                Close();
            });
        }

        private void OnSignupFailed()
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show("회원가입 실패. 다시 시도해주세요.", "회원가입", MessageBoxButton.OK, MessageBoxImage.Warning);
            });
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            _loginWindow.Show();
            Close();
        }

        private void SPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null && sender is PasswordBox box)
            {
                _viewModel.Password = box.Password;
            }
        }
    }
}
