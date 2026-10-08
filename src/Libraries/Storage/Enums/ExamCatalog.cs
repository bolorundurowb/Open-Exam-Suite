namespace OpenExamSuite.Storage.Enums;

/// <summary>
/// Which application owns an exam-list entry. Creator and Simulator keep separate lists
/// in the same database. Shipped samples are added to both.
/// </summary>
public enum ExamCatalog
{
    Creator = 1,
    Simulator = 2
}
