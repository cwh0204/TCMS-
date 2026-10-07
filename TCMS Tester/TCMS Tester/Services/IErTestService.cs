using System;
using System.Drawing;
using System.Threading.Tasks;

namespace TCMSTester.Services
{
    public interface IErTestService : IDisposable
    {
        string TargetIp { get; set; }
        int TargetPort { get; set; }
        string TesterIp { get; set; }
        int TesterPort { get; set; }

        Action<string, Color> OnLog { get; set; }
        Action<string, Color> OnFailLog { get; set; }

        // SSH 연동 및 CPM 포맷
        Task<bool> EnsureSshReadyAsync();
        Task<bool> FormatCpmAsync();

        // 디지털 입출력
        Task<bool[]> ReadDigitalInputsAsync(int timeoutMs = 2000);
        Task<bool> SetDigitalOutputAsync(int channel, bool state, int timeoutMs = 2000);
        Task<bool> SetDigitalOutputMaskAsync(byte outputMask, int timeoutMs = 2000);

        // DI 단일 채널 PLC ON/OFF 자동 검증 메서드
        Task<bool> VerifyDiChannelAsync(int channelIndex, Func<bool, Task> setPlcAction, int timeoutMs = 1500);

        // 통신 시험 (5종)
        Task<bool> TestLanPingAsync(int timeoutMs = 3000);
        Task<bool> TestTargetProbeAsync(int timeoutMs = 120000);
        Task<bool> TestMvbAsync(int timeoutMs = 3000);
        Task<bool> TestCpmAsync(int timeoutMs = 120000);
        Task<bool> TestUsbAsync(int timeoutMs = 120000);

        // 메모리 및 주변장치 시험 (5종)
        Task<bool> TestFlashMemoryAsync(int timeoutMs = 120000);
        Task<bool> TestSdramAsync(int timeoutMs = 120000);
        Task<bool> TestFramAsync(int timeoutMs = 120000);
        Task<bool> TestRtcAsync(int timeoutMs = 120000);
        Task<bool> TestHrsAsync(int timeoutMs = 120000);
    }
}