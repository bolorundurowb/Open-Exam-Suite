using System;
using System.Linq;
using System.Windows.Forms;
using Shared;
using Shared.Models;
using System.Collections.Generic;

namespace Simulator.GUI
{
    public partial class ExamSettingsUi : Form
    {
        private readonly Exam _exam;
        private Settings _settings;

        public ExamSettingsUi(Exam exam)
        {
            InitializeComponent();

            _exam = exam;

            num_questions.Maximum = _exam.NumberOfQuestions;

            SelectAll(null, null);

            int ypos = 0;
            foreach(var section in _exam.Sections)
            {
                CheckBox cbSection = new CheckBox();
                cbSection.Name = "cbSection_" + section.Title.Replace(" ","");
                cbSection.Checked = true;
                cbSection.Text = section.Title;
                cbSection.Width = 200;
                cbSection.Location = new System.Drawing.Point(0, 24 * ypos);
                pan_sectionSelection.Controls.Add(cbSection);

                NumericUpDown numSectionQuestions = new NumericUpDown();
                numSectionQuestions.Name = "numUD_" + section.Title.Replace(" ", "");
                numSectionQuestions.Value = section.NumberOfQuestionsToTake;
                numSectionQuestions.DecimalPlaces = 0;
                numSectionQuestions.Location = new System.Drawing.Point(205, 24 * ypos);
                pan_sectionSelection.Controls.Add(numSectionQuestions);

                ypos++;
            }
        }

        private void Close(object sender, EventArgs e)
        {
            Close();
        }

        private void CustomTimer(object sender, EventArgs e)
        {
            num_time_limit.Enabled = chk_enable_timer.Checked;
        }

        private void ChooseNumOfQuestions(object sender, EventArgs e)
        {
            num_questions.Enabled = rdb_fixed_number_questions.Checked;
        }

        private void ChooseSections(object sender, EventArgs e)
        {
            pan_sectionSelection.Enabled = rdb_selected_sections.Checked;
        }

        private void SelectAll(object sender, EventArgs e)
        {
            foreach (var con in pan_sectionSelection.Controls)
            {
                if (con.GetType() == typeof(CheckBox))
                {
                    CheckBox cb = (CheckBox)con;
                    cb.Checked = true;
                }
            }
        }

        private void DeselectAll(object sender, EventArgs e)
        {
            foreach(var con in pan_sectionSelection.Controls)
            {
                if(con.GetType() == typeof(CheckBox))
                {
                    CheckBox cb = (CheckBox)con;
                    cb.Checked = false;
                }
            }
        }

        private void Proceed(object sender, EventArgs e)
        {
            _settings = new Settings {CandidateName = txt_candidate_name.Text};
            Random rnd = new Random();

            if (chk_enable_timer.Checked)
                _settings.TimeLimit = (int) num_time_limit.Value;
            else
                _settings.TimeLimit = _exam.Properties.TimeLimit;

            if (rdb_selected_sections.Checked)
            {
                foreach (var sec in _exam.Sections)
                {
                    if (((CheckBox)pan_sectionSelection.Controls["cbSection_" + sec.Title.Replace(" ","")]).Checked)
                    {
                        Section newSec = new Section();
                        newSec.Title = sec.Title;
                        newSec = sec;
                        _settings.Sections.Add(newSec);

                        int numQuestions = (int)((NumericUpDown)pan_sectionSelection.Controls["numUD_" + sec.Title.Replace(" ", "")]).Value;
                        _settings.Questions.AddRange(_exam.Sections.Where(x=>x.Title == sec.Title).FirstOrDefault().Questions.OrderBy(x => rnd.Next()).Take(numQuestions));
                    }
                }
            }

            if (rdb_fixed_number_questions.Checked)
            {
                var numOfQuestions = (int) num_questions.Value;
                int[] numSectionQuestions = new int[_exam.Sections.Count];
                float[] numSectionQuestionsCalc = new float[_exam.Sections.Count];
                int sumNumSectionQuestions = 0;
                int totalQuestions = _exam.NumberOfQuestions;

                //compute number of questions per section
                for (int i = 0; i < _exam.Sections.Count; i++)
                {
                    numSectionQuestionsCalc[i] = (float) _exam.Sections[i].Questions.Count / totalQuestions * numOfQuestions;
                    numSectionQuestions[i] = (int) numSectionQuestionsCalc[i];
                    sumNumSectionQuestions += numSectionQuestions[i];
                    numSectionQuestionsCalc[i] = numSectionQuestionsCalc[i] - numSectionQuestions[i];
                }
                //fix calculation rounding error 
                if(numOfQuestions != sumNumSectionQuestions)
                {
                    int i = numSectionQuestionsCalc.ToList().IndexOf(numSectionQuestionsCalc.Max());
                    numSectionQuestions[i] += 1;
                }
                //for each section get random proportional size set of questions
                for (int i = 0; i < _exam.Sections.Count; i++)
                {
                    _settings.Sections.Add(_exam.Sections[i]);
                    _settings.Questions.AddRange(_exam.Sections[i].Questions.OrderBy(x => rnd.Next()).Take(numSectionQuestions[i]));
                }
                
                

                /*
                foreach (var section in _exam.Sections)
                {
                    if (sum + section.Questions.Count < numOfQuestions)
                    {
                        _settings.Sections.Add(section);
                        _settings.Questions.AddRange(section.Questions.ToArray());
                        sum += section.Questions.Count;
                    }
                    else if (sum + section.Questions.Count == numOfQuestions)
                    {
                        _settings.Sections.Add(section);
                        _settings.Questions.AddRange(section.Questions.ToArray());
                        break;
                    }
                    else
                    {
                        var difference = numOfQuestions - sum;
                        _settings.Sections.Add(section);
                        _settings.Questions.AddRange(section.Questions.GetRange(0, difference).ToArray());
                        break;
                    }
                }
                */
            }

            if (_settings.Questions.Count == 0)
            {
                MessageBox.Show(
                    "There are no questions to be displayed based on your selection. Please make a different selection.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var ui = new AssessmentUi(_exam, _settings);
            Hide();
            ui.ShowDialog();
            Close();
        }
    }
}