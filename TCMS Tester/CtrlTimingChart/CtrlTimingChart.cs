using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CtrlTimingChart
{
    public partial class CtrlTimingChart : UserControl
    {
        public CtrlTimingChart()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
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

            SolidBrush markBrush = new SolidBrush(Color.FromArgb(194, 198, 209));

            // draw background
            //
            grfx.Clear(Color.FromArgb(0, 16, 33));
        }
    }
}
