using OpenExamSuite.Creator.Engine.Models;
using OpenExamSuite.Shared.Utilities;
using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Interfaces;

namespace OpenExamSuite.Creator.Engine.Services;

/// <summary>
/// Builds the Creator's "recent exams" list for the start screen. Reads the shared exam library
/// (Creator catalog) and enriches each entry with question/section counts and the file's last
/// write time, ordered most-recently-edited first.
/// </summary>
public sealed class RecentExamsService
{
    private readonly IExamLibraryService _library;
    private readonly Reader _reader;

    public RecentExamsService(IExamLibraryService library, Reader reader)
    {
        _library = library;
        _reader = reader;
    }

    public List<RecentExam> GetRecents()
    {
        var entries = _library.GetExams(ExamCatalog.Creator);
        var recents = new List<RecentExam>(entries.Count);
        foreach (var entry in entries)
            recents.Add(BuildSafe(entry.FilePath, entry.Name));

        return recents.OrderByDescending(r => r.ModifiedAt).ToList();
    }

    private RecentExam BuildSafe(string filePath, string storedName)
    {
        try
        {
            return Build(filePath, storedName);
        }
        catch
        {
            // A single unreadable entry must never break the whole start screen.
            var title = string.IsNullOrWhiteSpace(storedName)
                ? Path.GetFileNameWithoutExtension(filePath)
                : storedName;
            return new RecentExam(filePath, title, 0, 0, DateTime.MinValue, false);
        }
    }

    private RecentExam Build(string filePath, string storedName)
    {
        var title = !string.IsNullOrWhiteSpace(storedName)
            ? storedName
            : Path.GetFileNameWithoutExtension(filePath);

        var exists = File.Exists(filePath);
        var modifiedAt = exists ? File.GetLastWriteTime(filePath) : DateTime.MinValue;
        var (questionCount, sectionCount, readable) = ReadCounts(filePath, exists);

        return new RecentExam(filePath, title, questionCount, sectionCount, modifiedAt, readable);
    }

    private (int Questions, int Sections, bool Readable) ReadCounts(string filePath, bool exists)
    {
        if (!exists)
            return (0, 0, false);

        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        var result = extension switch
        {
            ".json" => _reader.FromJsonFile(filePath),
            ".xml" => _reader.FromXmlFile(filePath),
            _ => _reader.FromOefFile(filePath)
        };

        if (!result.Success || result.Exam == null)
            return (0, 0, false);

        return (result.Exam.NumberOfQuestions, result.Exam.Sections.Count, true);
    }
}
