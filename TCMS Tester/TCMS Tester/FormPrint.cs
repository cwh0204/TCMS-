using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CITester
{
    public partial class FormPrint : Form
    {
        PrintDialog printDialog1 = new PrintDialog();
        public int TotalPages { get; set; } = 1;

        // 생성자에서 출력할 문서를 전달받습니다.
        public FormPrint(PrintDocument doc)
        {
            InitializeComponent();



            // 미리보기 컨트롤 설정
            printPreviewControl1.Document = doc;
            printPreviewControl1.Zoom = 1.0; // 100% 크기 시작

            // 휠 이벤트 연결
            this.MouseWheel += FormPrint_MouseWheel;
        }

        // 마우스 휠로 확대/축소 (사용자 편의 기능)
        private void FormPrint_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0) // 휠 위로: 확대
                printPreviewControl1.Zoom += 0.1;
            else if (printPreviewControl1.Zoom > 0.1) // 휠 아래로: 축소
                printPreviewControl1.Zoom -= 0.1;
        }

        // [인쇄하기] 버튼
        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (printPreviewControl1.Document != null)
            {
                if (printDialog1.ShowDialog() == DialogResult.OK)
                {
                    printPreviewControl1.Document.Print();
                    this.Close();
                }
            }
        }

        // [이전 페이지] 버튼
        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (printPreviewControl1.StartPage > 0)
            {
                printPreviewControl1.StartPage--;
                UpdatePageLabel();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            // 전체 페이지 수(TotalPages)까지만 넘어가도록 제한
            if (printPreviewControl1.StartPage < TotalPages - 1)
            {
                printPreviewControl1.StartPage++;
                UpdatePageLabel();
            }
        }

        // [닫기] 버튼
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 페이지 라벨 업데이트
        private void UpdatePageLabel()
        {
            // "1 / 5 Page" 형식으로 표시하면 더 직관적입니다.
            lblPage.Text = $"{printPreviewControl1.StartPage + 1} / {TotalPages} Page";
        }
    }
}
