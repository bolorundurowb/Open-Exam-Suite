using OpenExamSuite.Shared.Enums;

namespace OpenExamSuite.Creator.Localization;

public static class IoMessages
{
    public static string Explain(ExamIoError error, string? detail)
    {
        var summary = error switch
        {
            ExamIoError.FileNotFound => Strings.Get("Error_FileNotFound"),
            ExamIoError.UnsupportedOrCorrupt => Strings.Get("Error_Unsupported"),
            _ => Strings.Get("Error_WriteFailed")
        };

        return string.IsNullOrWhiteSpace(detail) ? summary : $"{summary} {detail}";
    }
}
