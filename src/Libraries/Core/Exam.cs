using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace OpenExamSuite.Shared;

/// <summary>
/// Matches Newtonsoft.Json: <c>char[]</c> is serialized as a JSON string, not a JSON array.
/// </summary>
public sealed class CharArrayAsStringConverter : JsonConverter<char[]>
{
    public override char[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.String:
                var s = reader.GetString();
                return string.IsNullOrEmpty(s) ? [] : s.ToCharArray();
            case JsonTokenType.StartArray:
                var list = new List<char>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    if (reader.TokenType == JsonTokenType.String)
                    {
                        var cs = reader.GetString();
                        if (!string.IsNullOrEmpty(cs))
                            list.Add(cs[0]);
                    }
                    else if (reader.TokenType == JsonTokenType.Number)
                        list.Add((char)reader.GetInt32());
                }

                return list.ToArray();
            default:
                throw new JsonException($"Unexpected token parsing char[]: {reader.TokenType}.");
        }
    }

    public override void Write(Utf8JsonWriter writer, char[]? value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(new string(value));
    }
}

[Serializable]
[ProtoContract]
public class Exam
{
    [ProtoIgnore]
    public int NumberOfQuestions => Sections.Sum(s => s.Questions.Count);

    [ProtoMember(1)]
    public Properties Properties { get; set; } = new();

    [ProtoMember(2)]
    public List<Section> Sections { get; set; } = [];

    // Methods
    public void AddSection(string sectionName)
    {
        var section = Sections.FirstOrDefault(s => s.Title == sectionName);
        if (section == null)
        {
            section = new Section { Title = sectionName };
            Sections.Add(section);
        }
    }

    public void RemoveSection(string sectionName)
    {
        var section = Sections.FirstOrDefault(s => s.Title == sectionName);
        if (section != null)
            Sections.Remove(section);
    }

    public void AddQuestion(string sectionName, Question question)
    {
        var section = Sections.FirstOrDefault(s => s.Title == sectionName);
        if (section == null)
        {
            section = new Section
            {
                Title = sectionName
            };
            Sections.Add(section);
        }

        question.No = 1;
        section.Questions.Add(question);
    }

    public void RemoveQuestion(string sectionName, Question question)
    {
        var section = Sections.FirstOrDefault(s => s.Title == sectionName);
        section?.Questions.Remove(question);
    }
}

[Serializable]
[ProtoContract]
public class Properties
{
    [ProtoMember(1)]
    public string Title { get; set; } = string.Empty;

    [ProtoMember(2)]
    public string Code { get; set; } = string.Empty;

    [ProtoMember(3)]
    public int Version { get; set; }

    [ProtoMember(4)]
    public double Passmark { get; set; }

    [ProtoMember(5)]
    public int TimeLimit { get; set; }

    [ProtoMember(6)]
    public string Instructions { get; set; } = string.Empty;

    [ProtoMember(7)]
    public bool HideAnswers { get; set; }
}

[Serializable]
[ProtoContract]
public class Section
{
    [ProtoMember(1)]
    public string Title { get; set; } = string.Empty;

    [ProtoMember(2)]
    public List<Question> Questions { get; set; } = [];

    public override string ToString()
    {
        return Title;
    }
}

[Serializable]
[ProtoContract]
public class Question
{
    [ProtoMember(1)]
    public int No { get; set; }

    [ProtoMember(2)]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// PNG-encoded image bytes. Cross-platform replacement for the legacy
    /// <c>System.Drawing.Bitmap Image</c> property: each UI layer (WinForms,
    /// Avalonia) decodes this into its native bitmap type at display time.
    /// Serialised via protobuf, JSON (default base64), and XML (base64Binary).
    /// </summary>
    [ProtoMember(3)]
    [XmlElement(DataType = "base64Binary")]
    public byte[]? ImageData { get; set; }

    [ProtoMember(4)]
    public char Answer { get; set; }

    [ProtoMember(5)]
    public bool IsMultipleChoice { get; set; }

    [ProtoMember(6)]
    [JsonConverter(typeof(CharArrayAsStringConverter))]
    public char[] Answers { get; set; } = [];

    [ProtoMember(7)]
    public List<Option> Options { get; set; } = [];

    [ProtoMember(8)]
    public string Explanation { get; set; } = string.Empty;
}

[Serializable]
[ProtoContract]
public class Option
{
    [ProtoMember(1)]
    public char Alphabet { get; set; }

    [ProtoMember(2)]
    public string Text { get; set; } = string.Empty;
}
