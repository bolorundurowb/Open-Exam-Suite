using OpenExamSuite.Shared;
using OpenExamSuite.Shared.Models;
using OpenExamSuite.Shared.Services;

namespace OpenExamSuite.Simulator.GUI;

public partial class ExamSettingsUi : Form
{
    private readonly Exam _exam;

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

    private void SelectAll(object? sender, EventArgs? e)
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
        var settings = new Settings { CandidateName = txt_candidate_name.Text };

        if (chk_enable_timer.Checked)
            settings.TimeLimit = (int)num_time_limit.Value;
        else
            settings.TimeLimit = _exam.Properties.TimeLimit;

        var composer = new ExamComposer();

        if (rdb_selected_sections.Checked)
        {
            var result = composer.SelectSections(_exam, clb_section_options.CheckedItems.Cast<Section>());
            settings.Sections = result.Sections;
            settings.Questions = result.Questions;
        }

        if (rdb_fixed_number_questions.Checked)
        {
            var result = composer.SelectFixedQuestions(_exam, (int)num_questions.Value);
            settings.Sections = result.Sections;
            settings.Questions = result.Questions;
        }

        if (settings.Questions.Count == 0)
        {
            MessageBox.Show(
                "There are no questions to be displayed based on your selection. Please make a different selection.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var ui = new AssessmentUi(_exam, settings);
        Hide();
        ui.ShowDialog();
        Close();
    }
}