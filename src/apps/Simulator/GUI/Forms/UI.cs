using System;
using System.IO;
using System.Windows.Forms;
using Simulator.Enums;
using Simulator.Util;
using Simulator.GUI.Dialogs;

namespace Simulator.GUI.Forms
{
    public partial class Ui : Form
    {
        public Ui()
        {
            InitializeComponent();
        }

        public Ui(string filePath)
        {
            InitializeComponent();
            if (string.IsNullOrWhiteSpace(filePath) || Path.GetExtension(filePath).ToLower() == ".oef")
            {
                Simulator.Properties.Settings.Default.ExamPaths.Add(filePath);
                Simulator.Properties.Settings.Default.Save();
            }
            else
            {
                MessageBox.Show("Selected file is not an OES Exam File", "File Type Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void Exit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void AddExam(object sender, EventArgs e)
        {
            if (ofd_exam.ShowDialog() != DialogResult.OK) return;
            foreach (var fileName in ofd_exam.FileNames)
            {
                if (!CheckIfExamExists(fileName))
                {
                    dgv_exams.Rows.Add(Path.GetFileNameWithoutExtension(fileName), fileName);
                }
            }
        }

        private bool CheckIfExamExists(string fileName)
        {
            var exists = false;
            foreach (DataGridViewRow row in dgv_exams.Rows)
            {
                if (row.Cells[1].Value.ToString() == fileName)
                    exists = true;
            }
            return exists;
        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            if (dgv_exams.SelectedRows.Count == 1)
            {
                btn_start.Enabled = true;
                btn_properties.Enabled = true;
                btn_remove.Enabled = true;
            }
            else if (dgv_exams.SelectedRows.Count > 1)
            {
                btn_start.Enabled = false;
                btn_properties.Enabled = false;
                btn_remove.Enabled = true;
            }
            else
            {
                btn_start.Enabled = false;
                btn_properties.Enabled = false;
                btn_remove.Enabled = false;
            }
        }

        private void Remove(object sender, EventArgs e)
        {
            RowManager.RemoveRow(dgv_exams);
        }

        private void Properties(object sender, EventArgs e)
        {
            DialogManager.DisplayDialog(DialogType.ExamProperties, dgv_exams);
        }

        private void About(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void Start(object sender, EventArgs e)
        {
            DialogManager.DisplayDialog(DialogType.ExamSettings, dgv_exams);
        }

        private void SaveAppData(object sender, FormClosingEventArgs e)
        {
            AppDataManager.SaveAppData(dgv_exams);
        }

        private void LoadAppData(object sender, EventArgs e)
        {
            AppDataManager.LoadAppData(dgv_exams);
        }

        private void ChangeHeaderSize(object sender, EventArgs e)
        {
            name.Width = dgv_exams.Width / 3;
            path.Width = dgv_exams.Width * 2 / 3;
        }
    }
}