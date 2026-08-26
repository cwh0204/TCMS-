using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;

namespace CITester
{
    [ToolboxItem(true)]
    public class MainTabControl : TabControl
    {
        private int _cornerRadius = 6;
        private Color _selectedColor = Color.FromArgb(255, 255, 255);
        private Color _unselectedColor = Color.FromArgb(215, 215, 215);
        private Color _hoverColor = Color.FromArgb(230, 230, 230);
        private Color _contentAreaColor = Color.FromArgb(255, 255, 255);
        private Color _selectedTextColor = Color.FromArgb(40, 40, 40);
        private Color _unselectedTextColor = Color.FromArgb(100, 100, 100);

        private int nHoveredIndex = -1;
        private bool bIsUpdatingSize = false;
        private bool bIsPainting = false;

        private Font _cachedBoldFont = null;
        private Font _cachedRegularFont = null;

        // ★ 100% 정확한 디자이너 감지 프로퍼티 (투명 버그 방어의 핵심)
        private bool bIsDesignMode => DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        public MainTabControl()
        {
            this.SetStyle(ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.SupportsTransparentBackColor, true);

            this.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.SizeMode = TabSizeMode.Fixed;
            this.ItemSize = new Size(130, 38);
            this.Padding = new Point(0, 0);
            this.BackColor = Color.White;
        }

        #region [ 디자인 창 속성 ]

        [Category("Custom Design"), Description("탭의 둥근 모서리 반경을 설정합니다.")]
        public int CornerRadius { get => _cornerRadius; set { _cornerRadius = Math.Max(0, value); Invalidate(); } }

        [Category("Custom Design"), Description("선택된 탭의 배경색을 설정합니다.")]
        public Color SelectedColor { get => _selectedColor; set { _selectedColor = value; Invalidate(); } }

        [Category("Custom Design"), Description("선택되지 않은 탭의 배경색을 설정합니다.")]
        public Color UnselectedColor { get => _unselectedColor; set { _unselectedColor = value; Invalidate(); } }

        [Category("Custom Design"), Description("마우스가 올라간(Hover) 탭의 배경색을 설정합니다.")]
        public Color HoverColor { get => _hoverColor; set { _hoverColor = value; Invalidate(); } }

        [Category("Custom Design"), Description("탭 페이지 하단 콘텐츠 영역의 배경색을 설정합니다.")]
        public Color ContentAreaColor { get => _contentAreaColor; set { _contentAreaColor = value; Invalidate(); } }

        [Category("Custom Design"), Description("선택된 탭의 글씨 색상을 설정합니다.")]
        public Color SelectedTextColor { get => _selectedTextColor; set { _selectedTextColor = value; Invalidate(); } }

        [Category("Custom Design"), Description("선택되지 않은 탭의 글씨 색상을 설정합니다.")]
        public Color UnselectedTextColor { get => _unselectedTextColor; set { _unselectedTextColor = value; Invalidate(); } }

        [Category("Custom Design"), Browsable(true), Description("컨트롤 전체의 배경색을 설정합니다.")]
        public override Color BackColor { get => base.BackColor; set { base.BackColor = value; Invalidate(); } }

        #endregion

        #region [ 마우스 호버 이벤트 ]

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int nNewHoverIndex = -1;
            for (int i = 0; i < this.TabCount; i++)
            {
                if (this.GetTabRect(i).Contains(e.Location))
                {
                    nNewHoverIndex = i;
                    break;
                }
            }
            if (nHoveredIndex != nNewHoverIndex)
            {
                nHoveredIndex = nNewHoverIndex;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            nHoveredIndex = -1;
            Invalidate();
        }

        #endregion

        #region [ 핵심 오버라이드 - 디자이너 보호 ]

        protected override void WndProc(ref Message m)
        {
            // 디자이너 환경을 확장 검증하여 기본 배경 깜빡임 메시지 차단
            if (!bIsDesignMode && m.Msg == 0x0014) { m.Result = (IntPtr)1; return; }
            base.WndProc(ref m);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (bIsDesignMode)
                base.OnPaintBackground(e);
        }

        public override Rectangle DisplayRectangle
        {
            get
            {
                // 디자이너 모드일 때는 자식 컨트롤들의 정상 배치를 위해 영역을 가공하지 않음
                if (bIsDesignMode) return base.DisplayRectangle;

                // [수정] 탭 선택 상태에 따라 가로 폭(Width)이 미세하게 흔들리는 base.DisplayRectangle을 과감히 버립니다.
                // 탭 컨트롤 자체의 물리적 상단 헤더 높이를 제외한 가용 영역을 완벽한 상수로 고정하여 단차를 원천 차단합니다.
                int nHeaderHeight = this.ItemSize.Height;

                // Left 오프셋을 0으로 고정하고 전체 Width를 탭페이지 크기와 1:1로 일치시킵니다.
                return new Rectangle(0, nHeaderHeight, this.Width, this.Height - nHeaderHeight);
            }
        }

        #endregion

        #region [ 기존 로직 유지 및 최적화 ]

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            RebuildFontCache();
            UpdateTabSize();
        }

