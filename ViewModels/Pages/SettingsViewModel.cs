using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using Wpf.Ui.Appearance;
using Wpf.Ui.Abstractions.Controls;

namespace Baru_Client.ViewModels.Pages
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _appVersion = string.Empty;

        [ObservableProperty]
        private ApplicationTheme _currentTheme = ApplicationTheme.Unknown;

        public Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
                InitializeViewModel();

            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private void InitializeViewModel()
        {
            CurrentTheme = ApplicationThemeManager.GetAppTheme();
            AppVersion = $"Baru ver {GetAssemblyVersion()}";
            _isInitialized = true;
        }

        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                   ?? string.Empty;
        }

        [RelayCommand]
        private void ChangeTheme(string parameter)
        {
            switch (parameter)
            {
                case "theme_light":
                    if (CurrentTheme != ApplicationTheme.Light)
                    {
                        ApplicationThemeManager.Apply(ApplicationTheme.Light);
                        CurrentTheme = ApplicationTheme.Light;
                    }
                    break;
                case "theme_dark":
                    if (CurrentTheme != ApplicationTheme.Dark)
                    {
                        ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                        CurrentTheme = ApplicationTheme.Dark;
                    }
                    break;
            }
        }
    }
}
