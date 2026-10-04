using OpenExamSuite.Shared.Enums;

namespace OpenExamSuite.Shared.Models;

public class ChangeRepresentationObject
{
    public ActionType Action { get; set; }
    public string SectionTitle { get; set; } = string.Empty;
    public string? PreviousSectionTitle { get; set; }
    public Question? Question { get; set; }
    public Question? PreviousQuestion { get; set; }
}
