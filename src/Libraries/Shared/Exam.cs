using Newtonsoft.Json;
using ProtoBuf;

namespace OpenExamSuite.Shared;

public class BitmapConverter : JsonConverter<Bitmap>
{
    public override void WriteJson(JsonWriter writer, Bitmap? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        using var ms = new MemoryStream();
        value.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        var bytes = ms.ToArray();
        writer.WriteValue(Convert.ToBase64String(bytes));
    }

    public override Bitmap? ReadJson(JsonReader reader, Type objectType, Bitmap? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        var base64 = (string?)reader.Value;
        if (string.IsNullOrEmpty(base64))
            return null;

        var bytes = Convert.FromBase64String(base64);
        using var ms = new MemoryStream(bytes);
        return new Bitmap(ms);
    }
}

[Serializable]
[ProtoContract]
public class Exam
{
    [ProtoIgnore]
    public int NumberOfQuestions
    {
        get
        {
            var numOfQuestions = 0;
            foreach (var section in this.Sections)
                numOfQuestions += section.Questions.Count;
            return numOfQuestions;
        }
    }

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

    [JsonConverter(typeof(BitmapConverter))]
    public Bitmap? Image { get; set; }

    [ProtoMember(3)]
    public byte[]? ImageBytes
    {
        get
        {
            if (Image == null) return null;
            using var ms = new MemoryStream();
            Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }
        set
        {
            if (value == null)
            {
                Image = null;
            }
            else
            {
                using var ms = new MemoryStream(value);
                Image = new Bitmap(ms);
            }
        }
    }

    [ProtoMember(4)]
    public char Answer { get; set; }

    [ProtoMember(5)]
    public bool IsMultipleChoice { get; set; }

    [ProtoMember(6)]
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