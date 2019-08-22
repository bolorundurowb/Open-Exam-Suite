using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Shared;
using Shared.Models;

namespace Simulator.GUI
{
    public partial class ExamSettingsUi : Form
    {
        private readonly Exam _exam;
        private readonly string _filePath;
        private Settings _settings;
        private string _reviewQuestionsTemplateString = @"Take all questions marked for review {0}";

        public ExamSettingsUi(Exam exam, string filePath)
        {
            InitializeComponent();

            _exam = exam;
            _filePath = filePath;

            rdbTakeAllMarkedQuestions.Text = string.Format(_reviewQuestionsTemplateString, exam.QuestionsMarkedForReviewCount);

            clb_section_options.Items.AddRange(_exam.Sections.ToArray());

            num_questions.Maximum = _exam.NumberOfQuestions;

            SelectAll(null, null);
        }

        private void Close(object sender, EventArgs e)
        {
            Close();
        }

        private void CustomTimer(object sender, EventArgs e)
        {
            num_time_limit.Enabled = chk_enable_timer.Checked;
        }

        private void rdbTakeAllMarkedQuestions_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ChooseNumOfQuestions(object sender, EventArgs e)
        {
            num_questions.Enabled = rdb_fixed_number_questions.Checked;
        }

        private void ChooseSections(object sender, EventArgs e)
        {
            clb_section_options.Enabled = rdb_selected_sections.Checked;
        }

        private void SelectAll(object sender, EventArgs e)
        {
            for (var i = 0; i < clb_section_options.Items.Count; i++)
            {
                clb_section_options.SetItemChecked(i, true);
            }
        }

        private void DeselectAll(object sender, EventArgs e)
        {
            for (var i = 0; i < clb_section_options.Items.Count; i++)
            {
                clb_section_options.SetItemChecked(i, false);
            }
        }

        private void Proceed(object sender, EventArgs e)
        {
            _settings = new Settings { CandidateName = txt_candidate_name.Text };
            if (chk_enable_timer.Checked)
                _settings.TimeLimit = (int)num_time_limit.Value;
            else
                _settings.TimeLimit = _exam.Properties.TimeLimit;

            if (rdb_selected_sections.Checked)
            {
                _settings.Sections = clb_section_options.CheckedItems.Cast<Section>().ToList();
                foreach (var section in _settings.Sections)
                    _settings.Questions.AddRange(section.Questions.ToArray());
            }

            if (rdb_fixed_number_questions.Checked)
            {
                var numOfQuestions = (int)num_questions.Value;
                var sum = 0;
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
            }

            if (rdbTakeAllMarkedQuestions.Checked)
            {
                _settings.Sections.AddRange(_exam.Sections);
                _settings.Questions.AddRange(_exam.AllQuestions);
                _settings.MarkedForReviewQuestionsOnly = true;
            }

            if (_settings.Questions.Count == 0)
            {
                MessageBox.Show(
                    "There are no questions to be displayed based on your selection. Please make a different selection.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var ui = new AssessmentUi(_exam, _settings, _filePath);
            Hide();
            ui.ShowDialog();
            Close();
        }

    }
}