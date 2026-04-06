using System.Text.Json;

namespace OpenExamSuite.Shared;

internal static class ExamJsonSerialization
{
    public static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
    };
}
