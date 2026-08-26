using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

namespace CustomControls
{
    public class QuickHistoryComboBox : UserControl
    {
        private TextBox m_txtInput;
        private Button m_btnAction;
        private ToolStripDropDown m_tddMenu;
        private DoubleBufferedListBox m_lbItems;
        private Panel m_pnlInputWrapper;

        private bool m_bIsEditMode = false;
        private bool m_bIsHovered = false;
        private int m_nHoveredIndex = -1;
        private string m_strPreviousText = string.Empty;

        private Color m_clsPrimaryBlue = Color.FromArgb(37, 99, 235);
        private Color m_clsPrimaryBlueHover = Color.FromArgb(29, 78, 216);
        private Color m_clsConfirmGreen = Color.FromArgb(16, 185, 129);
        private Color m_clsConfirmGreenHover = Color.FromArgb(5, 150, 105);
        private Color m_clsDeleteRed = Color.FromArgb(239, 68, 68);

        private Color m_clsBorderDefault = Color.FromArgb(209, 213, 219);
        private Color m_clsBorderHover = Color.FromArgb(59, 130, 246);
        private Color m_clsBgHover = Color.FromArgb(249, 250, 251);
        private Color m_clsListHover = Color.FromArgb(239, 246, 255);
        private Color m_clsArrowColor = Color.FromArgb(107, 114, 128);

        private int m_nItemHeight = 34;

        public event EventHandler<string> ItemAdded;
        public event EventHandler<string> ItemDeleted;
        public event EventHandler<string> ItemSelected;

        public QuickHistoryComboBox()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);
            this.UpdateStyles();

