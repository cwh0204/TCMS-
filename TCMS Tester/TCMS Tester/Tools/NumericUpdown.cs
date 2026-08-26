using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

[DefaultEvent("ValueChanged")]
public class CustomNumeric : UserControl
{
    // ── 자식 컨트롤 ──
    private TextBox txtValue;

    // ── 데이터 변수 (접두사 룰 반영) ──
    private double _value = 0;
    private double _minimum = -100000.0;
    private double _maximum = 100000.0;
    private double _step = 0.1;
    private int _decimalPlaces = 4;

    // ── 가상 버튼 영역 및 상태 ──
    private Rectangle _btnAreaRect;
    private Rectangle _upRect;
    private Rectangle _downRect;

    private bool bIsUpHovered;
    private bool bIsDownHovered;
    private bool bIsUpPressed;
    private bool bIsDownPressed;

    // ── GDI 객체 캐시 ──
    private Pen _borderPen;
    private Pen _separatorPen;
    private SolidBrush _arrowBrush;
    private SolidBrush _hoverArrowBrush;
    private SolidBrush _hoverBgBrush;
    private SolidBrush _pressedBgBrush;
    private Size _lastLayoutSize = Size.Empty;

    // ── 색상 ──
    private Color _arrowColor = Color.FromArgb(80, 80, 80);
    private Color _hoverColor = Color.FromArgb(0, 100, 200);

    public event System.EventHandler ValueChanged;

    #region Properties

    [Category("Appearance")]
    public Color ArrowColor
    {
        get => _arrowColor;
        set { _arrowColor = value; RebuildGdi(); Invalidate(); }
    }

    [Category("Appearance")]
    public Color HoverColor
    {
        get => _hoverColor;
        set { _hoverColor = value; RebuildGdi(); Invalidate(); }
    }

    [Category("Data"), DefaultValue(4)]
    [Description("최대 표시 가능한 소수점 자리수를 설정합니다.")]
    public int DecimalPlaces
    {
        get => _decimalPlaces;
        set { _decimalPlaces = Math.Max(0, value); UpdateValueAndSync(); }
    }

