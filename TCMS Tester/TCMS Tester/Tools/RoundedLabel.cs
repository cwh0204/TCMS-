using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;

namespace CITester
{
    public enum RoundedCornerStyle
    {
        All,    // 위, 아래 모두 둥글게
        Top,    // 위쪽만 둥글게
        Bottom, // 아래쪽만 둥글게
        None    // 모두 직각
    }

    [ToolboxItem(true)]
    public class RoundedLabel : Label
    {
        private int _cornerRadius = 20;
        private Color _fillColor = Color.RoyalBlue;
        private Color _borderColor = Color.White;
        private int _borderThickness = 2;

        private RoundedCornerStyle _cornerStyle = RoundedCornerStyle.All;

        private Image _customImage = null;
        private Size _imageSize = new Size(32, 32);
        private Point _imageLocation = new Point(0, 0);
        private bool _autoCenterImage = true;

        private bool _autoCenterText = true;
        private Point _textLocation = new Point(0, 0);

        // ── GDI+ 객체 캐싱 변수 ──
        private SolidBrush _cachedBrush;
        private Pen _cachedPen;
        private GraphicsPath _cachedPath;

        // ★ 100% 정확한 디자이너 감지 프로퍼티 (투명 버그 방어용)
        private bool bIsDesignMode => DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        public RoundedLabel()
        {
            this.SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor,
                true);

            this.Size = new Size(120, 40);
            this.BackColor = Color.Transparent;
            this.TextAlign = ContentAlignment.MiddleCenter;

            // ★ 생성자 시점에서 안전하게 초기 캐시 구조 매핑 
            UpdateBrushCache();
            UpdatePenCache();
            UpdatePathCache();
        }

        #region 캐시 업데이트 로직
        private void UpdateBrushCache()
        {
            _cachedBrush?.Dispose();
            _cachedBrush = new SolidBrush(_fillColor);
        }

        private void UpdatePenCache()
        {
            _cachedPen?.Dispose();
            if (_borderThickness > 0)
            {
                _cachedPen = new Pen(_borderColor, _borderThickness) { Alignment = PenAlignment.Center };
            }
            else
            {
                _cachedPen = null;
            }
        }

        private void UpdatePathCache()
        {
            _cachedPath?.Dispose();
            _cachedPath = null;

            // 크기가 0 이하일 때는 빈 경로 생성을 회피하여 Null 참조 크래시 원천 차단
            if (this.Width <= 0 || this.Height <= 0) return;

            float fOffset = _borderThickness / 2f;
            float fDrawWidth = Math.Max(1f, this.Width - _borderThickness - 1f);
            float fDrawHeight = Math.Max(1f, this.Height - _borderThickness - 1f);

            RectangleF rectF = new RectangleF(fOffset, fOffset, fDrawWidth, fDrawHeight);
            _cachedPath = GetRoundedRectanglePath(rectF, _cornerRadius);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdatePathCache();
        }
        #endregion

        #region 속성 정의
        [Category("Custom Design"), DefaultValue(RoundedCornerStyle.All)]
        public RoundedCornerStyle CornerStyle
        {
            get => _cornerStyle;
            set { if (_cornerStyle != value) { _cornerStyle = value; UpdatePathCache(); Invalidate(); } }
        }

        [Category("Custom Design"), DefaultValue(20)]
        public int CornerRadius
        {
            get => _cornerRadius;
            set { int nNewVal = Math.Max(1, value); if (_cornerRadius != nNewVal) { _cornerRadius = nNewVal; UpdatePathCache(); Invalidate(); } }
        }

        [Category("Custom Design"), DefaultValue(typeof(Color), "RoyalBlue")]
        public Color FillColor
        {
            get => _fillColor;
            set { if (_fillColor != value) { _fillColor = value; UpdateBrushCache(); Invalidate(); } }
        }

        [Category("Custom Design"), DefaultValue(typeof(Color), "White")]
        public Color BorderColor
        {
            get => _borderColor;
            set { if (_borderColor != value) { _borderColor = value; UpdatePenCache(); Invalidate(); } }
        }

        [Category("Custom Design"), DefaultValue(2)]
        public int BorderThickness
        {
            get => _borderThickness;
            set { int nNewVal = Math.Max(0, value); if (_borderThickness != nNewVal) { _borderThickness = nNewVal; UpdatePenCache(); UpdatePathCache(); Invalidate(); } }
        }

        [Category("Custom Image"), DefaultValue(null)]
        public Image CustomImage { get => _customImage; set { if (_customImage != value) { _customImage = value; Invalidate(); } } }

        [Category("Custom Image"), DefaultValue(typeof(Size), "32, 32")]
        public Size ImageSize { get => _imageSize; set { if (_imageSize != value) { _imageSize = value; Invalidate(); } } }

        [Category("Custom Image"), DefaultValue(true)]
        public bool AutoCenterImage { get => _autoCenterImage; set { if (_autoCenterImage != value) { _autoCenterImage = value; Invalidate(); } } }

        [Category("Custom Image"), DefaultValue(typeof(Point), "0, 0")]
        public Point ImageLocation { get => _imageLocation; set { if (_imageLocation != value) { _imageLocation = value; Invalidate(); } } }

        [Category("Custom Text Location"), DefaultValue(true)]
        public bool AutoCenterText { get => _autoCenterText; set { if (_autoCenterText != value) { _autoCenterText = value; Invalidate(); } } }

