using System.Windows.Media;
using System.Windows.Media.Media3D;
using Wpf.Ui.Controls;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace Baru_Client.Alarmes
{
    public class Alarme
    {
        public static Alarme Instance { get; } = new Alarme();

        private static SnackbarPresenter? _presenter = null;

        // 생성자에서 SnackbarPresenter 주입
        private Alarme() { }

        public void SetSnack(SnackbarPresenter snack)
        {
            if(snack != null)
                _presenter = snack;
        }

        /// <summary>
        /// 스낵바 알람표시
        /// </summary>
        /// <param name="message"> 메세지 내용</param>
        /// <param name="title"> 타이틀 주소 </param>
        /// <param name="appearance"> 타입 ( Danger = 오류 , Info = 정보 )</param>
        /// <param name="timeoutSeconds"> 기본값 1초뒤에 제거 </param>
        public static void ShowSnackbar(
            string message,
            string? title = null,
            ControlAppearance appearance = ControlAppearance.Danger,
            int timeoutSeconds = 2)
        {
            if (_presenter == null)
                return;

            var snackbar = new Snackbar(_presenter)
            {
                Appearance = appearance,
                Timeout = TimeSpan.FromSeconds(timeoutSeconds),
                Content = new TextBlock
                {
                    Text = message,
                    FontSize = 12,
                },
                Title = new TextBlock
                {
                    Text = title ?? "알림",
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                }

            };

            // 아이콘 설정
            if(appearance == ControlAppearance.Danger)
            {
                snackbar.Icon = new SymbolIcon { Symbol = SymbolRegular.Warning20 , FontSize = 20};
            }
            else
            {
                snackbar.Icon = new SymbolIcon { Symbol = SymbolRegular.Info20 , FontSize = 20 };
            }

            snackbar.Show(true);
        }
    }
}
