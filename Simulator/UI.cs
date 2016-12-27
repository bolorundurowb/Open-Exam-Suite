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

namespace Simulator
{
    public partial class UI : Form
    {
        string openedFilePath;
        public UI()
        {
            InitializeComponent();
        }

        public UI(string requestedFilePath)
        {
            InitializeComponent();
            if (requestedFilePath != string.Empty && Path.GetExtension(requestedFilePath).ToLower() == ".oef")
            {
                if (Properties.Settings.Default.ExamPaths != null)
                {
                    Properties.Settings.Default.ExamPaths.Add(requestedFilePath);
                    Properties.Settings.Default.ExamTitles.Add(Path.GetFileNameWithoutExtension(requestedFilePath));
                }
                else
                {
                    Properties.Settings.Default.ExamPaths = new System.Collections.Specialized.StringCollection();
                    Properties.Settings.Default.ExamPaths.Add(requestedFilePath);
                    Properties.Settings.Default.ExamTitles = new System.Collections.Specialized.StringCollection();
                    Properties.Settings.Default.ExamTitles.Add(Path.GetFileNameWithoutExtension(requestedFilePath));
                }
                Properties.Settings.Default.Save();
                openedFilePath = requestedFilePath;
            }
            else
            {
                MessageBox.Show("Selected file is not an OES Exam File", "File Type Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                openedFilePath = string.Empty;
            }
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
            ofd_select.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (ofd_select.ShowDialog() == DialogResult.OK)
            {
                List<string> paths = new List<string>();
                foreach (string filename in ofd_select.FileNames)
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

        private void btn_remove_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgv_exams.SelectedRows)
            {
                dgv_exams.Rows.RemoveAt(row.Index);
            }
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            if (File.Exists(dgv_exams.SelectedRows[0].Cells[1].Value.ToString()))
            {
                Exam_Settings sett = new Exam_Settings(dgv_exams.SelectedRows[0].Cells[1].Value.ToString(), dgv_exams.SelectedRows[0].Cells[0].Value.ToString());
                sett.ShowDialog();
            }
            else
            {
                MessageBox.Show("Sorry, the selected file has been moved or deleted.", "Access error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                dgv_exams.Rows.Remove(dgv_exams.SelectedRows[0]);
            }
        }

        private void btn_properties_Click(object sender, EventArgs e)
        {
            if (File.Exists(dgv_exams.SelectedRows[0].Cells[1].Value.ToString()))
            {
                Exam_Properties prop = new Exam_Properties(dgv_exams.SelectedRows[0].Cells[1].Value.ToString(), dgv_exams.SelectedRows[0].Cells[0].Value.ToString());
                prop.ShowDialog();
            }
            else
            {
                MessageBox.Show("Sorry, the selected file has been moved or deleted.", "Access error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                dgv_exams.Rows.Remove(dgv_exams.SelectedRows[0]);
            }
        }

        private void UI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Properties.Settings.Default.ExamPaths != null)
            {
                Properties.Settings.Default.ExamTitles.Clear();
                Properties.Settings.Default.ExamPaths.Clear();
            }
            Properties.Settings.Default.ExamTitles = new System.Collections.Specialized.StringCollection();
            Properties.Settings.Default.ExamPaths = new System.Collections.Specialized.StringCollection();
            for (int i = 0; i < dgv_exams.Rows.Count; i++)
            {
                Properties.Settings.Default.ExamTitles.Add(dgv_exams.Rows[i].Cells[0].Value.ToString());
                Properties.Settings.Default.ExamPaths.Add(dgv_exams.Rows[i].Cells[1].Value.ToString());
            }
            Properties.Settings.Default.Save();
        }

        private void UI_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.FirstRun)
            {
                string OESFolderPath = Path.GetDirectoryName(Application.StartupPath);
                string OESExamplesFolderPath = Path.Combine(OESFolderPath, "Examples");
                if (Directory.Exists(OESExamplesFolderPath))
                {
                    string[] oesFilePaths = Directory.GetFiles(OESExamplesFolderPath, "*.oef", SearchOption.TopDirectoryOnly);
                    List<string> paths = new List<string>();
                    foreach (string oesFile in oesFilePaths)
                    {
                        for (int i = 0; i < dgv_exams.Rows.Count; i++)
                        {
                            paths.Add(dgv_exams.Rows[i].Cells[1].Value.ToString());
                        }
                        if (!(paths.Exists(p => p == oesFile)))
                        {
                            dgv_exams.Rows.Add(Path.GetFileNameWithoutExtension(oesFile), oesFile);
                        }
                    }
                    Properties.Settings.Default.FirstRun = false;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Properties.Settings.Default.FirstRun = false;
                    Properties.Settings.Default.Save();
                }
            }
            Dictionary<string, string> exams = new Dictionary<string, string>();
            if (Properties.Settings.Default.ExamPaths != null)
            {
                for (int i = 0; i < Properties.Settings.Default.ExamPaths.Count; i++)
                {
                    try
                    {
                        exams.Add(Properties.Settings.Default.ExamPaths[i], Properties.Settings.Default.ExamTitles[i]);
                    }
                    catch (Exception ev)
                    {
                        GlobalPathVariables.WriteError(ev, "Simulator UI");
                    }
                }
            }
            dgv_exams.Rows.Clear();
            foreach (var exam in exams)
            {
                dgv_exams.Rows.Add(exam.Value, exam.Key);
            }

            //Select opened file
            if (!string.IsNullOrWhiteSpace(openedFilePath))
            {
                foreach(DataGridViewRow row in dgv_exams.Rows)
                {
                    if (row.Cells[1].Value.ToString() == openedFilePath)
                    {
                        row.Selected = true;
                    }
                }
            }
        }

        private void dgv_exams_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                dgv_exams.Rows[e.RowIndex].Selected = true;
            }
            catch (ArgumentException ex)
            {
                GlobalPathVariables.WriteError(ex, this.Name);
            }
            btn_start_Click(dgv_exams, e);         
        }
    }
}
