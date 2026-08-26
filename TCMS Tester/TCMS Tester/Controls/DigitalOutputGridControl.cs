using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Net.Sockets;
using System.Windows.Forms;
using CITester;
using TCMSTester.Config;

namespace TCMSTester.Controls
{
    /// <summary>
    /// Digital Output 항목들을 바둑판(Grid) 형태로 배치하여 화면에 출력하고, 선택 상태를 토글할 수 있는 사용자 지정 컨트롤입니다.
    /// </summary>
    public class DigitalOutputGridControl : UserControl
    {
        /// <summary>
        /// 상단 타이틀 영역을 포함하는 패널입니다.
        /// </summary>
        private Panel _topPanel;

        /// <summary>
        /// 컨트롤 상단에 표시되는 타이틀 라벨입니다.
        /// </summary>
        private Label _lblTitle;

        /// <summary>
        /// 그리드 레이아웃의 가로/세로 스크롤을 관리하는 패널입니다.
        /// </summary>
        private Panel _scrollPanel;

        /// <summary>
        /// Digital Output 셀(Label)들을 격자 형태로 동적 배치하는 테이블 레이아웃 패널입니다.
        /// </summary>
        private TableLayoutPanel _tableLayoutPanel;

        /// <summary>
        /// Digital Output 셀이 클릭되었을 때 발생하며, 변경된 항목 정보를 전달합니다.
        /// </summary>
        public event Action<DigitalItemConfig> ItemClicked;

        /// <summary>
        /// <see cref="DigitalOutputGridControl"/> 클래스의 새 인스턴스를 초기화하고 레이아웃 컨트롤들을 생성합니다.
        /// </summary>
        public DigitalOutputGridControl()
        {
            InitializeComponent();
            InitLayoutControls();
        }

        /// <summary>
        /// 사용자 컨트롤의 기본 구성 요소 및 초기 크기를 설정합니다.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "DigitalOutputGridControl";
            this.Size = new Size(1000, 700);
            this.ResumeLayout(false);
        }

        /// <summary>
        /// 상단 타이틀 패널, 스크롤 패널, 테이블 레이아웃 패널 등 내부 UI 레이아웃 구조를 초기화하고 배치합니다.
        /// </summary>
        private void InitLayoutControls()
        {
            this.Controls.Clear();

            _topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(245, 245, 245)
            };

            _topPanel.Resize += (s, e) => CenterTitleLabel();

            _lblTitle = new Label
            {
                Text = "Digital Output",
                Font = new Font("맑은 고딕", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 253, 231),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(180, 28)
            };
            _topPanel.Controls.Add(_lblTitle);

            _scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(245, 245, 245)
            };

            _tableLayoutPanel = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(245, 245, 245)
            };

            _scrollPanel.Controls.Add(_tableLayoutPanel);
            this.Controls.Add(_scrollPanel);
            this.Controls.Add(_topPanel);

            CenterTitleLabel();
        }

        /// <summary>
        /// 상단 타이틀 라벨(<see cref="_lblTitle"/>)을 상단 패널(<see cref="_topPanel"/>)의 가로 중앙 위치로 정렬합니다.
        /// </summary>
        private void CenterTitleLabel()
        {
            if (_lblTitle != null && _topPanel != null)
            {
                _lblTitle.Location = new Point((_topPanel.Width - _lblTitle.Width) / 2, 6);
            }
        }

        /// <summary>
        /// Digital Output 설정 데이터 리스트를 받아 한 열당 16개씩 바둑판 형태의 테이블 레이아웃으로 셀을 동적 생성하여 화면에 바인딩합니다.
        /// </summary>
        /// <param name="items">화면에 출력할 Digital Output 항목 설정 데이터 리스트</param>
        public void SetIoItems(List<DigitalItemConfig> items)
        {
            if (_tableLayoutPanel == null) return;

            _tableLayoutPanel.SuspendLayout();
            _tableLayoutPanel.Controls.Clear();
            _tableLayoutPanel.RowStyles.Clear();
            _tableLayoutPanel.ColumnStyles.Clear();

            if (items == null || items.Count == 0)
            {
                _tableLayoutPanel.ResumeLayout();
                return;
            }

            int itemsPerColumn = 16;
            int rowCount = itemsPerColumn;
            int colCount = (int)Math.Ceiling((double)items.Count / itemsPerColumn);

            _tableLayoutPanel.RowCount = rowCount;
            _tableLayoutPanel.ColumnCount = colCount;

            float columnWidth = 100f;
            for (int c = 0; c < colCount; c++)
            {
                _tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, columnWidth));
            }

            float rowHeight = 48f;
            for (int r = 0; r < rowCount; r++)
            {
                _tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, rowHeight));
            }

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                Label cell = new Label
                {
                    Text = item.Name,
                    Tag = item,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("맑은 고딕", 9.0F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(34, 34, 34),
                    BackColor = item.IsChecked ? Color.LightSkyBlue : Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(2)
                };

                cell.Click += Cell_Click;

                int row = i % itemsPerColumn;
                int col = i / itemsPerColumn;

                _tableLayoutPanel.Controls.Add(cell, col, row);
            }

            _tableLayoutPanel.ResumeLayout();
        }

        /// <summary>
        /// Digital Output 셀(Label) 클릭 시 체크 상태를 토글하고, 콘솔 출력 및 <see cref="ItemClicked"/> 이벤트를 호출합니다.
        /// </summary>
        /// <param name="sender">클릭 이벤트가 발생한 셀 컨트롤(<see cref="Label"/>)</param>
        /// <param name="e">이벤트 데이터</param>
        private void Cell_Click(object sender, EventArgs e)
        {

            string configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "config.json");


            if (sender is Label cell && cell.Tag is DigitalItemConfig item)
            {
                // 1. ON/OFF 상태 토글
                item.IsChecked = !item.IsChecked;
                cell.BackColor = item.IsChecked ? Color.LightSkyBlue : Color.White;
                item.ChannelNo = item.ChannelNo;

                // 2. IsChecked 상태에 따라 최종 출력 값 결정 (ON: OnValue, OFF: OffValue)
                int currentOutputValue = item.IsChecked ? item.OnValue : item.OffValue;

                // 3. 콘솔에 채널 번호 및 실제 출력값 출력
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [Digital Output] Ch:{item.ChannelNo} | Name:{item.Name} | State:{(item.IsChecked ? "ON" : "OFF")} | OutputValue:{currentOutputValue}");

                ItemClicked?.Invoke(item);
            }
        }
    }
}