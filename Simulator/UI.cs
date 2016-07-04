using Shared;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;

namespace Simulator
{
    public partial class UI : Form
    {
        public UI()
        {
            InitializeComponent();
        }

        public UI(string filePath)
        {
            InitializeComponent();
            //
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
            if(ofd_exam.ShowDialog() == DialogResult.OK)
            {
                foreach (string fileName in ofd_exam.FileNames)
                {
                    if (!CheckIfExamExists(fileName))
                    {
                        dgv_exams.Rows.Add(Path.GetFileNameWithoutExtension(fileName), fileName);
                    }
                }
            }
        }

        private bool CheckIfExamExists(string fileName)
        {
            bool exists = false;
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
            foreach (DataGridViewRow row in dgv_exams.SelectedRows)
            {
                dgv_exams.Rows.Remove(row);
            }
        }

        private void Properties(object sender, EventArgs e)
        {
            try
            {
                string filePath = dgv_exams.SelectedRows[0].Cells[1].Value.ToString();
                Exam exam = Helper.GetExamFromFile(filePath);
                Exam_Properties properties = new Exam_Properties(exam, filePath);
                properties.ShowDialog();
            }
            catch(FileNotFoundException)
            {
                MessageBox.Show("Sorry, the selected exam does not exist. It may have been moved or deleted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Remove(sender, e);
            }
            catch(NullReferenceException)
            {
                MessageBox.Show("Sorry, the selected exam is not a supported version. You can convert it using  the upgrade tool at:\n" + "https://sourceforge.net/projects/exam-upgrade-tool/", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Remove(sender, e);
            }
        }

        private void About(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void Start(object sender, EventArgs e)
        {
            try
            {
                string filePath = dgv_exams.SelectedRows[0].Cells[1].Value.ToString();
                Exam exam = Helper.GetExamFromFile(filePath);
                Exam_Settings settings = new Exam_Settings(exam);
                settings.ShowDialog();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Sorry, the selected exam does not exist. It may have been moved or deleted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Remove(sender, e);
            }
            catch(NullReferenceException)
            {
                MessageBox.Show("Sorry, the selected exam is not a supported version. You can convert it using  the upgrade tool at:\n" + "https://sourceforge.net/projects/exam-upgrade-tool/", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Remove(sender, e);
            }
        }

        private void SaveAppData(object sender, FormClosingEventArgs e)
        {
            Simulator.Properties.Settings.Default.ExamPaths = new StringCollection();
            foreach (DataGridViewRow row in dgv_exams.Rows)
                Simulator.Properties.Settings.Default.ExamPaths.Add(row.Cells[1].Value.ToString());
            Simulator.Properties.Settings.Default.Save();
        }

        private void LoadAppData(object sender, EventArgs e)
        {
            if(Simulator.Properties.Settings.Default.FirstRun)
            {
                string suiteRootFolder = Application.StartupPath;
                string samplesFolder = Path.Combine(suiteRootFolder, "Samples");
                string gmatSample = Path.Combine(samplesFolder, "GMAT Sample.oef");
                string basicScienceSample = Path.Combine(samplesFolder, "Basic Science.oef");
                //
                Simulator.Properties.Settings.Default.ExamPaths.Add(gmatSample);
                Simulator.Properties.Settings.Default.ExamPaths.Add(basicScienceSample);
                //
                Simulator.Properties.Settings.Default.FirstRun = false;
                //
                Simulator.Properties.Settings.Default.Save();
            }
            //
            if (Simulator.Properties.Settings.Default.ExamPaths != null)
            {
                foreach(string path in Simulator.Properties.Settings.Default.ExamPaths)
                {
                    dgv_exams.Rows.Add(Path.GetFileNameWithoutExtension(path), path);
                }
            }
        }

        private void ChangeHeaderSize(object sender, EventArgs e)
        {
            name.Width = dgv_exams.Width / 3;
            path.Width = dgv_exams.Width * 2 / 3;
        }
    }
}