using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Models;

namespace OpenExamSuite.Storage.Interfaces;

/// <summary>
/// One database shared by Creator and Simulator. Each application has its own exam list.
/// Attempt history is shared. Shipped samples are seeded into both lists.
/// </summary>
public interface IExamLibraryService
{
    void AddExam(ExamCatalog catalog, string filePath, string name);

    void RemoveExam(ExamCatalog catalog, string filePath);

    /// <summary>
    /// Removes exams the application has worked on. Shipped samples stay in the list.
    /// </summary>
    void ClearExams(ExamCatalog catalog);

    List<ExamEntry> GetExams(ExamCatalog catalog);

    /// <summary>
    /// Adds the bundled samples for <paramref name="catalog"/> once, when the sample
    /// files can be found beside <paramref name="applicationDirectory"/>.
    /// </summary>
    void SeedBundledSamples(ExamCatalog catalog, string applicationDirectory);

    void SaveAttempt(ExamAttempt attempt);

    List<ExamAttempt> GetAttempts(string examFilePath);

    void ClearAttempts();
}
