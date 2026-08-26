using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ExcelDataReader;

namespace CITester
{
    public class ExcelSheetLoader
    {
        private readonly Dictionary<string, DataGridView> _sheetMap = new Dictionary<string, DataGridView>();

        public string FilePath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IO리스트.xlsx");

        public ExcelSheetLoader AddMap(string sheetName, DataGridView dgv)
        {
            _sheetMap[sheetName] = dgv;
            return this;
        }

        public void Load()
        {
            if (!File.Exists(FilePath))
            {
                MessageBox.Show($"엑셀 파일을 찾을 수 없습니다.\n경로: {FilePath}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (var stream = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var conf = new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
                        };

                        var dataSet = reader.AsDataSet(conf);

                        foreach (var entry in _sheetMap)
                        {
                            string sheetName = entry.Key;
                            DataGridView dgv = entry.Value;

                            if (dataSet.Tables.Contains(sheetName))
                            {
                                // ⭐ 바인딩 하기 전에 데이터 수정
                                DataTable dt = ProcessDataTable(sheetName, dataSet.Tables[sheetName]);
                                BindToGrid(dgv, dt);
                            }
                            else
                            {
                                BindToGrid(dgv, new DataTable());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"엑셀 파일을 읽는 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ────────────────────────────────────────────────────────
        // ⭐ 데이터 테이블 전처리 (컬럼 자르기 및 순번 추가)
        // ────────────────────────────────────────────────────────
        private DataTable ProcessDataTable(string sheetName, DataTable dt)
        {
            if (dt == null) return new DataTable();

            if (sheetName == "TC" || sheetName == "CC")
            {
                while (dt.Columns.Count > 5)
                {
                    dt.Columns.RemoveAt(dt.Columns.Count - 1);
                }
            }

            if (!dt.Columns.Contains("순 번"))
            {
                DataColumn noCol = new DataColumn("순 번", typeof(int));
                dt.Columns.Add(noCol);

                noCol.SetOrdinal(0);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i]["순 번"] = i + 1;
                }
            }
            if (!dt.Columns.Contains("판 정"))
            {
                DataColumn resultCol = new DataColumn("판 정", typeof(string));
                resultCol.DefaultValue = "-";
                dt.Columns.Add(resultCol);
            }

            return dt;
        }

        // ────────────────────────────────────────────────────────
        // UI 바인딩 및 스타일링
        // ────────────────────────────────────────────────────────
        private void BindToGrid(DataGridView dgv, DataTable dt)
        {
            if (dgv == null) return;

            dgv.SuspendLayout();
            try
            {
                dgv.DataSource = null;
                dgv.DataSource = dt;
                ApplyGridStyle(dgv);
            }
            finally
            {
                dgv.ResumeLayout();
            }
        }

        private void ApplyGridStyle(DataGridView dgv)
        {
            var columnWidths = new Dictionary<string, int>
    {
                { "순 번", 90 },
                { "채널형식", 250 },
                { "타겟채널", 200 },
                { "핀번호", 150 },
                { "모듈이름", 150 },
                { "내용", 350 },
                { "판정", 100 }
    };

            Type dgvType = dgv.GetType();
            System.Reflection.PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (pi != null) pi.SetValue(dgv, true, null);
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 40;
            dgv.RowTemplate.Height = 35;
            dgv.EnableHeadersVisualStyles = false;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.AllowUserToAddRows = false;
            dgv.RowHeadersVisible = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 클릭 시 색상 변화 없애기
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.DefaultCellStyle.SelectionBackColor = dgv.DefaultCellStyle.BackColor;
            dgv.DefaultCellStyle.SelectionForeColor = dgv.DefaultCellStyle.ForeColor;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = dgv.ColumnHeadersDefaultCellStyle.BackColor;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = dgv.ColumnHeadersDefaultCellStyle.ForeColor;

            // 정렬 및 헤더 스타일 설정
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 244, 253);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(8, 31, 78);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(240, 244, 253);
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(8, 31, 78);
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (columnWidths.TryGetValue(col.Name, out int customWidth))
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    col.Width = customWidth;
                    col.Resizable = DataGridViewTriState.False;
                }
                else
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
        }
    }
}