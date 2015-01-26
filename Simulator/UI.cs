using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace OpenExam_Suite
{
    public partial class UI : Form
    {
        public UI()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form about = new About();
            about.ShowDialog();
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            opf_select.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (opf_select.ShowDialog() == DialogResult.OK)
            {
                List<string> paths = new List<string>();
                foreach (string filename in opf_select.FileNames)
                {
                    for (int i = 0; i < dgv_exams.Rows.Count; i++)
                    {
                        paths.Add(dgv_exams.Rows[i].Cells[1].Value.ToString());
                    }
                    if (!(paths.Exists(p => p == filename)))
                    {
                        dgv_exams.Rows.Add(Path.GetFileNameWithoutExtension(filename), filename);
                    }
                }
            }
            dgv_exams.Rows[0].Cells[0].Selected = false;
        }

        private void dgv_exams_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv_exams.SelectedRows.Count == 1)
            {
                btn_start.Enabled = true;
                btn_properties.Enabled = true;
                btn_remove.Enabled = true;
            }
            if (dgv_exams.SelectedRows.Count > 1)
            {
                btn_start.Enabled = false;
                btn_properties.Enabled = false;
                btn_remove.Enabled = true;
            }
            if (dgv_exams.SelectedRows.Count == 0)
            {
                btn_start.Enabled = false;
                btn_properties.Enabled = false;
                btn_remove.Enabled = false;
            }
        }

        private void dgv_exams_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                dgv_exams.Rows[e.RowIndex].Selected = true;
            }
            catch ( ArgumentException)
            {

            }
        }

        private void btn_remove_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow test in dgv_exams.SelectedRows)
            {
                dgv_exams.Rows.RemoveAt(test.Index);
            }
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            string fullFilePath = dgv_exams.SelectedRows[0].Cells[1].Value.ToString();
            Exam_Settings sett = new Exam_Settings(fullFilePath, dgv_exams.SelectedRows[0].Cells[0].Value.ToString());
            sett.ShowDialog();
        }

        private void btn_properties_Click(object sender, EventArgs e)
        {
            string fullFilePath = dgv_exams.SelectedRows[0].Cells[1].Value.ToString();
            Exam_Properties prop = new Exam_Properties(fullFilePath, dgv_exams.SelectedRows[0].Cells[0].Value.ToString());
            prop.ShowDialog();
        }

        private void UI_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.ExamTitles = "";
            Properties.Settings.Default.ExamPaths = "";
            for (int i = 0; i < dgv_exams.Rows.Count; i++)
            {
                Properties.Settings.Default.ExamTitles += ("," + dgv_exams.Rows[i].Cells[0].Value.ToString());
                Properties.Settings.Default.ExamPaths += ("," + dgv_exams.Rows[i].Cells[1].Value.ToString());
            }
            Properties.Settings.Default.Save();
        }

        private void UI_Load(object sender, EventArgs e)
        {
            string[] paths = Properties.Settings.Default.ExamPaths.Split(',');
            string[] titles = Properties.Settings.Default.ExamTitles.Split(',');
            if (!((paths == null) || (titles == null)))
            {
                dgv_exams.Rows.Clear();
                for (int i= 0; i < titles.Length; i++)
                {
                    if ((!(string.IsNullOrWhiteSpace(titles[i]))) && (!(string.IsNullOrWhiteSpace(paths[i]))))
                    {
                        dgv_exams.Rows.Add(titles[i], paths[i]);
                    }
                }
            }
            dgv_exams.Rows[0].Cells[0].Selected = false;
        }
    }
}
