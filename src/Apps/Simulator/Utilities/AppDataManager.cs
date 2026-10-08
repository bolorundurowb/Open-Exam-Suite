using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Interfaces;

namespace OpenExamSuite.Simulator.Utilities;

public static class AppDataManager
{
    public static void LoadAppData(DataGridView dataGridView, IExamLibraryService library)
    {
        library.SeedBundledSamples(ExamCatalog.Simulator, Application.StartupPath);

        foreach (var exam in library.GetExams(ExamCatalog.Simulator))
        {
            dataGridView.Rows.Add(exam.Name, exam.FilePath);
        }
    }
}
