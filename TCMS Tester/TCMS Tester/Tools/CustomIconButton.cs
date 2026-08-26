using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

public class CustomIconButton : Button
{
    private int _cornerRadius = 20;
    private float _iconScale = 0.4f;

    private Color _baseBorderColor = Color.Gray;
    private int _baseBorderThickness = 1;

    private Color _hoverBorderColor = Color.FromArgb(0, 204, 255);
    private int _hoverBorderThickness = 3;

    private bool _useHoverBackColor = true;
    private Color _hoverBackColor = Color.FromArgb(60, 60, 65);
    private Color _pressedBackColor = Color.FromArgb(35, 35, 38);

    private bool _autoCenterIcon = true;
    private Point _iconLocation = new Point(0, 0);

    private bool _autoCenterText = true;
    private Point _textLocation = new Point(0, 0);
    private int _textBottomMargin = 20;

    private bool isHovered = false;
    private bool isPressed = false;

    // ─────────────────────────────────────────────
    // ★ 부모 배경 캐시 (OnPaint마다 Bitmap 생성 방지)
    // ─────────────────────────────────────────────
    private Bitmap _parentBgCache = null;
    private Rectangle _cachedBounds = Rectangle.Empty; // 캐시가 유효한 조건 추적

    // ★ 디자인 모드 판별 (DesignMode만으론 불안정)
    private bool IsDesignMode => DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

    #region [ 커스텀 속성 - 디자인 창 ]

    [Category("커스텀 속성 - 공통")]
    public int CornerRadius { get => _cornerRadius; set { _cornerRadius = Math.Max(1, value); Invalidate(); } }

    [Category("커스텀 속성 - 공통")]
    public float IconScale { get => _iconScale; set { _iconScale = Math.Max(0.1f, Math.Min(1.0f, value)); Invalidate(); } }

    [Category("커스텀 속성 - 아이콘 위치")]
    public bool AutoCenterIcon { get => _autoCenterIcon; set { _autoCenterIcon = value; Invalidate(); } }

    [Category("커스텀 속성 - 아이콘 위치")]
    public Point IconLocation { get => _iconLocation; set { _iconLocation = value; Invalidate(); } }

    [Category("커스텀 속성 - 텍스트 위치")]
    public bool AutoCenterText { get => _autoCenterText; set { _autoCenterText = value; Invalidate(); } }

    [Category("커스텀 속성 - 텍스트 위치")]
    public Point TextLocation { get => _textLocation; set { _textLocation = value; Invalidate(); } }

    [Category("커스텀 속성 - 텍스트 위치")]
    public int TextBottomMargin { get => _textBottomMargin; set { _textBottomMargin = value; Invalidate(); } }

    [Category("커스텀 속성 - 기본 테두리")]
    public Color BaseBorderColor { get => _baseBorderColor; set { _baseBorderColor = value; Invalidate(); } }

    [Category("커스텀 속성 - 기본 테두리")]
    public int BaseBorderThickness { get => _baseBorderThickness; set { _baseBorderThickness = value; Invalidate(); } }

    [Category("커스텀 속성 - 호버/클릭 테두리")]
    public Color HoverBorderColor { get => _hoverBorderColor; set { _hoverBorderColor = value; Invalidate(); } }

    [Category("커스텀 속성 - 호버/클릭 테두리")]
    public int HoverBorderThickness { get => _hoverBorderThickness; set { _hoverBorderThickness = value; Invalidate(); } }

    [Category("커스텀 속성 - 배경색 상태")]
    [Description("마우스 호버 및 클릭 시 배경색을 변경할지 여부를 설정합니다.")]
    public bool UseHoverBackColor { get => _useHoverBackColor; set { _useHoverBackColor = value; Invalidate(); } }

    [Category("커스텀 속성 - 배경색 상태")]
    [Description("마우스 호버 시 적용될 버튼의 배경색입니다.")]
    public Color HoverBackColor { get => _hoverBackColor; set { _hoverBackColor = value; Invalidate(); } }

    [Category("커스텀 속성 - 배경색 상태")]
    [Description("버튼을 마우스로 누르고 있을 때(클릭 중) 적용될 배경색입니다.")]
    public Color PressedBackColor { get => _pressedBackColor; set { _pressedBackColor = value; Invalidate(); } }

    #endregion

    public CustomIconButton()
    {
        this.SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |                  // ★ 크기 변경 시 잔상 방지
            ControlStyles.SupportsTransparentBackColor,
            true);

