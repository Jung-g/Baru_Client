using System.Windows.Controls;
using Baru_Client.ViewModels.Pages;

namespace Baru_Client.Views.Pages
{
    public partial class DashboardPage : Page
    {
        public DashboardPage(DashboardViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        // 페이지가 로드될 때 데이터 새로 고침
        private void DashboardRoot_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = (DashboardViewModel)this.DataContext;
            viewModel.LoadFromServer();
        }
    }
}
