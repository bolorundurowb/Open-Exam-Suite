using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Creator
{
    public partial class Exam_and_Sections : Form
    {
        public Exam_and_Sections()
        {
            InitializeComponent();
        }

        private void btn_next_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void btn_new_Click(object sender, EventArgs e)
        {
            New_Section ns = new New_Section();
            ns.ShowDialog();
            string[] sections = ns.Sections;
            foreach (string s in sections)
            {
                bool exists = false;
                for (int i = 0; i <dgv_section_titles.Rows.Count; i++)
                {
                    if (dgv_section_titles.Rows[i].Cells[0].Value.ToString() == s)
                    {
                        exists = true;
                    }
                }
                if (!exists)
                {
                    dgv_section_titles.Rows.Add(s);
                }
            }
            dgv_section_titles.Rows[0].Cells[0].Selected = false;
        }

        private void dgv_section_titles_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv_section_titles.SelectedRows.Count == 1)
            {
                btn_rename.Enabled = true;
                btn_remove.Enabled = true;
                btn_move_down.Enabled = true;
                btn_move_up.Enabled = true;
            }
            if (dgv_section_titles.SelectedRows.Count > 1)
            {
                btn_rename.Enabled = false;
                btn_remove.Enabled = true;
                btn_move_down.Enabled = false;
                btn_move_up.Enabled = false;
            }
            if (dgv_section_titles.SelectedRows.Count == 0)
            {
                btn_rename.Enabled = false;
                btn_remove.Enabled = false;
                btn_move_down.Enabled = false;
                btn_move_up.Enabled = false;
            }
        }

        private void btn_rename_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgv_section_titles.SelectedRows)
            {
                dgv_section_titles.Rows[row.Index].Cells[0].ReadOnly = false;
                dgv_section_titles.CurrentCell = dgv_section_titles.Rows[row.Index].Cells[0];
                dgv_section_titles.BeginEdit(true);
            }
        }

        private void btn_remove_Click(object sender, EventArgs e)
        {
            foreach(DataGridViewRow row in dgv_section_titles.SelectedRows)
            {
                dgv_section_titles.Rows.RemoveAt(row.Index);
            }
        }

        private void btn_move_up_Click(object sender, EventArgs e)
        {
            try
            {
                int totalRows = dgv_section_titles.Rows.Count;
                int idx = dgv_section_titles.SelectedCells[0].OwningRow.Index;
                if (idx == 0)
                    return;
                int col = dgv_section_titles.SelectedCells[0].OwningColumn.Index;
                DataGridViewRowCollection rows = dgv_section_titles.Rows;
                DataGridViewRow row = rows[idx];
                rows.Remove(row);
                rows.Insert(idx - 1, row);
                dgv_section_titles.ClearSelection();
                dgv_section_titles.Rows[idx - 1].Cells[col].Selected = true;

            }
            catch { }
        }

        private void btn_move_down_Click(object sender, EventArgs e)
        {
            try
            {
                int totalRows = dgv_section_titles.Rows.Count;
                int idx = dgv_section_titles.SelectedCells[0].OwningRow.Index;
                if (idx == totalRows - 1)
                    return;
                int col = dgv_section_titles.SelectedCells[0].OwningColumn.Index;
                DataGridViewRowCollection rows = dgv_section_titles.Rows;
                DataGridViewRow row = rows[idx];
                rows.Remove(row);
                rows.Insert(idx + 1, row);
                dgv_section_titles.ClearSelection();
                dgv_section_titles.Rows[idx + 1].Cells[col].Selected = true;

            }
            catch { }
        }

        private void TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(((TextBox)sender).Text))
            {
                err_exam_wizard.SetError(((Control)sender ), "Please enter valid text!");
                btn_save.Enabled = false;
            }
            else
            {
                err_exam_wizard.Clear();
                btn_save.Enabled = true;
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ExamCode = txt_exam_code.Text;
            Properties.Settings.Default.ExamInstructions = txt_exam_instructions.Text;
            Properties.Settings.Default.ExamTitle = txt_exam_title.Text;
            Properties.Settings.Default.FileVersion = lbl_file_version.Text;
            Properties.Settings.Default.PassingScore = Convert.ToInt32(num_passing_score.Value);
            Properties.Settings.Default.TimeAllowed = Convert.ToInt32(num_time_limit.Value);            
            foreach(DataGridViewRow row in dgv_section_titles.Rows)
            {
                Properties.Settings.Default.SectionTitles.Add(row.Cells[0].Value.ToString());
            }
            Properties.Settings.Default.Save();
            this.Close();
        }
    }
}
