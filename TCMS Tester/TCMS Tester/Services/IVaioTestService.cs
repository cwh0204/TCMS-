using System;
using System.Drawing;
using System.Threading.Tasks;

namespace TCMSTester.Services
{
    public interface IVaioTestService : IDisposable
    {
        string TargetIp { get; set; }
        int TargetPort { get; set; }

        Action<string, Color>? OnLog { get; set; }
        Action<string, Color>? OnFailLog { get; set; }

        double[] LastAoVoltages { get; }

        Task<VaioAiResult?> ReadAnalogInputsAsync(int timeoutMs = 2000);
        Task<bool> WriteAnalogOutputsAsync(double v1, double v2, double v3, double v4, int timeoutMs = 1500);
    }
}