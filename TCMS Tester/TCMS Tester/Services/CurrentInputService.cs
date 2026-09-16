using System;
using System.Globalization;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CITester.Services
{
    public class CurrentInputService : IDisposable
    {
        private SerialPort _serialPort;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public bool IsOpen => _serialPort != null && _serialPort.IsOpen;
        public string PortName => _serialPort?.PortName ?? string.Empty;

        public Action<string> OnLog { get; set; }
        public Action<string> OnError { get; set; }

        public bool Open(string portName, int baudRate = 9600)
        {
            try
            {
                Close();

                _serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One)
                {
                    NewLine = "\r\n",
                    ReadTimeout = 500,
                    WriteTimeout = 500
                };

                _serialPort.Open();
                _serialPort.DiscardInBuffer();
                _serialPort.DiscardOutBuffer();

                OnLog?.Invoke($"[전류입력] 포트 오픈 성공: {portName} ({baudRate} bps)");
                return true;
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"[전류입력] 포트 오픈 실패 ({portName}): {ex.Message}");
                return false;
            }
        }

        public void Close()
        {
            if (_serialPort != null)
            {
                try
                {
                    if (_serialPort.IsOpen) _serialPort.Close();
                }
                catch { }
                finally
                {
                    _serialPort.Dispose();
                    _serialPort = null;
                }
            }
        }

        /// <summary>
        /// 1. 버전 읽기 (get.version.x / x는 0 고정)
        /// </summary>
        public async Task<string> GetVersionAsync(int boardIdx = 0, int timeoutMs = 500)
        {
            string cmd = $"get.version.{boardIdx}";
            string resp = await SendCommandAsync(cmd, timeoutMs);

            if (string.IsNullOrEmpty(resp)) return null;

            if (resp.IndexOf("syntax error", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                OnError?.Invoke($"[전류입력] 버전 읽기 문법 오류 (syntax error)");
                return null;
            }

            // 예: "reply.get.version.0 0.01-0"
            string prefix = $"reply.get.version.{boardIdx}";
            if (resp.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return resp.Substring(prefix.Length).Trim();
            }

            return resp;
        }

        /// <summary>
        /// 2. 장치명 읽기 (get.devicename.x / x는 0 고정)
        /// </summary>
        public async Task<string> GetDeviceNameAsync(int boardIdx = 0, int timeoutMs = 500)
        {
            string cmd = $"get.devicename.{boardIdx}";
            string resp = await SendCommandAsync(cmd, timeoutMs);

            if (string.IsNullOrEmpty(resp)) return null;

            if (resp.IndexOf("syntax error", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                OnError?.Invoke($"[전류입력] 장치명 읽기 문법 오류 (syntax error)");
                return null;
            }

            // 예: "reply.get.devicename.0 aiao-board.0"
            string prefix = $"reply.get.devicename.{boardIdx}";
            if (resp.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return resp.Substring(prefix.Length).Trim();
            }

            return resp;
        }

        /// <summary>
        /// 3. 아날로그 입력(AI) 계측값 읽기 (get.ai.x y)
        /// </summary>
        /// <param name="channel">채널 번호 (0~7)</param>
        /// <param name="boardIdx">보드 주소 (0 고정)</param>
        /// <returns>(성공여부, 계측 전압/전류 값, 원본 응답)</returns>
        public async Task<(bool Success, double Value, string RawResponse)> GetAnalogInputAsync(int channel, int boardIdx = 0, int timeoutMs = 500)
        {
            if (channel < 0 || channel > 7)
            {
                OnError?.Invoke($"[전류입력] 채널 범위 초과: {channel} (0~7 허용)");
                return (false, 0.0, null);
            }

            string cmd = $"get.ai.{boardIdx} {channel}";
            string resp = await SendCommandAsync(cmd, timeoutMs);

            if (string.IsNullOrEmpty(resp))
            {
                OnError?.Invoke($"[전류입력] Ch{channel} 응답 타임아웃");
                return (false, 0.0, null);
            }

            // 오류 응답 검증
            if (resp.IndexOf("syntax error", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                OnError?.Invoke($"[전류입력] 명령어 문법 오류 (syntax error)");
                return (false, 0.0, resp);
            }
            if (resp.IndexOf("the I2C error", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                OnError?.Invoke($"[전류입력] I2C 통신 오류: {resp}");
                return (false, 0.0, resp);
            }
            if (resp.IndexOf("the channel error", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                OnError?.Invoke($"[전류입력] 채널 설정 범위 오류: {resp}");
                return (false, 0.0, resp);
            }

            // 정상 응답 파싱: "reply.get.ai.x y [측정값]"
            try
            {
                string[] parts = resp.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // 마지막 토큰을 실수형(double)으로 변환
                if (parts.Length >= 2 && double.TryParse(parts[parts.Length - 1], NumberStyles.Any, CultureInfo.InvariantCulture, out double measuredVal))
                {
                    OnLog?.Invoke($"[전류입력] Ch{channel} 계측값 수신: {measuredVal:F3}");
                    return (true, measuredVal, resp);
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"[전류입력] AI 데이터 파싱 실패 ({resp}): {ex.Message}");
            }

            return (false, 0.0, resp);
        }

        /// <summary>
        /// 4. 아날로그 출력(AO) 전압 설정 (set.ao.x y z)
        /// </summary>
        /// <param name="channel">채널 번호 (0~7)</param>
        /// <param name="voltage">출력 전압 (0.0 ~ 15.0 V)</param>
        /// <param name="boardIdx">보드 주소 (0 고정)</param>
        public async Task<bool> SetAnalogOutputAsync(int channel, double voltage, int boardIdx = 0, int timeoutMs = 500)
        {
            if (channel < 0 || channel > 7)
            {
                OnError?.Invoke($"[전류입력/AO] 채널 범위 초과: {channel} (0~7 허용)");
                return false;
            }

            if (voltage < 0.0 || voltage > 15.0)
            {
                OnError?.Invoke($"[전류입력/AO] 전압 범위 초과: {voltage:F2}V (0.0~15.0V 허용)");
                return false;
            }

            string strVolt = voltage.ToString("0.0#", CultureInfo.InvariantCulture);
            string cmd = $"set.ao.{boardIdx} {channel} {strVolt}";
            string resp = await SendCommandAsync(cmd, timeoutMs);

            if (string.IsNullOrEmpty(resp))
            {
                OnError?.Invoke($"[전류입력/AO] Ch{channel} 응답 타임아웃");
                return false;
            }

            // 오류 검증
            if (resp.IndexOf("syntax error", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                OnError?.Invoke($"[전류입력/AO] 문법 오류 (syntax error)");
                return false;
            }
            if (resp.IndexOf("the range error", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                OnError?.Invoke($"[전류입력/AO] 설정값 범위 오류: {resp}");
                return false;
            }
            if (resp.IndexOf("the channel error", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                OnError?.Invoke($"[전류입력/AO] 채널 범위 오류: {resp}");
                return false;
            }

            // 정상 응답 확인: "reply.set.ao.x y z"
            string expectedPrefix = $"reply.set.ao.{boardIdx} {channel}";
            if (resp.IndexOf(expectedPrefix, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                OnLog?.Invoke($"[전류입력/AO] 출력 설정 성공 -> Ch{channel}: {strVolt}V");
                return true;
            }

            return false;
        }

        /// <summary>
        /// 시리얼 송수신 및 개행 문자 감지 헬퍼
        /// </summary>
        private async Task<string> SendCommandAsync(string command, int timeoutMs)
        {
            if (!IsOpen)
            {
                OnError?.Invoke("[전류입력] 시리얼 포트가 열려있지 않습니다.");
                return null;
            }

            await _lock.WaitAsync();

            try
            {
                _serialPort.DiscardInBuffer();
                _serialPort.DiscardOutBuffer();

                _serialPort.WriteLine(command);

                return await Task.Run(() =>
                {
                    StringBuilder sb = new StringBuilder();
                    DateTime limit = DateTime.Now.AddMilliseconds(timeoutMs);

                    while (DateTime.Now < limit)
                    {
                        if (_serialPort.BytesToRead > 0)
                        {
                            sb.Append(_serialPort.ReadExisting());

                            string currentStr = sb.ToString();
                            if (currentStr.Contains("\r") || currentStr.Contains("\n"))
                            {
                                return currentStr.Trim();
                            }
                        }
                        Thread.Sleep(10);
                    }

                    return sb.Length > 0 ? sb.ToString().Trim() : null;
                });
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"[전류입력] 송수신 예외: {ex.Message}");
                return null;
            }
            finally
            {
                _lock.Release();
            }
        }

        public void Dispose()
        {
            Close();
            _lock?.Dispose();
        }
    }
}