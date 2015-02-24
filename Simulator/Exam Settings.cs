using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ionic.Zip;
using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace Simulator
{
    public partial class Exam_Settings : Form
    {
        string fullFilePath;
        string filename;
        int examType;
        string selectedSections;
        string outputPath;
        int defaultExamDuration;

        public Exam_Settings()
        {
            InitializeComponent();
        }

        public Exam_Settings(string filePath, string fileName)
        {
            InitializeComponent();
            fullFilePath = filePath;
            filename = fileName;
        }

        private void rdb_selected_sections_CheckedChanged(object sender, EventArgs e)
        {
            if (rdb_selected_sections.Checked)
            {
                clb_section_options.Enabled = true;
                btn_deselect_all.Enabled = true;
                btn_select_all.Enabled = true;
                this.examType = 1;
            }
            else
            {
                clb_section_options.Enabled = false; 
                btn_deselect_all.Enabled = false;
                btn_select_all.Enabled = false;
                this.examType = 2;
            }
        }

        private void rdb_fixed_number_questions_CheckedChanged(object sender, EventArgs e)
        {
            if (rdb_fixed_number_questions.Checked)
            {
                num_exam_number.Enabled = true;
                this.examType = 2;
            }
            else
            {
                num_exam_number.Enabled = false;
                this.examType = 1;
            }
        }

        private void chk_enable_timer_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_enable_timer.Checked)
            {
                num_time_limit.Enabled = true;
            }
            else
            {
                num_time_limit.Enabled = false;
            }
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            if (rdb_selected_sections.Checked)
            {
                for (int i =0; i < clb_section_options.CheckedItems.Count; i++)
                {
                    this.selectedSections += ("," + clb_section_options.CheckedItems[i].ToString());
                }
            }
            else
            {
                for (int i = 0; i < clb_section_options.Items.Count; i++)
                {
                    this.selectedSections += ("," + clb_section_options.Items[i].ToString());
                }
            }
            if (rdb_fixed_number_questions.Checked)
            {
                this.examType = 2;
            }
            else
            {
                this.examType = 1;
            }
            Properties.Settings.Default.ExamPath = outputPath;
            Properties.Settings.Default.CandidatesName = txt_candidaate_name.Text;
            Properties.Settings.Default.ExamType = this.examType;
            Properties.Settings.Default.NumberOfQuestions = Convert.ToInt32(num_exam_number.Value);
            Properties.Settings.Default.SelectedSections = selectedSections;
            Properties.Settings.Default.TimerEnabled = chk_enable_timer.Checked;
            if (chk_enable_timer.Checked)
            {
                Properties.Settings.Default.TimerValue = Convert.ToInt32(num_time_limit.Value);
            }
            else
            {
                Properties.Settings.Default.TimerValue = defaultExamDuration;
            }
            Properties.Settings.Default.Save();
            Form exam = new Exam_UI(filename);
            this.Hide();
            exam.ShowDialog();
            this.Close();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Exam_Settings_Load(object sender, EventArgs e)
        {
            string temp = "Open Exam Files\\Simulator\\" + filename;
            outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), temp);
            if (!(Directory.Exists(outputPath)))
            {
                using (ZipFile zip = ZipFile.Read(fullFilePath))
                {
                    foreach (ZipEntry ent in zip)
                    {
                        ent.Extract(outputPath, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
            }
            int numOfQuestions = 0;
            //New try
            try
            {
                XPathDocument doc = new XPathDocument(outputPath + Path.DirectorySeparatorChar + filename + ".xml");
                XPathNavigator nav = doc.CreateNavigator();
                // Compile a standard XPath expression
                XPathExpression expr;
                expr = nav.Compile("//Section");
                XPathNodeIterator iterator = nav.Select(expr);
                // Iterate on the node set
                while (iterator.MoveNext())
                {
                    clb_section_options.Items.Add(iterator.Current.GetAttribute("Title", ""));
                }
                XPathExpression exp = nav.Compile("//Question");
                XPathNodeIterator iter = nav.Select(exp);
                while (iter.MoveNext())
                {
                    numOfQuestions += 1;
                }
                num_exam_number.Maximum = numOfQuestions;
                expr = nav.Compile("//TimeAllowed");
                iterator = nav.Select(expr);
                // Iterate on the node set
                while (iterator.MoveNext())
                {
                    defaultExamDuration = Convert.ToInt32(iterator.Current.Value);
                }
                expr = nav.Compile("//TimeAllowed");
                iterator = nav.Select(expr);
                // Iterate on the node set
                while (iterator.MoveNext())
                {
                    defaultExamDuration = Convert.ToInt32(iterator.Current.Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            for (int i = 0; i < clb_section_options.Items.Count; i++)
                clb_section_options.SetItemChecked(i, true);
        }

        private void btn_select_all_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clb_section_options.Items.Count; i++)
                clb_section_options.SetItemChecked(i, true);
        }

        private void btn_deselect_all_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clb_section_options.Items.Count; i++)
                clb_section_options.SetItemChecked(i, false);
        }
    }
}