        [Category("Custom Text Location"), DefaultValue(typeof(Point), "0, 0")]
        public Point TextLocation { get => _textLocation; set { if (_textLocation != value) { _textLocation = value; Invalidate(); } } }
        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // 디자이너 환경 대응 방어 보장
            if (_cachedPath == null) UpdatePathCache();

            // 1. 배경 및 테두리 안전 그리기
            if (_cachedPath != null)
            {
                if (_cachedBrush != null) g.FillPath(_cachedBrush, _cachedPath);
                if (_cachedPen != null) g.DrawPath(_cachedPen, _cachedPath);
            }
            else
            {
                // 패스 캐시 빌드가 예외적으로 실패했을 때 디자이너 구멍 뚫림 방지용 기본 그리기 우회
                if (_cachedBrush != null) g.FillRectangle(_cachedBrush, ClientRectangle);
            }

            // 2. 이미지 그리기
            if (_customImage != null)
            {
                int nImgX = _autoCenterImage ? (this.Width - _imageSize.Width) / 2 : _imageLocation.X;
                int nImgY = _autoCenterImage ? (this.Height - _imageSize.Height) / 2 : _imageLocation.Y;
                g.DrawImage(_customImage, new Rectangle(nImgX, nImgY, _imageSize.Width, _imageSize.Height));
            }

            // 3. 텍스트 그리기
            if (!string.IsNullOrEmpty(this.Text))
            {
                if (_autoCenterText)
                {
                    float fOffset = _borderThickness / 2f;
                    Rectangle textRect = new Rectangle(
                        (int)fOffset + this.Padding.Left,
                        (int)fOffset + this.Padding.Top,
                        this.Width - (int)(fOffset * 2) - this.Padding.Horizontal,
                        this.Height - (int)(fOffset * 2) - this.Padding.Vertical);

                    TextRenderer.DrawText(g, this.Text, this.Font, textRect,
                        this.ForeColor, GetTextFormatFlags(this.TextAlign));
                }
                else
                {
                    TextRenderer.DrawText(g, this.Text, this.Font, _textLocation, this.ForeColor);
                }
            }
        }

        private TextFormatFlags GetTextFormatFlags(ContentAlignment alignment)
        {
            TextFormatFlags flags = TextFormatFlags.WordBreak | TextFormatFlags.PreserveGraphicsClipping;

            if ((alignment & (ContentAlignment.TopLeft | ContentAlignment.TopCenter | ContentAlignment.TopRight)) != 0) flags |= TextFormatFlags.Top;
            else if ((alignment & (ContentAlignment.MiddleLeft | ContentAlignment.MiddleCenter | ContentAlignment.MiddleRight)) != 0) flags |= TextFormatFlags.VerticalCenter;
            else flags |= TextFormatFlags.Bottom;

            if ((alignment & (ContentAlignment.TopLeft | ContentAlignment.MiddleLeft | ContentAlignment.BottomLeft)) != 0) flags |= TextFormatFlags.Left;
            else if ((alignment & (ContentAlignment.TopCenter | ContentAlignment.MiddleCenter | ContentAlignment.BottomCenter)) != 0) flags |= TextFormatFlags.HorizontalCenter;
            else flags |= TextFormatFlags.Right;

            return flags;
        }

        private GraphicsPath GetRoundedRectanglePath(RectangleF rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float fDiameter = radius * 2f;

            fDiameter = Math.Min(fDiameter, Math.Min(rect.Width, rect.Height));
            if (fDiameter <= 0.1f) fDiameter = 0.1f;

            bool bRoundTop = (_cornerStyle == RoundedCornerStyle.All || _cornerStyle == RoundedCornerStyle.Top);
            bool bRoundBottom = (_cornerStyle == RoundedCornerStyle.All || _cornerStyle == RoundedCornerStyle.Bottom);

            // ★ 길이가 0인 의미 없는 유령 선(AddLine) 코드를 전면 제거하고 정석 좌표 회전 방식으로 리팩토링했습니다.
            // 1. 상단 좌측 코너
            if (bRoundTop)
                path.AddArc(rect.X, rect.Y, fDiameter, fDiameter, 180, 90);
            else
                path.AddLine(rect.X, rect.Y, rect.X + fDiameter / 2, rect.Y);

            // 2. 상단 우측 코너
            if (bRoundTop)
                path.AddArc(rect.Right - fDiameter, rect.Y, fDiameter, fDiameter, 270, 90);
            else
                path.AddLine(rect.Right, rect.Y, rect.Right, rect.Y + fDiameter / 2);

            // 3. 하단 우측 코너
            if (bRoundBottom)
                path.AddArc(rect.Right - fDiameter, rect.Bottom - fDiameter, fDiameter, fDiameter, 0, 90);
            else
                path.AddLine(rect.Right, rect.Bottom, rect.Right - fDiameter / 2, rect.Bottom);

            // 4. 하단 좌측 코너
            if (bRoundBottom)
                path.AddArc(rect.X, rect.Bottom - fDiameter, fDiameter, fDiameter, 90, 90);
            else
                path.AddLine(rect.X, rect.Bottom, rect.X, rect.Bottom - fDiameter / 2);

            path.CloseFigure();
            return path;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cachedBrush?.Dispose();
                _cachedPen?.Dispose();
                _cachedPath?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}