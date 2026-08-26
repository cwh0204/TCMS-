using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CITester
{
    [ToolboxItem(true)]
    public class CustomDataGridView : DataGridView
    {
        private int _customRowHeight = 40;
        private int _customHeaderHeight = 40;
        private bool _autoFillEmptyRows = true;

        private bool bIsUsingDummyData = true;
        private bool bIsInternalSetting = false;

        private bool bIsDesignMode => DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        #region [ 커스텀 속성 설정 ]

        [Category("Custom Layout"), DefaultValue(40), Description("데이터 행의 높이를 설정합니다.")]
        public int CustomRowHeight
        {
            get => _customRowHeight;
            set { _customRowHeight = Math.Max(20, value); this.RowTemplate.Height = _customRowHeight; RefreshLayoutAndFill(); }
        }

        [Category("Custom Layout"), DefaultValue(40), Description("머릿말(Header)의 높이를 설정합니다.")]
        public int CustomHeaderHeight
        {
            get => _customHeaderHeight;
            set { _customHeaderHeight = Math.Max(20, value); this.ColumnHeadersHeight = _customHeaderHeight; }
        }

        [Category("Custom Layout"), DefaultValue(true), Description("데이터가 없을 때 컨트롤 크기에 맞춰 빈 행을 자동으로 채울지 여부입니다.")]
        public bool AutoFillEmptyRows
        {
            get => _autoFillEmptyRows;
            set { _autoFillEmptyRows = value; RefreshLayoutAndFill(); }
        }

        #endregion

        public CustomDataGridView()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            InitializeDefaultSettings();
        }

        private void InitializeDefaultSettings()
        {
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.AllowUserToOrderColumns = false;
            this.AllowUserToResizeRows = false;
            this.AllowUserToResizeColumns = false;
            this.ReadOnly = true;
            this.MultiSelect = false;
            this.RowHeadersVisible = false;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.ScrollBars = ScrollBars.None;

            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.ColumnHeadersHeight = _customHeaderHeight;
            this.RowTemplate.Height = _customRowHeight;

            // ★ 버그 해결 핵심: 코드로 열을 강제 주입하던 구역을 완전히 제거했습니다.
            // 디자인 창 컬렉션 스키마를 그대로 보존하기 위해 자동 생성 플래그만 제어합니다.
            this.AutoGenerateColumns = false;

            this.ColumnHeadersDefaultCellStyle.Font = new Font("맑은 고딕", 13F, FontStyle.Bold);
            this.DefaultCellStyle.Font = new Font("맑은 고딕", 11F, FontStyle.Regular);
            this.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            this.EnableHeadersVisualStyles = false;
            Color colorHeaderBg = Color.FromArgb(240, 244, 253);
            Color colorHeaderFg = Color.FromArgb(8, 31, 78);
            this.ColumnHeadersDefaultCellStyle.BackColor = colorHeaderBg;
            this.ColumnHeadersDefaultCellStyle.ForeColor = colorHeaderFg;
            this.ColumnHeadersDefaultCellStyle.SelectionBackColor = colorHeaderBg;
            this.ColumnHeadersDefaultCellStyle.SelectionForeColor = colorHeaderFg;

            this.BorderStyle = BorderStyle.None;
            this.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            this.GridColor = Color.FromArgb(215, 220, 230);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            RefreshLayoutAndFill();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RefreshLayoutAndFill();
        }

        protected override void OnDataSourceChanged(EventArgs e)
        {
            base.OnDataSourceChanged(e);

            if (bIsInternalSetting) return;

            if (!bIsDesignMode && this.DataSource is DataTable dtTarget)
            {
                bool bHasRealData = false;

                // 디자인창에서 배치한 첫 번째 열의 이름을 동적으로 획득하여 유효 데이터를 검증합니다.
                if (this.Columns.Count > 0)
                {
                    string strFirstColName = this.Columns[0].Name;
                    if (dtTarget.Columns.Contains(strFirstColName))
                    {
                        foreach (DataRow row in dtTarget.Rows)
                        {
                            if (row[strFirstColName] != DBNull.Value && !string.IsNullOrEmpty(row[strFirstColName]?.ToString()))
                            {
                                bHasRealData = true;
                                break;
                            }
                        }
                    }
                }

                if (bHasRealData)
                {
                    bIsUsingDummyData = false;
                    try
                    {
                        bIsInternalSetting = true;
                        FillExactPageRows(dtTarget, true);
                    }
                    finally
                    {
                        bIsInternalSetting = false;
                    }
                }
            }

            UpdateScrollBarsState();
        }

        public void RefreshLayoutAndFill()
        {
            if (bIsInternalSetting) return;
            if (this.Columns.Count == 0) return; // 디자인창 컬렉션에 열이 등록되기 전이라면 연산을 대기합니다.

            if (_autoFillEmptyRows)
            {
                if (bIsDesignMode || this.DataSource == null || bIsUsingDummyData)
                {
                    try
                    {
                        bIsInternalSetting = true;
                        bIsUsingDummyData = true;

                        // ★ 핵심 변경: 사용자가 디자인창 컬렉션에 등록해 둔 열 구조를 동적으로 복사하여 도화지를 빌드합니다.
                        DataTable dtDynamicDummy = new DataTable();
                        foreach (DataGridViewColumn colTarget in this.Columns)
                        {
                            string strKey = string.IsNullOrEmpty(colTarget.DataPropertyName) ? colTarget.Name : colTarget.DataPropertyName;
                            if (!dtDynamicDummy.Columns.Contains(strKey))
                            {
                                dtDynamicDummy.Columns.Add(strKey);
                            }
                        }

                        FillExactPageRows(dtDynamicDummy, false);
                        this.DataSource = dtDynamicDummy;
                    }
                    finally
                    {
                        bIsInternalSetting = false;
                    }
                }
                else if (this.DataSource is DataTable dtReal)
                {
                    try
                    {
                        bIsInternalSetting = true;
                        FillExactPageRows(dtReal, true);
                    }
                    finally
                    {
                        bIsInternalSetting = false;
                    }
                }
            }

            UpdateScrollBarsState();
        }

        private void FillExactPageRows(DataTable dt, bool bHasRealData)
        {
            if (dt == null) return;

            int nAvailableHeight = this.ClientSize.Height - this.ColumnHeadersHeight;
            if (nAvailableHeight <= 0) return;

            int nTargetRowCount = nAvailableHeight / _customRowHeight;
            if (nAvailableHeight % _customRowHeight > 0)
            {
                nTargetRowCount++;
            }

            if (nTargetRowCount <= 0) nTargetRowCount = 10;

            if (!bHasRealData)
            {
                dt.Rows.Clear();
            }

            int nTotalExpectedRows = Math.Max(nTargetRowCount, dt.Rows.Count);
            if (!bHasRealData) nTotalExpectedRows = nTargetRowCount;

            while (dt.Rows.Count < nTotalExpectedRows)
            {
                dt.Rows.Add(dt.NewRow());
            }
        }

        private void UpdateScrollBarsState()
        {
            if (bIsDesignMode) return;
            if (this.Columns.Count == 0) return;

            int nRealRows = 0;
            string strFirstColName = this.Columns[0].Name;

            if (this.DataSource is DataTable dt && dt.Columns.Contains(strFirstColName))
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row[strFirstColName] != DBNull.Value && !string.IsNullOrEmpty(row[strFirstColName]?.ToString()))
                    {
                        nRealRows++;
                    }
                }
            }

            int nVisibleRows = (this.ClientSize.Height - this.ColumnHeadersHeight) / _customRowHeight;

            if (nRealRows > nVisibleRows)
            {
                this.ScrollBars = ScrollBars.Both;
            }
            else
            {
                this.ScrollBars = ScrollBars.None;
            }
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            base.OnCellPainting(e);

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewRow rowTarget = this.Rows[e.RowIndex];
                bool bIsEmptyRow = true;

                // 디자인창에서 커스텀 수정된 첫 번째 컬럼의 인덱스[0] 데이터 유무로 빈 행 여부를 정밀 판정합니다.
                if (this.Columns.Count > 0)
                {
                    var cellValue = rowTarget.Cells[0].Value;
                    if (cellValue != DBNull.Value && !string.IsNullOrEmpty(cellValue?.ToString()))
                    {
                        bIsEmptyRow = false;
                    }
                }

                if (bIsEmptyRow)
                {
                    e.PaintBackground(e.CellBounds, false);
                    e.Paint(e.CellBounds, DataGridViewPaintParts.Border);
                    e.Handled = true;
                }
            }
        }

        protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && this.Columns.Count > 0)
            {
                DataGridViewRow rowTarget = this.Rows[e.RowIndex];
                var cellValue = rowTarget.Cells[0].Value;

                if (cellValue == DBNull.Value || string.IsNullOrEmpty(cellValue?.ToString()))
                {
                    this.ClearSelection();
                    return;
                }
            }
            base.OnCellMouseDown(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.VerticalScrollBar != null && this.VerticalScrollBar.Visible)
            {
                Graphics gGraphics = e.Graphics;
                Rectangle rectScroll = this.VerticalScrollBar.Bounds;

                using (SolidBrush brushTrack = new SolidBrush(Color.FromArgb(245, 245, 247)))
                {
                    gGraphics.FillRectangle(brushTrack, rectScroll);
                }

                int nThumbWidth = 6;
                int nThumbX = rectScroll.X + (rectScroll.Width - nThumbWidth) / 2;

                int nThumbHeight = (int)(rectScroll.Height * ((double)this.Height / Math.Max(1, this.Rows.Count * _customRowHeight)));
                nThumbHeight = Math.Max(30, nThumbHeight);

                int nThumbY = rectScroll.Y + (int)((rectScroll.Height - nThumbHeight) * ((double)this.FirstDisplayedScrollingRowIndex / Math.Max(1, this.Rows.Count - 1)));

                Rectangle rectThumb = new Rectangle(nThumbX, nThumbY, nThumbWidth, nThumbHeight);

                gGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush brushThumb = new SolidBrush(Color.FromArgb(180, 185, 195)))
                using (GraphicsPath pathThumb = GetRoundedRectPath(rectThumb, 3))
                {
                    gGraphics.FillPath(brushThumb, pathThumb);
                }
            }
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rectTarget, int nRadius)
        {
            GraphicsPath pathResult = new GraphicsPath();
            int nDiameter = nRadius * 2;
            pathResult.AddArc(rectTarget.X, rectTarget.Y, nDiameter, nDiameter, 180, 90);
            pathResult.AddArc(rectTarget.Right - nDiameter, rectTarget.Y, nDiameter, nDiameter, 270, 90);
            pathResult.AddArc(rectTarget.Right - nDiameter, rectTarget.Bottom - nDiameter, nDiameter, nDiameter, 0, 90);
            pathResult.AddArc(rectTarget.X, rectTarget.Bottom - nDiameter, nDiameter, nDiameter, 90, 90);
            pathResult.CloseFigure();
            return pathResult;
        }
    }
}