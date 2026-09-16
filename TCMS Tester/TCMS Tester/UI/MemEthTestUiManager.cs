using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TCMSTester.UI
{
    public enum EMemTestState
    {
        Ready,      // 대기
        Testing,    // 검사 중
        Pass,       // 합격
        Fail        // 불합격
    }

    public class MemEthTestUiManager
    {
        private class MemCardUI
        {
            public Panel CardPanel { get; set; }
            public Label LblTitle { get; set; }
            public Label LblSubTitle { get; set; }
            public Button BtnSingleTest { get; set; }
            public Label LblStatus { get; set; }
            public Label LblDetail { get; set; }
            public Label LblResult { get; set; }
        }

        private readonly TableLayoutPanel _parentTable;
        private readonly Dictionary<string, MemCardUI> _cards = new Dictionary<string, MemCardUI>();

        public event Action<string> OnSingleTestRequested;

        public MemEthTestUiManager(TableLayoutPanel parentTable)
        {
            _parentTable = parentTable ?? throw new ArgumentNullException(nameof(parentTable));
            BuildLayout();
        }

        private void BuildLayout()
        {
            _parentTable.SuspendLayout();
            _parentTable.Controls.Clear();
            _parentTable.RowStyles.Clear();
            _parentTable.ColumnStyles.Clear();
            _cards.Clear();

            _parentTable.Dock = DockStyle.Fill;
            _parentTable.RowCount = 2;
            _parentTable.ColumnCount = 1;
            _parentTable.BackColor = Color.FromArgb(240, 244, 253);
            _parentTable.Padding = new Padding(8);

            _parentTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            _parentTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            _parentTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

            // 상단 4분할: VCPUT/860 메모리 4종
            TableLayoutPanel topTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 4,
                Margin = new Padding(0, 0, 0, 4),
                BackColor = Color.Transparent
            };
            topTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            for (int i = 0; i < 4; i++) topTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

            topTable.Controls.Add(CreateCard("DPRAM", "DPRAM", "860 ↔ iMX6SX 통신", "CMD: 0x0107 | 듀얼포트 RAM").CardPanel, 0, 0);
            topTable.Controls.Add(CreateCard("SDRAM", "SDRAM", "860 시스템 메모리", "CMD: 0x010A | 메인 RAM").CardPanel, 1, 0);
            topTable.Controls.Add(CreateCard("MRAM", "MRAM", "860 비휘발성 저장", "CMD: 0x010B | MRAM 검증").CardPanel, 2, 0);
            topTable.Controls.Add(CreateCard("FLASH", "FLASH", "860 펌웨어 ROM", "CMD: 0x010C | 플래시 메모리").CardPanel, 3, 0);

            // 하단 4분할: iMX6SX 저장소 & 이더넷 2포트
            TableLayoutPanel bottomTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 4,
                Margin = new Padding(0, 4, 0, 0),
                BackColor = Color.Transparent
            };
            bottomTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            for (int i = 0; i < 4; i++) bottomTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

            bottomTable.Controls.Add(CreateCard("EMMC", "eMMC", "iMX6SX 내장 플래시", "CMD: 0x0002 | 내장 저장소").CardPanel, 0, 0);
            bottomTable.Controls.Add(CreateCard("USB", "USB", "iMX6SX 외장 포트", "CMD: 0x0001 | 사전 삽입 확인").CardPanel, 1, 0);
            bottomTable.Controls.Add(CreateCard("ENET_1", "이더넷 #1", "10.0.1.11 : 5060", "CMD: 0x0003 | Echo-back").CardPanel, 2, 0);
            bottomTable.Controls.Add(CreateCard("ENET_2", "이더넷 #2", "10.0.2.11 : 5060", "CMD: 0x0003 | Echo-back").CardPanel, 3, 0);

            _parentTable.Controls.Add(topTable, 0, 0);
            _parentTable.Controls.Add(bottomTable, 0, 1);

            _parentTable.ResumeLayout(true);
        }

        private MemCardUI CreateCard(string key, string title, string subTitle, string defaultDetail)
        {
            Panel card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(5),
                Padding = new Padding(10)
            };

            // 1. 상단 정보 영역 컨테이너 (고정 높이로 안정적 배치)
            Panel pnlTopInfo = new Panel
            {
                Dock = DockStyle.Top,
                Height = 84,
                BackColor = Color.Transparent
            };

            // 1-1. 최상단 헤더 (타이틀 + 단독 버튼)
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 26,
                BackColor = Color.Transparent
            };

            Button btnSingle = new Button
            {
                Text = "단독 ▶",
                Font = new Font("맑은 고딕", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235),
                BackColor = Color.FromArgb(239, 246, 255),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(62, 24),
                Dock = DockStyle.Right,
                Cursor = Cursors.Hand
            };
            btnSingle.FlatAppearance.BorderColor = Color.FromArgb(191, 219, 254);
            btnSingle.Click += (s, e) => OnSingleTestRequested?.Invoke(key);

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("맑은 고딕", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(8, 31, 78),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(btnSingle);

            // 1-2. 보조 설명 라벨 (하위 스택)
            Label lblSub = new Label
            {
                Text = subTitle,
                Font = new Font("맑은 고딕", 8f, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(0, 28),
                Size = new Size(card.Width, 16),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            Label lblStatus = new Label
            {
                Text = "상태: 대기 중",
                Font = new Font("맑은 고딕", 8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(0, 46),
                Size = new Size(card.Width, 16),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            Label lblDetail = new Label
            {
                Text = defaultDetail,
                Font = new Font("맑은 고딕", 8f, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(0, 64),
                Size = new Size(card.Width, 16),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                AutoEllipsis = true
            };

            pnlTopInfo.Controls.Add(pnlHeader);
            pnlTopInfo.Controls.Add(lblSub);
            pnlTopInfo.Controls.Add(lblStatus);
            pnlTopInfo.Controls.Add(lblDetail);

            // 2. 결과 표시 영역 (하단 중앙에 큼직하게 배치)
            Label lblResult = new Label
            {
                Text = "READY",
                Font = new Font("맑은 고딕", 20f, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            card.Controls.Add(lblResult);
            card.Controls.Add(pnlTopInfo);

            var cardUI = new MemCardUI
            {
                CardPanel = card,
                LblTitle = lblTitle,
                LblSubTitle = lblSub,
                BtnSingleTest = btnSingle,
                LblStatus = lblStatus,
                LblDetail = lblDetail,
                LblResult = lblResult
            };

            _cards[key] = cardUI;
            return cardUI;
        }

        public void SetCardState(string key, EMemTestState state, string statusMsg = null, string detailMsg = null)
        {
            if (!_cards.ContainsKey(key)) return;
            var ui = _cards[key];

            if (ui.CardPanel.InvokeRequired)
            {
                ui.CardPanel.BeginInvoke(new Action(() => SetCardState(key, state, statusMsg, detailMsg)));
                return;
            }

            if (!string.IsNullOrEmpty(statusMsg)) ui.LblStatus.Text = $"상태: {statusMsg}";
            if (!string.IsNullOrEmpty(detailMsg)) ui.LblDetail.Text = detailMsg;

            switch (state)
            {
                case EMemTestState.Ready:
                    ui.LblResult.Text = "READY";
                    ui.LblResult.ForeColor = Color.FromArgb(148, 163, 184);
                    ui.CardPanel.BackColor = Color.White;
                    break;
                case EMemTestState.Testing:
                    ui.LblResult.Text = "TESTING...";
                    ui.LblResult.ForeColor = Color.FromArgb(217, 119, 6);
                    ui.CardPanel.BackColor = Color.FromArgb(254, 243, 199);
                    break;
                case EMemTestState.Pass:
                    ui.LblResult.Text = "PASS";
                    ui.LblResult.ForeColor = Color.FromArgb(37, 99, 235);
                    ui.CardPanel.BackColor = Color.FromArgb(239, 246, 255);
                    break;
                case EMemTestState.Fail:
                    ui.LblResult.Text = "FAIL";
                    ui.LblResult.ForeColor = Color.FromArgb(220, 38, 38);
                    ui.CardPanel.BackColor = Color.FromArgb(254, 226, 226);
                    break;
            }
        }

        public void SetAllButtonsEnabled(bool isEnabled)
        {
            foreach (var card in _cards.Values)
            {
                if (card.BtnSingleTest.InvokeRequired)
                    card.BtnSingleTest.BeginInvoke(new Action(() => card.BtnSingleTest.Enabled = isEnabled));
                else
                    card.BtnSingleTest.Enabled = isEnabled;
            }
        }

        public void ResetAll()
        {
            foreach (var key in _cards.Keys)
            {
                SetCardState(key, EMemTestState.Ready, "대기 중");
            }
        }
    }
}