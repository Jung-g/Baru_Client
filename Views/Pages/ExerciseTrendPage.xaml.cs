using System.Windows.Controls;
using Baru_Client.ViewModels.Pages;

namespace Baru_Client.Views.Pages
{
    public partial class ExerciseTrendPage : Page
    {
        public ExerciseTrendPage(ExerciseTrendViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