    [Category("Data"), DefaultValue(0.0)]
    public double Value
    {
        get => _value;
        set
        {
            double dClamped = Math.Max(_minimum, Math.Min(_maximum, value));
            dClamped = Math.Round(dClamped, _decimalPlaces);

            if (_value == dClamped) return;

            _value = dClamped;
            SyncTextBox();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    [Category("Data"), DefaultValue(-100000.0)]
    public double Minimum
    {
        get => _minimum;
        set { _minimum = value; if (Value < _minimum) Value = _minimum; }
    }

    [Category("Data"), DefaultValue(100000.0)]
    public double Maximum
    {
        get => _maximum;
        set { _maximum = value; if (Value > _maximum) Value = _maximum; }
    }

    [Category("Data"), DefaultValue(0.1)]
    public double Step
    {
        get => _step;
        set => _step = Math.Max(0.0, value);
    }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override string Text
    {
        get
        {
            string strFormat = _decimalPlaces > 0 ? "0." + new string('#', _decimalPlaces) : "0";
            return _value.ToString(strFormat);
        }
        set
        {
            if (double.TryParse(value, out double dRes))
            {
                Value = dRes;
            }
            // ★ 수정: 파싱 실패 혹은 성공 후 입력 폼 버퍼 값을 정확하게 재동기화하도록 고정
            SyncTextBox();
        }
    }

    #endregion

    public CustomNumeric()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

        RebuildGdi();
        InitializeComponents();
        Size = new Size(160, 30);
    }

    private void RebuildGdi()
    {
        _borderPen?.Dispose();
        _separatorPen?.Dispose();
        _arrowBrush?.Dispose();
        _hoverArrowBrush?.Dispose();
        _hoverBgBrush?.Dispose();
        _pressedBgBrush?.Dispose();

        _borderPen = new Pen(Color.FromArgb(180, 180, 180), 1);
        _separatorPen = new Pen(Color.FromArgb(200, 210, 220), 1);
        _arrowBrush = new SolidBrush(_arrowColor);
        _hoverArrowBrush = new SolidBrush(_hoverColor);

        _hoverBgBrush = new SolidBrush(Color.FromArgb(215, 235, 255));
        _pressedBgBrush = new SolidBrush(Color.FromArgb(195, 220, 245));
    }

    private void InitializeComponents()
    {
        BackColor = Color.White;
        Padding = new Padding(1);

        txtValue = new TextBox
        {
            BorderStyle = BorderStyle.None,
            Font = new Font("맑은 고딕", 11F, FontStyle.Regular),
            TextAlign = HorizontalAlignment.Left,
            Multiline = false,
            BackColor = Color.White
        };

        txtValue.KeyPress += TxtValue_KeyPress;
        txtValue.Leave += TxtValue_Leave;
        txtValue.KeyDown += TxtValue_KeyDown; // ★ 사용자 엔터 키 입력 편의성 확보를 위한 추가

        Controls.Add(txtValue);
    }

    private void TxtValue_KeyPress(object sender, KeyPressEventArgs e)
    {
        TextBox txt = (TextBox)sender;

        // 허용 문자 집합 판별
        if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '-' && e.KeyChar != '.')
        {
            e.Handled = true; return;
        }

        // 정수 설정 시 소수점 입력 제한
        if (e.KeyChar == '.' && _decimalPlaces <= 0)
        {
            e.Handled = true; return;
        }

        // 중복 소수점 입력 제한
        if (e.KeyChar == '.' && txt.Text.IndexOf('.') >= 0)
        {
            e.Handled = true; return;
        }

        // ★ 수정: 마이너스 기호 중복 기입 방지 및 입력 커서 위치가 맨 앞(0)이 아닐 때 블록 지정이 없는 경우 제한
        if (e.KeyChar == '-')
        {
            if (txt.Text.IndexOf('-') >= 0 || (txt.SelectionStart != 0 && txt.SelectionLength == 0))
            {
                e.Handled = true;
            }
        }
    }

    private void TxtValue_Leave(object sender, EventArgs e)
    {
        Text = txtValue.Text;
    }

    private void TxtValue_KeyDown(object sender, KeyEventArgs e)
    {
        // 수동 입력 후 엔터를 치면 즉시 데이터 동기화 라우팅 처리
        if (e.KeyCode == Keys.Enter)
        {
            Text = txtValue.Text;
            e.SuppressKeyPress = true; // 엔터 경고음 방지
        }
    }

    private void UpdateValueAndSync()
    {
        _value = Math.Round(_value, _decimalPlaces);
        SyncTextBox();
    }

