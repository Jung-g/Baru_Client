using System.Collections.ObjectModel;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using Baru_Client.FastAPI;

namespace Baru_Client.ViewModels.Pages
{
    public partial class AccuracyViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<string> exerciseNames = new();

        [ObservableProperty]
        private SeriesCollection accuracySeries = new();

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Today;

        [ObservableProperty]
        private string selectedDateString = string.Empty;

        public AccuracyViewModel()
        {
            SelectedDate = DateTime.Today;
            SelectedDateString = SelectedDate.ToString("yyyy년 MM월 dd일 (ddd)");
            LoadAccuracyData();
        }

        partial void OnSelectedDateChanged(DateTime value)
        {
            SelectedDateString = value.ToString("yyyy년 MM월 dd일 (ddd)");
            LoadAccuracyData();
        }

        // Accuracy 데이터를 불러와서 그래프를 구성합니다.
        public void LoadAccuracyData()
        {
            AccuracySeries.Clear();

            var beforeInfo = API.Instance.MyData;
            if (beforeInfo == null || beforeInfo.Exercises == null || beforeInfo.Exercises.Count == 0)
                return;

            // 선택된 날짜의 운동 데이터 가져오기
            var selectedDateStr = SelectedDate.ToString("yyyy-MM-dd");
            var exerciseOfTheDay = beforeInfo.Exercises
                .FirstOrDefault(e => e.Date == selectedDateStr);

            if (exerciseOfTheDay == null)
                return;

            // E_data가 파싱되어 있지 않으면 변환
            if (exerciseOfTheDay.E_data == null || exerciseOfTheDay.E_data.Count == 0)
                exerciseOfTheDay.Init_Data();

            if (exerciseOfTheDay.E_data == null || exerciseOfTheDay.E_data.Count == 0)
                return; // 운동 기록이 없으면 표시하지 않음

            // 운동명 목록을 얻어오기
            exerciseNames.Clear();
            foreach (var data in exerciseOfTheDay.E_data)
            {
                if (!string.IsNullOrWhiteSpace(data.Name))
                {
                    exerciseNames.Add(data.Name);
                    AddSeries(data.Name, data.Acc);
                }
            }
        }

        // 각 운동에 대한 정확도 그래프 추가
        private void AddSeries(string exerciseName, List<double> accuracies)
        {
            var values = new ChartValues<ObservableValue>();
            foreach (var accuracy in accuracies)
            {
                values.Add(new ObservableValue(accuracy));
            }

            AccuracySeries.Add(new LineSeries
            {
                Title = exerciseName switch
                {
                    "Squart" => "스쿼트",
                    "lunge" => "런지",
                    "sidestretch" => "사이드 스트레칭",
                    _ => exerciseName
                },
                Values = values,
                PointGeometry = DefaultGeometries.Circle,
                PointGeometrySize = 8,
                LabelPoint = chartPoint => $"{chartPoint.SeriesView.Title}: {chartPoint.Y:N2}%",
                Fill = System.Windows.Media.Brushes.Transparent
            });
        }
    }
}
