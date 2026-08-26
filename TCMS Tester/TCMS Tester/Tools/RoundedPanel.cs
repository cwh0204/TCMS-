using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CITester
{
    [ToolboxItem(true)]
    public class RoundedPanel : Panel
    {
        // ── 속성 값 변수 (접두사 룰 준수) ──
        private int _cornerRadius = 20;
        private Color _borderColor = Color.LightGray;
        private int _borderThickness = 2;
        private Color _fillColor = Color.White;

        // ── GDI 캐시 ──
        private GraphicsPath _cachedPath;
        private SolidBrush _fillBrush;
        private Pen _borderPen;

        // ★ 100% 정확한 디자이너 감지 프로퍼티
        private bool bIsDesignMode => DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        // ────────────────────────────────────────────────────
        // 생성자
        // ────────────────────────────────────────────────────
        public RoundedPanel()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw, true);

            Size = new Size(200, 100);

            // ★ 자식 컨트롤 클리핑 버그 유발 원인인 Region 코드를 완전히 대체하기 위해 
            // 배경색을 Transparent로 안전하게 제어합니다.
            BackColor = Color.Transparent;
        }

        // ────────────────────────────────────────────────────
        // 속성
        // ────────────────────────────────────────────────────
        #region Properties

        [Category("Custom Design"), DefaultValue(20)]
        public int CornerRadius
        {
            get => _cornerRadius;
            set
            {
                int nNewVal = Math.Max(1, value);
                if (_cornerRadius == nNewVal) return;
                _cornerRadius = nNewVal;
                RebuildPath();
                Invalidate();
            }
        }

        [Category("Custom Design"), DefaultValue(typeof(Color), "LightGray")]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                if (_borderColor == value) return;
                _borderColor = value;
                DisposeBorderPen();
                Invalidate();
            }
        }

        [Category("Custom Design"), DefaultValue(2)]
        public int BorderThickness
        {
            get => _borderThickness;
            set
            {
                int nNewVal = Math.Max(0, value);
                if (_borderThickness == nNewVal) return;
                _borderThickness = nNewVal;
                DisposeBorderPen();
                RebuildPath();
                Invalidate();
            }
        }

        [Category("Custom Design"), DefaultValue(typeof(Color), "White")]
        public Color FillColor
        {
            get => _fillColor;
            set
            {
                if (_fillColor == value) return;
                _fillColor = value;
                DisposeFillBrush();
                Invalidate();
            }
        }

        #endregion

        // ────────────────────────────────────────────────────
        // 경로 캐시 제어
        // ────────────────────────────────────────────────────
        private void RebuildPath()
        {
            _cachedPath?.Dispose();
            _cachedPath = null;

            if (Width <= 0 || Height <= 0) return;

            _cachedPath = BuildRoundedPath();

            // ★ [버그 수정] 자식 컨트롤을 가차 없이 잘라버리던 this.Region 코드를 전면 제거했습니다.
            // 이제 패널 본연의 부모 컨테이너 역할을 완벽하게 수행합니다.
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            RebuildPath();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RebuildPath();
        }

        // ────────────────────────────────────────────────────
        // OnPaint
        // ────────────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // 1. 크기가 0 이하이면 그려지지 않도록 사전 탈출
            if (this.Width <= 0 || this.Height <= 0) return;

            // 2. 캐시 업데이트 수행
            if (_cachedPath == null) UpdatePathCache();

            // 3. 업데이트 후에도 _cachedPath가 null이면 그려지기 차단 (ArgumentNullException 방지)
            if (_cachedPath == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // 4. 내부 채우기 (using으로 자원 자동 해제 및 색상 즉시 반영)
            using (var fillBrush = new SolidBrush(_fillColor))
            {
                g.FillPath(fillBrush, _cachedPath);
            }

            // 5. 외곽 테두리 드로잉
            if (_borderThickness > 0)
            {
                using (var borderPen = new Pen(_borderColor, _borderThickness) { Alignment = PenAlignment.Inset })
                {
                    g.DrawPath(borderPen, _cachedPath);
                }
            }
        }

        private void UpdatePathCache()
        {
            RebuildPath();
        }

        // ────────────────────────────────────────────────────
        // 경로 빌더
        // ────────────────────────────────────────────────────
        private GraphicsPath BuildRoundedPath()
        {
            float fOffset = _borderThickness / 2f;
            float fDrawWidth = Width - _borderThickness;
            float fDrawHeight = Height - _borderThickness;

            if (fDrawWidth <= 0 || fDrawHeight <= 0) return null;

            float fDiameter = Math.Min(_cornerRadius * 2f, Math.Min(fDrawWidth, fDrawHeight));

            var rect = new RectangleF(fOffset, fOffset, fDrawWidth, fDrawHeight);
            var path = new GraphicsPath();

            if (fDiameter > 0)
            {
                path.AddArc(rect.X, rect.Y, fDiameter, fDiameter, 180, 90);
                path.AddArc(rect.Right - fDiameter, rect.Y, fDiameter, fDiameter, 270, 90);
                path.AddArc(rect.Right - fDiameter, rect.Bottom - fDiameter, fDiameter, fDiameter, 0, 90);
                path.AddArc(rect.X, rect.Bottom - fDiameter, fDiameter, fDiameter, 90, 90);
            }
            else
            {
                path.AddRectangle(rect);
            }

            path.CloseFigure();
            return path;
        }

        private void DisposeFillBrush() { _fillBrush?.Dispose(); _fillBrush = null; }
        private void DisposeBorderPen() { _borderPen?.Dispose(); _borderPen = null; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cachedPath?.Dispose();
                DisposeFillBrush();
                DisposeBorderPen();
            }
            base.Dispose(disposing);
        }
    }
}