using System.Formats.Nrbf;
using System.Text.Json;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenExamSuite.Shared.Enums;
using ProtoBuf;

namespace OpenExamSuite.Shared.Utilities;

/// <summary>
/// Reads exams from .oef (protobuf or legacy NRBF), JSON and XML. Read operations never
/// write to the source path: a legacy NRBF payload is decoded and returned with an
/// <see cref="ExamReadResult.IsLegacy"/> flag, and upgrading is an explicit save by the caller.
/// Failures are surfaced as a typed <see cref="ExamReadResult"/> and routed through
/// <see cref="ILogger"/>.
/// </summary>
public sealed class Reader
{
    private readonly ILogger<Reader> _logger;

    public Reader(ILogger<Reader>? logger = null)
    {
        _logger = logger ?? NullLogger<Reader>.Instance;
    }

    public ExamReadResult FromOefFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Empty filepath", nameof(filePath));

        if (!File.Exists(filePath))
            return new ExamReadResult(null, false, ExamIoError.FileNotFound);

        try
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return FromOef(stream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open .oef file '{FilePath}'.", filePath);
            return new ExamReadResult(null, false, ExamIoError.UnsupportedOrCorrupt);
        }
    }

    /// <summary>
    /// Reads an .oef exam from a stream without mutating it. The caller's stream is never
    /// disposed or written to, and no upgrade side effects occur.
    /// </summary>
    public ExamReadResult FromOef(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var buffered = stream.CanSeek ? stream : Buffer(stream);
        var startPosition = buffered.Position;

        try
        {
            if (NrbfDecoder.StartsWithPayloadHeader(buffered))
            {
                buffered.Position = startPosition;

                var record = NrbfDecoder.Decode(buffered);
                var exam = record is ClassRecord classRecord ? MapFromNrbf(classRecord) : null;
                if (exam != null)
                    return new ExamReadResult(exam, true, ExamIoError.None, IsLegacy: true);

                return new ExamReadResult(null, false, ExamIoError.UnsupportedOrCorrupt, IsLegacy: true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to decode legacy NRBF .oef payload.");
            return new ExamReadResult(null, false, ExamIoError.UnsupportedOrCorrupt, IsLegacy: true);
        }

        buffered.Position = startPosition;

        try
        {
            var exam = Serializer.Deserialize<Exam>(buffered);
            return new ExamReadResult(exam, true, ExamIoError.None, IsLegacy: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize .oef file in protobuf format.");
            return new ExamReadResult(null, false, ExamIoError.UnsupportedOrCorrupt, IsLegacy: false);
        }
    }

    private static Stream Buffer(Stream stream)
    {
        var memory = new MemoryStream();
        stream.CopyTo(memory);
        memory.Position = 0;
        return memory;
    }

    public ExamReadResult FromJsonFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Empty filepath", nameof(filePath));

        if (!File.Exists(filePath))
            return new ExamReadResult(null, false, ExamIoError.FileNotFound);

        try
        {
            var jsonString = File.ReadAllText(filePath);
            return FromJsonString(jsonString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read JSON exam file '{FilePath}'.", filePath);
            return new ExamReadResult(null, false, ExamIoError.UnsupportedOrCorrupt);
        }
    }

    public ExamReadResult FromJson(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        try
        {
            using var reader = new StreamReader(stream, leaveOpen: true);
            return FromJsonString(reader.ReadToEnd());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read JSON exam from stream.");
            return new ExamReadResult(null, false, ExamIoError.UnsupportedOrCorrupt);
        }
    }

    public ExamReadResult FromXmlFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Empty filepath", nameof(filePath));

        if (!File.Exists(filePath))
            return new ExamReadResult(null, false, ExamIoError.FileNotFound);

        try
        {
            using var stream = File.OpenRead(filePath);
            return FromXml(stream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read XML exam file '{FilePath}'.", filePath);
            return new ExamReadResult(null, false, ExamIoError.UnsupportedOrCorrupt);
        }
    }

    public ExamReadResult FromXml(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        try
        {
            using var reader = new StreamReader(stream, leaveOpen: true);
            var xmlSerializer = new XmlSerializer(typeof(Exam));
            var exam = xmlSerializer.Deserialize(reader) as Exam;
            return new ExamReadResult(exam, exam != null, exam != null ? ExamIoError.None : ExamIoError.UnsupportedOrCorrupt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize XML exam.");
            return new ExamReadResult(null, false, ExamIoError.UnsupportedOrCorrupt);
        }
    }

    private ExamReadResult FromJsonString(string jsonString)
    {
        try
        {
            var exam = JsonSerializer.Deserialize<Exam>(jsonString, ExamJsonSerialization.Options);
            return new ExamReadResult(exam, exam != null, exam != null ? ExamIoError.None : ExamIoError.UnsupportedOrCorrupt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize JSON exam.");
            return new ExamReadResult(null, false, ExamIoError.UnsupportedOrCorrupt);
        }
    }

    private Exam? MapFromNrbf(ClassRecord classRecord)
    {
        if (classRecord.TypeName.Name != "Exam")
            return null;

        try
        {
            var exam = new Exam();

            if (classRecord.HasMember(FormatMemberName("Properties"))
                && classRecord.GetClassRecord(FormatMemberName("Properties")) is ClassRecord propsRecord)
            {
                exam.Properties.Title = GetString(propsRecord, "Title");
                exam.Properties.Code = GetString(propsRecord, "Code");
                exam.Properties.Version = GetInt32(propsRecord, "Version");
                exam.Properties.Passmark = GetDouble(propsRecord, "Passmark");
                exam.Properties.TimeLimit = GetInt32(propsRecord, "TimeLimit");
                exam.Properties.Instructions = GetString(propsRecord, "Instructions");
                exam.Properties.HideAnswers = GetBoolean(propsRecord, "HideAnswers");
            }

            if (classRecord.HasMember(FormatMemberName("Sections"))
                && classRecord.GetClassRecord(FormatMemberName("Sections")) is ClassRecord sectionsListRecord
                && sectionsListRecord.GetArrayRecord("_items") is SZArrayRecord<SerializationRecord> sectionsArray)
            {
                foreach (var sectionItem in sectionsArray.GetArray())
                {
                    if (sectionItem is not ClassRecord sectionRecord)
                        continue;

                    var section = new Section
                    {
                        Title = GetString(sectionRecord, "Title")
                    };

                    if (sectionRecord.HasMember(FormatMemberName("Questions"))
                        && sectionRecord.GetClassRecord(FormatMemberName("Questions")) is ClassRecord
                            questionsListRecord
                        && questionsListRecord.GetArrayRecord("_items") is SZArrayRecord<SerializationRecord>
                            questionsArray)
                    {
                        foreach (var questionItem in questionsArray.GetArray())
                        {
                            if (questionItem is not ClassRecord questionRecord)
                                continue;

                            var question = new Question
                            {
                                No = GetInt32(questionRecord, "No"),
                                Text = GetString(questionRecord, "Text"),
                                Answer = GetChar(questionRecord, "Answer"),
                                IsMultipleChoice = GetBoolean(questionRecord, "IsMultipleChoice"),
                                Explanation = GetString(questionRecord, "Explanation"),
                                ImageData = MapImageData(questionRecord)
                            };

                            if (questionRecord.HasMember(FormatMemberName("Answers"))
                                && questionRecord.GetArrayRecord(FormatMemberName("Answers")) is ArrayRecord
                                    answersRecord)
                            {
                                if (answersRecord is SZArrayRecord<char> charArray)
                                {
                                    question.Answers = charArray.GetArray() ?? [];
                                }
                                else
                                {
                                    var answersList = new List<char>();
                                    foreach (var ans in answersRecord.GetArray(typeof(object)))
                                    {
                                        if (ans is char c)
                                            answersList.Add(c);
                                        else if (ans is int i)
                                            answersList.Add((char)i);
                                    }

                                    question.Answers = answersList.ToArray();
                                }
                            }

                            if (questionRecord.HasMember(FormatMemberName("Options"))
                                && questionRecord.GetClassRecord(FormatMemberName("Options")) is ClassRecord
                                    optionsListRecord
                                && optionsListRecord.GetArrayRecord("_items") is SZArrayRecord<SerializationRecord>
                                    optionsArray)
                            {
                                foreach (var optItem in optionsArray.GetArray())
                                {
                                    if (optItem is not ClassRecord optRecord)
                                        continue;

                                    question.Options.Add(new Option
                                    {
                                        Alphabet = GetChar(optRecord, "Alphabet"),
                                        Text = GetString(optRecord, "Text")
                                    });
                                }
                            }

                            section.Questions.Add(question);
                        }
                    }

                    exam.Sections.Add(section);
                }
            }

            return exam;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to map legacy NRBF payload to an Exam.");
            return null;
        }
    }

    /// <summary>
    /// Maps the legacy question image member, when present.
    ///
    /// Inventory finding (2026-10-08): legacy NRBF payloads store question images as a
    /// <c>System.Drawing.Bitmap</c> in the <c>&lt;Image&gt;k__BackingField</c> member. The bundled
    /// legacy sample (<c>Basic Science.oef</c>) carries a null image on every question, and
    /// <see cref="System.Formats.Nrbf"/> cannot reconstruct a <c>System.Drawing.Bitmap</c> back into
    /// usable image bytes. We therefore return null and log a warning if a non-null image is ever
    /// encountered, so image data is never silently dropped by the mapper.
    /// </summary>
    private byte[]? MapImageData(ClassRecord questionRecord)
    {
        var imageMember = FormatMemberName("Image");

        if (!questionRecord.HasMember(imageMember))
            return null;

        var raw = questionRecord.GetRawValue(imageMember);
        if (raw == null)
            return null;

        _logger.LogWarning(
            "Legacy question image member '{ImageMember}' could not be mapped from NRBF and will be omitted.",
            imageMember);

        return null;
    }

    private string GetString(ClassRecord record, string member) =>
        record.HasMember(FormatMemberName(member))
            ? record.GetString(FormatMemberName(member)) ?? string.Empty
            : string.Empty;

    private int GetInt32(ClassRecord record, string member) =>
        record.HasMember(FormatMemberName(member))
            ? record.GetInt32(FormatMemberName(member))
            : 0;

    private double GetDouble(ClassRecord record, string member) =>
        record.HasMember(FormatMemberName(member))
            ? record.GetDouble(FormatMemberName(member))
            : 0d;

    private bool GetBoolean(ClassRecord record, string member) =>
        record.HasMember(FormatMemberName(member)) && record.GetBoolean(FormatMemberName(member));

    private char GetChar(ClassRecord record, string member) =>
        record.HasMember(FormatMemberName(member))
            ? record.GetChar(FormatMemberName(member))
            : '\0';

    private static string FormatMemberName(string memberName) => $"<{memberName}>k__BackingField";
}
