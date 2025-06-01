using Baru_Client.FastAPI;

namespace Baru_Client.ViewModels.Windows
{
    public partial class SignupViewModel : ObservableObject
    {
        // 회원가입 성공/실패 이벤트
        public event Action? SignSucceeded;
        public event Action? SignFailed;

        [ObservableProperty]
        private string userid = string.Empty;

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        public IRelayCommand SignupCommand { get; }

        public SignupViewModel()
        {
            SignupCommand = new AsyncRelayCommand(SignupAsync);
        }

        private async Task SignupAsync()
        {
            // 입력값 체크 (간단한 예시)
            if (string.IsNullOrWhiteSpace(Userid) ||
                string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password))
            {
                SignFailed?.Invoke();
                return;
            }

            // API 싱글톤 활용
            bool result = await API.Instance.SignupAsync(Userid, Password, Username);
            if (result)
                SignSucceeded?.Invoke();
            else
                SignFailed?.Invoke();
        }
    }
}
