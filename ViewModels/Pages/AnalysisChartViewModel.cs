using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using Baru_Client.FastAPI;

namespace Baru_Client.ViewModels.Pages
{
    public partial class AnalysisChartViewModel : ObservableObject
    {
        [ObservableProperty]
        private SeriesCollection pieSeries = new();

        [ObservableProperty]
        private string selectedDateString = string.Empty;

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Today;

        public AnalysisChartViewModel()
        {
            SelectedDate = DateTime.Today;
            SelectedDateString = SelectedDate.ToString("yyyy년 MM월 dd일 (ddd)");
            LoadDataForSelectedDate();
        }

        partial void OnSelectedDateChanged(DateTime value)
        {
            SelectedDateString = value.ToString("yyyy년 MM월 dd일 (ddd)");
            LoadDataForSelectedDate();
        }

        public void LoadDataForSelectedDate()
        {
            PieSeries.Clear();

            var beforeInfo = API.Instance.MyData;
            if (beforeInfo == null || beforeInfo.Exercises == null || beforeInfo.Exercises.Count == 0)
                return; // 데이터 없으면 아무것도 표시하지 않음

            string selDateString = SelectedDate.ToString("yyyy-MM-dd");
            var exerciseOfTheDay = beforeInfo.Exercises
                .Find(e => e.Date == selDateString);

            if (exerciseOfTheDay == null)
                return; // 해당 날짜 기록 없으면 표시하지 않음

            // E_data가 파싱되어 있지 않으면 변환
            if (exerciseOfTheDay.E_data == null || exerciseOfTheDay.E_data.Count == 0)
                exerciseOfTheDay.Init_Data();

            if (exerciseOfTheDay.E_data == null || exerciseOfTheDay.E_data.Count == 0)
                return; // 운동 기록이 없으면 표시하지 않음

            // E_data 내의 모든 운동 유형을 차트에 표시 (동적)
            foreach (var data in exerciseOfTheDay.E_data)
            {
                if (!string.IsNullOrWhiteSpace(data.Name) && data.Count > 0)
                {
                    PieSeries.Add(new PieSeries
                    {
                        Title = data.Name switch
                        {
                            "Squart" => "스쿼트",
                            "lunge" => "런지",
                            "sidestretch" => "사이드 스트레칭",
                            _ => data.Name  // 위에 해당하지 않는 경우 그대로 표시
                        },
                        Values = new ChartValues<ObservableValue> { new ObservableValue(data.Count) },
                        DataLabels = true,
                        LabelPoint = chartPoint => $"{chartPoint.SeriesView.Title}: {chartPoint.Y:N0}회"
                    });
                }
            }
        }
    }
}
