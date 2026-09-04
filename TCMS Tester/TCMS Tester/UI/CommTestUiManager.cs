using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TCMSTester.UI
{
    public enum ECommTestState
    {
        Ready,      // 대기
        Testing,    // 검사 중
        Pass,       // 합격
        Fail        // 불합격
    }

    public class CommTestUiManager
    {
        private class CommCardUI
        {
            public Panel CardPanel { get; set; }
            public Label LblTitle { get; set; }
            public Button BtnSingleTest { get; set; }
            public Label LblStatus { get; set; }
            public Label LblDetail { get; set; }
            public Label LblResult { get; set; }
        }

        private readonly TableLayoutPanel _parentTable;
        private readonly Dictionary<string, CommCardUI> _commCards = new Dictionary<string, CommCardUI>();

        // 단독 시험 버튼 클릭 시 FormMain으로 전달할 콜백 이벤트
        public event Action<string> OnSingleTestRequested;

        public CommTestUiManager(TableLayoutPanel parentTable)
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
            _commCards.Clear();

            _parentTable.Dock = DockStyle.Fill;
            _parentTable.RowCount = 2;
            _parentTable.ColumnCount = 1;
            _parentTable.BackColor = Color.FromArgb(240, 244, 253);
            _parentTable.Padding = new Padding(10);

            _parentTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            _parentTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            _parentTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

            // 상단 2분할 (WTB, MVB)
            TableLayoutPanel topTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 2,
                Margin = new Padding(0, 0, 0, 6),
                BackColor = Color.Transparent
            };
            topTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            topTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            topTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

            topTable.Controls.Add(CreateCommCard("WTB", "WTB 통신", "노드 주소: 0x01 | 텔레그램 검증").CardPanel, 0, 0);
            topTable.Controls.Add(CreateCommCard("MVB", "MVB 통신", "포트 주소: 41A0 | 주기 데이터 수신").CardPanel, 1, 0);

            // 하단 3분할 (RS485 #1, #2, #3)
            TableLayoutPanel bottomTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 3,
                Margin = new Padding(0, 6, 0, 0),
                BackColor = Color.Transparent
            };
            bottomTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            bottomTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            bottomTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            bottomTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34f));

            bottomTable.Controls.Add(CreateCommCard("RS485_1", "RS485 - #1", "115200 bps | 에코백 검증").CardPanel, 0, 0);
            bottomTable.Controls.Add(CreateCommCard("RS485_2", "RS485 - #2", "115200 bps | 에코백 검증").CardPanel, 1, 0);
            bottomTable.Controls.Add(CreateCommCard("RS485_3", "RS485 - #3", "9600 bps | 에코백 검증").CardPanel, 2, 0);

            _parentTable.Controls.Add(topTable, 0, 0);
            _parentTable.Controls.Add(bottomTable, 0, 1);

            _parentTable.ResumeLayout(true);
        }

        private CommCardUI CreateCommCard(string key, string title, string defaultDetail)
        {
            Panel card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(6),
                Padding = new Padding(14)
            };

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("맑은 고딕", 12f, FontStyle.Bold),
                ForeColor = Color.FromArgb(8, 31, 78),
                Location = new Point(14, 14),
                AutoSize = true
            };

            Button btnSingle = new Button
            {
                Text = "단독 시험 ▶",
                Font = new Font("맑은 고딕", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235),
                BackColor = Color.FromArgb(239, 246, 255),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(95, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
                ,
                Visible = false
            };
            btnSingle.FlatAppearance.BorderColor = Color.FromArgb(191, 219, 254);
            btnSingle.Location = new Point(card.Width - btnSingle.Width - 14, 12);
            btnSingle.Click += (s, e) => OnSingleTestRequested?.Invoke(key);

            Label lblStatus = new Label
            {
                Text = "상태: 대기 중",
                Font = new Font("맑은 고딕", 9.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(14, 48),
                AutoSize = true
            };

            Label lblDetail = new Label
            {
                Text = defaultDetail,
                Font = new Font("맑은 고딕", 9f, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(14, 72),
                AutoSize = true
            };

            Label lblResult = new Label
            {
                Text = "READY",
                Font = new Font("맑은 고딕", 20f, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Bottom,
                Height = 50
            };

            card.Controls.Add(lblTitle);
            card.Controls.Add(btnSingle);
            card.Controls.Add(lblStatus);
            card.Controls.Add(lblDetail);
            card.Controls.Add(lblResult);

            var cardUI = new CommCardUI
            {
                CardPanel = card,
                LblTitle = lblTitle,
                BtnSingleTest = btnSingle,
                LblStatus = lblStatus,
                LblDetail = lblDetail,
                LblResult = lblResult
            };

            _commCards[key] = cardUI;
            return cardUI;
        }

        public void SetCardState(string key, ECommTestState state, string statusMsg = null, string detailMsg = null)
        {
            if (!_commCards.ContainsKey(key)) return;
            var ui = _commCards[key];

            if (ui.CardPanel.InvokeRequired)
            {
                ui.CardPanel.BeginInvoke(new Action(() => SetCardState(key, state, statusMsg, detailMsg)));
                return;
            }

            if (!string.IsNullOrEmpty(statusMsg)) ui.LblStatus.Text = $"상태: {statusMsg}";
            if (!string.IsNullOrEmpty(detailMsg)) ui.LblDetail.Text = detailMsg;

            switch (state)
            {
                case ECommTestState.Ready:
                    ui.LblResult.Text = "READY";
                    ui.LblResult.ForeColor = Color.FromArgb(148, 163, 184);
                    ui.CardPanel.BackColor = Color.White;
                    break;
                case ECommTestState.Testing:
                    ui.LblResult.Text = "TESTING...";
                    ui.LblResult.ForeColor = Color.FromArgb(217, 119, 6);
                    ui.CardPanel.BackColor = Color.FromArgb(254, 243, 199);
                    break;
                case ECommTestState.Pass:
                    ui.LblResult.Text = "PASS";
                    ui.LblResult.ForeColor = Color.FromArgb(37, 99, 235);
                    ui.CardPanel.BackColor = Color.FromArgb(239, 246, 255);
                    break;
                case ECommTestState.Fail:
                    ui.LblResult.Text = "FAIL";
                    ui.LblResult.ForeColor = Color.FromArgb(220, 38, 38);
                    ui.CardPanel.BackColor = Color.FromArgb(254, 226, 226);
                    break;
            }
        }

        public void SetAllButtonsEnabled(bool isEnabled)
        {
            foreach (var card in _commCards.Values)
            {
                if (card.BtnSingleTest.InvokeRequired)
                    card.BtnSingleTest.BeginInvoke(new Action(() => card.BtnSingleTest.Enabled = isEnabled));
                else
                    card.BtnSingleTest.Enabled = isEnabled;
            }
        }

        /// <summary>
        /// 전체 카드를 READY(대기) 상태로 초기화합니다.
        /// </summary>
        public void ResetAll()
        {
            foreach (var key in _commCards.Keys)
            {
                SetCardState(key, ECommTestState.Ready, "대기 중");
            }
        }
    }
}