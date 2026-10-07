using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace TCMSTester.Services
{
    /// <summary>
    /// ER 제어기 SSH 원격 제어 서비스
    /// (상주 앱 종료, diag_gp 2번 모드 가동 및 CPM 포맷 자동화)
    /// </summary>
    public class ErSshService : IDisposable
    {
        private SshClient _sshClient;
        private ShellStream _shellStream;
        private readonly StringBuilder _consoleBuffer = new StringBuilder();
        private bool _isDisposed = false;

        /// <summary>
        /// 조합시험(diag_gp 2번) 구동 중 여부
        /// </summary>
        public bool IsRunning { get; private set; } = false;

        /// <summary>
        /// 상태 및 터미널 출력 로그 콜백
        /// </summary>
        public Action<string> OnLog { get; set; }

        /// <summary>
        /// PC 로컬에 캐시된 대상 IP의 SSH Host Key(known_hosts)를 자동 초기화합니다.
        /// (REMOTE HOST IDENTIFICATION HAS CHANGED 에러 방지)
        /// </summary>
        private void RemoveHostKeyFromKnownHosts(string targetIp)
        {
            try
            {
                string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string knownHostsPath = Path.Combine(userProfile, ".ssh", "known_hosts");

                if (File.Exists(knownHostsPath))
                {
                    var lines = File.ReadAllLines(knownHostsPath);
                    var updatedLines = new List<string>();
                    bool removed = false;

                    foreach (var line in lines)
                    {
                        // 대상 IP가 포함된 기존 호스트 키 라인은 제외(삭제)
                        if (line.StartsWith(targetIp) || line.StartsWith($"[{targetIp}]") || line.Contains(targetIp))
                        {
                            removed = true;
                            continue;
                        }
                        updatedLines.Add(line);
                    }

                    if (removed)
                    {
                        File.WriteAllLines(knownHostsPath, updatedLines);
                        OnLog?.Invoke($"[SSH] known_hosts 파일에서 {targetIp} 이전 호스트 키 캐시 초기화 완료.");
                    }
                }
            }
            catch (Exception ex)
            {
                OnLog?.Invoke($"[SSH 경고] known_hosts 캐시 정리 예외: {ex.Message}");
            }
        }

        /// <summary>
        /// ER 제어기 접속 -> AREXEvrApp.x 종료 -> diag_gp 실행 -> '2'번(Combination Test) 진입
        /// </summary>
        public async Task<bool> StartCombinationModeAsync(string targetIp, int timeoutSeconds = 15)
        {
            return await Task.Run(() =>
            {
                try
                {
                    Close(); // 기존 세션이 있다면 초기화

                    // 1. 호스트 키 변경 에러 방지: 로컬 known_hosts에서 이전 키 캐시 자동 제거
                    RemoveHostKeyFromKnownHosts(targetIp);

                    OnLog?.Invoke($"[SSH] ER 제어기({targetIp}:22) 원격 접속 시도 중...");

                    var connectionInfo = new ConnectionInfo(targetIp, 22, "root", new PasswordAuthenticationMethod("root", ""))
                    {
                        Timeout = TimeSpan.FromSeconds(3)
                    };

                    // 2. 부팅 직후 sshd 데몬 기동 지연(Connection Refused) 극복을 위한 재시도 루프
                    DateTime deadline = DateTime.Now.AddSeconds(timeoutSeconds);
                    bool isConnected = false;
                    string lastError = string.Empty;

                    while (DateTime.Now < deadline && !_isDisposed)
                    {
                        try
                        {
                            if (_sshClient != null)
                            {
                                try { _sshClient.Dispose(); } catch { }
                                _sshClient = null;
                            }

                            _sshClient = new SshClient(connectionInfo);
                            _sshClient.HostKeyReceived += (s, e) => { e.CanTrust = true; }; // 지문 변경 경고 무시 및 신뢰 승인
                            _sshClient.Connect();

                            if (_sshClient.IsConnected)
                            {
                                isConnected = true;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            lastError = ex.Message;
                            // 포트 22 미개방(연결 거부) 상태이면 1초 대기 후 재접속
                            Thread.Sleep(1000);
                        }
                    }

                    if (!isConnected)
                    {
                        OnLog?.Invoke($"[SSH 에러] SSH 연결 실패 ({timeoutSeconds}초 타임아웃): {lastError}");
                        return false;
                    }

                    OnLog?.Invoke("[SSH] 로그인 성공. 가상 터미널 세션 생성 중...");
                    _consoleBuffer.Clear();
                    _shellStream = _sshClient.CreateShellStream("vt100", 120, 40, 800, 600, 8192);
                    _shellStream.DataReceived += (s, e) =>
                    {
                        string text = Encoding.UTF8.GetString(e.Data);
                        lock (_consoleBuffer)
                        {
                            _consoleBuffer.Append(text);
                        }

                        // 터미널 제어문자 및 개행 정리 후 UI 로그로 전송
                        string cleanText = text.Trim('\r', '\n');
                        if (!string.IsNullOrWhiteSpace(cleanText))
                        {
                            OnLog?.Invoke($"[SSH 콘솔] {cleanText}");
                        }
                    };

                    Thread.Sleep(400);

                    // 3. /tmp 이동 및 기존 상주 앱 강제 종료
                    OnLog?.Invoke("[SSH] 기존 운영 프로그램(AREXEvrApp.x) 강제 종료 명령 전송...");
                    SendLine("cd /tmp && killall AREXEvrApp.x");
                    Thread.Sleep(800);

                    // 4. diag_gp 진단 데몬 구동
                    OnLog?.Invoke("[SSH] 진단 데몬(diag_gp) 실행...");
                    SendLine("diag_gp");

                    // 5. 메뉴 프롬프트 대기 후 2번 선택 전송
                    WaitForText("Select Menu", 4000);
                    Thread.Sleep(200);

                    OnLog?.Invoke("[SSH] '2'번(Combination Test) 자동 입력 전송...");
                    SendLine("2");

                    // 6. 모듈 활성화 완료 문구 확인 (CombiDispModule Module Start)
                    bool isReady = WaitForText("CombiDispModule Module Start", timeoutSeconds * 1000);
                    if (isReady)
                    {
                        IsRunning = true;
                        OnLog?.Invoke("[SSH] ★ ER Combination Test 모드 준비 완료! (UDP 포트 57722 활성화)");
                        return true;
                    }

                    OnLog?.Invoke("[SSH 에러] Combination Test 모드 기동 확인 타임아웃");
                    return false;
                }
                catch (Exception ex)
                {
                    OnLog?.Invoke($"[SSH 예외] {ex.Message}");
                    Close();
                    return false;
                }
            });
        }

        /// <summary>
        /// diag_gp를 강제 종료한 뒤 매뉴얼 그대로 AREXCPMTool.x -i를 실행하고 'Y'를 입력하여 CPM을 초기화합니다.
        /// </summary>
        public async Task<bool> RunCpmFormatAsync(int timeoutMs = 120000)
        {
            return await Task.Run(() =>
            {
                try
                {
                    OnLog?.Invoke("\n-------------------------------------------------------------");
                    OnLog?.Invoke("[CPM 초기화] CPM 포맷 도구(AREXCPMTool.x -i) 자동 실행 준비...");

                    // 1. diag_gp 프로세스 종료 (Ctrl+C 및 killall)
                    if (_shellStream != null && _shellStream.CanWrite)
                    {
                        _shellStream.Write(new byte[] { 0x03, 0x03 }, 0, 2); // Ctrl+C 2회
                        _shellStream.Flush();
                    }
                    Thread.Sleep(200);

                    if (_sshClient != null && _sshClient.IsConnected)
                    {
                        try
                        {
                            using (var cmd = _sshClient.CreateCommand("killall -9 diag_gp"))
                            {
                                cmd.Execute();
                            }
                        }
                        catch { }
                    }

                    Thread.Sleep(500);
                    IsRunning = false;

                    // 2. 쉘 버퍼 정리 및 프롬프트 복귀 확인
                    lock (_consoleBuffer)
                    {
                        _consoleBuffer.Clear();
                    }
                    SendLine("");
                    Thread.Sleep(300);

                    // 3. AREXCPMTool.x -i 단독 실행
                    OnLog?.Invoke("[CPM 초기화] AREXCPMTool.x -i 실행");
                    SendLine("AREXCPMTool.x -i");

                    // 4. 'Initialize CPM? (Y/N)' 프롬프트 대기
                    bool promptFound = WaitForText("Initialize CPM", 8000) || WaitForText("Y/N", 3000);
                    if (!promptFound)
                    {
                        OnLog?.Invoke("[CPM 초기화 경고] 프롬프트 응답 지연 -> 대문자 'Y' 강제 전송 시도");
                    }

                    Thread.Sleep(400);
                    OnLog?.Invoke("[CPM 초기화] 'Y' (포맷 승인) 전송");
                    SendLine("Y");

                    // 5. 완료 문구 확인 (InitCPM Ok! 또는 실패 감지)
                    DateTime expireTime = DateTime.Now.AddMilliseconds(timeoutMs);
                    while (DateTime.Now < expireTime && !_isDisposed)
                    {
                        string currentOutput;
                        lock (_consoleBuffer)
                        {
                            currentOutput = _consoleBuffer.ToString();
                        }

                        // 성공 판정
                        if (currentOutput.IndexOf("InitCPM Ok", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            OnLog?.Invoke("[CPM 초기화] ★ CPM 초기화 완료! (InitCPM Ok! 확인)");
                            OnLog?.Invoke("-------------------------------------------------------------\n");
                            return true;
                        }

                        // 하드웨어 통신 오류 판정
                        if (currentOutput.IndexOf("Initialize CPM Failed", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            currentOutput.IndexOf("poll timeout", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            OnLog?.Invoke("[CPM 초기화 실패] memif poll timeout 발생 (CPM 하드웨어 케이블/전원 확인 필요)");
                            OnLog?.Invoke("-------------------------------------------------------------\n");
                            return false;
                        }

                        Thread.Sleep(150);
                    }

                    OnLog?.Invoke("[CPM 초기화 에러] CPM 초기화 응답 타임아웃");
                    OnLog?.Invoke("-------------------------------------------------------------\n");
                    return false;
                }
                catch (Exception ex)
                {
                    OnLog?.Invoke($"[CPM 초기화 예외] {ex.Message}");
                    return false;
                }
            });
        }

        /// <summary>
        /// 가상 쉘에 리눅스 개행(\n)으로 명령 전송
        /// </summary>
        private void SendLine(string cmd)
        {
            if (_shellStream != null && _shellStream.CanWrite)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(cmd + "\n");
                _shellStream.Write(bytes, 0, bytes.Length);
                _shellStream.Flush();
            }
        }

        /// <summary>
        /// 대소문자를 구분하지 않고 터미널 출력 버퍼에서 특정 문자열이 수신될 때까지 대기
        /// </summary>
        private bool WaitForText(string targetText, int timeoutMs)
        {
            DateTime expireTime = DateTime.Now.AddMilliseconds(timeoutMs);
            while (DateTime.Now < expireTime && !_isDisposed)
            {
                lock (_consoleBuffer)
                {
                    if (_consoleBuffer.ToString().IndexOf(targetText, StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;
                }
                Thread.Sleep(100);
            }
            return false;
        }

        /// <summary>
        /// 실행 중인 진단 프로세스 정리 및 세션 종료
        /// </summary>
        public void Close()
        {
            IsRunning = false;
            try
            {
                if (_shellStream != null)
                {
                    try
                    {
                        _shellStream.Write(new byte[] { 0x03, 0x03 }, 0, 2);
                        _shellStream.Flush();
                    }
                    catch { }

                    _shellStream.Dispose();
                }

                if (_sshClient != null && _sshClient.IsConnected)
                {
                    try
                    {
                        using (var cmd = _sshClient.CreateCommand("killall -9 diag_gp"))
                        {
                            cmd.Execute();
                        }
                    }
                    catch { }

                    _sshClient.Disconnect();
                    _sshClient.Dispose();
                }
            }
            catch { }
            _shellStream = null;
            _sshClient = null;
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            Close();
        }
    }
}