using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Interfaces;

namespace OpenExamSuite.Simulator.Utilities;

public static class RowManager
{
    public static void RemoveRow(DataGridView dataGridView, IExamLibraryService library)
    {
        foreach (DataGridViewRow row in dataGridView.SelectedRows)
        {
            dataGridView.Rows.Remove(row);
            var cellValue = row.Cells[1].Value?.ToString();

            if (cellValue != null)
                library.RemoveExam(ExamCatalog.Simulator, cellValue);
        }
    }
}
