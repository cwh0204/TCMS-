using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO.Ports;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using main;
using TCMSTester;
using static CITester.FormLoad;
using static CITester.FormMain;
using System.Threading.Tasks;

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
            // 진단 시작 전 포트 리셋이 필요하다면 유지, 
            // 단 이미 연결된 포트를 불필요하게 끊지 않도록 주의합니다.
            frmMain?.ResetPort();
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

                    // 메인 폼에 자가진단 최종 결과 전달 (버튼 색상 변경 및 시험 버튼 활성화)
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
        // 1. PLC 통신 진단 및 포트 상시 오픈 유지
        // =========================================================================
        private async void button_PLC_Click(object sender, EventArgs e)
        {
            Timer_Check.Enabled = false;
            button_PLC.StartNewDiagnosis();
            button_PLC.Enabled = false;

            // 재진단 시 메인 폼이 점유 중인 PLC 포트를 먼저 닫아 충돌 방지
            frmMain?.ClosePlc();
            await Task.Delay(50); // OS 커널의 COM 포트 핸들 반환 대기

            // 재시험을 고려하여 이전 실패 목록에서 PLC 제거
            m_lstFailItems.Remove("PLC");

            string strTargetPort = ConfigJson.CurrentConfig?.Device?.Plc_COM;
            List<string> lstCandidatePorts = new List<string>();

            if (!string.IsNullOrEmpty(strTargetPort))
            {
                lstCandidatePorts.Add(strTargetPort);
            }

            string[] arrSystemPorts = SerialPort.GetPortNames();
            for (int nIdx = 0; nIdx < arrSystemPorts.Length; nIdx++)
            {
                string strPort = arrSystemPorts[nIdx];
                if (!lstCandidatePorts.Contains(strPort, StringComparer.OrdinalIgnoreCase))
                {
                    lstCandidatePorts.Add(strPort);
                }
            }

            bool bPingSuccess = false;
            string strFoundPort = string.Empty;

            // 1) 백그라운드에서 Cnet 핑(Ping)으로 진짜 PLC 포트 탐색
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

                            // PLC 읽기(%PW005) Cnet 프레임 송신
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
                        Console.WriteLine($"[PLC 핑 테스트 예외 - {strCurrentPort}] {ex.Message}");
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
                // 2) 설정 저장
                ConfigJson.CurrentConfig.Device.Plc_COM = strFoundPort;
                SaveCurrentJsonConfig();

                button_PLC.BackColor = Color.GreenYellow;
                button_PLC.CurrentStatus = eDiagStatus.Normal;

                // 3) 메인 폼의 PlcStart()를 직접 호출해 메인 폼에서 포트를 열어둠
                if (frmMain != null)
                {
                    bool bOpened = frmMain.PlcStart();
                    Console.WriteLine($"[진단] 메인 폼 PLC 포트({strFoundPort}) 상시 오픈: {(bOpened ? "성공" : "실패")}");
                }
            }
        }

        // =========================================================================
        // 2. DC 파워 진단 및 상시 오픈 (frmMain.OpenDCPower 호출)
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
        // 3. MVB 통신 진단 및 상시 오픈 (frmMain.OpenMvbBoard 호출)
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

        private void button_OutputBoard_Click(object sender, EventArgs e)
        {
            // 비활성화 항목: 즉시 정상(성공) 처리
            button_OutputBoard.BackColor = Color.GreenYellow;
            button_OutputBoard.CurrentStatus = eDiagStatus.Normal;
            Console.WriteLine("[진단] 출력 보드: 기본 정상 처리 완료");
        }

        private void button_InputBoard_Click(object sender, EventArgs e)
        {
            // 비활성화 항목: 즉시 정상(성공) 처리
            button_InputBoard.BackColor = Color.GreenYellow;
            button_InputBoard.CurrentStatus = eDiagStatus.Abnormal;

            if (!m_lstFailItems.Contains("입력 보드"))
            {
                m_lstFailItems.Add("입력 보드");
            }

            Console.WriteLine("[진단] 입력 보드: 기본 실패 처리 완료");
        }

        private void FormDiagnosis_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 창이 닫힐 때 포트를 닫지 않고, 변경된 설정값만 저장
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