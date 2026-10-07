using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO.Ports;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using main;
using TCMSTester;
using CITester.Services;
using static CITester.FormLoad;
using static CITester.FormMain;

namespace CITester
{
    public partial class FormDiagnosis : Form
    {
        FormMain frmMain = null;
        int m_nStep = 0;
        int m_nSubStep = 0;

        private List<string> m_lstFailItems = new List<string>();

        public FormDiagnosis()
        {
            InitializeComponent();
        }

        public FormDiagnosis(FormMain formMain)
        {
            InitializeComponent();
            frmMain = formMain;
        }

        private void FormDiagnosis_Load(object sender, EventArgs e)
        {
            //frmMain?.ResetPort();
        }

        public bool IsDiagnosisPassed
        {
            get { return (m_lstFailItems != null && m_lstFailItems.Count == 0); }
        }

        private void Timer_Check_Tick(object sender, EventArgs e)
        {
            // ★ PLC 2 추가로 총 6단계 완료 시 페이드아웃 실행
            if (m_nStep >= 6)
            {
                Timer_Check.Interval = 20;
                Opacity = Opacity - 0.05;

                if (Opacity <= 0)
                {
                    Timer_Check.Enabled = false;

                    // 저항/NTC/PT100/광보드 설정 초기화
                    frmMain.Trimmer_No_Ch_Value_Send("0", "0", "30");
                    frmMain.Trimmer_No_Ch_Value_Send("0", "1", "30");
                    frmMain.Trimmer_No_Ch_Value_Send("0", "2", "30");
                    frmMain.Trimmer_No_Ch_Value_Send("0", "3", "30");

                    frmMain.Trimmer2_No_Ch_Value_Send("0", "0", "237");
                    frmMain.Trimmer2_No_Ch_Value_Send("0", "1", "237");
                    frmMain.Trimmer2_No_Ch_Value_Send("0", "2", "237");
                    frmMain.Trimmer2_No_Ch_Value_Send("0", "3", "237");
                    frmMain.Trimmer2_No_Ch_Value_Send("0", "4", "237");
                    frmMain.Trimmer2_No_Ch_Value_Send("0", "5", "237");

                    for (int nIdx = 0; nIdx <= 4; nIdx++)
                    {
                        frmMain.OpticalCmd_Hz_Send(nIdx.ToString(), "10000");
                        frmMain.OpticalCmd_Duty_Send(nIdx.ToString(), "100");
                    }
                    for (int nIdx = 0; nIdx <= 5; nIdx++)
                    {
                        frmMain.OpticalCmd2_Hz_Send(nIdx.ToString(), "10000");
                        frmMain.OpticalCmd2_Duty_Send(nIdx.ToString(), "100");
                    }

                    string strSummaryHead = "[자가진단 결과]";
                    string strCheckCable = "위에 나열된 장치들의 케이블 연결 상태를 확인해 주십시오.";

                    bool bAllPassed = (m_lstFailItems.Count == 0);

                    // 메인 폼에 자가진단 최종 결과 전달
                    frmMain?.UpdateDiagnosisResultState(bAllPassed);

                    if (!bAllPassed)
                    {
                        string strMessage = $"{strSummaryHead}\n" + string.Join("\n", m_lstFailItems) + $"\n\n{strCheckCable}";
                        MessageBox.Show(strMessage, "자가진단 알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    Close();
                    return;
                }
            }

            Timer_Check.Enabled = true;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        // =========================================================================
        // [전체 자가진단] 6개 항목 순차 자동 실행
        // =========================================================================
        private async void buttonTestStart_Click(object sender, EventArgs e)
        {
            buttonTestStart.Enabled = false;
            Timer_Check.Enabled = false;
            m_lstFailItems.Clear();

            try
            {
                Console.WriteLine("\n================== [전체 자가진단 시퀀스 시작] ==================");

                // 1. PLC 1 통신 진단
                await Diagnose_PLC_Async();
                await Task.Delay(100);

                // 2. ★ PLC 2 통신 진단
                await Diagnose_PLC2_Async();
                await Task.Delay(100);

                // 3. DC 파워 진단
                Diagnose_PowerSupply();
                await Task.Delay(100);

                // 4. MVB 통신 진단 (9600 bps, get.devicename.0 검증)
                await Diagnose_MVB_Async();
                await Task.Delay(100);

                // 5. WTB 통신 진단 (9600 bps, COM2 고정 검증)
                await Diagnose_WTB_Async();
                await Task.Delay(100);

                // 6. 전류 입력 보드 진단
                await Diagnose_InputBoard_Async();
                await Task.Delay(100);

                Console.WriteLine("================== [전체 자가진단 완료] ==================\n");

                // ★ 6단계 완료 플래그 설정 -> Timer_Check 페이드아웃 및 결과 창 실행
                m_nStep = 6;
                Timer_Check.Interval = 20;
                Timer_Check.Enabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[전체 자가진단 오류] {ex.Message}");
            }
            finally
            {
                buttonTestStart.Enabled = true;
            }
        }

        // =========================================================================
        // 1. PLC 1 통신 진단
        // =========================================================================
        private async void button_PLC_Click(object sender, EventArgs e)
        {
            await Diagnose_PLC_Async();
        }

        private async Task Diagnose_PLC_Async()
        {
            Timer_Check.Enabled = false;
            button_PLC.StartNewDiagnosis();
            button_PLC.Enabled = false;

            frmMain?.ClosePlc();
            await Task.Delay(50);

            m_lstFailItems.Remove("PLC 1");
            m_lstFailItems.Remove("PLC");

            // PLC 2가 사용 중인 포트는 후보에서 제외
            string excludePort = ConfigJson.CurrentConfig?.Device?.Plc2_COM;
            List<string> lstCandidatePorts = GetCandidatePorts(ConfigJson.CurrentConfig?.Device?.Plc_COM, excludePort);

            bool bPingSuccess = false;
            string strFoundPort = string.Empty;

            await Task.Run(() =>
            {
                for (int nPortIdx = 0; nPortIdx < lstCandidatePorts.Count; nPortIdx++)
                {
                    string strCurrentPort = lstCandidatePorts[nPortIdx];

                    try
                    {
                        using (SerialPort testPort = new SerialPort(strCurrentPort, 9600, Parity.None, 8, StopBits.One)
                        {
                            ReadTimeout = 200,
                            WriteTimeout = 200
                        })
                        {
                            testPort.Open();
                            testPort.DiscardInBuffer();
                            testPort.DiscardOutBuffer();

                            string reqBody = "00rSS0106%PW005";
                            byte[] txBuf = new byte[reqBody.Length + 4];
                            int txPos = 0;
                            txBuf[txPos++] = 0x05; // ENQ
                            for (int i = 0; i < reqBody.Length; i++) txBuf[txPos++] = (byte)reqBody[i];
                            txBuf[txPos++] = 0x04; // EOT

                            byte bcc = 0;
                            for (int i = 0; i < txPos; i++) bcc += txBuf[i];
                            string bccHex = bcc.ToString("X2");
                            txBuf[txPos++] = (byte)bccHex[0];
                            txBuf[txPos++] = (byte)bccHex[1];

                            testPort.Write(txBuf, 0, txPos);

                            byte[] rxBuf = new byte[256];
                            int totalRead = 0;
                            DateTime dtLimit = DateTime.Now.AddMilliseconds(250);

                            while (DateTime.Now < dtLimit)
                            {
                                if (testPort.BytesToRead > 0)
                                {
                                    int r = testPort.Read(rxBuf, totalRead, rxBuf.Length - totalRead);
                                    totalRead += r;
                                }
                                Thread.Sleep(15);
                            }

                            if (totalRead > 0)
                            {
                                int ackIdx = -1;
                                for (int i = 0; i < totalRead; i++)
                                {
                                    if (rxBuf[i] == 0x06) { ackIdx = i; break; }
                                }

                                if (ackIdx != -1 && (totalRead - ackIdx) >= 7)
                                {
                                    string respHeader = Encoding.ASCII.GetString(rxBuf, ackIdx + 1, 5);
                                    if (respHeader.Equals("00rSS", StringComparison.OrdinalIgnoreCase))
                                    {
                                        bPingSuccess = true;
                                        strFoundPort = strCurrentPort;
                                    }
                                }
                            }

                            testPort.Close();
                        }

                        if (bPingSuccess) break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[PLC 1 핑 예외 - {strCurrentPort}] {ex.Message}");
                    }
                }
            });

            button_PLC.Enabled = true;

            if (!bPingSuccess)
            {
                button_PLC.BackColor = Color.Red;
                button_PLC.CurrentStatus = eDiagStatus.Abnormal;
                if (!m_lstFailItems.Contains("PLC 1")) m_lstFailItems.Add("PLC 1");
                Console.WriteLine("[진단 결과] PLC 1 통신 실패");
            }
            else
            {
                if (ConfigJson.CurrentConfig?.Device != null)
                {
                    ConfigJson.CurrentConfig.Device.Plc_COM = strFoundPort;
                    SaveCurrentJsonConfig();
                }

                button_PLC.BackColor = Color.GreenYellow;
                button_PLC.CurrentStatus = eDiagStatus.Normal;

                if (frmMain != null)
                {
                    bool bOpened = frmMain.PlcStart();
                    Console.WriteLine($"[진단] 메인 폼 PLC 1 포트({strFoundPort}) 상시 오픈: {(bOpened ? "성공" : "실패")}");
                }
            }
        }

        // =========================================================================
        // 2. ★ PLC 2 통신 진단
        // =========================================================================
        private async void button_PLC2_Click(object sender, EventArgs e)
        {
            await Diagnose_PLC2_Async();
        }

        private async Task Diagnose_PLC2_Async()
        {
            Timer_Check.Enabled = false;
            button_PLC2.StartNewDiagnosis();
            button_PLC2.Enabled = false;

            // 메인 폼에 ClosePlc2가 있다면 호출 (없을 경우를 대비해 리플렉션 또는 직접 호출)
            try { frmMain?.ClosePlc2(); } catch { }
            await Task.Delay(50);

            m_lstFailItems.Remove("PLC 2");

            // ★ PLC 1이 이미 점유한 포트는 PLC 2 후보에서 원천 배제
            string excludePort = ConfigJson.CurrentConfig?.Device?.Plc_COM;
            List<string> lstCandidatePorts = GetCandidatePorts(ConfigJson.CurrentConfig?.Device?.Plc2_COM, excludePort);

            bool bPingSuccess = false;
            string strFoundPort = string.Empty;

            await Task.Run(() =>
            {
                for (int nPortIdx = 0; nPortIdx < lstCandidatePorts.Count; nPortIdx++)
                {
                    string strCurrentPort = lstCandidatePorts[nPortIdx];

                    try
                    {
                        using (SerialPort testPort = new SerialPort(strCurrentPort, 9600, Parity.None, 8, StopBits.One)
                        {
                            ReadTimeout = 200,
                            WriteTimeout = 200
                        })
                        {
                            testPort.Open();
                            testPort.DiscardInBuffer();
                            testPort.DiscardOutBuffer();

                            string reqBody = "00rSS0106%PW005";
                            byte[] txBuf = new byte[reqBody.Length + 4];
                            int txPos = 0;
                            txBuf[txPos++] = 0x05; // ENQ
                            for (int i = 0; i < reqBody.Length; i++) txBuf[txPos++] = (byte)reqBody[i];
                            txBuf[txPos++] = 0x04; // EOT

                            byte bcc = 0;
                            for (int i = 0; i < txPos; i++) bcc += txBuf[i];
                            string bccHex = bcc.ToString("X2");
                            txBuf[txPos++] = (byte)bccHex[0];
                            txBuf[txPos++] = (byte)bccHex[1];

                            testPort.Write(txBuf, 0, txPos);

                            byte[] rxBuf = new byte[256];
                            int totalRead = 0;
                            DateTime dtLimit = DateTime.Now.AddMilliseconds(250);

                            while (DateTime.Now < dtLimit)
                            {
                                if (testPort.BytesToRead > 0)
                                {
                                    int r = testPort.Read(rxBuf, totalRead, rxBuf.Length - totalRead);
                                    totalRead += r;
                                }
                                Thread.Sleep(15);
                            }

                            if (totalRead > 0)
                            {
                                int ackIdx = -1;
                                for (int i = 0; i < totalRead; i++)
                                {
                                    if (rxBuf[i] == 0x06) { ackIdx = i; break; }
                                }

                                if (ackIdx != -1 && (totalRead - ackIdx) >= 7)
                                {
                                    string respHeader = Encoding.ASCII.GetString(rxBuf, ackIdx + 1, 5);
                                    if (respHeader.Equals("00rSS", StringComparison.OrdinalIgnoreCase))
                                    {
                                        bPingSuccess = true;
                                        strFoundPort = strCurrentPort;
                                    }
                                }
                            }

                            testPort.Close();
                        }

                        if (bPingSuccess) break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[PLC 2 핑 예외 - {strCurrentPort}] {ex.Message}");
                    }
                }
            });

            button_PLC2.Enabled = true;

            if (!bPingSuccess)
            {
                button_PLC2.BackColor = Color.Red;
                button_PLC2.CurrentStatus = eDiagStatus.Abnormal;
                if (!m_lstFailItems.Contains("PLC 2")) m_lstFailItems.Add("PLC 2");
                Console.WriteLine("[진단 결과] PLC 2 통신 실패");
            }
            else
            {
                if (ConfigJson.CurrentConfig?.Device != null)
                {
                    ConfigJson.CurrentConfig.Device.Plc2_COM = strFoundPort;
                    SaveCurrentJsonConfig();
                }

                button_PLC2.BackColor = Color.GreenYellow;
                button_PLC2.CurrentStatus = eDiagStatus.Normal;

                if (frmMain != null)
                {
                    try
                    {
                        bool bOpened = frmMain.Plc2Start();
                        Console.WriteLine($"[진단] 메인 폼 PLC 2 포트({strFoundPort}) 상시 오픈: {(bOpened ? "성공" : "실패")}");
                    }
                    catch { }
                }
            }
        }

        // =========================================================================
        // 3. DC 파워 진단
        // =========================================================================
        private void button_PowerSupply_Click(object sender, EventArgs e)
        {
            Diagnose_PowerSupply();
        }

        private void Diagnose_PowerSupply()
        {
            button_PowerSupply.StartNewDiagnosis();
            m_lstFailItems.Remove("DC 파워");

            if (frmMain == null || frmMain.OpenDCPower() == false)
            {
                button_PowerSupply.BackColor = Color.Red;
                button_PowerSupply.CurrentStatus = eDiagStatus.Abnormal;
                if (!m_lstFailItems.Contains("DC 파워")) m_lstFailItems.Add("DC 파워");
            }
            else
            {
                button_PowerSupply.BackColor = Color.GreenYellow;
                button_PowerSupply.CurrentStatus = eDiagStatus.Normal;
                Console.WriteLine("[진단] 메인 폼 DC 파워 통신 상시 오픈 완료");
            }
        }

        // =========================================================================
        // 4. MVB 통신 진단 (9600 bps, get.devicename.0 -> MVB-BOARD 검증)
        // =========================================================================
        private async void button_MVB_Click(object sender, EventArgs e)
        {
            await Diagnose_MVB_Async();
        }

        private async Task Diagnose_MVB_Async()
        {
            button_MVB.StartNewDiagnosis();
            button_MVB.Enabled = false;
            m_lstFailItems.Remove("MVB");

            await Task.Delay(50);

            string preferredPort = ConfigJson.CurrentConfig?.Device?.MVBBoard_ComPort ?? "COM2";
            List<string> lstPorts = GetCandidatePorts(preferredPort);

            bool bSuccess = false;
            string strFoundPort = string.Empty;

            await Task.Run(() =>
            {
                foreach (string port in lstPorts)
                {
                    try
                    {
                        using (SerialPort testPort = new SerialPort(port, 9600, Parity.None, 8, StopBits.One)
                        {
                            ReadTimeout = 300,
                            WriteTimeout = 300
                        })
                        {
                            testPort.Open();
                            testPort.DiscardInBuffer();
                            testPort.DiscardOutBuffer();

                            string reqCmd = "get.devicename.0\r\n";
                            byte[] txBytes = Encoding.ASCII.GetBytes(reqCmd);
                            testPort.Write(txBytes, 0, txBytes.Length);

                            StringBuilder sbRx = new StringBuilder();
                            DateTime dtLimit = DateTime.Now.AddMilliseconds(300);

                            while (DateTime.Now < dtLimit)
                            {
                                if (testPort.BytesToRead > 0)
                                {
                                    byte[] buf = new byte[testPort.BytesToRead];
                                    int read = testPort.Read(buf, 0, buf.Length);
                                    sbRx.Append(Encoding.ASCII.GetString(buf, 0, read));

                                    string currentRx = sbRx.ToString();
                                    if (currentRx.IndexOf("MVB-BOARD", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                        currentRx.IndexOf("reply.get.devicename", StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        bSuccess = true;
                                        strFoundPort = port;
                                        break;
                                    }
                                }
                                Thread.Sleep(15);
                            }

                            testPort.Close();
                        }

                        if (bSuccess) break;
                    }
                    catch
                    {
                    }
                }
            });

            button_MVB.Enabled = true;

            if (bSuccess)
            {
                if (ConfigJson.CurrentConfig?.Device != null)
                {
                    ConfigJson.CurrentConfig.Device.MVBBoard_ComPort = strFoundPort;
                    ConfigJson.CurrentConfig.Device.MVBBoard_BaudRate = 9600;
                    SaveCurrentJsonConfig();
                }

                button_MVB.BackColor = Color.GreenYellow;
                button_MVB.CurrentStatus = eDiagStatus.Normal;
                Console.WriteLine($"[진단] MVB 보드 연결 성공: {strFoundPort} (9600 bps)");
            }
            else
            {
                button_MVB.BackColor = Color.Red;
                button_MVB.CurrentStatus = eDiagStatus.Abnormal;
                if (!m_lstFailItems.Contains("MVB")) m_lstFailItems.Add("MVB");
                Console.WriteLine("[진단 결과] MVB 보드 응답 없음");
            }
        }

        // =========================================================================
        // 5. WTB 보드 진단 (9600 bps, COM2 직접 점검)
        // =========================================================================
        private async void button_OutputBoard_Click(object sender, EventArgs e)
        {
            await Diagnose_WTB_Async();
        }

        private async Task Diagnose_WTB_Async()
        {
            button_WTB.StartNewDiagnosis();
            button_WTB.Enabled = false;
            m_lstFailItems.Remove("WTB");

            await Task.Delay(50);

            string targetPort = "COM2";
            bool bSuccess = false;

            await Task.Run(() =>
            {
                try
                {
                    using (SerialPort testPort = new SerialPort(targetPort, 9600, Parity.None, 8, StopBits.One)
                    {
                        ReadTimeout = 200,
                        WriteTimeout = 200
                    })
                    {
                        testPort.Open();
                        testPort.DiscardInBuffer();
                        testPort.DiscardOutBuffer();

                        bSuccess = true;
                        testPort.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WTB 진단 예외] {targetPort} 열기 실패: {ex.Message}");
                }
            });

            button_WTB.Enabled = true;

            if (bSuccess)
            {
                if (ConfigJson.CurrentConfig?.Device != null)
                {
                    ConfigJson.CurrentConfig.Device.WTBBoard_ComPort = targetPort;
                    ConfigJson.CurrentConfig.Device.WTBBoard_BaudRate = 9600;
                    SaveCurrentJsonConfig();
                }

                button_WTB.BackColor = Color.GreenYellow;
                button_WTB.CurrentStatus = eDiagStatus.Normal;
                Console.WriteLine($"[진단] WTB 포트 연결 성공: {targetPort} (9600 bps)");
            }
            else
            {
                button_WTB.BackColor = Color.Red;
                button_WTB.CurrentStatus = eDiagStatus.Abnormal;
                if (!m_lstFailItems.Contains("WTB")) m_lstFailItems.Add("WTB");
                Console.WriteLine($"[진단 결과] WTB 포트 열기 실패 ({targetPort})");
            }
        }

        // =========================================================================
        // 6. 전류 입력 보드 진단 (CurrentInputService 활용)
        // =========================================================================
        private async void button_InputBoard_Click(object sender, EventArgs e)
        {
            await Diagnose_InputBoard_Async();
        }

        private async Task Diagnose_InputBoard_Async()
        {
            button_InputBoard.StartNewDiagnosis();
            button_InputBoard.Enabled = false;
            m_lstFailItems.Remove("전류 입력 보드");

            frmMain?.CloseCurrentInput();
            await Task.Delay(50);

            string preferredPort = ConfigJson.CurrentConfig?.Device?.CurrentInput_COM ?? "COM5";
            List<string> lstPorts = GetCandidatePorts(preferredPort);

            bool bSuccess = false;
            string strFoundPort = string.Empty;

            await Task.Run(async () =>
            {
                using (var ciService = new CurrentInputService())
                {
                    foreach (string port in lstPorts)
                    {
                        try
                        {
                            if (ciService.Open(port, 9600))
                            {
                                string devName = await ciService.GetDeviceNameAsync(boardIdx: 0, timeoutMs: 250);
                                ciService.Close();

                                if (!string.IsNullOrEmpty(devName) &&
                                    devName.IndexOf("aiao-board", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    bSuccess = true;
                                    strFoundPort = port;
                                    break;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            });

            button_InputBoard.Enabled = true;

            if (bSuccess)
            {
                if (ConfigJson.CurrentConfig?.Device != null)
                {
                    ConfigJson.CurrentConfig.Device.CurrentInput_COM = strFoundPort;
                    SaveCurrentJsonConfig();
                }

                button_InputBoard.BackColor = Color.GreenYellow;
                button_InputBoard.CurrentStatus = eDiagStatus.Normal;

                frmMain?.OpenCurrentInput(strFoundPort);
                Console.WriteLine($"[진단] 전류 입력 보드(aiao-board) 연결 성공: {strFoundPort}");
            }
            else
            {
                button_InputBoard.BackColor = Color.Red;
                button_InputBoard.CurrentStatus = eDiagStatus.Abnormal;
                if (!m_lstFailItems.Contains("전류 입력 보드")) m_lstFailItems.Add("전류 입력 보드");
                Console.WriteLine("[진단 결과] 전류 입력 보드 응답 없음");
            }
        }

        // =========================================================================
        // [공통 헬퍼] 우선순위 포트 포함 전체 COM 포트 목록 생성 (특정 포트 제외 기능 추가)
        // =========================================================================
        private List<string> GetCandidatePorts(string preferredPort, string excludePort = null)
        {
            List<string> candidatePorts = new List<string>();

            // 1. 유효한 우선순위 포트가 있고 COM1 또는 제외 포트가 아니면 최우선 등록
            if (!string.IsNullOrEmpty(preferredPort) &&
                !preferredPort.Equals("COM1", StringComparison.OrdinalIgnoreCase) &&
                (string.IsNullOrEmpty(excludePort) || !preferredPort.Equals(excludePort, StringComparison.OrdinalIgnoreCase)))
            {
                candidatePorts.Add(preferredPort);
            }

            // 2. PC에 인식된 포트 목록 가져오기 (COM1 및 excludePort 제외)
            var allPorts = SerialPort.GetPortNames();
            foreach (string port in allPorts)
            {
                if (port.Equals("COM1", StringComparison.OrdinalIgnoreCase)) continue;
                if (!string.IsNullOrEmpty(excludePort) && port.Equals(excludePort, StringComparison.OrdinalIgnoreCase)) continue;

                if (!candidatePorts.Contains(port, StringComparer.OrdinalIgnoreCase))
                    candidatePorts.Add(port);
            }

            // 3. COM1은 맨 마지막 후순위로 추가 (단, 제외 포트가 아닐 때만)
            if (allPorts.Contains("COM1", StringComparer.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(excludePort) || !"COM1".Equals(excludePort, StringComparison.OrdinalIgnoreCase))
                {
                    candidatePorts.Add("COM1");
                }
            }

            return candidatePorts;
        }

        private void FormDiagnosis_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveCurrentJsonConfig();
        }

        public bool SaveCurrentJsonConfig()
        {
            if (ConfigJson.CurrentConfig == null) return false;
            try
            {
                ConfigManager configManager = new ConfigManager();
                return configManager.SaveConfig(ConfigJson.CurrentConfig);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JSON 저장 예외: {ex.Message}");
                return false;
            }
        }
    }
}