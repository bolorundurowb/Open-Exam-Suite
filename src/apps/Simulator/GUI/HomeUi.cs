using OpenExamSuite.Simulator.Enums;
using OpenExamSuite.Simulator.Utilities;
using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Interfaces;
using OpenExamSuite.Storage.Models;
using OpenExamSuite.Storage.Services;

namespace OpenExamSuite.Simulator.GUI;

public partial class HomeUi : Form
{
    private readonly IAppSettingsService _appSettings;

    public HomeUi() : this(new AppSettingsService(), null)
    {
    }

    public HomeUi(IAppSettingsService appSettings) : this(appSettings, null)
    {
    }

    public HomeUi(IAppSettingsService appSettings, string? initialExamFile)
    {
        _appSettings = appSettings;
        InitializeComponent();

        if (string.IsNullOrWhiteSpace(initialExamFile))
            return;

        if (Path.GetExtension(initialExamFile).Equals(".oef", StringComparison.OrdinalIgnoreCase))
        {
            _appSettings.Add(new AppSetting
            {
                Name = Path.GetFileNameWithoutExtension(initialExamFile),
                FilePath = initialExamFile
            }, AppSettingsType.Simulator);
        }
        else
        {
            MessageBox.Show("Selected file is not an OES Exam File", "File Type Error", MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
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

                _appSettings.Add(new AppSetting
                {
                    Name = Path.GetFileNameWithoutExtension(fileName),
                    FilePath = fileName
                }, AppSettingsType.Simulator);
            }
        }
    }

    private bool CheckIfExamExists(string fileName)
    {
        var exists = false;
        foreach (DataGridViewRow row in dgv_exams.Rows)
        {
            if (row.Cells[1].Value?.ToString() == fileName)
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
        RowManager.RemoveRow(dgv_exams, _appSettings);
    }

    private void Properties(object sender, EventArgs e)
    {
        DialogManager.DisplayDialog(DialogType.ExamProperties, dgv_exams, _appSettings);
    }

    private void About(object sender, EventArgs e)
    {
        var about = new AboutUi();
        about.ShowDialog();
    }

    private void License(object sender, EventArgs e)
    {
        using var license = new OpenExamSuite.Shared.Dialogs.LicenseUi();
        license.ShowDialog();
    }

    private void Start(object sender, EventArgs e)
    {
        DialogManager.DisplayDialog(DialogType.ExamSettings, dgv_exams, _appSettings);
    }

    private void LoadAppData(object sender, EventArgs e)
    {
        AppDataManager.LoadAppData(dgv_exams, _appSettings);
    }

    private void ChangeHeaderSize(object sender, EventArgs e)
    {
        name.Width = dgv_exams.Width / 3;
    }
}
