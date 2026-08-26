using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;

namespace CITester
{
    // 1. 방향 옵션 확장 (위아래, 좌우 프리셋 추가)
    [Flags]
    public enum BorderSides
    {
        None = 0,
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8,
        TopAndBottom = Top | Bottom, // 위아래 동시
        LeftAndRight = Left | Right, // 좌우 동시
        All = Top | Bottom | Left | Right
    }

    [ToolboxItem(true)]
    public class CITesterLabel : Label
    {
        private Color _fillColor = Color.FromArgb(245, 248, 255);
        private Color _borderColor = Color.FromArgb(230, 235, 245);
        private int _borderThickness = 1;
        private BorderSides _visibleBorders = BorderSides.Bottom;

        public CITesterLabel()
        {
            // 컨트롤 최적화 및 투명 배경 설정
            this.SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor, true);

            this.Size = new Size(150, 45);
            this.BackColor = Color.Transparent;
            this.TextAlign = ContentAlignment.MiddleLeft;
            this.Padding = new Padding(10, 0, 0, 0);
        }

        #region 디자인 속성
        [Category("Custom Design"), Description("라벨의 배경색을 설정합니다.")]
        public Color FillColor { get => _fillColor; set { _fillColor = value; Invalidate(); } }

        [Category("Custom Design"), Description("테두리(구분선)의 색상을 설정합니다.")]
        public Color BorderColor { get => _borderColor; set { _borderColor = value; Invalidate(); } }

        [Category("Custom Design"), Description("테두리 두께를 설정합니다.")]
        public int BorderThickness { get => _borderThickness; set { _borderThickness = value; Invalidate(); } }

        [Category("Custom Design"), Description("표시할 테두리 방향을 선택합니다. (TopAndBottom 등을 활용해 보세요)")]
        public BorderSides VisibleBorders { get => _visibleBorders; set { _visibleBorders = value; Invalidate(); } }
        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // 직선을 그릴 때는 AntiAlias보다 None이나 HighSpeed가 훨씬 선명합니다.
            g.SmoothingMode = SmoothingMode.None;
            g.PixelOffsetMode = PixelOffsetMode.None;

            // 1. 부모 배경 복사 (생략 - 기존 로직 유지)
            if (this.Parent != null) { /* ... 기존과 동일 ... */ }

            // 2. 전체 면 채우기 (배경색)
            using (SolidBrush brush = new SolidBrush(_fillColor))
            {
                g.FillRectangle(brush, this.ClientRectangle);
            }

            // 3. 선택적 테두리(구분선) 그리기
            // 두께가 1일 때는 좌표를 정수로 딱 맞추는 것이 가장 굵고 선명합니다.
            using (Pen pen = new Pen(_borderColor, _borderThickness))
            {
                int t = _borderThickness;

                // 상단: Y좌표 0
                if (_visibleBorders.HasFlag(BorderSides.Top))
                    g.DrawLine(pen, 0, 0, this.Width, 0);

                // 하단: Y좌표는 Height - 두께
                if (_visibleBorders.HasFlag(BorderSides.Bottom))
                    g.DrawLine(pen, 0, this.Height - t, this.Width, this.Height - t);

                // 좌측: X좌표 0
                if (_visibleBorders.HasFlag(BorderSides.Left))
                    g.DrawLine(pen, 0, 0, 0, this.Height);

                // 우측: X좌표는 Width - 두께
                if (_visibleBorders.HasFlag(BorderSides.Right))
                    g.DrawLine(pen, this.Width - t, 0, this.Width - t, this.Height);
            }

            // 4. 텍스트 그리기는 부드러워야 하므로 다시 품질 설정
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle textRect = new Rectangle(Padding.Left, Padding.Top,
                Width - Padding.Horizontal, Height - Padding.Vertical);

            TextRenderer.DrawText(g, this.Text, this.Font, textRect, this.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
        }
    }
}