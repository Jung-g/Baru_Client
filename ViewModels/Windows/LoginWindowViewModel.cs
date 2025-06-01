using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Baru_Client.FastAPI;
using System;
using System.Threading.Tasks;

namespace Baru_Client.ViewModels.Windows
{
    public partial class LoginViewModel : ObservableObject
    {
        // 로그인 성공/실패 이벤트
        public event Action? LoginSucceeded;
        public event Action? LoginFailed;

        [ObservableProperty]
        private string userid = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        public IRelayCommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new AsyncRelayCommand(LoginAsync);
        }

        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Userid) || string.IsNullOrWhiteSpace(Password))
            {
                LoginFailed?.Invoke();
                return;
            }

            // API.Instance를 통해 로그인 시도
            bool result = await API.Instance.Login(Userid, Password);
            if (result)
                LoginSucceeded?.Invoke();
            else
                LoginFailed?.Invoke();
        }
    }
}
