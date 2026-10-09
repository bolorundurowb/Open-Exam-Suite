namespace OpenExamSuite.Creator.Session.Models;

/// <summary>
/// A recently used exam shown on the Creator start screen. The display title is the name stored
/// in the library, falling back to the file name. Question and section counts are read from the
/// file; <see cref="IsReadable"/> is false when the file is missing or could not be parsed.
/// </summary>
public sealed record RecentExam(
    string FilePath,
    string Title,
    int QuestionCount,
    int SectionCount,
    DateTime ModifiedAt,
    bool IsReadable);
