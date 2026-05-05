namespace OpenExamSuite.Shared.Models;

public record SectionResult(string SectionTitle, int Total, int Correct);

public class Settings
{
    public string CandidateName { get; set; } = string.Empty;

    public List<Section> Sections { get; set; } = [];

    public List<Question> Questions { get; set; } = [];

    public int TimeLimit { get; set; }

    public TimeSpan ElapsedTime { get; set; }

    public int NumberOfCorrectAnswers { get; set; }

    public List<SectionResult> ResultSpread { get; set; } = [];
}