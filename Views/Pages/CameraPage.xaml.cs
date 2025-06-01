using System.Windows.Controls;
using Baru_Client.ViewModels.Pages;

namespace Baru_Client.Views.Pages
{
    /// <summary>
    /// CameraPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CameraPage : Page
    {
        private readonly CameraViewModel _viewModel;

        public CameraPage()
        {
            InitializeComponent();
            _viewModel = new CameraViewModel();
            DataContext = _viewModel;

            _viewModel.OnUidReceived += (uid) =>
            {
                cameraControl.Visibility = Visibility.Visible;
                cameraControl.StartCamera(uid);
            };

            _viewModel.OnVideoPlayRequested += (videoPath) =>
            {
                ExerciseVideo.Source = new Uri(videoPath, UriKind.Relative);
                ExerciseVideo.Play();
            };

            _viewModel.OnStopRequested += () =>
            {
                cameraControl.StopCamera();
                cameraControl.Visibility = Visibility.Collapsed;

                ExerciseVideo.Stop();
                ExerciseVideo.Source = null;
            };
        }

        private void ExerciseVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (ExerciseVideo.Source != null)
            {
                ExerciseVideo.Position = TimeSpan.Zero;
                ExerciseVideo.Play();
            }
        }
    }
}
