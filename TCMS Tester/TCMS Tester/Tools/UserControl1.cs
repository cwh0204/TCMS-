using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CITester
{
    public class FlatTabControl : TabControl
    {
        private int _nHoveredIndex = -1;
        private int _nLinePadding = 20;
        private bool _bUseSingleLine = true;

        private bool _bShowContentBorder = false;
        private Color _clrContentBorder = Color.LightGray;

        private bool _bHideTabHeader = false;

        private Font _boldFont;
        private StringFormat _sf;

        private bool IsDesignMode => DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        #region Properties

        [Category("Flat Design")]
        [Description("탭 버튼(헤더) 영역을 완전히 숨길지 여부를 설정합니다.")]
        public bool HideTabHeader
        {
            get => _bHideTabHeader;
            set
            {
                _bHideTabHeader = value;
                UpdateTabPageBounds();
                Invalidate();
            }
        }

        [Category("Flat Design")]
        [Description("탭 페이지의 본문(내용) 영역 배경색을 일괄 설정합니다.")]
        public Color ContentBackColor { get; set; } = Color.White;

        [Category("Flat Design")]
        [Description("탭 본문(내용) 영역에 외곽선을 그릴지 여부를 설정합니다.")]
        public bool ShowContentBorder
        {
            get => _bShowContentBorder;
            set { _bShowContentBorder = value; Invalidate(); }
        }

        [Category("Flat Design")]
        [Description("탭 본문(내용) 영역의 외곽선 색상을 설정합니다.")]
        public Color ContentBorderColor
        {
            get => _clrContentBorder;
            set { _clrContentBorder = value; Invalidate(); }
        }

        [Category("Flat Design")]
        public new Size ItemSize
        {
            get => base.ItemSize;
            set { base.ItemSize = value; Invalidate(); }
        }

        [Category("Flat Design")]
        public bool UseSingleLine
        {
            get => _bUseSingleLine;
            set { _bUseSingleLine = value; RebuildStringFormat(); Invalidate(); }
        }

        [Category("Flat Design")]
        public int LinePadding
        {
            get => _nLinePadding;
            set { _nLinePadding = value; Invalidate(); }
        }

        [Category("Flat Design")]
        [Description("탭 버튼의 위쪽 모서리 둥글기(반경)를 설정합니다.")]
        public int TabRadius { get; set; } = 6;

        [Category("Flat Design")]
        [Description("탭 버튼에 외곽선(테두리)을 그릴지 여부를 설정합니다.")]
        public bool ShowTabBorders { get; set; } = false;

        [Category("Flat Design")]
        [Description("탭 버튼의 테두리 색상을 설정합니다.")]
        public Color TabBorderColor { get; set; } = Color.LightGray;

        [Category("Flat Design")] public Color TabColor { get; set; } = Color.FromArgb(240, 240, 240);
        [Category("Flat Design")] public Color HoverTextColor { get; set; } = Color.FromArgb(0, 122, 204);
        [Category("Flat Design")] public Color SelectedColor { get; set; } = Color.White;
        [Category("Flat Design")] public Color TextColor { get; set; } = Color.DimGray;
        [Category("Flat Design")] public Color SelectedTextColor { get; set; } = Color.Black;
        [Category("Flat Design")] public Color LineColor { get; set; } = Color.LightGray;
        [Category("Flat Design")] public Color SelectedLineColor { get; set; } = Color.FromArgb(0, 122, 204);
        [Category("Flat Design")] public Color HeaderBackColor { get; set; } = Color.FromArgb(240, 240, 240);

        #endregion

        public FlatTabControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            DrawMode = TabDrawMode.OwnerDrawFixed;
            base.ItemSize = new Size(120, 40);
            SizeMode = TabSizeMode.Fixed;
            Padding = new Point(0, 0);

            RebuildStringFormat();
        }

        protected override void WndProc(ref Message m)
        {
            if (!IsDesignMode)
            {
                if (m.Msg == 0x0014) { m.Result = (IntPtr)1; return; }
                if (m.Msg == 0x0085) { m.Result = IntPtr.Zero; return; }
            }
            base.WndProc(ref m);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (!IsDesignMode)
                {
                    cp.ExStyle &= ~0x00000200;
                    cp.Style &= ~0x00800000;
                }
                return cp;
            }
        }

        public override Rectangle DisplayRectangle
        {
            get
            {
                if (IsDesignMode && !_bHideTabHeader) return base.DisplayRectangle;

                int nHeaderH = ItemSize.Height;

                if (ShowContentBorder)
                {
                    return new Rectangle(1, nHeaderH + 1, Width - 2, Height - nHeaderH - 2);
                }
                else
                {
                    return new Rectangle(0, nHeaderH, Width, Height - nHeaderH);
                }
            }
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            UpdateTabPageBounds();
            Invalidate();
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            UpdateTabPageBounds();

            foreach (Control ctrlChild in Controls)
            {
                if (!(ctrlChild is TabPage))
                {
                    ctrlChild.BringToFront();
                }
            }
        }

        private void UpdateTabPageBounds()
        {
            if (IsDesignMode || SelectedTab == null) return;

            try
            {
                Rectangle rectCorrect = DisplayRectangle;
                if (SelectedTab.Bounds != rectCorrect)
                {
                    SelectedTab.Bounds = rectCorrect;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error] 탭 레이아웃 동기화 중 예외 발생: {ex.Message}");
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            RebuildBoldFont();
            Invalidate();
            Update();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            RebuildBoldFont();
            Invalidate();
            Update();
        }

        private void RebuildBoldFont()
        {
            _boldFont?.Dispose();
            _boldFont = new Font(this.Font, FontStyle.Bold);
        }

        private void RebuildStringFormat()
        {
            _sf?.Dispose();
            _sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            if (_bUseSingleLine)
            {
                _sf.FormatFlags = StringFormatFlags.NoWrap;
                _sf.Trimming = StringTrimming.None;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _boldFont?.Dispose();
                _sf?.Dispose();
            }
            base.Dispose(disposing);
        }

        private GraphicsPath GetTabRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int width = rect.Width - 1;
            int height = rect.Height - 1;

            if (radius <= 0)
            {
                path.AddRectangle(new Rectangle(rect.X, rect.Y, width, height));
                return path;
            }

            int d = radius * 2;
            d = Math.Min(d, Math.Min(width, height));

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.X + width - d, rect.Y, d, d, 270, 90);
            path.AddLine(rect.X + width, rect.Y + height, rect.X, rect.Y + height);

            path.CloseFigure();
            return path;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_boldFont == null) RebuildBoldFont();
            if (_sf == null) RebuildStringFormat();

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;

            using (var br = new SolidBrush(HeaderBackColor))
                g.FillRectangle(br, ClientRectangle);

            if (TabCount > 0 && SelectedIndex >= 0)
            {
                int nHeaderH = ItemSize.Height;
                Rectangle bodyRect = new Rectangle(0, nHeaderH, Width, Height - nHeaderH);

                using (var bodyBr = new SolidBrush(ContentBackColor))
                    g.FillRectangle(bodyBr, bodyRect);

                if (ShowContentBorder)
                {
                    Rectangle borderRect = new Rectangle(0, nHeaderH, Width - 1, Height - nHeaderH - 1);
                    using (var borderPen = new Pen(ContentBorderColor, 1f))
                    {
                        g.DrawRectangle(borderPen, borderRect);
                    }
                }
            }

            if (!_bHideTabHeader)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (var selBr = new SolidBrush(SelectedColor))
                using (var tabBr = new SolidBrush(TabColor))
                using (var selTxtBr = new SolidBrush(SelectedTextColor))
                using (var txtBr = new SolidBrush(TextColor))
                using (var hovBr = new SolidBrush(HoverTextColor))
                using (var linePen = new Pen(LineColor, 1f))
                using (var selPen = new Pen(SelectedLineColor, 2f))
                using (var tabBorderPen = new Pen(TabBorderColor, 1f))
                {
                    for (int i = 0; i < TabCount; i++)
                        DrawTab(g, i, selBr, tabBr, selTxtBr, txtBr, hovBr, linePen, selPen, tabBorderPen);
                }
            }
        }

        private void DrawTab(Graphics g, int index,
            Brush selBr, Brush tabBr,
            Brush selTxtBr, Brush txtBr, Brush hovBr,
            Pen linePen, Pen selPen, Pen tabBorderPen)
        {
            if (index < 0 || index >= TabPages.Count) return;

            Rectangle tabRect = GetTabRect(index);
            if (tabRect.Width <= 0 || tabRect.Height <= 0) return;

            bool isSelected = (SelectedIndex == index);
            bool isHovered = (_nHoveredIndex == index);

            Brush bgBrush = isSelected ? selBr : tabBr;

            Rectangle fillRect = isSelected
                ? new Rectangle(tabRect.X, tabRect.Y, tabRect.Width, tabRect.Height + 1)
                : tabRect;

            using (GraphicsPath path = GetTabRoundedPath(fillRect, TabRadius))
            {
                g.FillPath(bgBrush, path);

                if (ShowTabBorders)
                    g.DrawPath(tabBorderPen, path);
            }

            if (isSelected && (ShowContentBorder || ShowTabBorders))
            {
                int nHeaderH = ItemSize.Height;
                using (var coverPen = new Pen(SelectedColor, 1f))
                {
                    g.DrawLine(coverPen, fillRect.Left + 1, nHeaderH, fillRect.Right - 2, nHeaderH);
                }
            }

            Font drawFont = (isSelected || isHovered) ? _boldFont : Font;
            Brush drawBrush = isSelected ? selTxtBr : (isHovered ? hovBr : txtBr);
            Rectangle textRect = new Rectangle(
                tabRect.X, tabRect.Y,
                tabRect.Width, tabRect.Height - 5);
            g.DrawString(TabPages[index].Text, drawFont, drawBrush, textRect, _sf);

            int lineY = tabRect.Bottom - 6;
            int startX = tabRect.Left + _nLinePadding;
            int endX = tabRect.Right - _nLinePadding;
            if (startX < endX)
                g.DrawLine(isSelected ? selPen : linePen, startX, lineY, endX, lineY);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_bHideTabHeader) return;

            int newHover = -1;
            for (int i = 0; i < TabCount; i++)
            {
                if (GetTabRect(i).Contains(e.Location)) { newHover = i; break; }
            }

            if (newHover == _nHoveredIndex) return;

            InvalidateTabArea(_nHoveredIndex);
            InvalidateTabArea(newHover);
            _nHoveredIndex = newHover;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_bHideTabHeader || _nHoveredIndex == -1) return;
            int old = _nHoveredIndex;
            _nHoveredIndex = -1;
            InvalidateTabArea(old);
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (e.Control is TabPage tabPage)
            {
                tabPage.Padding = new Padding(0);
                tabPage.Margin = new Padding(0);
            }
            else
            {
                e.Control.BringToFront();
            }
        }

        private void InvalidateTabArea(int index)
        {
            if (index < 0 || index >= TabCount) return;
            Rectangle r = GetTabRect(index);
            if (index == SelectedIndex) r.Height += 1;
            Invalidate(r);
        }
    }
}