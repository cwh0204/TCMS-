using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CITester.Services
{
    public class CurrentOutputService : IDisposable
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

                OnLog?.Invoke($"[전류출력] 포트 오픈 성공: {portName} ({baudRate} bps)");
                return true;
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"[전류출력] 포트 오픈 실패 ({portName}): {ex.Message}");
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
        /// 1. 버전 읽기 (get.version.x)
        /// </summary>
        public async Task<string> GetVersionAsync(int boardIdx = 0, int timeoutMs = 500)
        {
            string cmd = $"get.version.{boardIdx}";
            string resp = await SendCommandAsync(cmd, timeoutMs);

            if (string.IsNullOrEmpty(resp)) return null;

            // 예: "reply.get.version.0 0.01-0"
            string prefix = $"reply.get.version.{boardIdx}";
            if (resp.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return resp.Substring(prefix.Length).Trim();
            }

            return resp;
        }

        /// <summary>
        /// 2. 장치명 읽기 (get.devicename.x)
        /// </summary>
        public async Task<string> GetDeviceNameAsync(int boardIdx = 0, int timeoutMs = 500)
        {
            string cmd = $"get.devicename.{boardIdx}";
            string resp = await SendCommandAsync(cmd, timeoutMs);

            if (string.IsNullOrEmpty(resp)) return null;

            // 예: "reply.get.devicename.0 voltage to current"
            string prefix = $"reply.get.devicename.{boardIdx}";
            if (resp.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return resp.Substring(prefix.Length).Trim();
            }

            return resp;
        }

        /// <summary>
        /// 3. 전류 출력 설정 (set.current.x y z)
        /// </summary>
        /// <param name="boardIdx">보드 위치 (0~15)</param>
        /// <param name="channel">채널 (0~7)</param>
        /// <param name="current_mA">인가할 전류 (0~100 mA)</param>
        public async Task<bool> SetCurrentAsync(int boardIdx, int channel, int current_mA, int timeoutMs = 500)
        {
            if (boardIdx < 0 || boardIdx > 15 || channel < 0 || channel > 7 || current_mA < 0 || current_mA > 100)
            {
                OnError?.Invoke($"[전류출력] 파라미터 범위 초과 (보드:{boardIdx}, 채널:{channel}, 전류:{current_mA})");
                return false;
            }

            string cmd = $"set.current.{boardIdx} {channel} {current_mA}";
            string resp = await SendCommandAsync(cmd, timeoutMs);

            if (string.IsNullOrEmpty(resp))
            {
                OnError?.Invoke($"[전류출력] Ch{channel} 응답 타임아웃");
                return false;
            }

            // 오류 체크: "the range error.x y z"
            if (resp.IndexOf("the range error", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                OnError?.Invoke($"[전류출력] 보드 에러 반환: {resp}");
                return false;
            }

            // 성공 체크: "reply.set.current.x y z"
            string expectedPrefix = $"reply.set.current.{boardIdx} {channel} {current_mA}";
            if (resp.IndexOf(expectedPrefix, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                OnLog?.Invoke($"[전류출력] 설정 완료 -> Ch{channel}: {current_mA} mA");
                return true;
            }

            return false;
        }

        /// <summary>
        /// 4. 설정된 전류값 읽기 (get.setcurrent.x y)
        /// </summary>
        public async Task<(bool Success, int Current_mA)> GetSetCurrentAsync(int boardIdx, int channel, int timeoutMs = 500)
        {
            string cmd = $"get.setcurrent.{boardIdx} {channel}";
            string resp = await SendCommandAsync(cmd, timeoutMs);

            if (string.IsNullOrEmpty(resp)) return (false, -1);

            // 오류 체크: "the channel error.x"
            if (resp.IndexOf("the channel error", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                OnError?.Invoke($"[전류출력] 채널 에러: {resp}");
                return (false, -1);
            }

            // 응답 파싱: "reply.get.setcurrent.y z" 형태 분리
            try
            {
                string[] parts = resp.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && int.TryParse(parts[parts.Length - 1], out int currentVal))
                {
                    return (true, currentVal);
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"[전류출력] 응답 파싱 실패 ({resp}): {ex.Message}");
            }

            return (false, -1);
        }

        /// <summary>
        /// 저수준 Request-Reply 시리얼 전송 공통 함수
        /// </summary>
        private async Task<string> SendCommandAsync(string command, int timeoutMs)
        {
            if (!IsOpen)
            {
                OnError?.Invoke("[전류출력] 시리얼 포트가 열려있지 않습니다.");
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

                            // 줄바꿈이 감지되면 패킷 수신 완료로 판단
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
                OnError?.Invoke($"[전류출력] 송수신 오류: {ex.Message}");
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