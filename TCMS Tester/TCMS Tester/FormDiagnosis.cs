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

            // 1. 후보 포트 수집 (기존 설정 포트 우선, 그 뒤 시스템 전체 포트)
            string strTargetPort = ConfigJson.CurrentConfig.Device.Plc_COM;
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

            await Task.Run(() =>
            {
                for (int nPortIdx = 0; nPortIdx < lstCandidatePorts.Count; nPortIdx++)
                {
                    string strCurrentPort = lstCandidatePorts[nPortIdx];
                    SerialPort testPort = null;

                    try
                    {
                        testPort = new SerialPort(strCurrentPort, 9600, Parity.None, 8, StopBits.One);
                        testPort.ReadTimeout = 200;
                        testPort.WriteTimeout = 200;
                        testPort.Open();

                        testPort.DiscardInBuffer();
                        testPort.DiscardOutBuffer();

                        // 2. PLC 전용 Cnet 읽기 핑 패킷 구성 (%PW005 1워드 읽기)
                        // 국번(00) + 'r' + "SS" + 블록수(01) + 변수길이(06) + 변수명(%PW005)
                        string reqBody = "00rSS0106%PW005";
                        byte[] txBuf = new byte[reqBody.Length + 4];
                        int txPos = 0;
                        txBuf[txPos++] = 0x05; // ENQ
                        for (int i = 0; i < reqBody.Length; i++) txBuf[txPos++] = (byte)reqBody[i];
                        txBuf[txPos++] = 0x04; // EOT

                        // BCC 계산
                        byte bcc = 0;
                        for (int i = 0; i < txPos; i++) bcc += txBuf[i];
                        string bccHex = bcc.ToString("X2");
                        txBuf[txPos++] = (byte)bccHex[0];
                        txBuf[txPos++] = (byte)bccHex[1];

                        // 송신
                        testPort.Write(txBuf, 0, txPos);

                        // 3. 응답 대기 (최대 250ms 동안 모으기)
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
                            // 4. PLC 응답 판정 (에코백 무시하고 ACK 0x06 + "00rSS"가 존재하는지 검사)
                            int ackIdx = -1;
                            for (int i = 0; i < totalRead; i++)
                            {
                                if (rxBuf[i] == 0x06) // ACK 헤더 발견
                                {
                                    ackIdx = i;
                                    break;
                                }
                            }

                            if (ackIdx != -1 && (totalRead - ackIdx) >= 7)
                            {
                                string respHeader = Encoding.ASCII.GetString(rxBuf, ackIdx + 1, 5); // "00rSS" 확인
                                if (respHeader.Equals("00rSS", StringComparison.OrdinalIgnoreCase))
                                {
                                    Console.WriteLine($"★ [PLC 식별 완료] {strCurrentPort} -> 정상 Cnet 응답(ACK 00rSS) 수신!");
                                    bPingSuccess = true;
                                    strFoundPort = strCurrentPort;
                                }
                            }
                        }

                        testPort.Close();
                        testPort.Dispose();

                        if (bPingSuccess) break; // PLC를 찾았으므로 즉시 루프 종료
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[포트 진단 스킵] {strCurrentPort}: {ex.Message}");
                        try { testPort?.Close(); testPort?.Dispose(); } catch { }
                    }
                }
            });

            // 5. 결과 반영 및 영구 저장
            button_PLC.Enabled = true;

            if (!bPingSuccess)
            {
                button_PLC.BackColor = Color.Red;
                button_PLC.CurrentStatus = eDiagStatus.Abnormal;
                m_lstFailItems.Add("PLC");
                Console.WriteLine("[진단 결과] 시스템 내 연결된 PLC를 찾지 못했습니다.");
            }
            else
            {
                ConfigJson.CurrentConfig.Device.Plc_COM = strFoundPort;
                button_PLC.BackColor = Color.GreenYellow;
                button_PLC.CurrentStatus = eDiagStatus.Normal;

                bool bSaved = SaveCurrentJsonConfig();
                Console.WriteLine($"[진단 결과] PLC 포트 확정: {strFoundPort} (설정 파일 저장: {(bSaved ? "성공" : "실패")})");
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