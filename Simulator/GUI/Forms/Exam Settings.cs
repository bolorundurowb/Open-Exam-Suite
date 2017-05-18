using System;
using System.Linq;
using System.Windows.Forms;
using Shared;

namespace Simulator.GUI.Forms
{
    public partial class Exam_Settings : Form
    {
        private Exam exam;
        private Settings settings;

        public Exam_Settings(Exam _exam)
        {
            InitializeComponent();
            //
            exam = _exam;
            //
            clb_section_options.Items.AddRange(exam.Sections.ToArray());
            //
            num_questions.Maximum = exam.NumberOfQuestions;
            //
            SelectAll(null, null);
        }

        private void Close(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CustomTimer(object sender, EventArgs e)
        {
            if(chk_enable_timer.Checked)
            {
                num_time_limit.Enabled = true;
            }
            else
            {
                num_time_limit.Enabled = false;
            }
        }

        private void ChooseNumOfQuestions(object sender, EventArgs e)
        {
            if (rdb_fixed_number_questions.Checked)
            {
                num_questions.Enabled = true;
            }
            else
            {
                num_questions.Enabled = false;
            }
        }

        private void ChooseSections(object sender, EventArgs e)
        {
            if(rdb_selected_sections.Checked)
            {
                clb_section_options.Enabled = true;
            }
            else
            {
                clb_section_options.Enabled = false;
            }
        }

        private void SelectAll(object sender, EventArgs e)
        {
            for (int i = 0; i < clb_section_options.Items.Count; i++)
            {
                clb_section_options.SetItemChecked(i, true);
            }
        }

        private void DeselectAll(object sender, EventArgs e)
        {
            for (int i = 0; i < clb_section_options.Items.Count; i++)
            {
                clb_section_options.SetItemChecked(i, false);
            }
        }

        private void Proceed(object sender, EventArgs e)
        {
            settings = new Settings();
            settings.CandidateName = txt_candidate_name.Text;
            if (chk_enable_timer.Checked)
                settings.TimeLimit = (int)num_time_limit.Value;
            else
                settings.TimeLimit = exam.Properties.TimeLimit;
            //
            if (rdb_selected_sections.Checked)
            {
                settings.Sections = clb_section_options.CheckedItems.Cast<Section>().ToList(); 
                foreach (var section in settings.Sections)
                    settings.Questions.AddRange(section.Questions.ToArray());
            }
            //
            if(rdb_fixed_number_questions.Checked)
            {
                int numOfQuestions = (int)num_questions.Value;
                int sum = 0;
                foreach(Section section in exam.Sections)
                {
                    if (sum + section.Questions.Count < numOfQuestions)
                    {
                        settings.Sections.Add(section);
                        settings.Questions.AddRange(section.Questions.ToArray());
                        sum += section.Questions.Count;
                    }
                    else if (sum + section.Questions.Count == numOfQuestions)
                    {
                        settings.Sections.Add(section);
                        settings.Questions.AddRange(section.Questions.ToArray());
                        break;
                    }
                    else
                    {
                        int difference = numOfQuestions - sum;
                        settings.Sections.Add(section);
                        settings.Questions.AddRange(section.Questions.GetRange(0, difference).ToArray());
                        break;
                    }
                }
            }
            //
            Exam_UI ui = new Exam_UI(exam, settings);
            this.Hide();
            ui.ShowDialog();
            this.Close();
        }
    }
}