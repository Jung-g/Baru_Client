using Baru_Client.ViewModels.Windows;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Baru_Client.Views.Windows
{
    public partial class MainWindow : INavigationWindow
    {
        public MainWindow(MainWindowViewModel viewModel, INavigationViewPageProvider pageProvider, INavigationService navService)
        {
            InitializeComponent();

            ViewModel = viewModel;
            DataContext = ViewModel;

            SystemThemeWatcher.Watch(this);

            RootNavigation.SetPageProviderService(pageProvider);
            navService.SetNavigationControl(RootNavigation);
        }

        public MainWindowViewModel ViewModel { get; }
        public INavigationView GetNavigation() => RootNavigation;
        public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);
        public void SetPageService(INavigationViewPageProvider provider) => RootNavigation.SetPageProviderService(provider);
        public void ShowWindow() => Show();
        public void CloseWindow() => Close();

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            // 필요시 구현
        }

    }
}
