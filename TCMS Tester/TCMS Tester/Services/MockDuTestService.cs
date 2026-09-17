using System;
using System.Drawing;
using System.Threading.Tasks;

namespace TCMSTester.Services
{
    public class MockDuTestService : IDuTestService
    {
        public string TargetIp { get; set; } = "10.0.1.21";
        public int TargetPort { get; set; } = 5060;

        public Action<string, Color>? OnLog { get; set; }
        public Action<string, Color>? OnFailLog { get; set; }

        // 부팅 완료 플래그 (1회차에만 20초 대기, 2회차부터는 즉시 통과)
        private bool _isBooted = false;

        /// <summary>
        /// 첫 통신 시도시 DU 시스템 부팅 대기 (20초 소요, 5초 주기 로그)
        /// </summary>
        private async Task EnsureBootedAsync()
        {
            if (_isBooted) return;

            OnLog?.Invoke("[가상 DU] DU 표시기 전원 인가 감지. 시스템 부팅 및 OS 초기화 대기 중 (약 20초 소요)...", Color.DarkCyan);

            const int BOOT_SECONDS = 20;
            for (int sec = 1; sec <= BOOT_SECONDS; sec++)
            {
                await Task.Delay(1000);

                if (sec % 5 == 0 || sec == BOOT_SECONDS)
                {
                    OnLog?.Invoke($"[가상 DU] DU 부팅 및 통신 스택 준비 중... ({sec}/{BOOT_SECONDS}초)", Color.Gray);
                }
            }

            _isBooted = true;
            OnLog?.Invoke("[가상 DU] DU 시스템 부팅 완료! 통신 링크 준비 완료.", Color.DarkGreen);
        }

        /// <summary>
        /// 부팅 상태 수동 리셋 (새로운 시험 시작 시 재호출 가능)
        /// </summary>
        public void ResetBoot()
        {
            _isBooted = false;
        }

        public async Task<bool> TestMvbAsync(int timeoutMs = 2000)
        {
            // 첫 호출 시 20초 부팅 대기 수행
            await EnsureBootedAsync();

            await Task.Delay(400); // TCMS 요청 후 응답 수신 대기 시뮬레이션
            OnLog?.Invoke("[가상 DU] TCMS로부터 MVB 정상 응답 패킷 수신 (PASS)", Color.DarkGreen);
            return true;
        }

        public async Task<bool> TestRs485Async(int timeoutMs = 2000)
        {
            // MVB 시험 없이 RS-485만 단독 실행되더라도 부팅 대기 보장
            await EnsureBootedAsync();

            await Task.Delay(400);
            OnLog?.Invoke("[가상 DU] TCMS로부터 RS-485 에코백 패킷 수신 (PASS)", Color.DarkGreen);
            return true;
        }

        public async Task<DuTestResult> RunDuCommTestAsync(int timeoutMs = 3000)
        {
            await EnsureBootedAsync();

            bool mvb = await TestMvbAsync(timeoutMs);
            bool rs485 = await TestRs485Async(timeoutMs);

            return new DuTestResult
            {
                IsMvbPass = mvb,
                IsRs485Pass = rs485,
                MvbStatusMessage = mvb ? "정상" : "통신 실패",
                Rs485StatusMessage = rs485 ? "정상" : "루프백 에러"
            };
        }

        public void Dispose()
        {
            _isBooted = false;
        }
    }
}