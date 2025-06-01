using System.Collections.ObjectModel;
using System.Globalization;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using Baru_Client.FastAPI;
using System.Diagnostics.Metrics;

namespace Baru_Client.ViewModels.Pages
{
    public partial class ExerciseTrendViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<string> availableMonths = new();

        [ObservableProperty]
        private string selectedMonth = string.Empty;

        [ObservableProperty]
        private SeriesCollection trendSeries = new();

        [ObservableProperty]
        private AxesCollection trendAxisX = new();

        [ObservableProperty]
        private AxesCollection trendAxisY = new();

        public ExerciseTrendViewModel()
        {
            InitMonths();
            if (AvailableMonths.Any())
            {
                SelectedMonth = AvailableMonths.First();
            }
        }

        // 월 목록 초기화 (운동 기록에서 자동 추출)
        private void InitMonths()
        {
            var beforeInfo = API.Instance.MyData;
            if (beforeInfo?.Exercises == null || beforeInfo.Exercises.Count == 0)
                return;

            // yyyy-MM 포맷으로 월 목록 생성 (중복 제거)
            var months = beforeInfo.Exercises
                .Select(e => SafeParseDate(e.Date))
                .Where(d => d != null)
                .Select(d => d.Value.ToString("yyyy-MM"))
                .Distinct()
                .OrderByDescending(m => m)
                .ToList();

            AvailableMonths = new ObservableCollection<string>(months);
        }

        // 선택 월 변경 시 라인차트 자동 갱신
        partial void OnSelectedMonthChanged(string value)
        {
            LoadTrendSeries(value);
        }

        // 선택 월에 맞는 운동량 추이 그래프 구성
        public void LoadTrendSeries(string month)
        {
            TrendSeries.Clear();
            TrendAxisX.Clear();
            TrendAxisY.Clear();

            var beforeInfo = API.Instance.MyData;
            if (beforeInfo?.Exercises == null) return;

            // 해당 월의 날짜별 운동 기록 추출
            var monthExercises = beforeInfo.Exercises
                .Where(e => {
                    var d = SafeParseDate(e.Date);
                    return d != null && d.Value.ToString("yyyy-MM") == month;
                })
                .ToList();

            if (!monthExercises.Any()) return;

            // X축: 일(day), Y축: 운동 횟수
            // 운동 유형 구분 (E_data.Name 기준)
            var exerciseNames = monthExercises
                .SelectMany(e => {
                    if (e.E_data == null || e.E_data.Count == 0) e.Init_Data();
                    return e.E_data?.Select(d => d.Name) ?? Enumerable.Empty<string>();
                })
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct()
                .ToList();

            // X축 날짜 (1~31일) 생성
            var dayLabels = new ObservableCollection<string>();
            for (int i = 1; i <= 31; i++) // 1일부터 31일까지 모두 라벨에 추가
            {
                dayLabels.Add(i.ToString("D2"));
            }

            // Y축 값 범위 설정
            double maxYValue = 0;
            double minYValue = double.MaxValue;

            // 각 운동별로 Series 생성
            foreach (var exerciseName in exerciseNames)
            {
                var values = new ChartValues<ObservableValue>();

                foreach (var day in dayLabels) // 날짜별로 데이터를 추가
                {
                    var recordForDay = monthExercises.FirstOrDefault(e => SafeParseDate(e.Date)?.Day.ToString("D2") == day);
                    if (recordForDay != null)
                    {
                        // 해당 날짜의 운동 데이터가 있다면 그 값을 사용
                        var eData = recordForDay.E_data?.FirstOrDefault(d => d.Name == exerciseName);
                        int count = eData?.Count ?? 0;
                        values.Add(new ObservableValue(eData?.Count ?? 0));

                        // Y축 최대값과 최소값 업데이트
                        if (count > maxYValue)
                        {
                            maxYValue = count;
                        }
                        if (count < minYValue)
                        {
                            minYValue = count;
                        }
                    }
                    else
                    {
                        // 해당 날짜의 운동 데이터가 없다면 0으로 처리
                        values.Add(new ObservableValue(0));
                    }
                }

                TrendSeries.Add(new LineSeries
                {
                    Title = exerciseName switch
                    {
                        "Squat" => "스쿼트",
                        "lunge" => "런지",
                        "sidestretch" => "사이드 스트레칭",
                        _ => exerciseName  // 위에 해당하지 않는 경우 그대로 표시
                    },
                    Values = values,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 8,
                    Fill = System.Windows.Media.Brushes.Transparent
                });
            }

            // X축 레이블 (1일부터 31일까지)
            TrendAxisX.Add(new Axis
            {
                Title = "일",
                Labels = dayLabels
            });

            // Y축: 횟수 (동적 범위)
            TrendAxisY.Add(new Axis
            {
                Title = "운동 횟수",
                MinValue = 0,
                MaxValue = maxYValue + 10
            });
        }


        // yyyy-MM-dd 형태 파싱
        private DateTime? SafeParseDate(string? dateStr)
        {
            if (DateTime.TryParseExact(dateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return dt;
            return null;
        }
    }
}
