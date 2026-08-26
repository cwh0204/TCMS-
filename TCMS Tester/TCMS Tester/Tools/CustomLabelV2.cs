using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace main
{
    public enum IconAlignmentMode
    {
        Left,
        Center,
        Right,
        Manual
    }

    public class CustomRowLabel : Control
    {
        private string strLeftText = "차량번호";
        private string strRightText = "0000";
        private Color colorLeftText = Color.FromArgb(20, 40, 100);
        private Color colorRightText = Color.FromArgb(0, 80, 200);
        private Image imgCenterIcon = null;
        private int nCornerRadius = 15;
        private Color colorBgColor = Color.White;

        private IconAlignmentMode enumIconAlignment = IconAlignmentMode.Center;
        private Point ptManualIconLocation = new Point(0, 0);

        private int nLeftTextMargin = 0;
        private int nRightTextMargin = 0;
        private Size sizeIconSize = new Size(24, 24);

        // [신규 기능] 우측 수직선 레이아웃 제어를 위한 필드
        private bool bShowRightDivider = true;
        private int nDividerVerticalPadding = 12; // 수직선 상하단 여백 (값이 커질수록 선이 짧아집니다)
        private Color colorDividerColor = Color.FromArgb(225, 230, 240);

        public CustomRowLabel()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Color.Transparent;
            this.Size = new Size(250, 45);
        }

        #region [디자이너 연동 속성 배치]

        [Category("Custom Property"), Description("좌측에 배치할 텍스트입니다.")]
        public string LeftText
        {
            get => strLeftText;
            set { strLeftText = value; this.Invalidate(); }
        }

        [Category("Custom Property"), Description("우측에 배치할 텍스트입니다.")]
        public string RightText
        {
            get => strRightText;
            set { strRightText = value; this.Invalidate(); }
        }

        [Category("Custom Property"), Description("좌측 텍스트의 색상입니다.")]
        public Color LeftTextColor
        {
            get => colorLeftText;
            set { colorLeftText = value; this.Invalidate(); }
        }

        [Category("Custom Property"), Description("우측 텍스트의 색상입니다.")]
        public Color RightTextColor
        {
            get => colorRightText;
            set { colorRightText = value; this.Invalidate(); }
        }

        [Category("Custom Property"), Description("배치할 아이콘 이미지입니다.")]
        public Image CenterIcon
        {
            get => imgCenterIcon;
            set { imgCenterIcon = value; this.Invalidate(); }
        }

        [Category("Custom Property"), Description("아이콘의 출력 크기(Width, Height)를 강제 지정합니다.")]
        public Size IconSize
        {
            get => sizeIconSize;
            set { sizeIconSize = value; this.Invalidate(); }
        }

        [Category("Custom Property"), Description("아이콘의 정렬 기준 위치를 결정합니다.")]
        public IconAlignmentMode IconAlignment
        {
            get => enumIconAlignment;
            set { enumIconAlignment = value; this.Invalidate(); }
        }

        [Category("Custom Property"), Description("IconAlignment 속성이 Manual일 때 적용되는 아이콘의 강제 물리 좌표(X, Y)입니다.")]
        public Point ManualIconLocation
        {
            get => ptManualIconLocation;
            set { ptManualIconLocation = value; this.Invalidate(); }
        }

        [Category("Custom Property"), Description("좌측 텍스트의 추가 마진(양수는 우측 이동, 음수는 좌측 이동)입니다.")]
        public int LeftTextMargin
        {
            get => nLeftTextMargin;
            set { nLeftTextMargin = value; this.Invalidate(); }
        }

        [Category("Custom Property"), Description("우측 텍스트의 추가 마진(양수는 좌측 이동, 음수는 우측 이동)입니다.")]
        public int RightTextMargin
        {
            get => nRightTextMargin;
            set { nRightTextMargin = value; this.Invalidate(); }
        }

        [Category("Custom Property"), Description("모서리의 둥근 반경(R값)입니다.")]
        public int CornerRadius
        {
            get => nCornerRadius;
            set { nCornerRadius = value; this.Invalidate(); }
        }

        [Category("Custom Property"), Description("컨트롤의 내부 배경 색상입니다.")]
        public Color BoxBackColor
        {
            get => colorBgColor;
            set { colorBgColor = value; this.Invalidate(); }
        }

        // ── [신규 추가 속성] 우측 구분선 제어 커스텀 세그먼트 ──
        [Category("Custom Property - Divider"), Description("우측 수직 구분선을 표시할지 여부입니다.")]
        public bool ShowRightDivider
        {
            get => bShowRightDivider;
            set { bShowRightDivider = value; this.Invalidate(); }
        }

        [Category("Custom Property - Divider"), Description("우측 수직선의 상하단 공백 크기입니다. 값이 클수록 선의 길이가 짧아집니다.")]
        public int DividerVerticalPadding
        {
            get => nDividerVerticalPadding;
            set { nDividerVerticalPadding = value; this.Invalidate(); }
        }

        [Category("Custom Property - Divider"), Description("우측 수직선의 색상입니다.")]
        public Color DividerColor
        {
            get => colorDividerColor;
            set { colorDividerColor = value; this.Invalidate(); }
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            if (this.Width <= 0 || this.Height <= 0) return;

            try
            {
                // 1. [요청 변경] 외곽 테두리 펜 생략 및 배경 바운더리만 채우기
                Rectangle rectBounds = new Rectangle(0, 0, this.Width, this.Height);
                using (GraphicsPath pathBounds = GetRoundedRectanglePath(rectBounds, nCornerRadius))
                {
                    using (Brush brushBg = new SolidBrush(colorBgColor))
                    {
                        g.FillPath(brushBg, pathBounds);
                    }
                }

                float fBasePaddingX = nCornerRadius > 10 ? nCornerRadius : 15;

                // 2. 좌측 텍스트 드로잉
                if (!string.IsNullOrEmpty(strLeftText))
                {
                    using (Brush brushLeft = new SolidBrush(colorLeftText))
                    {
                        StringFormat sfLeft = new StringFormat
                        {
                            Alignment = StringAlignment.Near,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter,
                            FormatFlags = StringFormatFlags.NoWrap
                        };

                        float fTargetLeftX = fBasePaddingX + nLeftTextMargin;
                        float fAvailableWidth = this.Width - fTargetLeftX - fBasePaddingX;
                        if (fAvailableWidth < 10) fAvailableWidth = 10;

                        RectangleF rectLeft = new RectangleF(fTargetLeftX, 0, fAvailableWidth, this.Height);
                        g.DrawString(strLeftText, this.Font, brushLeft, rectLeft, sfLeft);
                    }
                }

                // 3. 아이콘 렌더링
                if (imgCenterIcon != null)
                {
                    int nImgWidth = sizeIconSize.Width;
                    int nImgHeight = sizeIconSize.Height;

                    int nImgX = 0;
                    int nImgY = (this.Height - nImgHeight) / 2;

                    switch (enumIconAlignment)
                    {
                        case IconAlignmentMode.Left:
                            nImgX = (int)fBasePaddingX;
                            break;

                        case IconAlignmentMode.Center:
                            nImgX = (this.Width - nImgWidth) / 2;
                            break;

                        case IconAlignmentMode.Right:
                            nImgX = this.Width - nImgWidth - (int)fBasePaddingX;
                            break;

                        case IconAlignmentMode.Manual:
                            nImgX = ptManualIconLocation.X;
                            nImgY = ptManualIconLocation.Y;
                            break;
                    }

                    g.DrawImage(imgCenterIcon, new Rectangle(nImgX, nImgY, nImgWidth, nImgHeight));
                }

                // 4. 우측 텍스트 드로잉
                if (!string.IsNullOrEmpty(strRightText))
                {
                    using (Brush brushRight = new SolidBrush(colorRightText))
                    {
                        StringFormat sfRight = new StringFormat
                        {
                            Alignment = StringAlignment.Far,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter,
                            FormatFlags = StringFormatFlags.NoWrap
                        };

                        float fTargetRightWidth = fBasePaddingX + nRightTextMargin;
                        RectangleF rectRight = new RectangleF(fBasePaddingX, 0, this.Width - fBasePaddingX - fTargetRightWidth, this.Height);
                        g.DrawString(strRightText, this.Font, brushRight, rectRight, sfRight);
                    }
                }

                // 5. [신규 기능] 우측 한계선에 커스텀 수직선(|) 렌더링 세그먼트
                if (bShowRightDivider)
                {
                    using (Pen penDivider = new Pen(colorDividerColor, 1.0f))
                    {
                        // 디자이너창에서 설정한 마진 값을 이용해 동적으로 선의 상/하단 Y축 정밀 산출
                        int nLineTop = nDividerVerticalPadding;
                        int nLineBottom = this.Height - nDividerVerticalPadding;
                        int nLineX = this.Width - 1; // 맨 우측 엣지 라인

                        g.DrawLine(penDivider, nLineX, nLineTop, nLineX, nLineBottom);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CustomRowLabel Paint Error: {ex.Message}");
            }
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle rectTarget, int nRadius)
        {
            GraphicsPath pathResult = new GraphicsPath();
            int nDiameter = nRadius * 2;

            if (nDiameter <= 0)
            {
                pathResult.AddRectangle(rectTarget);
                return pathResult;
            }

            if (nDiameter > rectTarget.Width) nDiameter = rectTarget.Width;
            if (nDiameter > rectTarget.Height) nDiameter = rectTarget.Height;

            Size sizeArc = new Size(nDiameter, nDiameter);
            Rectangle rectArc = new Rectangle(rectTarget.Location, sizeArc);

            pathResult.AddArc(rectArc, 180, 90);

            rectArc.X = rectTarget.Right - nDiameter;
            pathResult.AddArc(rectArc, 270, 90);

            rectArc.Y = rectTarget.Bottom - nDiameter;
            pathResult.AddArc(rectArc, 0, 90);

            rectArc.X = rectTarget.Left;
            pathResult.AddArc(rectArc, 90, 90);

            pathResult.CloseFigure();
            return pathResult;
        }
    }
}