using Baru_Client.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Baru_Client.Alarmes
{
    class TimerExample
    {
        private static readonly string[] messages = new string[]
        {
            "잠시 일어나서 몸을 쭉 펴보세요! 스트레칭 시간입니다.",
            "오랜 시간 앉아있었죠? 간단한 스트레칭으로 혈액순환을 도와주세요.",
            "목과 어깨가 뻐근하다면, 잠깐 스트레칭으로 풀어주세요!",
            "작은 움직임이 큰 변화를 만듭니다. 지금 바로 스트레칭하세요!",
            "건강을 위해 1분만 투자! 가벼운 스트레칭 해볼까요?",
            "바쁜 하루, 잠깐 멈추고 몸을 편안하게 만들어 주세요.",
            "장시간 컴퓨터 앞에 앉아 있다면, 스트레칭으로 자세를 바로잡아 주세요.",
            "피로가 쌓였나요? 스트레칭으로 에너지 충전하세요!",
            "몸이 굳기 전에, 잠시 일어나서 가볍게 스트레칭 해봐요.",
            "건강한 습관, 1시간마다 스트레칭! 지금 바로 시작해요."
        };

        private static readonly Random random = new Random();

        public static string GetRandomStretchMessage()
        {
            int index = random.Next(messages.Length);
            return messages[index];
        }

        private static Timer? _timer;

        public static void StartTimer()
        {
            _timer?.Stop();
            _timer?.Dispose();

            _timer = new Timer(TimeSpan.FromHours(1).TotalMilliseconds);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        private static void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Toast.ShowToast(GetRandomStretchMessage());
        }

    }
}
