using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace Baru_Client.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string applicationTitle = "Baru";

        [ObservableProperty]
        private ObservableCollection<object> menuItems;

        [ObservableProperty]
        private ObservableCollection<object> footerMenuItems;

        // 필요시 트레이 메뉴 (TrayMenuItems)도 관리, 불필요하면 제거
        //[ObservableProperty]
        //private ObservableCollection<MenuItem> trayMenuItems;

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

            // 트레이 메뉴(필요시)
            //trayMenuItems = new ObservableCollection<MenuItem>
            //{
            //    new MenuItem { Header = "Home", Tag = "tray_home" }
            //};
        }
    }
}
