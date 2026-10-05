namespace OpenExamSuite.Shared.Enums;

public enum ExamFileLoadError
{
    None,
    EmptyOrInvalidJson,
    EmptyOrInvalidXml,
    InvalidXml,
    UnknownOrCorrupt
}
