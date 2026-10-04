using LiteDB;

namespace OpenExamSuite.Storage.Models;

/// <summary>
/// A persisted key/value setting. Exam history entries use the file path as the
/// <see cref="Key"/> and the display name as the <see cref="Value"/>; preferences
/// use an arbitrary key/value pair.
/// </summary>
public class AppSetting
{
    public int Id { get; set; }

    // The BSON field names are kept as the legacy "FilePath"/"Name" so that exam
    // history stored by earlier versions remains readable.
    [BsonField("FilePath")]
    public string Key { get; set; } = string.Empty;

    [BsonField("Name")]
    public string Value { get; set; } = string.Empty;
}
