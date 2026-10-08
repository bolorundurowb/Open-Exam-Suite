namespace OpenExamSuite.Shared.Enums;

public enum ExamFileLoadError
{
    None,
    FileNotFound,
    EmptyOrInvalidJson,
    EmptyOrInvalidXml,
    InvalidXml,
    UnknownOrCorrupt
}
