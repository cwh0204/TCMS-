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
using CITester.Services; // 1. 방금 제작한 서비스 네임스페이스 추가
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
            if (m_nStep >= 5)
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
        // 1. PLC 통신 진단
        // =========================================================================
        private async void button_PLC_Click(object sender, EventArgs e)
        {
            Timer_Check.Enabled = false;
            button_PLC.StartNewDiagnosis();
            button_PLC.Enabled = false;

            frmMain?.ClosePlc();
            await Task.Delay(50);

            m_lstFailItems.Remove("PLC");

            List<string> lstCandidatePorts = GetCandidatePorts(ConfigJson.CurrentConfig?.Device?.Plc_COM);

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
                        Console.WriteLine($"[PLC 핑 예외 - {strCurrentPort}] {ex.Message}");
                    }
                }
            });

            button_PLC.Enabled = true;

            if (!bPingSuccess)
            {
                button_PLC.BackColor = Color.Red;
                button_PLC.CurrentStatus = eDiagStatus.Abnormal;
                if (!m_lstFailItems.Contains("PLC")) m_lstFailItems.Add("PLC");
                Console.WriteLine("[진단 결과] PLC 통신 실패");
            }
            else
            {
                ConfigJson.CurrentConfig.Device.Plc_COM = strFoundPort;
                SaveCurrentJsonConfig();

                button_PLC.BackColor = Color.GreenYellow;
                button_PLC.CurrentStatus = eDiagStatus.Normal;

                if (frmMain != null)
                {
                    bool bOpened = frmMain.PlcStart();
                    Console.WriteLine($"[진단] 메인 폼 PLC 포트({strFoundPort}) 상시 오픈: {(bOpened ? "성공" : "실패")}");
                }
            }
        }

        // =========================================================================
        // 2. DC 파워 진단
        // =========================================================================
        private void button_PowerSupply_Click(object sender, EventArgs e)
        {
            button_PowerSupply.StartNewDiagnosis();
            if (frmMain == null || frmMain.OpenDCPower() == false)
            {
                button_PowerSupply.BackColor = Color.Red;
                button_PowerSupply.CurrentStatus = eDiagStatus.Abnormal;
                m_lstFailItems.Add("DC 파워");
            }
            else
            {
                button_PowerSupply.BackColor = Color.GreenYellow;
                button_PowerSupply.CurrentStatus = eDiagStatus.Normal;
                Console.WriteLine("[진단] 메인 폼 DC 파워 통신 상시 오픈 완료");
            }
        }

        // =========================================================================
        // 3. MVB 통신 진단
        // =========================================================================
        private void button_MVB_Click(object sender, EventArgs e)
        {
            button_MVB.StartNewDiagnosis();
            if (frmMain == null || frmMain.OpenMvbBoard() == false)
            {
                button_MVB.BackColor = Color.Red;
                button_MVB.CurrentStatus = eDiagStatus.Abnormal;
                m_lstFailItems.Add("MVB");
            }
            else
            {
                button_MVB.BackColor = Color.GreenYellow;
                button_MVB.CurrentStatus = eDiagStatus.Normal;
                Console.WriteLine("[진단] 메인 폼 MVB 통신 상시 오픈 완료");
            }
        }

        // =========================================================================
        // 4. 전류 출력 보드 진단 (CurrentOutputService 활용)
        // =========================================================================
        private async void button_OutputBoard_Click(object sender, EventArgs e)
        {
            button_OutputBoard.StartNewDiagnosis();
            button_OutputBoard.Enabled = false;
            m_lstFailItems.Remove("전류 출력 보드");

            // 재진단 시 기존 점유 해제
            frmMain?.CloseCurrentOutput();
            await Task.Delay(50);

            string preferredPort = ConfigJson.CurrentConfig?.Device?.CurrentOutput_COM ?? "COM1";
            List<string> lstPorts = GetCandidatePorts(preferredPort);

            bool bSuccess = false;
            string strFoundPort = string.Empty;

            await Task.Run(async () =>
            {
                using (var coService = new CurrentOutputService())
                {
                    foreach (string port in lstPorts)
                    {
                        try
                        {
                            if (coService.Open(port, 9600))
                            {
                                // "get.devicename.0" 송신 및 응답 장치명 확인
                                string devName = await coService.GetDeviceNameAsync(boardIdx: 0, timeoutMs: 250);
                                coService.Close();

                                if (!string.IsNullOrEmpty(devName) &&
                                    devName.IndexOf("voltage-to-current", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    bSuccess = true;
                                    strFoundPort = port;
                                    break;
                                }
                            }
                        }
                        catch
                        {
                            // 포트 접근 불가 시 다음 포트 탐색
                        }
                    }
                }
            });

            button_OutputBoard.Enabled = true;

            if (bSuccess)
            {
                if (ConfigJson.CurrentConfig?.Device != null)
                {
                    ConfigJson.CurrentConfig.Device.CurrentOutput_COM = strFoundPort;
                    SaveCurrentJsonConfig();
                }

                button_OutputBoard.BackColor = Color.GreenYellow;
                button_OutputBoard.CurrentStatus = eDiagStatus.Normal;

                // 메인 폼에 오픈 요청
                frmMain?.OpenCurrentOutput(strFoundPort);
                Console.WriteLine($"[진단] 전류 출력 보드(voltage-to-current) 연결 성공: {strFoundPort}");
            }
            else
            {
                button_OutputBoard.BackColor = Color.Red;
                button_OutputBoard.CurrentStatus = eDiagStatus.Abnormal;
                if (!m_lstFailItems.Contains("전류 출력 보드")) m_lstFailItems.Add("전류 출력 보드");
                Console.WriteLine("[진단 결과] 전류 출력 보드 응답 없음");
            }
        }

        // =========================================================================
        // 5. 전류 입력 보드 진단 (CurrentInputService 활용)
        // =========================================================================
        private async void button_InputBoard_Click(object sender, EventArgs e)
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
                                // "get.devicename.0" 송신 및 응답 장치명 확인
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
                            // 포트 접근 불가 시 다음 포트 탐색
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

                // 메인 폼에 오픈 요청
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
        // [공통 헬퍼] 우선순위 포트 포함 전체 COM 포트 목록 생성
        // =========================================================================
        private List<string> GetCandidatePorts(string preferredPort)
        {
            List<string> candidatePorts = new List<string>();
            if (!string.IsNullOrEmpty(preferredPort))
            {
                candidatePorts.Add(preferredPort);
            }

            foreach (string port in SerialPort.GetPortNames())
            {
                if (!candidatePorts.Contains(port, StringComparer.OrdinalIgnoreCase))
                    candidatePorts.Add(port);
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