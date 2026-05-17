using System.Threading.Tasks;
using OpenExamSuite.Creator.ViewModels.Nodes;
using OpenExamSuite.Shared;

namespace OpenExamSuite.Creator.Services;

public enum PrintScope
{
    CurrentQuestion,
    CurrentSection,
    AllQuestions,
}

public interface IPrintService
{
    /// <summary>
    /// Renders the requested scope to a temporary PDF and opens it in the OS
    /// default handler — replacing the WinForms <c>PrintDocument</c> +
    /// <c>PrintDialog</c> chain.
    /// </summary>
    Task PrintAsync(Exam exam, NodeViewModel? selected, PrintScope scope);

    /// <summary>Same as <see cref="PrintAsync"/> but for preview.</summary>
    Task PreviewAsync(Exam exam, NodeViewModel? selected, PrintScope scope);
}
