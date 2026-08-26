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
    public partial class CtrlOscilloscope : UserControl
    {
        private double m_dMinY = -25.0;
        public double MIN_Y
        {
            get { return m_dMinY; }
            set { m_dMinY = value; Invalidate(); }
        }

        private double m_dMaxY = 25.0;
        public double MAX_Y
        {
            get { return m_dMaxY; }
            set { m_dMaxY = value; Invalidate(); }
        }

        private double m_dMinX = 0;
        public double MIN_X
        {
            get { return m_dMinX; }
            set { m_dMinX = value; Invalidate(); }
        }

        private double m_dMaxX = 1000;
        public double MAX_X
        {
            get { return m_dMaxX; }
            set { m_dMaxX = value; Invalidate(); }
        }

        private double m_dUnit = 0.2;
        public double UNIT
        {
            get { return m_dUnit; }
            set { m_dUnit = value; Invalidate(); }
        }

        private double m_dOffset = 0;
        public double OFFSET
        {
            get { return m_dOffset; }
            set { m_dOffset = value; Invalidate(); }
        }
        
        private Color m_LineColor = Color.LightGray;
        public Color LINE_COLOR
        {
            get { return m_LineColor; }
            set { m_LineColor = value; Invalidate(); }
        }

        private Color m_GraphColor = Color.Yellow;
        public Color GRAPH_COLOR
        {
            get { return m_GraphColor; }
            set { m_GraphColor = value; Invalidate(); }
        }

        private int m_nColumn = 10;
        public int COLUMN
        {
            get { return m_nColumn; }
            set { m_nColumn = value; Invalidate(); }
        }

        private int m_nRow = 10;
        public int ROW
        {
            get { return m_nRow; }
            set { m_nRow = value; Invalidate(); }
        }

        private byte[] m_Data = new byte[10];
        private int m_nDataSize = 0;

        public CtrlOscilloscope()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        }

        public void SetData(byte[] data, int nSize)
        {
            int nLenCount = data[1] - 48;
            string strLen = "";
            for (int i = 0; i < nLenCount; ++i)
            {
                strLen += (data[2 + i] - 48).ToString();
            }

            int nDataCount = 0;
            try
            {
                nDataCount = int.Parse(strLen);
            }
            catch
            {
            }
            if (nDataCount > 0)
            {
                m_Data = new byte[nDataCount];
                m_nDataSize = nDataCount;
                Array.Copy(data, nLenCount + 2, m_Data, 0, nDataCount);
                Invalidate();
            }
        }

        public void ResetData()
        {
            m_nDataSize = 0;
            Invalidate();
        }

        private void CtrlOscilloscope_Paint(object sender, PaintEventArgs e)
        {
            Graphics grfx = e.Graphics;
            float fLeft = 70;
            float fRight = this.Size.Width - 20;
            float fTop = 20;
            float fBottom = this.Size.Height - 40;

            float fWidth = fRight - fLeft;
            float fHeight = fBottom - fTop;
            float fCenterX = fLeft + fWidth / 2;
            float fCenterY = fTop + fHeight / 2;
            float fUnitX = fWidth / m_nColumn;
            float fSubUnitX = fUnitX / 5;
            float fUnitY = fHeight / m_nRow;
            float fSubUnitY = fUnitY / 5;

            Pen penGraph = new Pen(m_GraphColor, 1);
            Pen penLine = new Pen(m_LineColor, 1);
            Pen penLineDot = new Pen(m_LineColor, 1);
            penLineDot.DashStyle = DashStyle.DashDot;
            penLineDot.DashPattern = new float[] { 1, 5 };

            // draw background
            //
            grfx.Clear(this.BackColor);

            for (int i = 0; i < m_nColumn; ++i)
            {
                grfx.DrawLine(penLineDot, fLeft + (i * fUnitX), fTop, fLeft + (i * fUnitX), fBottom);
                for(int j = 0; j < 5; ++j)
                    grfx.DrawLine(penLine, fLeft + (i * fUnitX) + (j * fSubUnitX), fCenterY - 4, fLeft + (i * fUnitX) + (j * fSubUnitX), fCenterY + 4);
            }

            for (int i = 0; i < m_nRow; ++i)
            {
                grfx.DrawLine(penLineDot, fLeft, fTop + (i * fUnitY), fRight, fTop + (i * fUnitY));
                for (int j = 0; j < 5; ++j)
                    grfx.DrawLine(penLine, fCenterX - 4, fTop + (i * fUnitY) + (j * fSubUnitY), fCenterX + 4, fTop + (i * fUnitY) + (j * fSubUnitY));
            }

            penLine.Width = 1;
            grfx.DrawRectangle(penLine, fLeft, fTop, fWidth, fHeight);
            grfx.DrawLine(penLine, fLeft, fCenterY, fRight, fCenterY);
            grfx.DrawLine(penLine, fCenterX, fTop, fCenterX, fBottom);
            grfx.DrawLine(penLine, fLeft - 10, fTop, fLeft, fTop);
            grfx.DrawLine(penLine, fLeft - 10, fCenterY, fLeft, fCenterY);
            grfx.DrawLine(penLine, fLeft - 10, fBottom, fLeft, fBottom);
            grfx.DrawLine(penLine, fLeft, fBottom, fLeft, fBottom + 10);
            grfx.DrawLine(penLine, fRight, fBottom, fRight, fBottom + 10);

            SolidBrush brushText = new SolidBrush(this.ForeColor);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Far;
            sf.LineAlignment = StringAlignment.Center;
            
            float fFontOffset = this.Font.Height / 2;
            grfx.DrawString("전압[V]", this.Font, brushText, 5f, fCenterY / 2);
            grfx.DrawString("Time[ms]", this.Font, brushText, fCenterX + fWidth / 4, fBottom + 10);
            
            grfx.DrawString(m_dMaxY.ToString(), this.Font, brushText, new RectangleF(0, fTop - fFontOffset, fLeft - 15, this.Font.Height), sf);
            grfx.DrawString("0", this.Font, brushText, new RectangleF(0, fCenterY - fFontOffset, fLeft - 15, this.Font.Height), sf);
            grfx.DrawString(m_dMinY.ToString(), this.Font, brushText, new RectangleF(0, fBottom - fFontOffset, fLeft - 15, this.Font.Height), sf);

            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            grfx.DrawString(m_dMinX.ToString(), this.Font, brushText, new RectangleF(fLeft - 20, fBottom + 15, 40, this.Font.Height), sf);
            grfx.DrawString(m_dMaxX.ToString(), this.Font, brushText, new RectangleF(fRight - 30, fBottom + 15, 50, this.Font.Height), sf);

            brushText.Dispose();

            float fX0 = 0;
            float fY0 = 0;
            float fX1 = 0;
            float fY1 = 0;
            float fValue = 0;
            if (m_nDataSize > 2)
            {
                // (m_dMax - fValue) : fYBand = x : fHeight
                float fYBand = (float)(m_dMaxY - m_dMinY);
                fX0 = fLeft;
                fValue = (float)(((double)(sbyte)m_Data[0] - m_dOffset) * m_dUnit);
                fY0 = fTop + (float)(((m_dMaxY - fValue) * fHeight) / fYBand);
                for (int i = 1; i < m_nDataSize; ++i)
                {
                    // i : m_nDataSize = y : fWidth 
                    fX1 = fLeft + (i * fWidth) / m_nDataSize;

                    fValue = (float)(((double)(sbyte)m_Data[i] - m_dOffset) * m_dUnit);
                    fY1 = fTop + (float)(((m_dMaxY - fValue) * fHeight) / fYBand);

                    grfx.DrawLine(penGraph, fX0, fY0, fX1, fY1);

                    fX0 = fX1;
                    fY0 = fY1;
                }
            }

            penGraph.Dispose();
            penLine.Dispose();
            penLineDot.Dispose();
        }
    }
}
