using System.Net.Http;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using Baru_Client.Data;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Baru_Client.FastAPI
{
    public class API
    {
        #region 싱글톤
        public static API Instance { get; } = new API();

        private API() { }
        #endregion

        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _baseUrl = "http://192.168.219.103:80";
        private readonly string _client_baseUrl = "http://192.168.219.103:8080";

        private string _uid = string.Empty;

        public string MyID = string.Empty;
        public string MyPW = string.Empty;
        public string MyName = string.Empty;

        public BeforeInfo MyData = new BeforeInfo();


        /// <summary>
        /// 로그인 메소드
        /// </summary>
        /// <param name="id">유저 아이디</param>
        /// <param name="pw">유저 비밀번호</param>
        /// <returns>성공시 True</returns>
        public async Task<bool> Login(string id, string pw)
        {
            string endpoint = $"{_baseUrl}/user/login/";

            endpoint += $"?id={id}";
            endpoint += $"&pw={pw}";

            var response = await _httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                string contentString = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ServerResponse>(contentString);

                if (data == null || data.Name == null || data.Data == null)
                    return false;


                #region 로그인 성공 후 데이터 저장

                MyID = id;
                MyPW = pw;
                MyName = data.Name;

                foreach (var exercise in data.Data.Exercises)
                {
                    exercise.Init_Data();
                }
                MyData = data.Data;

                #endregion

                return true;
            }
            return false;
        }

        /// <summary>
        /// 회원가입 메소드
        /// </summary>
        /// <param name="id">유저아이디</param>
        /// <param name="pw">유저비번</param>
        /// <param name="username">닉넴</param>
        /// <returns>성공시 True</returns>
        public async Task<bool> SignupAsync(string id, string pw, string username)
        {
            string endpoint = $"{_baseUrl}/user/insert/";

            endpoint += $"?id={id}";
            endpoint += $"&pw={pw}";
            endpoint += $"&name={username}";

            var response = await _httpClient.PostAsync(endpoint, null);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// 목표치 업데이트 메소드
        /// </summary>
        /// <returns></returns>
        public async Task<bool> InsertGoal(string baruId, int GoalA, int GoalB, int GoalC)
        {
            string endpoint = $"{_baseUrl}/data/insert/beforeinfo";
            var beforeInfo = new
            {
                baru_id = baruId,
                goalA = GoalA,
                goalB = GoalB,
                goalC = GoalC
            };

            string json = JsonConvert.SerializeObject(beforeInfo);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(endpoint, content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        

        public async Task<bool> InsertExercise(string exercise)
        {
            string url = $"{_client_baseUrl}/start/";
            url += $"?exercise={exercise}";

            var response = await _httpClient.PostAsync(url, null);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var obj = JObject.Parse(json);
                _uid = obj["uid"]?.ToString();
                return !string.IsNullOrEmpty(_uid);
            }

            return false;
        }

        public string GetCurrentUid() => _uid;
        public async Task<Current?> Getcurrent(string uid)
        {
            if (string.IsNullOrEmpty(uid)) return null;
            string url = $"{_client_baseUrl}/get/current/?uid={uid}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Current>(json);
                return result;
            }
            return null;
        }

        public async Task<Current?> GetAccuary()
        {
            if (string.IsNullOrEmpty(MyID)) return null;
            string url = $"{_client_baseUrl}/get/accuracy/?id={MyID}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Current>(json);

                _uid = string.Empty;
                return result;
            }
            else
                return null;
        }

        public async Task<bool> LoadBeforeInfoAsync()
        {
            string url = $"{_baseUrl}/data/get/?id={MyID}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return false;
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<BeforeInfo>(json);

            if (data == null)
                return false;

            foreach (var exercise in data.Exercises)
            {
                exercise.Init_Data();
            }
            MyData = data;

            return true;
        }
    }
}
