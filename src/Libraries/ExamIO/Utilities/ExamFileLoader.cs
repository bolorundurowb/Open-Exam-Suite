using OpenExamSuite.Logging;
using OpenExamSuite.Shared.Enums;

namespace OpenExamSuite.Shared.Utilities;

/// <summary>
/// Loads an <see cref="Exam"/> from a file path based on extension, with user-facing error messages for Creator.
/// </summary>
public static class ExamFileLoader
{
    public static ExamFileLoadResult TryLoad(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Empty filepath", nameof(filePath));

        var fileExt = Path.GetExtension(filePath)?.ToLowerInvariant();

        if (fileExt == ".json")
        {
            var exam = Reader.FromJsonFile(filePath);
            if (exam == null || exam.NumberOfQuestions == 0)
            {
                return new ExamFileLoadResult(
                    null,
                    false,
                    ExamFileLoadError.EmptyOrInvalidJson,
                    null);
            }

            return new ExamFileLoadResult(exam, true, ExamFileLoadError.None, filePath);
        }

        if (fileExt == ".xml")
        {
            try
            {
                var exam = Reader.FromXmlFile(filePath);
                if (exam == null || exam.NumberOfQuestions == 0)
                {
                    return new ExamFileLoadResult(
                        null,
                        false,
                        ExamFileLoadError.EmptyOrInvalidXml,
                        null);
                }

                return new ExamFileLoadResult(exam, true, ExamFileLoadError.None, filePath);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return new ExamFileLoadResult(
                    null,
                    false,
                    ExamFileLoadError.InvalidXml,
                    null);
            }
        }

        var oefExam = Reader.FromOefFile(filePath);
        if (oefExam != null)
            return new ExamFileLoadResult(oefExam, true, ExamFileLoadError.None, filePath);

        return new ExamFileLoadResult(
            null,
            false,
            ExamFileLoadError.UnknownOrCorrupt,
            null);
    }
}

public sealed record ExamFileLoadResult(
    Exam? Exam,
    bool Success,
    ExamFileLoadError Error,
    string? PathForHistory);
