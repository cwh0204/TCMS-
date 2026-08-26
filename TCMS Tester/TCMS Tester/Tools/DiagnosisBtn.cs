using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace TCMSTester
{
    public enum eDiagStatus
    {
        Ready,
        Diagnosing,
        Normal,
        Abnormal
    }

    public class DiagnoseButton : Button
    {
        private eDiagStatus _eCurrentStatus = eDiagStatus.Ready;
        private readonly Timer _timerScan;
        private float _fScanLineY;
        private const float fScanSpeed = 2.0f;

        private bool _bIsMouseHover;
        private bool _bIsMouseDown;
        private bool _bUseAutoCenterLayout = true;
        private Image _imgButtonIcon;
        private Point _ptImageLocation = new Point(15, 15);
        private Point _ptTextLocation = new Point(50, 15);
        private Size _sizeIconCustom = new Size(16, 16);
        private Color _colorReadyBorder = Color.FromArgb(210, 215, 225);
        private float _fBorderThickness = 1.0f;

        #region Designer Properties

        [Category("Custom Design"), Description("True면 이미지·텍스트를 자동 중앙 정렬, False면 수동 좌표로 배치합니다.")]
        public bool UseAutoCenterLayout
        {
            get => _bUseAutoCenterLayout;
            set { _bUseAutoCenterLayout = value; Invalidate(); }
        }

        [Category("Custom Design"), Description("버튼에 표시할 아이콘 이미지입니다.")]
        public Image ButtonIcon
        {
            get => _imgButtonIcon;
            set { _imgButtonIcon = value; Invalidate(); }
        }

        [Category("Custom Design"), Description("아이콘 이미지의 커스텀 가로/세로 크기입니다.")]
        public Size IconSize
        {
            get => _sizeIconCustom;
            set { _sizeIconCustom = value; Invalidate(); }
        }

        [Category("Custom Design"), Description("Ready 상태 테두리 색상입니다.")]
        public Color ReadyBorderColor
        {
            get => _colorReadyBorder;
            set { _colorReadyBorder = value; Invalidate(); }
        }

        [Category("Custom Design"), Description("Ready 상태 테두리 굵기입니다.")]
        public float BorderThickness
        {
            get => _fBorderThickness;
            set { _fBorderThickness = Math.Max(1f, value); Invalidate(); }
        }

        [Category("Custom Design"), Description("수동 모드일 때 이미지 시작 좌표입니다.")]
        public Point ImageLocation
        {
            get => _ptImageLocation;
            set { _ptImageLocation = value; Invalidate(); }
        }

        [Category("Custom Design"), Description("수동 모드일 때 텍스트 시작 좌표입니다.")]
        public Point TextLocation
        {
            get => _ptTextLocation;
            set { _ptTextLocation = value; Invalidate(); }
        }

        [Category("Custom Status"), Description("현재 자가진단 상태입니다.")]
        public eDiagStatus CurrentStatus
        {
            get => _eCurrentStatus;
            set
            {
                if (_eCurrentStatus == value) return;
                _eCurrentStatus = value;
                if (_eCurrentStatus != eDiagStatus.Diagnosing)
                    StopScanAnimation();
                Invalidate();
            }
        }

        #endregion

        public DiagnoseButton()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw, true);

            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Size = new Size(180, 50);
            Font = new Font("맑은 고딕", 11, FontStyle.Bold);
            BackColor = Color.FromArgb(255, 255, 255);

            _timerScan = new Timer { Interval = 30 };
            _timerScan.Tick += TimerScan_Tick;
        }

        #region Mouse Interaction

        protected override void OnMouseEnter(EventArgs e)
        {
            _bIsMouseHover = true; Invalidate(); base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            _bIsMouseHover = false; _bIsMouseDown = false; Invalidate(); base.OnMouseLeave(e);
        }
        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (mevent.Button == MouseButtons.Left) _bIsMouseDown = true;
            Invalidate(); base.OnMouseDown(mevent);
        }
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            _bIsMouseDown = false; Invalidate(); base.OnMouseUp(mevent);
        }

        #endregion

        #region Scan Animation

        public void StartNewDiagnosis()
        {
            _fScanLineY = 0f;
            _eCurrentStatus = eDiagStatus.Diagnosing;
            _timerScan.Start();
            Invalidate();
        }

        private void StopScanAnimation()
        {
            if (_timerScan != null && _timerScan.Enabled)
                _timerScan.Stop();
        }

        private void TimerScan_Tick(object sender, EventArgs e)
        {
            if (_eCurrentStatus != eDiagStatus.Diagnosing)
            {
                StopScanAnimation();
                return;
            }

            // 레이저 라인 좌표 하강 및 경계 유효성 검증
            _fScanLineY += fScanSpeed;
            if (_fScanLineY >= Height) _fScanLineY = 0f;

            // [수정] 외부 통신 제어 흐름과의 간섭 및 충돌을 방지하기 위해 독단적인 자가 타임아웃 상태 변경 코드를 제거하고 순수 애니메이션 무한 루프 갱신만 수행합니다.
            Invalidate();
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 0 || Height <= 0) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // 현재 진단 상태 정보를 분석하여 렌더링에 사용할 배경색 및 테두리 속성 추출
            Color colorBg, colorBorder;
            float fThickness;
            ResolveStateColors(out colorBg, out colorBorder, out fThickness);

            // 산출된 상태별 배경색으로 버튼 제어 영역 채우기 수행
            RectangleF rectBounds = new RectangleF(0, 0, Width, Height);
            using (var brushBg = new SolidBrush(colorBg))
            {
                g.FillRectangle(brushBg, rectBounds);
            }

            // 진단 진행 중일 때 레이저 스캔 하이테크 그라데이션 효과 드로잉
            if (_eCurrentStatus == eDiagStatus.Diagnosing)
            {
                DrawScanEffect(g);
            }

            // 외곽선 잔상 현상을 방지하기 위해 펜 두께 보정 연산을 거친 정확한 경계면 테두리 드로잉
            float fHalfThick = fThickness / 2f;
            RectangleF rectBorder = new RectangleF(fHalfThick, fHalfThick, Width - fThickness, Height - fThickness);
            using (var penBorder = new Pen(colorBorder, fThickness))
            {
                penBorder.Alignment = PenAlignment.Center;
                g.DrawRectangle(penBorder, rectBorder.X, rectBorder.Y, rectBorder.Width, rectBorder.Height);
            }

            // 상태별 텍스트 포맷 구성 및 배치 모드 설정에 따른 최종 콘텐츠 출력
            string strDisplayText = BuildDisplayText();
            Color colorText = (_eCurrentStatus == eDiagStatus.Abnormal) ? Color.Crimson : Color.FromArgb(45, 55, 72);

            if (_bUseAutoCenterLayout)
                DrawCentered(g, strDisplayText, colorText);
            else
                DrawManual(g, strDisplayText, colorText);
        }

        private void ResolveStateColors(out Color bg, out Color border, out float thickness)
        {
            switch (_eCurrentStatus)
            {
                case eDiagStatus.Diagnosing:
                    bg = Color.FromArgb(242, 247, 255);
                    border = Color.FromArgb(14, 165, 233);
                    thickness = 2.0f;
                    break;
                case eDiagStatus.Normal:
                    bg = _bIsMouseHover ? Color.FromArgb(230, 247, 230) : Color.FromArgb(242, 249, 242);
                    border = Color.FromArgb(34, 197, 94);
                    thickness = 2.0f;
                    break;
                case eDiagStatus.Abnormal:
                    bg = _bIsMouseHover ? Color.FromArgb(254, 236, 236) : Color.FromArgb(255, 245, 245);
                    border = Color.FromArgb(239, 68, 68);
                    thickness = 2.0f;
                    break;
                default:
                    bg = _bIsMouseDown
                        ? Color.FromArgb(226, 232, 240)
                        : _bIsMouseHover
                            ? Color.FromArgb(241, 245, 249)
                            : Color.FromArgb(255, 255, 255);
                    border = _bIsMouseHover ? Color.FromArgb(148, 163, 184) : _colorReadyBorder;
                    thickness = _fBorderThickness;
                    break;
            }
        }

        private void DrawScanEffect(Graphics g)
        {
            float fBlurHeight = 6f;
            float fTop = _fScanLineY - fBlurHeight;
            float fBottom = _fScanLineY + fBlurHeight;

            if (Math.Abs(fBottom - fTop) < 1f) fBottom = fTop + 1f;

            using (var brushScan = new LinearGradientBrush(
                new PointF(0f, fTop), new PointF(0f, fBottom),
                Color.Transparent, Color.FromArgb(40, 14, 165, 233)))
            {
                ColorBlend blend = new ColorBlend(3);
                blend.Colors = new Color[] { Color.Transparent, Color.FromArgb(55, 14, 165, 233), Color.Transparent };
                blend.Positions = new float[] { 0.0f, 0.5f, 1.0f };
                brushScan.InterpolationColors = blend;

                g.FillRectangle(brushScan, 0f, fTop, Width, fBottom - fTop);
            }

            using (var penWire = new Pen(Color.FromArgb(56, 189, 248), 1.8f))
                g.DrawLine(penWire, 1f, _fScanLineY, Width - 1f, _fScanLineY);
        }

        private string BuildDisplayText()
        {
            switch (_eCurrentStatus)
            {
                case eDiagStatus.Diagnosing: return "진단 중...";
                case eDiagStatus.Normal: return Text + " [정상]";
                case eDiagStatus.Abnormal: return Text + " [이상]";
                default: return Text;
            }
        }

        private void DrawCentered(Graphics g, string text, Color textColor)
        {
            SizeF szText = g.MeasureString(text, Font);
            float imgW = _imgButtonIcon != null ? _sizeIconCustom.Width + 8f : 0f;
            float startX = (Width - imgW - szText.Width) / 2f;

            if (_imgButtonIcon != null)
            {
                float imgY = (Height - _sizeIconCustom.Height) / 2f;
                g.DrawImage(_imgButtonIcon,
                    new RectangleF(startX, imgY, _sizeIconCustom.Width, _sizeIconCustom.Height));
            }

            using (var brushText = new SolidBrush(textColor))
                g.DrawString(text, Font, brushText, startX + imgW, (Height - szText.Height) / 2f);
        }

        private void DrawManual(Graphics g, string text, Color textColor)
        {
            if (_imgButtonIcon != null)
                g.DrawImage(_imgButtonIcon,
                    new Rectangle(_ptImageLocation.X, _ptImageLocation.Y,
                        _sizeIconCustom.Width, _sizeIconCustom.Height));

            using (var brushText = new SolidBrush(textColor))
                g.DrawString(text, Font, brushText, _ptTextLocation.X, _ptTextLocation.Y);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timerScan?.Stop();
                _timerScan?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}