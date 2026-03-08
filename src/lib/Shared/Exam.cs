using Newtonsoft.Json;

namespace Shared;

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

    public override Bitmap? ReadJson(JsonReader reader, Type objectType, Bitmap? existingValue, bool hasExistingValue, JsonSerializer serializer)
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
public class Exam
{
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

    public Properties Properties { get; set; }

    public List<Section> Sections { get; set; }

    public Exam()
    {
        Sections = [];
        Properties = new Properties();
    }

    // Methods
    public void AddSection(string sectionName)
    {
        var section = Sections.FirstOrDefault(s => s.Title == sectionName);
        if (section == null)
        {
            section = new Section {Title = sectionName};
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
            section = new Section();
            section.Title = sectionName;
            Sections.Add(section);
            question.No = 1;
            section.Questions.Add(question);
        }
        else
        {
            question.No = 1;
            section.Questions.Add(question);
        }
    }

    public void RemoveQuestion(string sectionName, Question question)
    {
        var section = Sections.FirstOrDefault(s => s.Title == sectionName);
        section?.Questions.Remove(question);
    }
}

[Serializable]
public class Properties
{
    public string Title { get; set; }

    public string Code { get; set; }

    public int Version { get; set; }

    public double Passmark { get; set; }

    public int TimeLimit { get; set; }

    public string Instructions { get; set; }
}

[Serializable]
public class Section
{
    public string Title { get; set; }

    public List<Question> Questions { get; set; }

    public Section()
    {
        Questions = [];
    }

    public override string ToString()
    {
        return Title;
    }
}

[Serializable]
public class Question
{
    public int No { get; set; }

    public string Text { get; set; }
        
    [JsonConverter(typeof(BitmapConverter))]
    public Bitmap? Image { get; set; }

    public char Answer { get; set; }

    public bool IsMultipleChoice { get; set; }

    public char[] Answers { get; set; }

    public List<Option> Options { get; set; }

    public string Explanation { get; set; }

    public Question()
    {
        Options = [];
    }
}

[Serializable]
public class Option
{
    public char Alphabet { get; set; }

    public string Text { get; set; }
}