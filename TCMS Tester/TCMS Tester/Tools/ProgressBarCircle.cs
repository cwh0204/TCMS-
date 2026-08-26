using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CITester
{
    public class CircularProgressBar : UserControl
    {
        private int m_nValue = 0;
        private int m_nMaximum = 100;

        private Color m_clrProgressColor = Color.FromArgb(30, 144, 255);
        private Color m_clrBaseColor = Color.FromArgb(240, 244, 248);
        private Font m_fontPercent = new Font("맑은 고딕", 20F, FontStyle.Bold);
        private Font m_fontSubText = new Font("맑은 고딕", 10F, FontStyle.Regular);
        private Color m_clrPercentColor = Color.FromArgb(50, 50, 50);
        private Color m_clrSubTextColor = Color.Gray;

        // 좌표: 디자인 창에서 편집 가능한 미세 좌표 오프셋 변수 정의 (기본값 세팅)
        private Point m_ptPercentOffset = new Point(0, -10);
        private Point m_ptSubTextOffset = new Point(0, 15);

        [Category("Custom Properties"), Browsable(true), Description("현재 진행률 값")]
        public int Value
        {
            get => m_nValue;
            set
            {
                if (value < 0) m_nValue = 0;
                else if (value > m_nMaximum) m_nValue = m_nMaximum;
                else m_nValue = value;

                this.Invalidate();
            }
        }

        [Category("Custom Properties"), Browsable(true), Description("최대 제한 설정값")]
        public int Maximum
        {
            get => m_nMaximum;
            set { if (value > 0) m_nMaximum = value; this.Invalidate(); }
        }

        [Category("Custom Properties"), Browsable(true), Description("활성화된 로딩 바의 색상")]
        public Color ProgressColor
        {
            get => m_clrProgressColor;
            set { m_clrProgressColor = value; this.Invalidate(); }
        }

        [Category("Custom Properties"), Browsable(true), Description("바탕 원의 테두리 색상")]
        public Color BaseColor
        {
            get => m_clrBaseColor;
            set { m_clrBaseColor = value; this.Invalidate(); }
        }

        [Category("Custom Properties"), Browsable(true), Description("중앙 퍼센트(%) 수치의 글꼴 및 크기")]
        public Font PercentFont
        {
            get => m_fontPercent;
            set { m_fontPercent = value; this.Invalidate(); }
        }

        [Category("Custom Properties"), Browsable(true), Description("중앙 퍼센트(%) 수치의 글자 색상")]
        public Color PercentColor
        {
            get => m_clrPercentColor;
            set { m_clrPercentColor = value; this.Invalidate(); }
        }

        [Category("Custom Properties"), Browsable(true), Description("하단 진행률 문구의 글꼴 및 크기")]
        public Font SubTextFont
        {
            get => m_fontSubText;
            set { m_fontSubText = value; this.Invalidate(); }
        }

        [Category("Custom Properties"), Browsable(true), Description("하단 진행률 문구의 글자 색상")]
        public Color SubTextColor
        {
            get => m_clrSubTextColor;
            set { m_clrSubTextColor = value; this.Invalidate(); }
        }

        [Category("Custom Properties"), Browsable(true), Description("중앙 퍼센트 텍스트의 상대 좌표 오프셋 (X, Y)")]
        public Point PercentOffset
        {
            get => m_ptPercentOffset;
            set { m_ptPercentOffset = value; this.Invalidate(); }
        }

        [Category("Custom Properties"), Browsable(true), Description("하단 진행률 텍스트의 상대 좌표 오프셋 (X, Y)")]
        public Point SubTextOffset
        {
            get => m_ptSubTextOffset;
            set { m_ptSubTextOffset = value; this.Invalidate(); }
        }

        public CircularProgressBar()
        {
            this.SetStyle(ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
            this.Size = new Size(120, 120); // 크기: 기본 크기 120x120
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int nThickness = 8; // 크기: 선 두께 8
            Rectangle rectBounds = new Rectangle(nThickness, nThickness, this.Width - (nThickness * 2), this.Height - (nThickness * 2)); // 크기 및 좌표: 영역 계산

            using (Pen penBase = new Pen(m_clrBaseColor, nThickness))
            {
                g.DrawEllipse(penBase, rectBounds);
            }

            float fCurrentAngle = (float)m_nValue / m_nMaximum * 360f;
            if (fCurrentAngle > 0)
            {
                using (Pen penProgress = new Pen(m_clrProgressColor, nThickness))
                {
                    penProgress.StartCap = LineCap.Round;
                    penProgress.EndCap = LineCap.Round;
                    g.DrawArc(penProgress, rectBounds, -90, fCurrentAngle);
                }
            }

            string strPercent = $"{(int)((float)m_nValue / m_nMaximum * 100)}%";
            string strSubText = "진행률";

            Size sizePercent = TextRenderer.MeasureText(strPercent, m_fontPercent, Size.Empty, TextFormatFlags.NoPadding); // 크기: 텍스트 실제 크기 계산
            Size sizeSub = TextRenderer.MeasureText(strSubText, m_fontSubText, Size.Empty, TextFormatFlags.NoPadding); // 크기: 텍스트 실제 크기 계산

            int nCenterX = this.Width / 2;  // 좌표: 컨트롤 가로 중앙축 기준선 계산
            int nCenterY = this.Height / 2; // 좌표: 컨트롤 세로 중앙축 기준선 계산

            // 좌표: 속성 창에서 입력한 오프셋 수치(m_ptPercentOffset)를 연산하여 최종 출력 위치 결정
            Point ptPercent = new Point(nCenterX - (sizePercent.Width / 2) + m_ptPercentOffset.X, nCenterY - (sizePercent.Height / 2) + m_ptPercentOffset.Y);
            TextRenderer.DrawText(g, strPercent, m_fontPercent, ptPercent, m_clrPercentColor, TextFormatFlags.NoPadding);

            // 좌표: 속성 창에서 입력한 오프셋 수치(m_ptSubTextOffset)를 연산하여 최종 출력 위치 결정
            Point ptSub = new Point(nCenterX - (sizeSub.Width / 2) + m_ptSubTextOffset.X, nCenterY - (sizeSub.Height / 2) + m_ptSubTextOffset.Y);
            TextRenderer.DrawText(g, strSubText, m_fontSubText, ptSub, m_clrSubTextColor, TextFormatFlags.NoPadding);
        }
    }
}