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

        private classCnet cnetToPlc;

        SerialPort serialPlcPort = new SerialPort();


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
            frmMain.ResetPort();

            cnetToPlc = new classCnet(serialPlcPort);
            serialPlcPort.PortName = ConfigJson.CurrentConfig.Device.Plc_COM;
            serialPlcPort.BaudRate = 9600;    // PLC 환경에 맞게 속도 조절 (예: 9600, 115200 등)
            serialPlcPort.DataBits = 8;
            serialPlcPort.Parity = Parity.None;
            serialPlcPort.StopBits = StopBits.One;
            serialPlcPort.Handshake = Handshake.None;

        }

        private void Timer_Check_Tick(object sender, EventArgs e)
        {

            // 종료 및 하드웨어 초기값 설정
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

                    string strDiagTitle = "자가진단 알림";
                    string strSummaryHead = "[자가진단 결과]";
                    string strCheckCable = "위에 나열된 장치들의 케이블 연결 상태를 확인해 주십시오.";

                    // 실패 목록 경고창 팝업 출력
                    if (m_lstFailItems.Count > 0)
                    {
                        string strMessage = $"{strSummaryHead}\n" + string.Join("\n", m_lstFailItems) + $"\n\n{strCheckCable}";
                    }
                    else
                    {
                        string strMessage = $"{strSummaryHead}\n" + string.Join("\n", m_lstFailItems) + $"\n\n{strCheckCable}";
                    }

                    Close();
                    return; 
                }
            }

            // 시퀀스가 완전히 끝나기 전까지 타이머 재가동
            Timer_Check.Enabled = true;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonTestStart_Click(object sender, EventArgs e)
        {

        }

        // 3. PLC 통신 진단 (비동기 Task.Run 및 로컬 포트 사용하여 멈춤 원천 차단)
        private async void button_PLC_Click(object sender, EventArgs e)
        {
            Timer_Check.Enabled = false;
            button_PLC.StartNewDiagnosis();
            button_PLC.Enabled = false;

            string strTargetPort = ConfigJson.CurrentConfig?.Device?.Plc_COM;
            List<string> lstCandidatePorts = new List<string>();

            if (!string.IsNullOrEmpty(strTargetPort))
            {
                lstCandidatePorts.Add(strTargetPort);
            }

            string[] arrSystemPorts = System.IO.Ports.SerialPort.GetPortNames();
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

            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        if (serialPlcPort != null && serialPlcPort.IsOpen)
                        {
                            serialPlcPort.Close();
                        }
                    }
                    catch { }

                    for (int nPortIdx = 0; nPortIdx < lstCandidatePorts.Count; nPortIdx++)
                    {
                        string strCurrentPort = lstCandidatePorts[nPortIdx];

                        using (var tempPort = new System.IO.Ports.SerialPort())
                        {
                            try
                            {
                                tempPort.PortName = strCurrentPort;
                                tempPort.BaudRate = 115200;
                                tempPort.ReadTimeout = 100;
                                tempPort.WriteTimeout = 100;
                                tempPort.Open();

                                tempPort.DiscardInBuffer();
                                tempPort.DiscardOutBuffer();

                                for (int nRetry = 0; nRetry < 2; nRetry++)
                                {
                                    string strReqPacket = string.Format("{0:X2}{1}{2}{3}{4}{5}", 0, "r", "SS", "01", "06", "%PX000");
                                    cnetToPlc.Request(strReqPacket.ToCharArray());

                                    int nWaitCount = 0;
                                    const int nMaxWait = 10;

                                    while (nWaitCount < nMaxWait)
                                    {
                                        Thread.Sleep(10);

                                        if (tempPort.IsOpen && tempPort.BytesToRead >= 5)
                                        {
                                            ushort[] arrCnetAnswer;
                                            int nCnetResult = cnetToPlc.Answer("00".ToCharArray(), 'w', "SS".ToCharArray(), out arrCnetAnswer);

                                            if (nCnetResult >= 0)
                                            {
                                                bPingSuccess = true;
                                                strFoundPort = strCurrentPort;
                                                break;
                                            }
                                        }

                                        nWaitCount++;
                                    }

                                    if (bPingSuccess) break;
                                }

                                tempPort.Close();
                                if (bPingSuccess) break;
                            }
                            catch (Exception)
                            {
                                try { if (tempPort.IsOpen) tempPort.Close(); } catch { }
                            }
                        }
                    }
                });
            }
            finally
            {
                button_PLC.Enabled = true;
                Timer_Check.Enabled = true; // 타이머 복구
            }

            if (!bPingSuccess)
            {
                button_PLC.BackColor = Color.Red;
                button_PLC.CurrentStatus = eDiagStatus.Abnormal;
                if (!m_lstFailItems.Contains("PLC"))
                {
                    m_lstFailItems.Add("PLC");
                }
            }
            else
            {
                ConfigJson.CurrentConfig.Device.Plc_COM = strFoundPort;
                button_PLC.BackColor = Color.GreenYellow;
                button_PLC.CurrentStatus = eDiagStatus.Normal;
                m_lstFailItems.Remove("PLC");

                try { serialPlcPort.PortName = strFoundPort; } catch { }
            }
        }


        private void button_PowerSupply_Click(object sender, EventArgs e)
        {
            button_PowerSupply.StartNewDiagnosis();
            if (frmMain.OpenDCPower() == false)
            {
                button_PowerSupply.BackColor = Color.Red;
                button_PowerSupply.CurrentStatus = eDiagStatus.Abnormal;
                m_lstFailItems.Add("DC 파워");
            }
            else
            {
                button_PowerSupply.BackColor = Color.GreenYellow;
                button_PowerSupply.CurrentStatus = eDiagStatus.Normal;
            }

        }

        private void button_MVB_Click(object sender, EventArgs e)
        {

            button_MVB.StartNewDiagnosis();
            if (frmMain.OpenMvbBoard() == false)
            {
                button_MVB.BackColor = Color.Red;
                button_MVB.CurrentStatus = eDiagStatus.Abnormal;
            }
            else
            {
                button_MVB.BackColor = Color.GreenYellow;
                button_MVB.CurrentStatus = eDiagStatus.Normal;
            }
        }

        private void button_OutputBoard_Click(object sender, EventArgs e)
        {

            return; // 출력보드 진단 기능 비활성화

            int nFailBoard = 0;

            button_OutputBoard.StartNewDiagnosis();
            if (frmMain.OpenCurrentOutBoard(ref nFailBoard) == false)
            {
                button_OutputBoard.BackColor = Color.Red;
                button_OutputBoard.CurrentStatus = eDiagStatus.Abnormal;
            }
            else
            {
                button_OutputBoard.BackColor = Color.GreenYellow;
                button_OutputBoard.CurrentStatus = eDiagStatus.Normal;
            }
        }

        private void FormDiagnosis_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveCurrentJsonConfig();
        }

        // 4. 자가진단 폼 설정값 저장
        public bool SaveCurrentJsonConfig()
        {
            // [수정] Json_Config -> ConfigJson 으로 통일
            if (ConfigJson.CurrentConfig == null)
            {
                Console.WriteLine("설정 인스턴스(ConfigJson.CurrentConfig)가 null 상태입니다.");
                return false;
            }

            try
            {
                ConfigManager configManager = new ConfigManager();
                bool bSaveResult = configManager.SaveConfig(ConfigJson.CurrentConfig);

                if (!bSaveResult)
                {
                    Console.WriteLine("현재 설정값의 JSON 파일(config.json) 저장에 실패했습니다.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JSON 파일 저장 중 예외 발생: {ex.Message}");
                return false;
            }
        }

        private void button_InputBoard_Click(object sender, EventArgs e)
        {
            return; // 입력보드 진단 기능 비활성화
        }
    }

}