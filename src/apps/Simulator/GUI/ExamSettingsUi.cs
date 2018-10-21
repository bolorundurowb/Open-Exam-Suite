using System;
using System.Linq;
using System.Windows.Forms;
using Shared;
using Shared.Models;

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
            Exam tmp_exam;
            if (chk_RandomizeAnswers.Checked)
            {
                Random rnd = new Random();
                tmp_exam = new Exam(_exam);
                for(int isec = 0; isec < tmp_exam.Sections.Count; isec++)
                {
                    for (int ique = 0; ique < tmp_exam.Sections[isec].Questions.Count; ique++)
                    {
                        tmp_exam.Sections[isec].Questions[ique].Options = tmp_exam.Sections[isec].Questions[ique].Options.OrderBy(x => rnd.Next()).ToList();
                        char alphabet = 'A';
                        char newAlphabet = '\0';
                        for (int i = 0; i < tmp_exam.Sections[isec].Questions[ique].Options.Count; i++)
                        {
                            if(tmp_exam.Sections[isec].Questions[ique].Answer == tmp_exam.Sections[isec].Questions[ique].Options[i].Alphabet)
                            {
                                newAlphabet = alphabet;
                            }
                            tmp_exam.Sections[isec].Questions[ique].Options[i].Alphabet = alphabet++;
                        }
                        tmp_exam.Sections[isec].Questions[ique].Answer = newAlphabet;
                    }
                }
            }
            else
            {
                tmp_exam = _exam;
            }

            _settings = new Settings {CandidateName = txt_candidate_name.Text};
            if (chk_enable_timer.Checked)
                _settings.TimeLimit = (int) num_time_limit.Value;
            else
                _settings.TimeLimit = tmp_exam.Properties.TimeLimit;

            if (rdb_selected_sections.Checked)
            {
                _settings.Sections = clb_section_options.CheckedItems.Cast<Section>().ToList();
                _settings.Sections = (from a in clb_section_options.CheckedItems.Cast<Section>().ToList()
                                     join b in tmp_exam.Sections
                                     on a.Title equals b.Title
                                     select b
                                     ).ToList();
                foreach (var section in _settings.Sections)
                    _settings.Questions.AddRange(section.Questions.ToArray());
            }

            if (rdb_fixed_number_questions.Checked)
            {
                var numOfQuestions = (int) num_questions.Value;
                var sum = 0;
                foreach (var section in tmp_exam.Sections)
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

            if (_settings.Questions.Count == 0)
            {
                MessageBox.Show(
                    "There are no questions to be displayed based on your selection. Please make a different selection.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var ui = new AssessmentUi(tmp_exam, _settings);
            Hide();
            ui.ShowDialog();
            Close();
        }
    }
}