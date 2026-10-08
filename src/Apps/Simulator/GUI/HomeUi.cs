using OpenExamSuite.Shared.Utilities;
using OpenExamSuite.Simulator.Enums;
using OpenExamSuite.Simulator.Utilities;
using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Interfaces;
using OpenExamSuite.Storage.Services;

namespace OpenExamSuite.Simulator.GUI;

public partial class HomeUi : Form
{
    private readonly IExamLibraryService _library;
    private readonly Reader _reader;

    public HomeUi() : this(new ExamLibraryService(), new Reader(), null)
    {
    }

    public HomeUi(IExamLibraryService library) : this(library, new Reader(), null)
    {
    }

    public HomeUi(IExamLibraryService library, Reader reader) : this(library, reader, null)
    {
    }

    public HomeUi(IExamLibraryService library, Reader reader, string? initialExamFile)
    {
        _library = library;
        _reader = reader;
        InitializeComponent();

        if (string.IsNullOrWhiteSpace(initialExamFile))
            return;

        if (Path.GetExtension(initialExamFile).Equals(".oef", StringComparison.OrdinalIgnoreCase))
        {
            _library.AddExam(ExamCatalog.Simulator, initialExamFile, Path.GetFileNameWithoutExtension(initialExamFile));
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

                _library.AddExam(ExamCatalog.Simulator, fileName, Path.GetFileNameWithoutExtension(fileName));
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
        RowManager.RemoveRow(dgv_exams, _library);
    }

    private void Properties(object sender, EventArgs e)
    {
        DialogManager.DisplayDialog(DialogType.ExamProperties, dgv_exams, _library, _reader);
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

    private void Changelog(object sender, EventArgs e)
    {
        using var changelog = new OpenExamSuite.Shared.Dialogs.ChangelogUi();
        changelog.ShowDialog();
    }

    private void Start(object sender, EventArgs e)
    {
        DialogManager.DisplayDialog(DialogType.ExamSettings, dgv_exams, _library, _reader);
    }

    private void LoadAppData(object sender, EventArgs e)
    {
        AppDataManager.LoadAppData(dgv_exams, _library);
    }

    private void ChangeHeaderSize(object sender, EventArgs e)
    {
        name.Width = dgv_exams.Width / 3;
    }
}
