using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Baru_Client.FastAPI;
using Baru_Client.Data;
using Baru_Client.Alarmes;

namespace Baru_Client.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private string todayDateText = DateTime.Today.ToString("yyyy년 MM월 dd일 (ddd)");

        public ObservableCollection<ExerciseGoalViewModel> ExerciseGoals { get; } = new();

        public IRelayCommand SetGoalCommand { get; }

        public DashboardViewModel()
        {
            SetGoalCommand = new RelayCommand<ExerciseGoalViewModel>(SetGoal);
            LoadFromServer();
        }

        public void LoadFromServer()
        {
            ExerciseGoals.Clear();

            var beforeInfo = API.Instance.MyData;
            if (beforeInfo == null)
                return;

            string todayStr = DateTime.Today.ToString("yyyy-MM-dd");
            var todayRecord = beforeInfo.Exercises?.FirstOrDefault(e => e.Date == todayStr);

            // 오늘 기록이 있으면 Init_Data() 호출, 없으면 그대로 넘어가서 count = 0 유지
            if (todayRecord != null)
            {
                todayRecord.Init_Data();
            }

            // 운동명, 목표치, 표기명 매핑표
            var exerciseMap = new[]
            {
                new { JsonKey = "Squart", Goal = beforeInfo.GoalA ?? 0, Display = "스쿼트" },
                new { JsonKey = "lunge", Goal = beforeInfo.GoalB ?? 0, Display = "런지" },
                new { JsonKey = "sidestretch", Goal = beforeInfo.GoalC ?? 0, Display = "사이드 스트레칭" }
            };

            foreach (var ex in exerciseMap)
            {
                int count = 0;

                if (todayRecord != null && todayRecord.E_data != null)
                {
                    var found = todayRecord.E_data.FirstOrDefault(d => d.Name == ex.JsonKey);
                    count = found?.Count ?? 0;
                }

                ExerciseGoals.Add(new ExerciseGoalViewModel(ex.Display, ex.Goal, count));
            }
        }

        private async void SetGoal(ExerciseGoalViewModel goal)
        {
            if (int.TryParse(goal.GoalInputText, out int newGoal) && newGoal > 0)
            {
                goal.Goal = newGoal;
                var MyData = API.Instance.MyData;
                if (goal.ExerciseName == "스쿼트")
                {
                    MyData.GoalA = newGoal;
                }
                else if (goal.ExerciseName == "런지")
                {
                    MyData.GoalB = newGoal;
                }
                else if (goal.ExerciseName == "사이드스트레칭")
                {
                    MyData.GoalC = newGoal;
                }

                string baruId = API.Instance.MyID;
                int goalA = MyData.GoalA.GetValueOrDefault();
                int goalB = MyData.GoalB.GetValueOrDefault();
                int goalC = MyData.GoalC.GetValueOrDefault();

                bool success = await API.Instance.InsertGoal(baruId, goalA, goalB, goalC);

                if (success)
                {
                    Alarme.ShowSnackbar("저장 성공", "알람", Wpf.Ui.Controls.ControlAppearance.Success);
                }
                else
                {
                    Alarme.ShowSnackbar("저장 실패", "알람");
                }
            }
        }
    }

    public partial class ExerciseGoalViewModel : ObservableObject
    {
        public ExerciseGoalViewModel(string exerciseName, int goal, int currentCount)
        {
            ExerciseName = exerciseName;
            Goal = goal;
            CurrentCount = currentCount;
            GoalInputText = goal > 0 ? goal.ToString() : "";
        }

        [ObservableProperty]
        private string exerciseName;

        [ObservableProperty]
        private int goal;

        [ObservableProperty]
        private string goalInputText;

        [ObservableProperty]
        private int currentCount;

        public double ProgressBarValue => Goal > 0 ? Math.Min(CurrentCount * 100.0 / Goal, 100) : 0;

        public double AchievementPercentage => Goal > 0 ? (CurrentCount * 100.0 / Goal) : 0;

        partial void OnGoalChanged(int value)
        {
            OnPropertyChanged(nameof(ProgressBarValue));
            OnPropertyChanged(nameof(AchievementPercentage));
        }
        partial void OnCurrentCountChanged(int value)
        {
            OnPropertyChanged(nameof(ProgressBarValue));
            OnPropertyChanged(nameof(AchievementPercentage));
        }
    }

}
