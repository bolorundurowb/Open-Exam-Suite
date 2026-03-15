using System.Formats.Nrbf;
using System.Xml.Serialization;
using Newtonsoft.Json;
using OpenExamSuite.Logging;
using ProtoBuf;

namespace OpenExamSuite.Shared.Utilities;

public static class Reader
{
    public static Exam? FromOefFile(string filePath, bool throwOnError = false)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Empty filepath");

        if (!File.Exists(filePath))
            throw new FileNotFoundException("File specified does not exist");

        Exam? exam = null;
        var isLegacy = false;

        try
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (NrbfDecoder.StartsWithPayloadHeader(stream))
                {
                    isLegacy = true;
                    // The BinaryFormatter is deprecated in .NET 10.
                    // We try to decode it via NRBF.
                    var result = (ClassRecord)NrbfDecoder.Decode(stream);
                    exam = MapFromNrbf(result);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
        }

        if (exam == null)
        {
            try
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                exam = Serializer.Deserialize<Exam>(stream);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                if (throwOnError)
                {
                    throw new Exception("Unsupported or corrupted .oef file format.", ex);
                }

                return null;
            }
        }

        if (isLegacy && exam != null)
        {
            Logger.Log("Migrated legacy .oef file to protobuf format.");
            Writer.ToOef(exam, filePath);
        }

        return exam;
    }

    public static Exam? FromJsonFile(string filePath)
    {
        try
        {
            var jsonString = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Exam>(jsonString);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            return null;
        }
    }

    public static Exam? FromXmlFile(string filePath)
    {
        try
        {
            var xmlSerializer = new XmlSerializer(typeof(Exam));
            using var streamReader = new StreamReader(filePath);
            return xmlSerializer.Deserialize(streamReader) as Exam;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            return null;
        }
    }

    private static Exam? MapFromNrbf(ClassRecord classRecord)
    {
        if (classRecord.TypeName.Name != "Exam")
            return null;

        try
        {
            var exam = new Exam();

            if (classRecord.HasMember(FormatMemberName("Properties"))
                && classRecord.GetClassRecord(FormatMemberName("Properties")) is ClassRecord propsRecord)
            {
                exam.Properties.Title = propsRecord.GetString(FormatMemberName("Title")) ?? string.Empty;
                exam.Properties.Code = propsRecord.GetString(FormatMemberName("Code")) ?? string.Empty;
                exam.Properties.Version = propsRecord.GetInt32(FormatMemberName("Version"));
                exam.Properties.Passmark = propsRecord.GetDouble(FormatMemberName("Passmark"));
                exam.Properties.TimeLimit = propsRecord.GetInt32(FormatMemberName("TimeLimit"));
                exam.Properties.Instructions = propsRecord.GetString(FormatMemberName("Instructions")) ?? string.Empty;
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
                        Title = sectionRecord.GetString(FormatMemberName("Title")) ?? string.Empty
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
                                No = questionRecord.GetInt32(FormatMemberName("No")),
                                Text = questionRecord.GetString(FormatMemberName("Text")) ?? string.Empty,
                                Answer = questionRecord.GetChar(FormatMemberName("Answer")),
                                IsMultipleChoice = questionRecord.GetBoolean(FormatMemberName("IsMultipleChoice")),
                                Explanation = questionRecord.GetString(FormatMemberName("Explanation")) ?? string.Empty
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
                                        Alphabet = optRecord.GetChar(FormatMemberName("Alphabet")),
                                        Text = optRecord.GetString(FormatMemberName("Text")) ?? string.Empty
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
            Logger.LogException(ex);
            return null;
        }
    }

    private static string FormatMemberName(string memberName) => $"<{memberName}>k__BackingField";
}