            InitializeComponentLayout();
            SetEditMode(false);
        }

        [Category("Data")]
        [Description("디자인 창에서 미리 등록할 초기 리스트 항목들입니다.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
        public ListBox.ObjectCollection Items => m_lbItems.Items;

        [Category("Appearance")]
        [Description("입력 텍스트 박스의 현재 값입니다.")]
        public string TextValue
        {
            get => m_txtInput.Text;
            set
            {
                m_txtInput.Text = value;
                m_pnlInputWrapper?.Invalidate();
            }
        }

        private void InitializeComponentLayout()
        {
            this.SuspendLayout();
            this.Padding = new Padding(1);
            this.BackColor = m_clsBorderDefault;

            m_pnlInputWrapper = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };
            m_pnlInputWrapper.Paint += PnlInputWrapper_Paint;
            m_pnlInputWrapper.Click += TxtInput_Click;

            m_txtInput = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("맑은 고딕", 9.75F, FontStyle.Regular),
                BorderStyle = BorderStyle.None,
                Visible = false
            };
            m_txtInput.KeyDown += TxtInput_KeyDown;

            m_pnlInputWrapper.Controls.Add(m_txtInput);

            m_btnAction = new Button
            {
                Width = 28,
                Dock = DockStyle.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };
            m_btnAction.FlatAppearance.BorderSize = 0;
            m_btnAction.Paint += BtnAction_Paint;
            m_btnAction.Click += BtnAction_Click;

            m_lbItems = new DoubleBufferedListBox
            {
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = m_nItemHeight,
                BorderStyle = BorderStyle.None,
                Font = new Font("맑은 고딕", 9.5F, FontStyle.Regular)
            };
            m_lbItems.DrawItem += LbItems_DrawItem;
            m_lbItems.MouseMove += LbItems_MouseMove;
            m_lbItems.MouseLeave += (sender, e) => { m_nHoveredIndex = -1; m_lbItems.Invalidate(); };
            m_lbItems.MouseDown += LbItems_MouseDown;

            ToolStripControlHost clsHost = new ToolStripControlHost(m_lbItems) { Margin = Padding.Empty, Padding = Padding.Empty, AutoSize = false };
            m_tddMenu = new ToolStripDropDown { Margin = Padding.Empty, Padding = Padding.Empty, AutoSize = false };
            m_tddMenu.Items.Add(clsHost);

            BindHoverEvents(this);
            BindHoverEvents(m_pnlInputWrapper);
            BindHoverEvents(m_btnAction);

            this.Controls.Add(m_pnlInputWrapper);
            this.Controls.Add(m_btnAction);
            this.Size = new Size(280, 32);

            this.ResumeLayout(false);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            if (m_txtInput != null && m_pnlInputWrapper != null)
            {
                int nTopPadding = Math.Max(0, (m_pnlInputWrapper.Height - m_txtInput.PreferredHeight) / 2);
                m_pnlInputWrapper.Padding = new Padding(8, nTopPadding, 24, 0);
            }
        }

        private void BindHoverEvents(Control clsControl)
        {
            clsControl.MouseEnter += (sender, e) => UpdateHoverState(true);
            clsControl.MouseLeave += (sender, e) =>
            {
                Point ptMouse = this.PointToClient(Cursor.Position);
                if (!this.ClientRectangle.Contains(ptMouse))
                {
                    UpdateHoverState(false);
                }
            };
        }

        private void UpdateHoverState(bool bIsHover)
        {
            m_bIsHovered = bIsHover;
            this.BackColor = (bIsHover || m_bIsEditMode) ? m_clsBorderHover : m_clsBorderDefault;

            if (!m_bIsEditMode)
            {
                m_pnlInputWrapper.BackColor = bIsHover ? m_clsBgHover : Color.White;
                m_btnAction.BackColor = bIsHover ? m_clsBgHover : Color.White;
            }
            this.Invalidate();
        }

        private void PnlInputWrapper_Paint(object sender, PaintEventArgs e)
        {
            if (m_bIsEditMode) return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rectText = new Rectangle(8, 0, m_pnlInputWrapper.Width - 32, m_pnlInputWrapper.Height);
            TextRenderer.DrawText(e.Graphics, m_txtInput.Text, m_txtInput.Font, rectText, Color.FromArgb(31, 41, 55), TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

            int nArrowWidth = 20;
            Rectangle rectArrow = new Rectangle(m_pnlInputWrapper.Width - nArrowWidth, 0, nArrowWidth, m_pnlInputWrapper.Height);
            int nCenterX = rectArrow.X + (rectArrow.Width / 2) - 2;
            int nCenterY = rectArrow.Height / 2;

            Point[] arrArrowPoints = new Point[]
            {
                new Point(nCenterX - 3, nCenterY - 1),
                new Point(nCenterX + 3, nCenterY - 1),
                new Point(nCenterX, nCenterY + 3)
            };

            using (SolidBrush brushArrow = new SolidBrush(m_clsArrowColor))
            {
                e.Graphics.FillPolygon(brushArrow, arrArrowPoints);
            }
        }

        private void BtnAction_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(m_pnlInputWrapper.BackColor);

            int nBtnSize = 22;
            Rectangle rectCircle = new Rectangle((m_btnAction.Width - nBtnSize) / 2, (m_btnAction.Height - nBtnSize) / 2, nBtnSize, nBtnSize);

            Point ptMouse = m_btnAction.PointToClient(Cursor.Position);
            bool bIsBtnHover = m_btnAction.ClientRectangle.Contains(ptMouse);

            if (!m_bIsEditMode)
            {
                Color clsBgColor = bIsBtnHover ? m_clsPrimaryBlueHover : m_clsPrimaryBlue;
                using (SolidBrush brushCircle = new SolidBrush(clsBgColor))
                {
                    e.Graphics.FillEllipse(brushCircle, rectCircle);
                }

                int nCenterX = rectCircle.X + (rectCircle.Width / 2);
                int nCenterY = rectCircle.Y + (rectCircle.Height / 2);
                int nSize = 4;

                using (Pen penPlus = new Pen(Color.White, 2.0f))
                {
                    e.Graphics.DrawLine(penPlus, nCenterX - nSize, nCenterY, nCenterX + nSize, nCenterY);
                    e.Graphics.DrawLine(penPlus, nCenterX, nCenterY - nSize, nCenterX, nCenterY + nSize);
                }
            }
            else
            {
                Color clsBgColor = bIsBtnHover ? m_clsConfirmGreenHover : m_clsConfirmGreen;
                using (SolidBrush brushCircle = new SolidBrush(clsBgColor))
                {
                    e.Graphics.FillEllipse(brushCircle, rectCircle);
                }

                int nCenterX = rectCircle.X + (rectCircle.Width / 2);
                int nCenterY = rectCircle.Y + (rectCircle.Height / 2);

                Point[] arrCheckPoints = new Point[]
                {
                    new Point(nCenterX - 4, nCenterY),
                    new Point(nCenterX - 1, nCenterY + 3),
                    new Point(nCenterX + 4, nCenterY - 3)
                };

                using (Pen penCheck = new Pen(Color.White, 2.0f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    e.Graphics.DrawLines(penCheck, arrCheckPoints);
                }
            }
        }

        private void SetEditMode(bool bEnable)
        {
            m_bIsEditMode = bEnable;

            if (bEnable)
            {
                m_strPreviousText = m_txtInput.Text;
                m_txtInput.Clear();

                m_txtInput.Visible = true;
                m_txtInput.Focus();

                m_pnlInputWrapper.BackColor = Color.White;
                m_btnAction.BackColor = Color.White;
                this.BackColor = m_clsBorderHover;
            }
            else
            {
                m_txtInput.Visible = false;
                UpdateHoverState(m_bIsHovered);
            }

            m_pnlInputWrapper.Invalidate();
            m_btnAction.Invalidate();
        }

        private void TxtInput_Click(object sender, EventArgs e)
        {
            if (!m_bIsEditMode)
            {
                BtnDropdown_Click(this, EventArgs.Empty);
            }
        }

        private void BtnAction_Click(object sender, EventArgs e)
        {
            if (!m_bIsEditMode)
            {
                SetEditMode(true);
            }
            else
            {
                SaveCurrentInput();
            }
        }

        private void SaveCurrentInput()
        {
            string strText = m_txtInput.Text.Trim();
            if (!string.IsNullOrEmpty(strText))
            {
                AddItem(strText);
                m_txtInput.Text = strText;
            }
            else
            {
                m_txtInput.Text = m_strPreviousText;
            }
            SetEditMode(false);
        }

        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && m_bIsEditMode)
            {
                SaveCurrentInput();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape && m_bIsEditMode)
            {
                m_txtInput.Text = m_strPreviousText;
                SetEditMode(false);
                e.SuppressKeyPress = true;
            }
        }

        public void AddItem(string strText)
        {
            if (string.IsNullOrWhiteSpace(strText)) return;

            if (!m_lbItems.Items.Contains(strText))
            {
                m_lbItems.Items.Insert(0, strText);
                ItemAdded?.Invoke(this, strText);
            }
        }

        public void RemoveItem(string strText)
        {
            if (m_lbItems.Items.Contains(strText))
            {
                m_lbItems.Items.Remove(strText);
                ItemDeleted?.Invoke(this, strText);
            }
        }

        private void BtnDropdown_Click(object sender, EventArgs e)
        {
            if (m_lbItems.Items.Count == 0) return;

            int nWidth = this.Width;
            int nHeight = Math.Min(m_lbItems.Items.Count * m_nItemHeight + 4, 220);

            m_lbItems.Size = new Size(nWidth, nHeight);
            m_tddMenu.Size = new Size(nWidth, nHeight);

            m_tddMenu.Show(this, new Point(0, this.Height));
        }

        private void LbItems_MouseMove(object sender, MouseEventArgs e)
        {
            int nIndex = m_lbItems.IndexFromPoint(e.Location);
            if (nIndex != m_nHoveredIndex)
            {
                int nOldIndex = m_nHoveredIndex;
                m_nHoveredIndex = nIndex;

                if (nOldIndex >= 0 && nOldIndex < m_lbItems.Items.Count)
                {
                    m_lbItems.Invalidate(m_lbItems.GetItemRectangle(nOldIndex));
                }

                if (m_nHoveredIndex >= 0 && m_nHoveredIndex < m_lbItems.Items.Count)
                {
                    m_lbItems.Invalidate(m_lbItems.GetItemRectangle(m_nHoveredIndex));
                }
            }
        }

        private void LbItems_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= m_lbItems.Items.Count) return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            string strItemText = m_lbItems.Items[e.Index].ToString();
            bool bIsHovered = (e.Index == m_nHoveredIndex);
            bool bIsSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            Color clsBgColor = (bIsHovered || bIsSelected) ? m_clsListHover : Color.White;
            using (SolidBrush brushBg = new SolidBrush(clsBgColor))
            {
                e.Graphics.FillRectangle(brushBg, e.Bounds);
            }

            Rectangle rectText = new Rectangle(e.Bounds.X + 10, e.Bounds.Y, e.Bounds.Width - 40, e.Bounds.Height);
            TextRenderer.DrawText(e.Graphics, strItemText, m_lbItems.Font, rectText, Color.FromArgb(31, 41, 55), TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

            int nBtnSize = 20;
            Rectangle rectDeleteBtn = new Rectangle(e.Bounds.Right - 28, e.Bounds.Y + (e.Bounds.Height - nBtnSize) / 2, nBtnSize, nBtnSize);

            using (SolidBrush brushDelBg = new SolidBrush(m_clsDeleteRed))
            {
                e.Graphics.FillEllipse(brushDelBg, rectDeleteBtn);
            }

            using (Pen penMinus = new Pen(Color.White, 2.0f))
            {
                int nLineY = rectDeleteBtn.Y + (rectDeleteBtn.Height / 2);
                e.Graphics.DrawLine(penMinus, rectDeleteBtn.X + 5, nLineY, rectDeleteBtn.Right - 5, nLineY);
            }
        }

        private void LbItems_MouseDown(object sender, MouseEventArgs e)
        {
            int nIndex = m_lbItems.IndexFromPoint(e.Location);
            if (nIndex == ListBox.NoMatches) return;

            Rectangle rectItem = m_lbItems.GetItemRectangle(nIndex);
            int nBtnSize = 20;
            Rectangle rectDeleteBtn = new Rectangle(rectItem.Right - 28, rectItem.Y + (rectItem.Height - nBtnSize) / 2, nBtnSize, nBtnSize);

            if (rectDeleteBtn.Contains(e.Location))
            {
                string strTarget = m_lbItems.Items[nIndex].ToString();
                RemoveItem(strTarget);

                if (m_lbItems.Items.Count == 0)
                {
                    m_tddMenu.Close();
                }
                else
                {
                    int nHeight = Math.Min(m_lbItems.Items.Count * m_nItemHeight + 4, 220);
                    m_lbItems.Size = new Size(this.Width, nHeight);
                    m_tddMenu.Size = new Size(this.Width, nHeight);
                }
            }
            else
            {
                string strSelected = m_lbItems.Items[nIndex].ToString();
                TextValue = strSelected;
                ItemSelected?.Invoke(this, strSelected);
                m_tddMenu.Close();
                SetEditMode(false);
            }
        }
    }

    internal class DoubleBufferedListBox : ListBox
    {
        private const int WM_ERASEBKGND = 0x0014;

        public DoubleBufferedListBox()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            PropertyInfo propDoubleBuffer = typeof(ListBox).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
            propDoubleBuffer?.SetValue(this, true, null);
        }

        protected override void WndProc(ref Message m)
        {
            // Windows OS의 기본 배경 삭제 메시지를 차단하여 백색 깜빡임(Flicker) 완벽 방지
            if (m.Msg == WM_ERASEBKGND)
            {
                m.Result = (IntPtr)1;
                return;
            }
            base.WndProc(ref m);
        }
    }
}