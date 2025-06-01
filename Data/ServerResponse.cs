using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Baru_Client.Data
{
    public class ServerResponse
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("data")]
        public BeforeInfo? Data { get; set; }
    }

    public class BeforeInfo
    {
        [JsonProperty("목표치A")]
        public int? GoalA { get; set; }

        [JsonProperty("목표치B")]
        public int? GoalB { get; set; }

        [JsonProperty("목표치C")]
        public int? GoalC { get; set; }

        [JsonProperty("운동들")]
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();
    }

    public class Exercise
    {
        [JsonProperty("날짜")]
        public string? Date { get; set; }

        [JsonIgnore]
        public List<E_Data> E_data { get; set; } = new List<E_Data>();

        [JsonExtensionData]
        public Dictionary<string, JToken> ExerciseTypes { get; set; } = new Dictionary<string, JToken>();

        public void Init_Data()
        {
            E_data.Clear();

            if (ExerciseTypes == null || ExerciseTypes.Count == 0)
            {
                return;
            }

            foreach (var temp in ExerciseTypes)
            {
                string exerciseName = temp.Key;
                JToken? exerciseDetails = temp.Value;

                int count = exerciseDetails?["총개수"]?.Value<int>() ?? 0;

                List<double> accList = new List<double>();
                if (exerciseDetails?["정확도"] is JArray accuracyArray)
                {
                    try
                    {
                        accList = accuracyArray.Select(x => (double)x).ToList();
                    }
                    catch (Exception)
                    {
                    }
                }

                E_data.Add(new E_Data
                {
                    Name = exerciseName,
                    Count = count,
                    Acc = accList
                });
            }
        }
    }

    public class E_Data
    {
        public string? Name { get; set; }
        public int Count { get; set; }
        public List<double> Acc { get; set; } = new List<double>();
    }

    public class Current
    {
        public int Count { get; set; }
        public List<double> Accuracies { get; set; } = new List<double>();
    }
}
