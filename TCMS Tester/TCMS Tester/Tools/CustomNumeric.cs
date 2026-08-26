using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace CustomControl
{
    public class StepCountControl : Control
    {
        private int nValue = 1;
        private int nMinValue = 1;
        private int nMaxValue = 10;

        private Rectangle rectMinusButton;
        private Rectangle rectPlusButton;
        private Rectangle rectCenterArea;

        private bool bIsMinusPressed = false;
        private bool bIsPlusPressed = false;
        private bool bIsMinusHovered = false;
        private bool bIsPlusHovered = false;

        private TextBox txtInput;

        public event EventHandler ValueChanged;

        public int Value
        {
            get => nValue;
            set
            {
                int nTargetValue = Math.Max(nMinValue, Math.Min(nMaxValue, value));
                if (nValue != nTargetValue)
                {
                    nValue = nTargetValue;
                    Invalidate();
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public StepCountControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            Size = new Size(240, 60);
            Font = new Font("Malgun Gothic", 16, FontStyle.Bold);

            InitializeTextBox();
        }

        private void InitializeTextBox()
        {
            txtInput = new TextBox
            {
                BorderStyle = BorderStyle.None,
                TextAlign = HorizontalAlignment.Center,
                Visible = false,
                Multiline = false
            };

            txtInput.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ApplyTextBoxValue();
                    e.SuppressKeyPress = true;
                    this.Focus();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    HideTextBox();
                    this.Focus();
                }
            };

            txtInput.LostFocus += (s, e) => ApplyTextBoxValue();
            Controls.Add(txtInput);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (Parent != null)
            {
                Parent.Click -= Parent_Click;
                Parent.Click += Parent_Click;
            }
        }

        private void Parent_Click(object sender, EventArgs e)
        {
            if (txtInput.Visible)
            {
                ApplyTextBoxValue();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CalculateLayout();
        }

        private void CalculateLayout()
        {
            int nButtonWidth = Width / 4;
            rectMinusButton = new Rectangle(1, 1, nButtonWidth - 1, Height - 2);
            rectPlusButton = new Rectangle(Width - nButtonWidth, 1, nButtonWidth - 1, Height - 2);
            rectCenterArea = new Rectangle(nButtonWidth, 1, Width - (nButtonWidth * 2), Height - 2);

            if (txtInput != null)
            {
                txtInput.Font = Font;
                int nTxtHeight = txtInput.PreferredHeight;
                txtInput.Bounds = new Rectangle(
                    rectCenterArea.X + 10,
                    (Height - nTxtHeight) / 2,
                    rectCenterArea.Width - 20,
                    nTxtHeight
                );
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle rectDraw = new Rectangle(1, 1, Width - 2, Height - 2);

            using (GraphicsPath path = GetRoundedRectanglePath(rectDraw, 10))
            {
                g.SetClip(path);

                using (SolidBrush brushBg = new SolidBrush(Color.White))
                {
                    g.FillPath(brushBg, path);
                }

                DrawButtonState(g, rectMinusButton, bIsMinusHovered, bIsMinusPressed);
                DrawButtonState(g, rectPlusButton, bIsPlusHovered, bIsPlusPressed);

                using (Pen penInnerLine = new Pen(Color.FromArgb(215, 222, 235), 1))
                {
                    g.DrawLine(penInnerLine, rectMinusButton.Right, 0, rectMinusButton.Right, Height);
                    g.DrawLine(penInnerLine, rectPlusButton.Left, 0, rectPlusButton.Left, Height);
                }

                Color colorMinus = nValue > nMinValue ? Color.FromArgb(20, 80, 200) : Color.LightGray;
                Color colorPlus = nValue < nMaxValue ? Color.FromArgb(20, 80, 200) : Color.LightGray;

                DrawMinusSign(g, rectMinusButton, colorMinus);
                DrawPlusSign(g, rectPlusButton, colorPlus);

                if (!txtInput.Visible)
                {
                    RenderValueText(g);
                }

                g.ResetClip();
                using (Pen penBorder = new Pen(Color.FromArgb(150, 165, 185), 2))
                {
                    g.DrawPath(penBorder, path);
                }
            }
        }

        private void DrawButtonState(Graphics g, Rectangle rect, bool bIsHover, bool bIsPress)
        {
            if (bIsPress)
            {
                using (SolidBrush brushPress = new SolidBrush(Color.FromArgb(232, 238, 248)))
                    g.FillRectangle(brushPress, rect);
            }
            else if (bIsHover)
            {
                using (SolidBrush brushHover = new SolidBrush(Color.FromArgb(244, 247, 254)))
                    g.FillRectangle(brushHover, rect);
            }
        }

        private void DrawMinusSign(Graphics g, Rectangle rect, Color color)
        {
            using (Pen penSign = new Pen(color, 2))
            {
                int nCenterX = rect.X + (rect.Width / 2);
                int nCenterY = rect.Y + (rect.Height / 2);
                g.DrawLine(penSign, nCenterX - 7, nCenterY, nCenterX + 7, nCenterY);
            }
        }

        private void DrawPlusSign(Graphics g, Rectangle rect, Color color)
        {
            using (Pen penSign = new Pen(color, 2))
            {
                int nCenterX = rect.X + (rect.Width / 2);
                int nCenterY = rect.Y + (rect.Height / 2);
                g.DrawLine(penSign, nCenterX - 7, nCenterY, nCenterX + 7, nCenterY);
                g.DrawLine(penSign, nCenterX, nCenterY - 7, nCenterX, nCenterY + 7);
            }
        }

        private void RenderValueText(Graphics g)
        {
            string strMainText = nValue.ToString();
            string strUnitText = " 회";

            using (Font fontUnit = new Font("Malgun Gothic", 11, FontStyle.Regular))
            {
                Size sizeMain = TextRenderer.MeasureText(strMainText, Font);
                Size sizeUnit = TextRenderer.MeasureText(strUnitText, fontUnit);

                int nTotalWidth = sizeMain.Width + sizeUnit.Width;
                int nStartX = rectCenterArea.Left + (rectCenterArea.Width - nTotalWidth) / 2;
                int nStartY = (Height - sizeMain.Height) / 2;

                TextRenderer.DrawText(g, strMainText, Font, new Point(nStartX, nStartY), Color.Black);
                TextRenderer.DrawText(g, strUnitText, fontUnit, new Point(nStartX + sizeMain.Width, nStartY + (sizeMain.Height - sizeUnit.Height)), Color.FromArgb(80, 90, 100));
            }
        }

        // 마우스 휠 스크롤 이벤트를 재정의하여 값 증감 및 텍스트박스 자동 적용 구현
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            // 텍스트 입력창이 활성화된 상태에서 휠 이동 시 이전 값 적용
            if (txtInput != null && txtInput.Visible)
            {
                ApplyTextBoxValue();
            }

            // 휠 업: 값 증가, 휠 다운: 값 감소
            if (e.Delta > 0)
            {
                Value++;
            }
            else if (e.Delta < 0)
            {
                Value--;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Point pt = e.Location;

            bool bPrevMinusHover = bIsMinusHovered;
            bool bPrevPlusHover = bIsPlusHovered;

            bIsMinusHovered = rectMinusButton.Contains(pt);
            bIsPlusHovered = rectPlusButton.Contains(pt);
            bool bIsCenterHovered = rectCenterArea.Contains(pt);

            if (bIsMinusHovered || bIsPlusHovered) Cursor = Cursors.Hand;
            else if (bIsCenterHovered) Cursor = Cursors.IBeam;
            else Cursor = Cursors.Default;

            if (bPrevMinusHover != bIsMinusHovered || bPrevPlusHover != bIsPlusHovered)
            {
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Point pt = e.Location;

            if (txtInput.Visible && !rectCenterArea.Contains(pt))
            {
                ApplyTextBoxValue();
            }

            if (rectMinusButton.Contains(pt))
            {
                bIsMinusPressed = true;
                Value--;
                this.Focus();
                Invalidate();
            }
            else if (rectPlusButton.Contains(pt))
            {
                bIsPlusPressed = true;
                Value++;
                this.Focus();
                Invalidate();
            }
            else if (rectCenterArea.Contains(pt))
            {
                ShowTextBox();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            bIsMinusPressed = false;
            bIsPlusPressed = false;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            bIsMinusHovered = false;
            bIsPlusHovered = false;
            bIsMinusPressed = false;
            bIsPlusPressed = false;
            Cursor = Cursors.Default;
            Invalidate();
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            ApplyTextBoxValue();
        }

        private void ShowTextBox()
        {
            if (txtInput.Visible) return;
            txtInput.Text = nValue.ToString();
            txtInput.Visible = true;
            txtInput.Focus();
            txtInput.SelectAll();
            Invalidate();
        }

        private void HideTextBox()
        {
            txtInput.Visible = false;
            Invalidate();
        }

        private void ApplyTextBoxValue()
        {
            if (!txtInput.Visible) return;

            if (int.TryParse(txtInput.Text, out int nParsedValue))
            {
                Value = nParsedValue;
            }
            HideTextBox();
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int nRadius)
        {
            GraphicsPath path = new GraphicsPath();
            int nDiameter = nRadius * 2;
            path.AddArc(rect.X, rect.Y, nDiameter, nDiameter, 180, 90);
            path.AddArc(rect.Right - nDiameter, rect.Y, nDiameter, nDiameter, 270, 90);
            path.AddArc(rect.Right - nDiameter, rect.Bottom - nDiameter, nDiameter, nDiameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - nDiameter, nDiameter, nDiameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Parent != null)
            {
                Parent.Click -= Parent_Click;
            }
            base.Dispose(disposing);
        }
    }
}