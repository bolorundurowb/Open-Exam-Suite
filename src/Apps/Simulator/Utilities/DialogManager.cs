using OpenExamSuite.Shared;
using OpenExamSuite.Shared.Enums;
using OpenExamSuite.Shared.Utilities;
using OpenExamSuite.Simulator.Enums;
using OpenExamSuite.Simulator.GUI;
using OpenExamSuite.Storage.Interfaces;

namespace OpenExamSuite.Simulator.Utilities;

public static class DialogManager
{
    public static void DisplayDialog(
        DialogType dialogType,
        DataGridView dataGridView,
        IExamLibraryService library,
        Reader reader)
    {
        var selectedFilePath = dataGridView.SelectedRows[0].Cells[1].Value?.ToString();
        if (selectedFilePath == null)
            return;

        var result = reader.FromOefFile(selectedFilePath);

        if (!result.Success || result.Exam == null)
        {
            if (result.Error == ExamIoError.FileNotFound)
            {
                MessageBox.Show("Sorry, the selected exam does not exist. It may have been moved or deleted.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(
                    "Sorry, the exam selected is either old or corrupt. If it is an old exam, please upgrade it with the upgrade tool at:\nhttps://sourceforge.net/projects/exam-upgrade-tool/",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            RowManager.RemoveRow(dataGridView, library);
            return;
        }

        if (dialogType == DialogType.ExamSettings)
        {
            InitialiseExamSettings(result.Exam);
        }
        else if (dialogType == DialogType.ExamProperties)
        {
            InitialiseExamProperties(result.Exam, selectedFilePath);
        }
    }

    private static void InitialiseExamProperties(Exam exam, string filePath)
    {
        var properties = new ExamPropertiesUi(exam, filePath);
        properties.ShowDialog();
    }

    private static void InitialiseExamSettings(Exam exam)
    {
        var settings = new ExamSettingsUi(exam);
        settings.ShowDialog();
    }
}
