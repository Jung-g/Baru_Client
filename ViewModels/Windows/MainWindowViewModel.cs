using Baru_Client.Alarmes;
using Baru_Client.Views.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows.Automation;
using Wpf.Ui.Controls;
using Timer = System.Timers.Timer;

namespace Baru_Client.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private static int _time = 1;


        [ObservableProperty]
        private string applicationTitle = "Baru";

        [ObservableProperty]
        private ObservableCollection<object> menuItems;

        [ObservableProperty]
        private ObservableCollection<object> footerMenuItems;


        public MainWindowViewModel()
        {
            // 메뉴 항목 동적 구성 (여기서만 관리)
            menuItems = new ObservableCollection<object>
            {
                new NavigationViewItem()
                {
                    Content = "Home",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                    TargetPageType = typeof(Views.Pages.DashboardPage)
                },
                new NavigationViewItem()
                {
                    Content = "운동하기",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.DataPie24 },
                    TargetPageType = typeof(Views.Pages.CameraPage)
                },
                new NavigationViewItem()
                {
                    Content = "일일 분석",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.DataPie24 },
                    TargetPageType = typeof(Views.Pages.AnalysisChartPage)
                },
                new NavigationViewItem()
                {
                    Content = "운동량 추이(월별)",
                    Icon = new SymbolIcon() { Symbol = SymbolRegular.Pulse24 },
                    TargetPageType = typeof(Views.Pages.ExerciseTrendPage)
                },
                new NavigationViewItem()
                {
                    Content = "일일 운동 정확도",
                    Icon = new SymbolIcon() { Symbol = SymbolRegular.Target24 },
                    TargetPageType = typeof(Views.Pages.AccuracyPage)
                }
            };

            footerMenuItems = new ObservableCollection<object>
            {
                new NavigationViewItem()
                {
                    Content = "설정",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                    TargetPageType = typeof(Views.Pages.SettingsPage)
                }
            };

            OnInit();
        }
        private void OnInit()
        {
            // 1시간 마다 알람
            Timer timer = new Timer(TimeSpan.FromHours(1).TotalMilliseconds);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;

            // 스트레칭 유도 알람
            TimerExample.StartTimer();
        }
        public void OnTimedEvent(object? sender, ElapsedEventArgs e)
        {
            // 서비스 이용시간 알람
            Toast.ShowToast($"이용하신지 {_time++}시간 되셨습니다.");
        }

    }
}
