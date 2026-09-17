using System;
using System.Drawing;
using System.Threading.Tasks;

namespace TCMSTester.Services
{
    public interface IDuTestService : IDisposable
    {
        string TargetIp { get; set; }
        int TargetPort { get; set; }

        Action<string, Color>? OnLog { get; set; }
        Action<string, Color>? OnFailLog { get; set; }

        /// <summary>
        /// DU MVB 통신 링크 및 데이터 송수신 시험
        /// </summary>
        Task<bool> TestMvbAsync(int timeoutMs = 2000);

        /// <summary>
        /// DU RS-485 통신 송수신 루프백 시험
        /// </summary>
        Task<bool> TestRs485Async(int timeoutMs = 2000);

        /// <summary>
        /// MVB 및 RS-485 일괄 시험 실행
        /// </summary>
        Task<DuTestResult> RunDuCommTestAsync(int timeoutMs = 3000);
    }
}