using System.Windows.Controls;
using Baru_Client.ViewModels.Pages;

namespace Baru_Client.Views.Pages
{
    public partial class AccuracyPage : Page
    {
        public AccuracyPage(AccuracyViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        // 날짜가 선택될 때마다 데이터 새로 고침
        private void OnDateChanged(object sender, RoutedEventArgs e)
        {
            var viewModel = (AccuracyViewModel)this.DataContext;
            viewModel.LoadAccuracyData();
        }

        // 페이지가 로드될 때 데이터 새로 고침
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = (AccuracyViewModel)this.DataContext;
            viewModel.LoadAccuracyData();
        }
    }
}
