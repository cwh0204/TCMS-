using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace CITester
{
    public partial class CtrlTimingChart : UserControl
    {
        private int[] m_nGuideStartSlots = new int[100];
        public int[] GUIDE_START_SLOTS
        {
            get { return m_nGuideStartSlots; }
            set { m_nGuideStartSlots = value; Invalidate(); }
        }

        // 각 채널별 가이드선 종료 슬롯 위치 (0 ~ MAXIMUM_SLOT * 10)
        private int[] m_nGuideEndSlots = new int[100];
        public int[] GUIDE_END_SLOTS
        {
            get { return m_nGuideEndSlots; }
            set { m_nGuideEndSlots = value; Invalidate(); }
        }

        private bool m_bShowGuideLine = false;
        public bool SHOW_GUIDE_LINE
        {
            get { return m_bShowGuideLine; }
            set { m_bShowGuideLine = value; Invalidate(); }
        }

        private bool m_bShowVoltage = false;
        public bool SHOW_VOLTAGE
        {
            get { return m_bShowVoltage; }
            set { m_bShowVoltage = value; Invalidate(); }
        }
        private int m_nItem = 5;
        private int[] m_nInitialGraphState = new int[100];
        public int ITEM
        {
            get { return m_nItem; }
            set { m_nItem = value; ResetTitleBuffer(); Invalidate(); }
        }

        private Color m_BackColor = Color.White;
        public Color BACK_COLOR
        {
            get { return m_BackColor; }
            set { m_BackColor = value; Invalidate(); }
        }

        private Color m_LineColor = Color.Black;
        public Color LINE_COLOR
        {
            get { return m_LineColor; }
            set { m_LineColor = value; Invalidate(); }
        }

        private int m_nLineWidth = 1;
        public int LINE_WIDTH
        {
            get { return m_nLineWidth; }
            set { m_nLineWidth = value; Invalidate(); }
        }

        private Color m_ChartColor = Color.Red;
        public Color CHART_COLOR
        {
            get { return m_ChartColor; }
            set { m_ChartColor = value; Invalidate(); }
        }

        private int m_nMargin = 15;
        public int MARGIN
        {
            get { return m_nMargin; }
            set { m_nMargin = value; Invalidate(); }
        }

        private int m_nTitleWidth = 120;
        public int TITLE_WIDTH
        {
            get { return m_nTitleWidth; }
            set { m_nTitleWidth = value; Invalidate(); }
        }
        private Color m_TitleColor = Color.Black;
        public Color TITLE_COLOR
        {
            get { return m_TitleColor; }
            set { m_TitleColor = value; Invalidate(); }
        }
        private string[] m_strTitles = new string[100];
        public int[] m_nState = new int[100];

        const int m_nChartMargin = 4;

        public enum EdgeType { None, Rising, Falling };

        private int m_nMaximumSlot = 30;
        public int MAXIMUM_SLOT
        {
            get { return m_nMaximumSlot; }
            set { m_nMaximumSlot = value; Invalidate(); }
        }
        private EdgeType[,] m_SlotData = new EdgeType[5, 300];


        public int m_nSlotCount = 0;

        private DateTime m_StartTime;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public CtrlTimingChart()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);

            ResetTitleBuffer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public void ResetData()
        {
            m_SlotData = new EdgeType[m_nItem, m_nMaximumSlot * 10];
            m_nSlotCount = 0;
            for (int i = 0; i < m_nItem; ++i)
            {
                for (int j = 0; j < (m_nMaximumSlot * 10); ++j)
                    m_SlotData[i, j] = EdgeType.None;

                // 중요: Reset 시점의 전압을 기준으로 시작 상태(0 또는 1)를 고정
                // m_nState[i]가 25(2.5V) 이상이면 1에서 시작, 아니면 0에서 시작
                m_nInitialGraphState[i] = (m_nState[i] >= 25) ? 1 : 0;
            }

            m_StartTime = DateTime.Now;
            Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nItem"></param>
        /// <param name="nSlot"></param>
        /// <param name="edge"></param>
        /// 
        public void SetSlotData(int nItem, EdgeType edge)
        {
            m_SlotData[nItem, m_nSlotCount] = edge;
            Invalidate();
        }


        /// <summary>
        /// 
        /// </summary>
        /// 
        public void IncreaseSlotCount()
        {
            DateTime dtCurrent = DateTime.Now;
            TimeSpan dtSpan = dtCurrent - m_StartTime;

            m_nSlotCount = (int)(dtSpan.TotalMilliseconds / 100.0);
            if (m_nSlotCount >= (m_nMaximumSlot * 10))
                m_nSlotCount = (m_nMaximumSlot * 10) - 1;

            Invalidate();
        }


        /// <summary>
        /// 
        /// </summary>
        /// 
        private void ResetTitleBuffer()
        {
            m_strTitles = new string[m_nItem];
            m_nState = new int[m_nItem];
            m_nGuideStartSlots = new int[m_nItem]; // 초기화
            m_nGuideEndSlots = new int[m_nItem];   // 초기화
            for (int i = 0; i < m_nItem; ++i)
            {
                m_strTitles[i] = "abc";
                m_nState[i] = 0;
                m_nGuideStartSlots[i] = 5 + (i * 2);  // 기본값: 계단식 예시
                m_nGuideEndSlots[i] = 25 - (i * 2);   // 기본값: 계단식 예시
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="strTitle"></param>
        /// 
        public void SetTitle(int nIndex, string strTitle)
        {
            if (nIndex >= m_nItem)
                return;

            m_strTitles[nIndex] = strTitle;
            Invalidate();
        }
        public void SetStateValue(int nIndex, int nValue)
        {
            if (nIndex >= 0 && nIndex < m_nItem)
            {
                m_nState[nIndex] = nValue;
                // Invalidate()를 호출하지 않는 이유는 IncreaseSlotCount에서 어차피 호출되기 때문입니다.
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void CtrlTimingChart_Paint(object sender, PaintEventArgs e)
        {
            Graphics grfx = e.Graphics;
            int nWidth = this.Size.Width - 1;
            int nHeight = this.Size.Height - 1;

            // 1. 전체적인 여백(Margin)을 5에서 10~15 정도로 늘려 테두리 밖 흰색 배경 확보
            int nOuterMargin = 15;
            // 시간 표시를 위한 하단 여백 (약 20픽셀) 추가
            int nBottomLabelHeight = 20;
            double dItemHeight = (double)(nHeight - (m_nMargin * 2) - nBottomLabelHeight) / (double)m_nItem;

            int nChartStart = m_nMargin + m_nTitleWidth + 1;
            int nChartEnd = nWidth - m_nMargin;
            // int nChartEnd = nWidth - (m_nMargin * 2) - 1;
            int nChartWidth = nChartEnd - nChartStart;

            // 300 슬롯 기준 (30초 * 10슬롯/초)
            double dChartStep = (double)nChartWidth / (double)(m_nMaximumSlot * 10);
            double dSlotWidth = (double)(nChartWidth) / (double)m_nMaximumSlot;

            Pen penLine = new Pen(m_LineColor, m_nLineWidth);
            Pen penLineDot = new Pen(Color.LightGray, m_nLineWidth);
            penLineDot.DashStyle = DashStyle.DashDot;
            Pen penChart = new Pen(m_ChartColor, 2);
            SolidBrush textBrush = new SolidBrush(m_TitleColor);

            // 가이드선 굵게 및 둥근 모서리 처리
            Pen penGuide = new Pen(Color.FromArgb(220, 220, 220), 12);
            penGuide.LineJoin = LineJoin.Round;

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Far;
            sf.LineAlignment = StringAlignment.Center;

            Font valueFont = new Font("Consolas", 8);
            Font timeFont = new Font("Arial", 8); // 시간 표시용 폰트

            grfx.Clear(m_BackColor);

            // 차트 메인 테두리 (하단 여백 제외)
            grfx.DrawRectangle(penLine, m_nMargin, m_nMargin, nWidth - (m_nMargin * 2), nHeight - (m_nMargin * 2) - nBottomLabelHeight);

            // 수평 구분선
            for (int i = 1; i < m_nItem; ++i)
            {
                int y = m_nMargin + (int)(i * dItemHeight);
                grfx.DrawLine(penLineDot, m_nMargin, y, nWidth - m_nMargin, y);
            }

            // 시간 숫자 표시 간격 결정 (30초면 1초 단위, 그 외에는 5초 단위)
            int labelStep = (m_nMaximumSlot == 30) ? 1 : 5;

            for (int i = 0; i <= m_nMaximumSlot; ++i)
            {
                int x = nChartStart + (int)(i * dSlotWidth);

                // 1. 수직 점선(Grid Line) 그리기
                if (i > 0 && i < m_nMaximumSlot)
                {
                    grfx.DrawLine(penLineDot, x, m_nMargin, x, nHeight - m_nMargin - nBottomLabelHeight);
                }

                // 2. 하단 시간 숫자(Label) 그리기
                // 숫자는 정해진 간격(1초 또는 5초)일 때만 그립니다.
                if (i % labelStep == 0)
                {
                    string timeLabel = i.ToString();
                    SizeF size = grfx.MeasureString(timeLabel, timeFont);

                    // 텍스트를 눈금선 중앙에 맞춰서 출력
                    grfx.DrawString(timeLabel, timeFont, Brushes.Black, x - (size.Width / 2), nHeight - nBottomLabelHeight);
                }
            }

            // 타이틀 영역 구분 수직선
            grfx.DrawLine(penLine, m_nMargin + m_nTitleWidth, m_nMargin, m_nMargin + m_nTitleWidth, nHeight - m_nMargin - nBottomLabelHeight);

            if (m_strTitles != null)
            {
                for (int i = 0; i < m_nItem; ++i)
                {
                    int nItemHeightStart = m_nMargin + (int)(i * dItemHeight) + 1;
                    int nItemHeightEnd = nItemHeightStart + (int)dItemHeight - 2;

                    // 타이틀 표시
                    if (m_strTitles[i] != "")
                    {
                        Rectangle rect = new Rectangle(m_nMargin + 10, m_nMargin + (int)(i * dItemHeight), m_nTitleWidth - 30, (int)dItemHeight);
                        grfx.DrawString(m_strTitles[i], this.Font, textBrush, rect, sf);
                    }

                    // 가이드선 (Tolerance Band)
                    if (m_bShowGuideLine)
                    {
                        int yBase = nItemHeightEnd - m_nChartMargin;
                        int yTop = nItemHeightStart + m_nChartMargin + 5;

                        int xHatStart = nChartStart + (int)(m_nGuideStartSlots[i] * dChartStep);
                        int xHatEnd = nChartStart + (int)(m_nGuideEndSlots[i] * dChartStep);

                        int xSafeEnd = nChartEnd - 2;

                        if (xHatStart < xHatEnd)
                        {
                            Point[] hatPoints = {
                        new Point(nChartStart, yBase),
                        new Point(xHatStart, yBase),
                        new Point(xHatStart, yTop),
                        new Point(xHatEnd, yTop),
                        new Point(xHatEnd, yBase),
                        new Point(xSafeEnd, yBase)
                    };
                            grfx.DrawLines(penGuide, hatPoints);
                        }
                        else
                        {
                            grfx.DrawLine(penGuide, nChartStart, yBase, xSafeEnd, yBase);
                        }
                    }

                    // 실시간 전압 값 표시
                    if (m_bShowVoltage)
                    {
                        float voltValue = m_nState[i] / 10.0f;
                        string voltStr = $"{voltValue:F1}V";
                        grfx.DrawString(voltStr, valueFont, Brushes.Blue, m_nMargin + m_nTitleWidth - 45, nItemHeightEnd - 12);
                    }

                    // 실제 그래프(붉은 선) 그리기
                    if (m_nSlotCount > 0)
                    {
                        int nS = 0;
                        int nE = 0;
                        int nState = m_nInitialGraphState[i];
                        int yHigh = nItemHeightStart + m_nChartMargin + 5; // High 상태의 높이 (숫자가 커질수록 내려감)
                        int yLow = nItemHeightEnd - m_nChartMargin;      // Low 상태의 높이

                        for (int j = 0; j <= m_nSlotCount; ++j)
                        {
                            if (m_SlotData[i, j] != EdgeType.None)
                            {
                                nE = j;
                                // [수정] 현재 상태에 따라 yHigh 또는 yLow 선택
                                int yCurrent = (nState == 0) ? yLow : yHigh;

                                if (m_SlotData[i, j] == EdgeType.Rising)
                                {
                                    // 1. 현재 상태(Low)로 수평선 그리기
                                    grfx.DrawLine(penChart, nChartStart + (int)(nS * dChartStep), yCurrent, nChartStart + (int)(nE * dChartStep), yCurrent);
                                    // 2. nE 위치에서 다음 상태(High)인 yHigh까지 수직선 그리기
                                    grfx.DrawLine(penChart, nChartStart + (int)(nE * dChartStep), yCurrent, nChartStart + (int)(nE * dChartStep), yHigh);
                                    nState = 1;
                                }
                                else if (m_SlotData[i, j] == EdgeType.Falling)
                                {
                                    // 1. 현재 상태(High)로 수평선 그리기
                                    grfx.DrawLine(penChart, nChartStart + (int)(nS * dChartStep), yCurrent, nChartStart + (int)(nE * dChartStep), yCurrent);
                                    // 2. nE 위치에서 다음 상태(Low)인 yLow까지 수직선 그리기
                                    grfx.DrawLine(penChart, nChartStart + (int)(nE * dChartStep), yCurrent, nChartStart + (int)(nE * dChartStep), yLow);
                                    nState = 0;
                                }
                                nS = j;
                            }
                        }
                        int lastY = (nState == 0) ? yLow : yHigh;
                        // 마지막 지점의 X좌표 계산
                        int lastX = nChartStart + (int)(m_nSlotCount * dChartStep);

                        // [수정] 만약 마지막 슬롯이라면 nChartEnd를 넘지 않도록 강제 제한
                        if (lastX > nChartEnd) lastX = nChartEnd;

                        grfx.DrawLine(penChart, nChartStart + (int)(nS * dChartStep), lastY, nChartStart + (int)(m_nSlotCount * dChartStep), lastY);
                    }
                }
            }

            // 자원 해제
            penLine.Dispose();
            penLineDot.Dispose();
            penChart.Dispose();
            textBrush.Dispose();
            penGuide.Dispose();
            valueFont.Dispose();
            timeFont.Dispose();
        }
    }
}
