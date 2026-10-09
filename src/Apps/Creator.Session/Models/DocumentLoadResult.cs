using OpenExamSuite.Shared.Enums;

namespace OpenExamSuite.Creator.Session.Models;

public sealed class DocumentLoadResult
{
    public bool Success { get; }

    public ExamIoError Error { get; }

    public bool IsLegacy { get; }

    public string? FilePath { get; }

    private DocumentLoadResult(bool success, ExamIoError error, bool isLegacy, string? filePath)
    {
        Success = success;
        Error = error;
        IsLegacy = isLegacy;
        FilePath = filePath;
    }

    public static DocumentLoadResult Ok(string filePath, bool isLegacy) =>
        new(true, ExamIoError.None, isLegacy, filePath);

    public static DocumentLoadResult Fail(ExamIoError error, string? filePath = null) =>
        new(false, error, false, filePath);
}

public sealed class DocumentSaveResult
{
    public bool Success { get; }

    public ExamIoError Error { get; }

    public string? Detail { get; }

    public string? FilePath { get; }

    private DocumentSaveResult(bool success, ExamIoError error, string? detail, string? filePath)
    {
        Success = success;
        Error = error;
        Detail = detail;
        FilePath = filePath;
    }

    public static DocumentSaveResult Ok(string filePath) =>
        new(true, ExamIoError.None, null, filePath);

    public static DocumentSaveResult Fail(ExamIoError error, string? detail = null) =>
        new(false, error, detail, null);
}
