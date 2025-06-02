using Timer = System.Timers.Timer;
using Baru_Client.Data;
using Baru_Client.FastAPI;
using Baru_Client.Alarmes;

namespace Baru_Client.ViewModels.Pages
{
    public partial class CameraViewModel : ObservableObject
    {
        private string? _currentUid;
        private string? _currentExerciseName;
        private Timer? _pollingTimer;

        [ObservableProperty]
        private int _currentCount;

        [ObservableProperty]
        private int _currentAccuracy;

        public event Action<string>? OnUidReceived;
        public event Action<string>? OnVideoPlayRequested;
        public event Action? OnStopRequested;

        [RelayCommand]
        private async Task PlaySquatAsync() => await StartExerciseAsync("Squat", "Videos/squat.mp4");

        [RelayCommand]
        private async Task PlayLungeAsync() => await StartExerciseAsync("lunge", "Videos/lunge.mp4");

        [RelayCommand]
        private async Task PlaySideStretchAsync() => await StartExerciseAsync("sidestretch", "Videos/sidestretch.mp4");

        [RelayCommand]
        private async Task StopAsync()
        {
            if (_pollingTimer != null)
            {
                _pollingTimer.Stop();
                _pollingTimer.Dispose();
                _pollingTimer = null;
            }

            await API.Instance.GetAccuary();
            await API.Instance.LoadBeforeInfoAsync();

            _currentUid = null;
            _currentExerciseName = null;

            CurrentCount = 0;
            CurrentAccuracy = 0;

            OnStopRequested?.Invoke();
        }

        private async Task StartExerciseAsync(string exerciseName, string videoPath)
        {
            if (_currentUid != null)
            {
                if (_currentExerciseName == exerciseName)
                    return; // 동일 운동이라면 아무 작업도 하지 않음
                await StopAsync();
            }

            bool inserted = await API.Instance.InsertExercise(exerciseName);
            if (!inserted)
                return;

            _currentUid = API.Instance.GetCurrentUid();
            if (string.IsNullOrEmpty(_currentUid))
                return;

            _currentExerciseName = exerciseName;

            OnUidReceived?.Invoke(_currentUid);
            OnVideoPlayRequested?.Invoke(videoPath);

            _pollingTimer = new Timer(1000);
            _pollingTimer.Elapsed += async (s, e) => await PollCurrentAsync();
            _pollingTimer.AutoReset = true;
            _pollingTimer.Start();

            // 운동 안한지 1시간 단위로 스트레칭 유도 알람
            TimerExample.StartTimer();
        }

        private async Task PollCurrentAsync()
        {
            if (string.IsNullOrEmpty(_currentUid))
                return;

            Current? result = await API.Instance.Getcurrent(_currentUid);
            if (result != null)
            {
                CurrentCount = result.Count;
                if (result.Accuracies != null && result.Accuracies.Count > 0)
                {
                    CurrentAccuracy = (int)result.Accuracies[^1];
                }
            }
        }
    }
}
