using System.Windows.Controls;
using Baru_Client.ViewModels.Pages;

namespace Baru_Client.Views.Pages
{
    public partial class AnalysisChartPage : Page
    {
        public AnalysisChartPage(AnalysisChartViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
