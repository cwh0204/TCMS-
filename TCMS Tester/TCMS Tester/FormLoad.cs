using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
namespace CITester
{
    public partial class FormLoad : Form
    {
        int m_nStep = 0;
        int m_nSubStep = 0;
        public FormMain frmMain;
        bool isEng = GlobalSettings.strLanguage.StartsWith("en");
        public string strLanguage = "";
        public static bool bShowUnitSelect = false;
        public static string strControlUnitInfo;
        public static bool bControUnit;
        private bool bLoadConfig;
        ConfigManager configManager = new ConfigManager();
        ConfigJson loadedConfig;

        // List to store names of failed items
        private List<string> m_lstFailItems = new List<string>();
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED (모든 자식 컨트롤 더블 버퍼링 일괄 적용)
                return cp;
            }
        }
        public FormLoad()
        {
            ApplyConfigLanguage();
            InitializeComponent();
        }

        private void FormLoad_Load(object sender, EventArgs e)
        {
            CreateSeparatorLines();
            customProgressBar1.Maximum = 14;
            customProgressBar1.Value = 0;
            m_Timer.Enabled = true;
            m_chkStep1.Checked = true;
            m_chkStep1.ForeColor = Color.Black;
            m_lbResult1.ForeColor = Color.RoyalBlue;
            m_lbResult1.Text = "확인 중...";

            if (configManager.LoadConfig(out loadedConfig))
            {
                ConfigJson.CurrentConfig = loadedConfig;
                bLoadConfig = true;
            }
            else
            {
                bLoadConfig = false;
            }
        }

        private void M_Timer_Tick(object sender, EventArgs e)
        {
            int i;
            int nFailBoard = 0;
            if (m_nStep <= 14)
            {
                customProgressBar1.Value = m_nStep;
            }
            string strFail = isEng ? "FAIL" : "실패";

            // Step 0: Config & Database
            if (m_nStep == 0)
            {
                m_Timer.Enabled = false;
                Show();
                BringToFront();

                if (bLoadConfig == false)
                {
                    m_lbResult1.Text = "FAIL";
                    m_lbResult1.ForeColor = Color.Red;
                    m_lstFailItems.Add("Configuration Load");
                }

                if (frmMain.OpenDatabase() == false)
                {
                    m_lbResult1.Text = "FAIL";
                    m_lbResult1.ForeColor = Color.Red;
                    m_lstFailItems.Add("Database Connection");
                }
                if (m_lbResult1.Text != "FAIL")
                {
                    m_lbResult1.Text = "OK";
                    m_lbResult1.ForeColor = Color.RoyalBlue;
                }
              /*  try
                {
                    new ExcelSheetLoader()
                        .AddMap("TC", frmMain.dataGridViewDigitalTC)
                        .AddMap("CC", frmMain.dataGridViewDigitalCC)
                        .AddMap("ER", frmMain.dataGridViewAnalogTC)
                        .AddMap("PLC", frmMain.dataGridViewAnalogCC)
                        .Load();
                }
                catch (Exception ex)
                {
                    //m_lstFailItems.Add("IO List Excel Load");
                    // 디버깅용: Console.WriteLine(ex.Message);
                }*/
                ++m_nStep;
                m_chkStep2.Checked = true;
                m_chkStep2.ForeColor = Color.Black;
                m_lbResult2.ForeColor = Color.RoyalBlue;
                m_lbResult2.Text = "확인 중...";
                m_nStep = 14;
                m_Timer.Enabled = true;
            }
            // Step 1: PLC Connection
            else if (m_nStep == 1)
            {
                m_chkStep1.ForeColor = Color.Black;
                m_Timer.Enabled = false;
                if (frmMain.m_strPLCAddress != "0.0.0.0")
                {
                    Ping ping = new Ping();
                    byte[] btBuf = ASCIIEncoding.ASCII.GetBytes("1234567890");
                    bool bPingSuccess = false;

                    for (i = 0; i < 3; i++)
                    {
                        PingReply reply = ping.Send(frmMain.m_strPLCAddress, 100, btBuf, new PingOptions { DontFragment = true });
                        if (reply.Status == IPStatus.Success)
                        {
                            bPingSuccess = true;
                            break;
                        }
                    }

                    if (!bPingSuccess)
                    {
                        //m_nStep = 2;
                        m_lbResult2.Text = "FAIL";
                        m_lbResult2.ForeColor = Color.Red;
                        m_lstFailItems.Add(isEng ? "PLC Connection (Ping)" : "PLC 연결 상태 (Ping)");
                    }
                }
                if (m_lbResult2.Text != "FAIL")
                {
                    m_lbResult2.Text = "OK";
                    m_lbResult2.ForeColor = Color.RoyalBlue;
                }

                ++m_nStep;
                m_nStep = 14;
                m_Timer.Enabled = true;
                m_chkStep3.Checked = true;
                m_chkStep3.ForeColor = Color.Black;
                m_lbResult3.ForeColor = Color.RoyalBlue;
                m_lbResult3.Text = "확인 중...";
            }
            // Step 2: Oscilloscope
            else if (m_nStep == 2)
            {
                m_Timer.Enabled = false;

                if (frmMain.ConnectOscilloscope() == false)
                {
                    m_lbResult3.Text = "FAIL";
                    m_lbResult3.ForeColor = Color.Red;
                    m_lstFailItems.Add(isEng ? "Oscilloscope Connection" : "오실로스코프 연결");
                }

                if (m_lbResult3.Text != "FAIL")
                {
                    m_lbResult3.Text = "OK";
                    m_lbResult3.ForeColor = Color.RoyalBlue;
                }
                ++m_nStep;
                m_chkStep4.Checked = true;
                m_chkStep4.ForeColor = Color.Black;
                m_lbResult4.ForeColor = Color.RoyalBlue;
                m_lbResult4.Text = "확인 중...";
                m_Timer.Enabled = true;
            }
            // Step 3: Digital Multimeter (DMM)
            else if (m_nStep == 3)
            {
                m_Timer.Enabled = false;

                if (frmMain.ConnectDMM() == false)
                {
                    m_lbResult4.Text = "FAIL";
                    m_lbResult4.ForeColor = Color.Red;
                    m_lstFailItems.Add(isEng ? "Digital Multimeter (DMM)" : "디지털 멀티미터 (DMM)");
                }

                ++m_nStep;
                m_chkStep10.Checked = true;
                m_chkStep10.ForeColor = Color.Black;
                m_lbResult10.ForeColor = Color.RoyalBlue;
                m_lbResult10.Text = "확인 중...";
                m_Timer.Enabled = true;
            }
            // Step 4: MVB Board0
            else if (m_nStep == 4)
            {
                m_Timer.Enabled = false;

                if (frmMain.OpenMvbBoard() == false)
                {
                    m_lbResult10.Text = "FAIL";
                    m_lbResult10.ForeColor = Color.Red;
                    m_lstFailItems.Add(isEng ? "MVB Board" : "MVB 보드");
                }

                if (m_lbResult10.Text != "FAIL")
                {
                    m_lbResult10.Text = "OK";
                    m_lbResult10.ForeColor = Color.RoyalBlue;
                }

                ++m_nStep;
                m_chkStep6.Checked = true;
                m_chkStep6.ForeColor = Color.Black;
                m_lbResult6.ForeColor = Color.RoyalBlue;
                m_lbResult6.Text = "확인 중...";
                m_Timer.Enabled = true;
            }
            // Step 5: AC Power (SpeedOut)
            else if (m_nStep == 5)
            {
                m_Timer.Enabled = false;

                if (frmMain.OpenSpeedOut() == false)
                {
                    m_lbResult6.Text = "FAIL";
                    m_lbResult6.ForeColor = Color.Red;
                    m_lstFailItems.Add(isEng ? "AC Variable Power" : "AC 가변 파워");
                }

                if (m_lbResult6.Text != "FAIL")
                {
                    m_lbResult6.Text = "OK";
                    m_lbResult6.ForeColor = Color.RoyalBlue;
                }

                ++m_nStep;
                m_chkStep7.Checked = true;
                m_chkStep7.ForeColor = Color.Black;
                m_lbResult7.ForeColor = Color.RoyalBlue;
                m_lbResult7.Text = "확인 중...";
                m_Timer.Enabled = true;
            }
            // Step 6: PWM (Skipped)
            else if (m_nStep == 6)
            {
                ++m_nStep;
                m_chkStep8.Checked = true;
                m_chkStep8.ForeColor = Color.Black;
                m_lbResult8.ForeColor = Color.RoyalBlue;
                m_lbResult8.Text = "확인 중...";
                m_Timer.Enabled = true;
            }
            // Step 7: Current Output Board
            else if (m_nStep == 7)
            {
                m_Timer.Enabled = false;

                if (frmMain.OpenCurrentOutBoard(ref nFailBoard) == false)
                {
                    m_lbResult8.Text = "FAIL";
                    m_lbResult8.ForeColor = Color.Red;
                    m_lstFailItems.Add(isEng ? "Current Output Board" : "전류 출력 보드");
                }

                if (m_lbResult8.Text != "FAIL")
                {
                    m_lbResult8.Text = "OK";
                    m_lbResult8.ForeColor = Color.RoyalBlue;
                }

                ++m_nStep;
                m_chkStep9.Checked = true;
                m_chkStep9.ForeColor = Color.Black;
                m_lbResult9.ForeColor = Color.RoyalBlue;
                m_lbResult9.Text = "확인 중...";
                m_Timer.Enabled = true;
            }
            // Step 8: Optical Board 1
            else if (m_nStep == 8)
            {
                m_Timer.Enabled = false;

                if (frmMain.OpenSpeedOut() == false)
                {
                    m_lbResult9.Text = "FAIL";
                    m_lbResult9.ForeColor = Color.Red;
                    m_lstFailItems.Add(isEng ? "Optical Board 1" : "광보드 1");
                }

                if (m_lbResult9.Text != "FAIL")
                {
                    m_lbResult9.Text = "OK";
                    m_lbResult9.ForeColor = Color.RoyalBlue;
                }

                ++m_nStep;
                m_chkStep11.Checked = true;
                m_chkStep11.ForeColor = Color.Black;
                m_lbResult11.ForeColor = Color.RoyalBlue;
                m_lbResult11.Text = "확인 중...";
                m_Timer.Enabled = true;
            }
            // Step 9: Optical Board 2
            else if (m_nStep == 9)
            {
                m_Timer.Enabled = false;

                if (frmMain.OpenSpeedOut() == false)
                {
                    m_lbResult11.Text = "FAIL";
                    m_lbResult11.ForeColor = Color.Red;
                    m_lstFailItems.Add(isEng ? "Optical Board 2" : "광보드 2");
                }

                if (m_lbResult11.Text != "FAIL")
                {
                    m_lbResult11.Text = "OK";
                    m_lbResult11.ForeColor = Color.RoyalBlue;
                }

                ++m_nStep;
                m_chkStep5.Checked = true;
                m_chkStep5.ForeColor = Color.Black;
                m_lbResult5.ForeColor = Color.RoyalBlue;
                m_lbResult5.Text = "확인 중...";
                m_Timer.Enabled = true;
            }
            // Step 10: DC Power Supply
            else if (m_nStep == 10)
            {
                m_Timer.Enabled = false;

                if (frmMain.OpenDCPower() == false)
                {
                    m_lbResult5.Text = "FAIL";
                    m_lbResult5.ForeColor = Color.Red;
                    m_lstFailItems.Add(isEng ? "DC Power Supply" : "DC 파워");
                }

                if (m_lbResult5.Text != "FAIL")
                {
                    m_lbResult5.Text = "OK";
                    m_lbResult5.ForeColor = Color.RoyalBlue;
                }

                ++m_nStep;
                m_chkStep12.Checked = true;
                m_chkStep12.ForeColor = Color.Black;
                m_lbResult12.ForeColor = Color.RoyalBlue;
                m_lbResult12.Text = "확인 중...";
                m_Timer.Enabled = true;
            }
            // Step 11: Trimmer Board 1
            else if (m_nStep == 11)
            {
                m_Timer.Enabled = false;

                if (frmMain.TrimmerBoard1() == false)
                {
                    m_lbResult12.Text = "FAIL";
                    m_lbResult12.ForeColor = Color.Red;
                    m_lstFailItems.Add(isEng ? "Trimmer Board 1" : "TRIMMER1 보드");
                }

                if (m_lbResult12.Text != "FAIL")
                {
                    m_lbResult12.Text = "OK";
                    m_lbResult12.ForeColor = Color.RoyalBlue;
                }

                ++m_nStep;
                m_chkStep13.Checked = true;
                m_chkStep13.ForeColor = Color.Black;
                m_lbResult13.ForeColor = Color.RoyalBlue;
                m_lbResult13.Text = "확인 중...";
                m_Timer.Enabled = true;
            }
            // Step 12: Trimmer Board 2
            else if (m_nStep == 12)
            {
                m_Timer.Enabled = false;

                if (frmMain.TrimmerBoard2() == false)
                {
                    m_lbResult13.Text = "FAIL";
                    m_lbResult13.ForeColor = Color.Red;
                    m_lstFailItems.Add(isEng ? "Trimmer Board 2" : "TRIMMER2 보드");
                }

                if (m_lbResult13.Text != "FAIL")
                {
                    m_lbResult13.Text = "OK";
                    m_lbResult13.ForeColor = Color.RoyalBlue;
                }

                ++m_nStep;
                m_chkStep14.Checked = true;
                m_chkStep14.ForeColor = Color.Black;
                m_lbResult14.ForeColor = Color.RoyalBlue;
                m_lbResult14.Text = "확인 중...";
                m_Timer.Enabled = true;
            }
            // Step 13: Line Voltage Board 0 (Final Test)
            else if (m_nStep == 13)
            {
                m_Timer.Enabled = false;

                if (frmMain.LineVoltageBoard0() == false)
                {
                    m_lbResult14.Text = "FAIL";
                    m_lbResult14.ForeColor = Color.Red;
                    m_lstFailItems.Add(isEng ? "Line Voltage Board 0" : "Line Voltage 출력 보드");
                }

                m_lbResult14.Visible = true;

                // --- Display Error Summary if any failures occurred ---
                if (m_lstFailItems.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(isEng ? "[Self-Diagnosis Failed Items]" : "[자가 진단 실패 항목]");
                    sb.AppendLine("");

                    foreach (var item in m_lstFailItems)
                    {
                        sb.AppendLine("• " + item);
                    }

                    sb.AppendLine("-----------------------------------------");
                    // 요청하신 케이블 확인 및 프로그램 재실행 문구
                    sb.AppendLine(isEng ? "Please check cable connections of the devices listed above," : "위에 나열된 장치들의 케이블 연결 상태를 확인해 주세요.");
                    sb.AppendLine(isEng ? "and restart the program." : "");

                    MessageBox.Show(sb.ToString(), "Self-Diagnosis Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                // -----------------------------------------------------

                ++m_nStep;
                m_Timer.Enabled = true;
            }
            // Step 14+: Closing (Fade-out)
            else if (m_nStep >= 14)
            {
                m_Timer.Interval = 50;
                Opacity -= 0.05;

                if (Opacity <= 0)
                {
                    m_Timer.Enabled = false;
                    Close();
                }
            }
        }
        public static class GlobalSettings
        {
            // 전역에서 공유될 언어 변수 (기본값 KO-KR)
            public static string strLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
        }

        private void ApplyConfigLanguage()
        {
            try
            {
                string configPath = Application.StartupPath + @"\Config.xml";
                if (System.IO.File.Exists(configPath))
                {
                    XDocument doc = XDocument.Load(configPath);
                    // XML에서 Language 값 읽기 (없으면 KO-KR 기본값)
                    string lang = doc.Root.Element("General").Element("Language")?.Value ?? "ko-kr";

                    // CultureInfo 설정
                    CultureInfo culture = new CultureInfo(lang);

                    // 현재 쓰레드의 언어 설정 변경
                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;
                    GlobalSettings.strLanguage = lang;
                    string showTag = doc.Root.Element("General").Element("제어편성표시")?.Value ?? "TRUE";

                    if (showTag.ToUpper() == "TRUE")
                    {
                        bControUnit = true;
                    }
                    else
                    {
                        bControUnit = false;
                    }
                    //strControlUnitI nfo = doc.Root.Element("General").Element("제어편성")?.Value ?? "1,2,3";
                    // 디버깅 확인용 (출력창에서 확인 가능)
                    //System.Diagnostics.Debug.WriteLine($"편성 정보 읽기: {strControlUnitInfo}, 표시 여부: {bShowUnitSelect}");
                }
            }
            catch (Exception ex)
            {
                // 파일이 없거나 에러 발생 시 기본값 유지
                MessageBox.Show("설정 파일을 불러오는 중 오류가 발생했습니다: " + ex.Message);
            }
        }
        private void CreateSeparatorLines()
        {
            // 1. 선들이 들어갈 부모 컨테이너 지정 (리스트가 들어있는 패널 이름으로 변경하세요)
            // 만약 폼 바탕에 바로 있다면 Control parent = this; 로 변경합니다.
            Control parent = this.roundedPanel1;

            int lineCount = 12;       // 라벨의 개수 (선의 개수)
            int ySpacing = 40;        // 세로 간격 (요청하신 40px)

            // 2. 위치 및 크기 설정
            // 첫 번째 라벨의 바로 아래쪽 Y좌표를 찾아서 입력하세요 (예: 80)
            int startY = 59;
            int paddingX = 20;        // 좌우 여백
            int lineWidth = parent.Width - (paddingX * 2); // 패널 너비에서 양쪽 여백을 뺀 길이

            for (int i = 0; i < lineCount; i++)
            {
                Label line = new Label();
                line.AutoSize = false;
                line.Height = 1; // 선의 두께 (1px)
                line.Width = lineWidth;

                // 이미지와 가장 비슷한 아주 연한 회색으로 색상 지정
                line.BackColor = Color.FromArgb(238, 239, 241);

                // X, Y 좌표 계산 및 지정
                line.Left = paddingX;
                line.Top = startY + (i * ySpacing);

                // 컨테이너에 생성한 선 추가
                parent.Controls.Add(line);

                // 선이 다른 컨트롤(체크박스, 글자 등) 위를 덮지 않도록 맨 뒤로 보냅니다.
                line.SendToBack();
            }
        }
    }
}
