using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenExamSuite.Shared.Enums;

namespace OpenExamSuite.Shared.Utilities;

/// <summary>
/// Loads an <see cref="Exam"/> from a file path based on extension, with user-facing error messages for Creator.
/// </summary>
public sealed class ExamFileLoader
{
    private readonly Reader _reader;
    private readonly ILogger<ExamFileLoader> _logger;

    public ExamFileLoader(Reader? reader = null, ILogger<ExamFileLoader>? logger = null)
    {
        _reader = reader ?? new Reader();
        _logger = logger ?? NullLogger<ExamFileLoader>.Instance;
    }

    public ExamFileLoadResult TryLoad(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Empty filepath", nameof(filePath));

        var fileExt = Path.GetExtension(filePath)?.ToLowerInvariant();

        if (fileExt == ".json")
        {
            var jsonResult = _reader.FromJsonFile(filePath);
            if (!jsonResult.Success || jsonResult.Exam == null || jsonResult.Exam.NumberOfQuestions == 0)
            {
                return new ExamFileLoadResult(
                    null,
                    false,
                    ExamFileLoadError.EmptyOrInvalidJson,
                    null);
            }

            return new ExamFileLoadResult(jsonResult.Exam, true, ExamFileLoadError.None, filePath);
        }

        if (fileExt == ".xml")
        {
            var xmlResult = _reader.FromXmlFile(filePath);
            if (!xmlResult.Success || xmlResult.Exam == null || xmlResult.Exam.NumberOfQuestions == 0)
            {
                return new ExamFileLoadResult(
                    null,
                    false,
                    ExamFileLoadError.EmptyOrInvalidXml,
                    null);
            }

            return new ExamFileLoadResult(xmlResult.Exam, true, ExamFileLoadError.None, filePath);
        }

        var oefResult = _reader.FromOefFile(filePath);
        if (oefResult.Success && oefResult.Exam != null)
            return new ExamFileLoadResult(oefResult.Exam, true, ExamFileLoadError.None, filePath);

        return new ExamFileLoadResult(
            null,
            false,
            oefResult.Error == ExamIoError.FileNotFound
                ? ExamFileLoadError.FileNotFound
                : ExamFileLoadError.UnknownOrCorrupt,
            null);
    }
}

public sealed record ExamFileLoadResult(
    Exam? Exam,
    bool Success,
    ExamFileLoadError Error,
    string? PathForHistory);