        private void RebuildFontCache()
        {
            _cachedBoldFont?.Dispose();
            _cachedRegularFont?.Dispose();
            _cachedBoldFont = new Font("맑은 고딕", 13f, FontStyle.Bold);
            _cachedRegularFont = new Font("맑은 고딕", 13f, FontStyle.Regular);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateTabSize();
            this.Invalidate();
        }

        public void ForceUpdateTabSize()
        {
            if (this.TabCount <= 0 || this.Width <= 0) return;
            int nTotalWidth = this.Width - 3;
            int nBaseWidth = Math.Max(20, nTotalWidth / this.TabCount);
            this.ItemSize = new Size(nBaseWidth, 38);
            this.Invalidate();
        }

        private void UpdateTabSize()
        {
            if (bIsUpdatingSize || this.TabCount <= 0 || this.Width <= 0) return;
            try
            {
                bIsUpdatingSize = true;
                int nTotalWidth = this.Width - 3;
                int nBaseWidth = Math.Max(20, nTotalWidth / this.TabCount);
                Size newSize = new Size(nBaseWidth, 38);

                // 크기 속성이 실제로 다를 때만 대입하여 무한 레이아웃 루프(떨림 버그) 차단
                if (this.ItemSize.Width != newSize.Width || this.ItemSize.Height != newSize.Height)
                    this.ItemSize = newSize;
            }
            finally { bIsUpdatingSize = false; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (bIsPainting) return;
            bIsPainting = true;

            try
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                Color parentColor = this.Parent != null ? this.Parent.BackColor : this.BackColor;
                g.Clear(parentColor);

                if (this.TabCount <= 0) return;

                // 2. 하단 콘텐츠 영역 배경 채우기
                int nHeaderHeight = this.ItemSize.Height;
                using (SolidBrush fillBrush = new SolidBrush(_contentAreaColor))
                {
                    g.FillRectangle(fillBrush, 0, nHeaderHeight, this.Width, this.Height - nHeaderHeight);
                }

                // 3. 탭 버튼 전체 출력
                DrawAllTabs(g);
            }
            finally { bIsPainting = false; }
        }

        private void DrawAllTabs(Graphics g)
        {
            for (int i = 0; i < this.TabCount; i++)
            {
                Rectangle rect = this.GetTabRect(i);
                bool bIsSelected = (this.SelectedIndex == i);
                bool bIsHovered = (nHoveredIndex == i);

                if (i == 0) { int nOffset = rect.X; rect.X = 0; rect.Width += nOffset; }

                // 마지막 탭 우측 여백 채우기 오차 보정
                if (i == this.TabCount - 1)
                {
                    int nRemainingWidth = this.Width - rect.X;
                    if (Math.Abs(nRemainingWidth - rect.Width) <= 10)
                    {
                        rect.Width = nRemainingWidth;
                    }
                }

                if (bIsSelected) { rect.Y -= 1; rect.Height += 2; }
                else { rect.Y += 1; rect.Height -= 1; }

                DrawRoundedTab(g, rect, i, bIsSelected, bIsHovered);
            }
        }

        private void DrawRoundedTab(Graphics g, Rectangle rect, int nIndex, bool bIsSelected, bool bIsHovered)
        {
            if (nIndex < 0 || nIndex >= this.TabPages.Count || rect.Width <= 0 || rect.Height <= 0) return;

            int nDiameter = _cornerRadius * 2;
            if (nDiameter > rect.Width) nDiameter = rect.Width;
            if (nDiameter > rect.Height) nDiameter = rect.Height;

            // 선 두께가 삐져나가지 않도록 드로잉 사각형 1px 축소 정돈
            rect.Width -= 1;

            using (GraphicsPath path = new GraphicsPath())
            {
                if (nDiameter > 0)
                {
                    // ★ 수동 선 연결 논리 오류를 수정하고 호와 패스의 정석 조립법으로 복구했습니다.
                    path.AddArc(rect.X, rect.Y, nDiameter, nDiameter, 180, 90);
                    path.AddArc(rect.Right - nDiameter, rect.Y, nDiameter, nDiameter, 270, 90);
                    path.AddLine(rect.Right, rect.Y + nDiameter, rect.Right, rect.Bottom);
                    path.AddLine(rect.Right, rect.Bottom, rect.X, rect.Bottom);
                }
                else
                {
                    path.AddRectangle(rect);
                }
                path.CloseFigure();

                // 상태별 배경 브러시 채우기
                Color fillStyleColor = bIsSelected ? _selectedColor : (bIsHovered ? _hoverColor : _unselectedColor);
                using (SolidBrush br = new SolidBrush(fillStyleColor))
                    g.FillPath(br, path);

                // 테두리 드로잉
                Color borderStyleColor = bIsSelected ? Color.FromArgb(200, 200, 200) : Color.FromArgb(190, 190, 190);
                using (Pen pen = new Pen(borderStyleColor, 1))
                {
                    g.DrawPath(pen, path);
                }

                // 텍스트 출력
                string strTabText = this.TabPages[nIndex].Text;
                Font fontStyle = bIsSelected ? _cachedBoldFont : _cachedRegularFont;
                Color textStyleColor = bIsSelected ? _selectedTextColor : _unselectedTextColor;

                TextRenderer.DrawText(g, strTabText, fontStyle, rect, textStyleColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cachedBoldFont?.Dispose();
                _cachedRegularFont?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}