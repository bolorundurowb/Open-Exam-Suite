using System;
using System.IO;
using System.Threading.Tasks;
using OpenExamSuite.Creator.ViewModels.Nodes;
using OpenExamSuite.Shared;
using OpenExamSuite.Shared.Avalonia.Services;
using OpenExamSuite.Shared.Utilities;

namespace OpenExamSuite.Creator.Services;

/// <summary>
/// Replaces the GDI+ <see cref="System.Drawing.Printing.PrintDocument"/> in
/// the WinForms <c>HomeUi.PrintPage</c>. We re-use <see cref="Writer.ToPdf"/>
/// (already used by File → Export → PDF) to render a single, scope-filtered
/// exam to a temp PDF, and open it in the OS default handler.
/// </summary>
public sealed class PdfPrintService : IPrintService
{
    private readonly IOpenUrlService _opener;

    public PdfPrintService(IOpenUrlService opener)
    {
        _opener = opener;
    }

    public Task PrintAsync(Exam exam, NodeViewModel? selected, PrintScope scope)
        => RenderAndOpenAsync(exam, selected, scope);

    public Task PreviewAsync(Exam exam, NodeViewModel? selected, PrintScope scope)
        => RenderAndOpenAsync(exam, selected, scope);

    private async Task RenderAndOpenAsync(Exam exam, NodeViewModel? selected, PrintScope scope)
    {
        // Build a scope-filtered exam, then write to a temp PDF.
        var filtered = BuildScopedExam(exam, selected, scope);
        var tempPath = Path.Combine(Path.GetTempPath(),
            $"OpenExamSuite_Print_{DateTime.Now:yyyyMMdd_HHmmssfff}.pdf");

        var ok = Writer.ToPdf(filtered, tempPath);
        if (!ok) return;

        await _opener.OpenFileAsync(tempPath);
    }

    private static Exam BuildScopedExam(Exam src, NodeViewModel? selected, PrintScope scope)
    {
        // Faithful to the WinForms PrintOption rules (see Creator HomeUi.PrintPage
        // and the PrintOptions dialog): the dialog disables radio buttons such
        // that exam-level selection only allows AllQuestions, section-level
        // selection allows AllQuestions/CurrentSection, and question-level
        // selection allows everything.
        if (scope == PrintScope.AllQuestions)
            return src;

        if (scope == PrintScope.CurrentSection && selected is SectionNodeViewModel sec)
        {
            var copy = new Exam { Properties = src.Properties };
            var section = new Section { Title = sec.Title };
            foreach (var n in sec.Children)
                if (n is QuestionNodeViewModel q)
                    section.Questions.Add(q.Question);
            copy.Sections.Add(section);
            return copy;
        }

        if (scope == PrintScope.CurrentSection && selected is QuestionNodeViewModel qNode)
        {
            // The WinForms code expects the section to be found via parent.
            // We re-locate the section in the source exam by title match.
            var match = FindParentSection(src, qNode.Question);
            if (match is not null)
            {
                var copy = new Exam { Properties = src.Properties };
                copy.Sections.Add(match);
                return copy;
            }
        }

        if (scope == PrintScope.CurrentQuestion && selected is QuestionNodeViewModel question)
        {
            var parent = FindParentSection(src, question.Question);
            var copy = new Exam { Properties = src.Properties };
            var section = new Section { Title = parent?.Title ?? string.Empty };
            section.Questions.Add(question.Question);
            copy.Sections.Add(section);
            return copy;
        }

        // Fallback — print everything.
        return src;
    }

    private static Section? FindParentSection(Exam exam, Question target)
    {
        foreach (var s in exam.Sections)
            foreach (var q in s.Questions)
                if (ReferenceEquals(q, target))
                    return s;
        return null;
    }
}
