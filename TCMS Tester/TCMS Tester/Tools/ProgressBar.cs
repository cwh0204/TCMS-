using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace YourNamespace
{
    public class CustomProgressBar : Control
    {
        // ─── 필드 ────────────────────────────────────────────────────────────
        private int _targetValue = 0;
        private float _currentAnimatedValue = 0f;
        private Timer _animationTimer;

        private int _maximum = 100;
        private Color _progressColor = Color.DodgerBlue;
        private Color _trackColor = Color.LightGray;
        private int _cornerRadius = 3;
        private int _textMargin = 10;
        private int _barThickness = 6;

        // [추가 및 수정] 퍼센트 표시 제어 필드 및 캐시 레이아웃 관리 유닛
        private bool _showPercentage = true;
        private string _cachedText = "0%";
        private Size _cachedTextSize = Size.Empty;
        private int _maxTextWidth = 0;
        private int _lastDisplayedPercent = -1;
        private bool _layoutDirty = true;

        // ─── 속성 ─────────────────────────────────────────────────────────────
        // [추가] 디자인 타임 창에서 텍스트 노출 여부를 동적으로 켜고 끌 수 있는 마스터 속성
        [Category("Custom Properties"), Description("우측 퍼센트(%) 텍스트를 화면에 표시할지 여부를 설정합니다.")]
        public bool ShowPercentage
        {
            get => _showPercentage;
            set
            {
                if (_showPercentage == value) return;
                _showPercentage = value;
                _layoutDirty = true;
                Invalidate(); // 속성 변경 즉시 컨트롤 레이아웃 재연산 유도
            }
        }

        [Category("Custom Properties"), Description("값이 변경될 때 부드럽게 채워지는 애니메이션을 사용합니다.")]
        public bool UseAnimation { get; set; } = true;

        [Category("Custom Properties"), Description("프로그레스바(선) 자체의 두께를 조절합니다.")]
        public int BarThickness
        {
            get => _barThickness;
            set { _barThickness = Math.Max(1, value); Invalidate(); }
        }

        [Category("Custom Properties"), Description("양 끝 테두리의 둥근 정도(반경)를 조절합니다.")]
        public int CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = Math.Max(0, value); Invalidate(); }
        }

        [Category("Custom Properties"), Description("프로그레스바와 우측 퍼센트 텍스트 사이의 간격입니다.")]
        public int TextMargin
        {
            get => _textMargin;
            set { _textMargin = Math.Max(0, value); Invalidate(); }
        }

        [Category("Custom Properties"), Description("프로그레스바가 채워지는 색상입니다.")]
        public Color ProgressColor
        {
            get => _progressColor;
            set { _progressColor = value; Invalidate(); }
        }

        [Category("Custom Properties"), Description("프로그레스바의 배경 색상입니다.")]
        public Color TrackColor
        {
            get => _trackColor;
            set { _trackColor = value; Invalidate(); }
        }

        [Category("Custom Properties"), Description("현재 진행률 값입니다.")]
        public int Value
        {
            get => _targetValue;
            set
            {
                int clamped = Math.Max(0, Math.Min(_maximum, value));
                if (_targetValue == clamped) return;

                _targetValue = clamped;

                if (UseAnimation && !DesignMode)
                {
                    _animationTimer.Start();
                }
                else
                {
                    _currentAnimatedValue = _targetValue;
                    _layoutDirty = true;
                    Invalidate();
                }
            }
        }

        [Category("Custom Properties"), Description("진행률의 최대 값입니다.")]
        public int Maximum
        {
            get => _maximum;
            set
            {
                if (_maximum == value) return;
                _maximum = value;
                _layoutDirty = true;
                Invalidate();
            }
        }

        // ─── 생성자 ──────────────────────────────────────────────────────────
        public CustomProgressBar()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.SupportsTransparentBackColor,
                true);

            Size = new Size(200, 25);
            Font = new Font("맑은 고딕", 9F, FontStyle.Regular);
            ForeColor = Color.Black;

            _animationTimer = new Timer { Interval = 16 };
            _animationTimer.Tick += AnimationTimer_Tick;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _animationTimer != null)
            {
                _animationTimer.Stop();
                _animationTimer.Dispose();
            }
            base.Dispose(disposing);
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            float diff = _targetValue - _currentAnimatedValue;

            if (Math.Abs(diff) < 0.1f)
            {
                _currentAnimatedValue = _targetValue;
                _animationTimer.Stop();
            }
            else
            {
                _currentAnimatedValue += diff * 0.15f;
            }

            Invalidate();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _layoutDirty = true;
            Invalidate();
        }

        private void UpdateTextCacheIfNeeded(float percent)
        {
            int currentPercentInt = (int)(percent * 100);

            if (_layoutDirty || _lastDisplayedPercent != currentPercentInt)
            {
                _lastDisplayedPercent = currentPercentInt;
                _cachedText = $"{currentPercentInt}%";
                _cachedTextSize = TextRenderer.MeasureText(_cachedText, Font);

                if (_layoutDirty)
                {
                    _maxTextWidth = TextRenderer.MeasureText("100%", Font).Width;
                    _layoutDirty = false;
                }
            }
        }

        private static GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            radius = Math.Min(radius, Math.Min(rect.Width / 2, rect.Height / 2));

            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        // ─── 그리기 ──────────────────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float percent = _maximum > 0 ? _currentAnimatedValue / _maximum : 0f;

            UpdateTextCacheIfNeeded(percent);

            // [레이아웃 보정 논리] 퍼센트 가시성 스위칭 상태에 따른 가로 폭 가변 산출 규칙 처리
            int nRightMarginSpace = _showPercentage ? (_maxTextWidth + _textMargin) : 0;
            int barAreaWidth = Width - nRightMarginSpace;

            if (barAreaWidth > 0)
            {
                int actualThickness = Math.Min(_barThickness, Height);
                int barY = (Height - actualThickness) / 2;

                Rectangle trackRect = new Rectangle(0, barY, barAreaWidth, actualThickness);

                // 배경 트랙 드로우
                using (GraphicsPath trackPath = GetRoundedPath(trackRect, _cornerRadius))
                using (SolidBrush trackBrush = new SolidBrush(_trackColor))
                {
                    g.FillPath(trackBrush, trackPath);

                    // 진행률 바 채우기 영역 연산
                    int fillWidth = (int)(barAreaWidth * percent);

                    if (fillWidth > 0)
                    {
                        // 둥근 모서리 클리핑 처리 범위를 채우기 폭에 연동하여 마킹
                        Rectangle fillRect = new Rectangle(0, barY, fillWidth, actualThickness);
                        g.SetClip(fillRect);

                        using (SolidBrush fillBrush = new SolidBrush(_progressColor))
                        {
                            g.FillPath(fillBrush, trackPath);
                        }
                        g.ResetClip();
                    }
                }
            }

            // [요청사항 반영] ShowPercentage가 true일 때만 문자열 렌더링 장치 작동
            if (_showPercentage)
            {
                int textX = Width - _cachedTextSize.Width;
                int textY = (Height - _cachedTextSize.Height) / 2;

                TextRenderer.DrawText(
                    g,
                    _cachedText,
                    Font,
                    new Point(textX, textY),
                    ForeColor,
                    TextFormatFlags.NoPadding);
            }
        }
    }
}