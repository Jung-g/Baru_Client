using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Baru_Client.Camera
{
    public partial class Camera_Control : UserControl
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private VideoCapture? _capture = null;
        private Mat? _frame = null;
        private bool _isCameraActive = false;
        private string _currentUid = string.Empty;

        public Camera_Control()
        {
            InitializeComponent();
        }

        public void StartCamera(string uid)
        {
            if (_isCameraActive) return;

            _currentUid = uid;
            _isCameraActive = true;
            Task.Run(() => CameraLoopAsync());
        }

        public void StopCamera()
        {
            _isCameraActive = false;
        }

        private async Task CameraLoopAsync()
        {
            _capture = new VideoCapture(0, VideoCapture.API.DShow);
            _frame = new Mat();

            while (_isCameraActive)
            {
                await Task.Delay(30);

                if (_capture != null && _capture.IsOpened)
                {
                    _capture.Read(_frame!);
                    if (!_frame!.IsEmpty)
                    {
                        var bitmapSource = ConvertMatToBitmapSource(_frame!);
                        if (bitmapSource != null)
                        {
                            await Dispatcher.InvokeAsync(() =>
                            {
                                imageBox.Source = bitmapSource;
                            });
                        }

                        var jpegBytes = _frame.ToImage<Bgr, byte>().ToJpegData();
                        _ = SendFrameToServerAsync(jpegBytes);
                    }
                }
            }

            try
            {
                _capture?.Dispose();
                _frame?.Dispose();
            }
            catch { }
        }

        private BitmapSource? ConvertMatToBitmapSource(Mat mat)
        {
            try
            {
                using var bitmap = mat.ToBitmap();
                using var ms = new MemoryStream();
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);

                var decoder = new BmpBitmapDecoder(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                return decoder.Frames[0];
            }
            catch
            {
                return null;
            }
        }

        private async Task SendFrameToServerAsync(byte[] jpegBytes)
        {
            if (string.IsNullOrEmpty(_currentUid)) return;

            using var content = new ByteArrayContent(jpegBytes);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            content.Headers.Add("x-uid", _currentUid);

            try
            {
                await _httpClient.PostAsync("http://192.168.219.103:8080/byte/", content);
            }
            catch
            {
                // 전송 실패 시 로깅 또는 예외 처리 필요 시 구현
            }
        }
    }
}
