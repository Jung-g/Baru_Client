using Baru_Client.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace Baru_Client.Views.Pages
{
    public partial class SettingsPage : INavigableView<SettingsViewModel>
    {
        public SettingsViewModel ViewModel { get; }

        public SettingsPage(SettingsViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = viewModel;

            InitializeComponent();
        }
    }
}
