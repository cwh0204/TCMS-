using System;
using System.Drawing;
using System.Threading.Tasks;

namespace TCMSTester.Services
{
    public class MockVaioTestService : IVaioTestService
    {
        public string TargetIp { get; set; } = "127.0.0.1";
        public int TargetPort { get; set; } = 5060;

        public Action<string, Color>? OnLog { get; set; }
        public Action<string, Color>? OnFailLog { get; set; }

        // ★ AO 4개 채널 실측 전압 보관
        public double[] LastAoVoltages { get; private set; } = new double[] { 5.0, 5.0, 5.0, 5.0 };

        private readonly Random _rand = new Random();

        public async Task<VaioAiResult?> ReadAnalogInputsAsync(int timeoutMs = 2000)
        {
            await Task.Delay(50);

            // AI 정밀 오차 (±0.01 ~ ±0.05)
            int ch1Offset = GetRandomJitterCount(1, 5);
            ushort raw1 = (ushort)Math.Max(0, 500 + ch1Offset);

            int ch2Offset = GetRandomJitterCount(1, 2);
            ushort raw2 = (ushort)Math.Max(0, 500 + ch2Offset);

            int ch3Offset = GetRandomJitterCount(1, 5);
            ushort raw3 = (ushort)Math.Max(0, 500 + ch3Offset);

            int ch4Offset = GetRandomJitterCount(1, 5);
            ushort raw4 = (ushort)Math.Max(0, 500 + ch4Offset);

            int ch5Offset = GetRandomJitterCount(1, 3);
            ushort raw5 = (ushort)(1500 + ch5Offset);

            var result = new VaioAiResult
            {
                RawCh1 = raw1,
                RawCh2 = raw2,
                RawCh3 = raw3,
                RawCh4 = raw4,
                RawCh5 = raw5
            };

            OnLog?.Invoke(
                $"[VAIO-RX] Ch1:{result.Ch1_mA:F2}mA | Ch2:{result.Ch2_mA:F2}mA | Ch3:{result.Ch3_V:F2}V | Ch4:{result.Ch4_V:F2}V | Mascon:{result.Ch5_V:F2}V",
                Color.DarkGreen);

            return result;
        }

        public async Task<bool> WriteAnalogOutputsAsync(double v1, double v2, double v3, double v4, int timeoutMs = 1500)
        {
            await Task.Delay(30);

            // ★ AO 출력 피드백에도 각각 ±0.01 ~ ±0.05V 오차 부여
            double v1_fb = Math.Round(v1 + (GetRandomJitterCount(1, 5) * 0.01), 2);
            double v2_fb = Math.Round(v2 + (GetRandomJitterCount(1, 5) * 0.01), 2);
            double v3_fb = Math.Round(v3 + (GetRandomJitterCount(1, 5) * 0.01), 2);
            double v4_fb = Math.Round(v4 + (GetRandomJitterCount(1, 5) * 0.01), 2);

            LastAoVoltages = new double[] { v1_fb, v2_fb, v3_fb, v4_fb };

            OnLog?.Invoke(
                $"[VAIO-AO] 5.0V 출력 피드백 실측 -> Ch1:{v1_fb:F2}V | Ch2:{v2_fb:F2}V | Ch3:{v3_fb:F2}V | Ch4:{v4_fb:F2}V (정상 반영)",
                Color.DarkGreen);

            return true;
        }

        public void Dispose() { }

        private int GetRandomJitterCount(int minCount, int maxCount)
        {
            int sign = _rand.Next(0, 2) == 0 ? -1 : 1;
            return sign * _rand.Next(minCount, maxCount + 1);
        }
    }
}