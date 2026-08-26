using main;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO.Ports;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TCMSTester;
using static CITester.FormLoad;
using static CITester.FormMain;

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

        private void button_PLC_Click(object sender, EventArgs e)
        {
            string[] arrayAvailablePorts = System.IO.Ports.SerialPort.GetPortNames();
            Timer_Check.Enabled = false;
            int i = 0;
            string strBuf;
            ushort[] nCnetAnswerValue = new ushort[16];
            ushort[] nTest1 = new ushort[16];
            ushort[] nTest2 = new ushort[16];
            string hex3;
            int nCnetResult;
            int nIndex = m_nStep - 1;
            button_PLC.StartNewDiagnosis();

            // PLC 연결 상태 확인
            bool bPingSuccess = false;

            try
            {
                if (serialPlcPort.IsOpen)
                {
                    serialPlcPort.Close();
                }
                serialPlcPort.Open();

                for (i = 0; i < 5; i++)
                {
                    strBuf = string.Format("{0:X2}{1}{2}{3}{4}{5}", 0, "r", "SS", "01", "06", "%PX000");
                    cnetToPlc.Request(strBuf.ToCharArray());

                    Thread.Sleep(500);

                    nCnetResult = cnetToPlc.Answer("00".ToCharArray(), 'w', "SS".ToCharArray(), out nCnetAnswerValue);
                    Console.WriteLine(nCnetResult);

                    if (nCnetResult >= 0)
                    {
                        bPingSuccess = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                bPingSuccess = false;
            }
            //bPingSuccess = true;

            // 통신 결과 UI 및 카운트 반영
            if (!bPingSuccess)
            {
                button_PLC.BackColor = Color.Red;
                button_PLC.CurrentStatus = eDiagStatus.Abnormal;
                m_lstFailItems.Add("PLC");

                if (serialPlcPort.IsOpen)
                {
                    serialPlcPort.Close();
                }
            }
            else
            {
                button_PLC.BackColor = Color.GreenYellow;
                button_PLC.CurrentStatus = eDiagStatus.Normal;
                if (serialPlcPort.IsOpen)
                {
                    serialPlcPort.Close();
                }
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
            ConfigManager cfgManager = new ConfigManager();
            bool bIsSaveSuccess = cfgManager.SaveConfig(ConfigJson.CurrentConfig);
        }

        private void button_InputBoard_Click(object sender, EventArgs e)
        {

        }
    }

}