    private void SyncTextBox()
    {
        if (txtValue == null) return;
        string strFormat = _decimalPlaces > 0 ? "0." + new string('#', _decimalPlaces) : "0";
        string strTarget = _value.ToString(strFormat);
        if (txtValue.Text != strTarget) txtValue.Text = strTarget;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        SyncTextBox();
        PerformLayout();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (Size == _lastLayoutSize) return;
        _lastLayoutSize = Size;

        if (txtValue == null) return;

        int nBtnWidth = 20;
        int nBtnX = Width - nBtnWidth - Padding.Right;
        _btnAreaRect = new Rectangle(nBtnX, Padding.Top, nBtnWidth, Height - Padding.Vertical);

        int nHalfH = _btnAreaRect.Height / 2;
        _upRect = new Rectangle(nBtnX, Padding.Top, nBtnWidth, nHalfH);
        _downRect = new Rectangle(nBtnX, Padding.Top + nHalfH, nBtnWidth, _btnAreaRect.Height - nHalfH);

        int nAvailableW = nBtnX - Padding.Left - 10 - 5;
        txtValue.SetBounds(
            Padding.Left + 10,
            (Height - txtValue.PreferredHeight) / 2,
            Math.Max(0, nAvailableW),
            txtValue.PreferredHeight);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        bool bNewUpHover = _upRect.Contains(e.Location);
        bool bNewDownHover = _downRect.Contains(e.Location);

        if (bIsUpHovered != bNewUpHover || bIsDownHovered != bNewDownHover)
        {
            bIsUpHovered = bNewUpHover;
            bIsDownHovered = bNewDownHover;
            Invalidate(_btnAreaRect);
        }

        this.Cursor = (bIsUpHovered || bIsDownHovered) ? Cursors.Hand : Cursors.Default;
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        bIsUpHovered = bIsDownHovered = bIsUpPressed = bIsDownPressed = false;
        Invalidate(_btnAreaRect);
        this.Cursor = Cursors.Default;
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button == MouseButtons.Left)
        {
            // ★ 수정: 부동소수점 2진수 연산 누적 오차를 사전 제거하기 위해 정밀 오프셋 보정 가미
            if (bIsUpHovered)
            {
                bIsUpPressed = true;
                Value = Math.Round(_value + _step, _decimalPlaces);
            }
            if (bIsDownHovered)
            {
                bIsDownPressed = true;
                Value = Math.Round(_value - _step, _decimalPlaces);
            }
            Invalidate(_btnAreaRect);
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        bIsUpPressed = bIsDownPressed = false;
        Invalidate(_btnAreaRect);
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        if (e.Delta > 0) Value = Math.Round(_value + _step, _decimalPlaces);
        else Value = Math.Round(_value - _step, _decimalPlaces);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;

        g.DrawRectangle(_borderPen, 0, 0, Width - 1, Height - 1);
        g.DrawLine(_separatorPen, _btnAreaRect.Left - 1, 1, _btnAreaRect.Left - 1, Height - 2);

        g.SmoothingMode = SmoothingMode.AntiAlias;

        if (bIsUpPressed) g.FillRectangle(_pressedBgBrush, _upRect);
        else if (bIsUpHovered) g.FillRectangle(_hoverBgBrush, _upRect);
        DrawArrow(g, _upRect, true, (bIsUpHovered || bIsUpPressed) ? _hoverArrowBrush : _arrowBrush);

        if (bIsDownPressed) g.FillRectangle(_pressedBgBrush, _downRect);
        else if (bIsDownHovered) g.FillRectangle(_hoverBgBrush, _downRect);
        DrawArrow(g, _downRect, false, (bIsDownHovered || bIsDownPressed) ? _hoverArrowBrush : _arrowBrush);
    }

    private void DrawArrow(Graphics g, Rectangle rect, bool isUp, Brush brush)
    {
        int cx = rect.Left + rect.Width / 2;
        int cy = rect.Top + rect.Height / 2;
        const int W = 8, H = 4;
        int topY = cy - H / 2;

        Point[] pts = new Point[3];
        if (isUp)
        {
            pts[0] = new Point(cx, topY);
            pts[1] = new Point(cx - W / 2, topY + H);
            pts[2] = new Point(cx + W / 2, topY + H);
        }
        else
        {
            pts[0] = new Point(cx - W / 2, topY);
            pts[1] = new Point(cx + W / 2, topY);
            pts[2] = new Point(cx, topY + H);
        }
        g.FillPolygon(brush, pts);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _borderPen?.Dispose();
            _separatorPen?.Dispose();
            _arrowBrush?.Dispose();
            _hoverArrowBrush?.Dispose();
            _hoverBgBrush?.Dispose();
            _pressedBgBrush?.Dispose();

            // ★ 디자이너 크래시(유령화 버그) 방지를 위해 공유 객체인 Font.Dispose() 구문을 삭제했습니다.
        }
        base.Dispose(disposing);
    }
}