        this.Size = new Size(120, 120);
        this.BackColor = Color.FromArgb(45, 45, 48);
        this.ForeColor = Color.White;
        this.FlatStyle = FlatStyle.Flat;
        this.FlatAppearance.BorderSize = 0;
    }

    // ─────────────────────────────────────────────
    // ★ 디자인창 투명 버그 수정
    //   - 런타임: 비워둬서 깜박임 방지 (OnPaint가 전부 처리)
    //   - 디자인타임: base 호출해야 디자이너가 배경을 정상 렌더링
    // ─────────────────────────────────────────────
    protected override void OnPaintBackground(PaintEventArgs e)
    {
        if (IsDesignMode)
            base.OnPaintBackground(e);
        // 런타임은 의도적으로 아무것도 안 함 (OnPaint에서 전부 처리)
    }

    // ─────────────────────────────────────────────
    // ★ 부모 배경 캐시 무효화 타이밍 관리
    // ─────────────────────────────────────────────
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        InvalidateParentCache();
    }

    protected override void OnParentChanged(EventArgs e)
    {
        base.OnParentChanged(e);
        InvalidateParentCache();
    }

    protected override void OnMove(EventArgs e)
    {
        base.OnMove(e);
        InvalidateParentCache(); // 위치 변경 시 부모 캡처 영역이 달라짐
    }

    private void InvalidateParentCache()
    {
        _parentBgCache?.Dispose();
        _parentBgCache = null;
        _cachedBounds = Rectangle.Empty;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) InvalidateParentCache();
        base.Dispose(disposing);
    }

    protected override void OnMouseEnter(EventArgs e) { isHovered = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { isHovered = false; isPressed = false; Invalidate(); base.OnMouseLeave(e); }
    protected override void OnMouseDown(MouseEventArgs mevent) { isPressed = true; Invalidate(); base.OnMouseDown(mevent); }
    protected override void OnMouseUp(MouseEventArgs mevent) { isPressed = false; Invalidate(); base.OnMouseUp(mevent); }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        int currentThickness = (isHovered || isPressed) ? _hoverBorderThickness : _baseBorderThickness;
        Color currentBorderColor = (isHovered || isPressed) ? _hoverBorderColor : _baseBorderColor;
        Color currentBackColor = this.BackColor;
        if (_useHoverBackColor)
        {
            if (isPressed) currentBackColor = _pressedBackColor;
            else if (isHovered) currentBackColor = _hoverBackColor;
        }
        float penOffset = currentThickness / 2.0f;
        float w = this.Width - currentThickness;
        float h = this.Height - currentThickness;

        // ─────────────────────────────────────────────
        // 1. 부모 배경 렌더링
        //    - 디자인타임: g.Clear()로 명시적 초기화 (잔상/이미지 깨짐 방지)
        //    - 런타임: 캐시된 Bitmap 재사용 (매 프레임 생성 X)
        // ─────────────────────────────────────────────
        if (IsDesignMode)
        {
            // ★ 디자인타임: 명시적 클리어로 이전 프레임 잔상 및 이미지 깨짐 방지
            g.Clear(this.Parent?.BackColor ?? SystemColors.Control);
        }
        else if (this.Parent != null)
        {
            Rectangle currentBounds = new Rectangle(this.Left, this.Top, this.Width, this.Height);
            // 캐시가 없거나 컨트롤 크기/위치가 바뀐 경우에만 새로 캡처
            if (_parentBgCache == null || _cachedBounds != currentBounds)
            {
                _parentBgCache?.Dispose();
                _parentBgCache = new Bitmap(this.Width, this.Height);
                using (Graphics tempG = Graphics.FromImage(_parentBgCache))
                {
                    tempG.Clear(this.Parent.BackColor);
                    tempG.TranslateTransform(-this.Left, -this.Top);
                    var flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod;
                    var clipRect = new Rectangle(this.Left, this.Top, this.Width, this.Height);
                    using (var pe = new PaintEventArgs(tempG, clipRect))
                    {
                        typeof(Control).InvokeMember("InvokePaintBackground", flags, null, this.Parent, new object[] { this.Parent, pe });
                        typeof(Control).InvokeMember("InvokePaint", flags, null, this.Parent, new object[] { this.Parent, pe });
                    }
                }
                _cachedBounds = currentBounds;
            }
            g.DrawImage(_parentBgCache, 0, 0);
        }

        // ─────────────────────────────────────────────
        // 2. 둥근 사각형 본체
        // ─────────────────────────────────────────────
        using (GraphicsPath path = new GraphicsPath())
        {
            float diameter = _cornerRadius * 2;
            if (diameter > w) diameter = w;
            if (diameter > h) diameter = h;

            path.AddArc(penOffset, penOffset, diameter, diameter, 180, 90);
            path.AddArc(penOffset + w - diameter, penOffset, diameter, diameter, 270, 90);
            path.AddArc(penOffset + w - diameter, penOffset + h - diameter, diameter, diameter, 0, 90);
            path.AddArc(penOffset, penOffset + h - diameter, diameter, diameter, 90, 90);
            path.CloseAllFigures();

            using (SolidBrush brush = new SolidBrush(currentBackColor))
                g.FillPath(brush, path);

            if (this.BackgroundImage != null)
            {
                g.SetClip(path);
                g.DrawImage(this.BackgroundImage, 0, 0, this.Width, this.Height);
                g.ResetClip();
            }

            if (currentThickness > 0)
            {
                using (Pen pen = new Pen(currentBorderColor, currentThickness))
                {
                    pen.LineJoin = LineJoin.Round;
                    g.DrawPath(pen, path);
                }
            }
        }

        // ─────────────────────────────────────────────
        // 3. 아이콘 그리기
        // ─────────────────────────────────────────────
        if (this.Image != null && this.BackgroundImage == null)
        {
            int iconSize = (int)(Math.Min(this.Width, this.Height) * _iconScale);
            int drawX = _autoCenterIcon ? (this.Width - iconSize) / 2 : _iconLocation.X;
            int drawY = _autoCenterIcon ? ((this.Height - iconSize) / 2) - (int)(this.Height * 0.1) : _iconLocation.Y;

            if (isPressed) drawY += 2;
            g.DrawImage(this.Image, drawX, drawY, iconSize, iconSize);
        }

        // ─────────────────────────────────────────────
        // 4. 텍스트 그리기
        // ─────────────────────────────────────────────
        if (!string.IsNullOrEmpty(this.Text))
        {
            if (_autoCenterText)
            {
                TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom | TextFormatFlags.WordBreak;
                Rectangle textRect = new Rectangle(5, 0, this.Width - 10, this.Height - _textBottomMargin);
                if (isPressed) textRect.Offset(0, 2);
                TextRenderer.DrawText(g, this.Text, this.Font, textRect, this.ForeColor, flags);
            }
            else
            {
                Point drawTextPos = _textLocation;
                if (isPressed) drawTextPos.Y += 2;
                TextRenderer.DrawText(g, this.Text, this.Font, drawTextPos, this.ForeColor);
            }
        }
    }
}