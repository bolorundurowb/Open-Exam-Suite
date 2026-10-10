namespace OpenExamSuite.Simulator.Engine.Models;

/// <summary>
/// Outcome of a Library action. The host maps the status to a localised message.
/// </summary>
public enum LibraryActionStatus
{
    Success,
    FileNotFound,
    UnsupportedFormat,
    CorruptFile,
    AlreadyInLibrary,
    WriteFailed
}

public sealed record LibraryActionResult(LibraryActionStatus Status, string? FilePath = null)
{
    public bool Success => Status == LibraryActionStatus.Success;
}

/// <summary>
/// File-level facts for the Properties summary: path, size, version and counts.
/// </summary>
public sealed record ExamFileProperties(
    string FilePath,
    string Title,
    string Code,
    int Version,
    long FileSizeBytes,
    DateTime ModifiedAt,
    int QuestionCount,
    int SectionCount,
    int TimeLimitMinutes,
    double PassMarkPercent,
    bool HideAnswers,
    bool IsLegacy);
