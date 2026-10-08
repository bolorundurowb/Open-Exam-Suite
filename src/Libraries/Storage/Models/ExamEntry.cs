using LiteDB;
using OpenExamSuite.Storage.Enums;

namespace OpenExamSuite.Storage.Models;

/// <summary>
/// An exam in one application's list. Creator and Simulator store separate entries,
/// even when a shipped sample is present in both.
/// </summary>
public class ExamEntry
{
    public int Id { get; set; }

    [BsonField("Catalog")]
    public ExamCatalog Catalog { get; set; }

    [BsonField("FilePath")]
    public string FilePath { get; set; } = string.Empty;

    [BsonField("Name")]
    public string Name { get; set; } = string.Empty;

    [BsonField("IsSample")]
    public bool IsSample { get; set; }
}
