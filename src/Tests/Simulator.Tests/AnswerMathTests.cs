using System.Collections.Immutable;
using OmniAssert;
using OpenExamSuite.Shared;
using OpenExamSuite.Simulator.Models;
using OpenExamSuite.Simulator.Engine.Models;
using Xunit;

namespace OpenExamSuite.Simulator.Tests;

public class AnswerMathTests
{
    private static Question Single() => new() { Answer = 'B', IsMultipleChoice = false };

    private static Question Multiple() => new() { IsMultipleChoice = true, Answers = ['C', 'A'] };

    [Fact]
    public void SingleChoice_ReplacesTheSelection()
    {
        var selection = AnswerMath.Activate(Single(), AnswerSelection.Unanswered(), 'A');
        selection = AnswerMath.Activate(Single(), selection, 'B');

        string.Join(",", AnswerMath.SelectedLetters(selection)).Must().Be("B");
        AnswerMath.IsCorrect(Single(), selection).Must().BeTrue();
    }

    [Fact]
    public void MultipleChoice_TogglesAndKeepsFlag()
    {
        var selection = AnswerSelection.Unanswered(isFlagged: true);
        selection = AnswerMath.Activate(Multiple(), selection, 'C');
        selection = AnswerMath.Activate(Multiple(), selection, 'A');

        string.Join(",", AnswerMath.SelectedLetters(selection)).Must().Be("A,C");
        selection.IsFlagged.Must().BeTrue();
        AnswerMath.IsCorrect(Multiple(), selection).Must().BeTrue();
    }

    [Fact]
    public void MultipleChoice_ClearingEveryOptionLeavesTheQuestionUnanswered()
    {
        var selection = AnswerMath.Activate(Multiple(), AnswerSelection.Unanswered(), 'A');
        selection = AnswerMath.Activate(Multiple(), selection, 'A');

        selection.IsAnswered.Must().BeFalse();
        AnswerMath.IsCorrect(Multiple(), selection).Must().BeFalse();
    }

    [Fact]
    public void PartialMultipleSelection_IsNotCorrect()
    {
        var selection = AnswerSelection.Answered(ImmutableArray.Create('A'));

        AnswerMath.IsCorrect(Multiple(), selection).Must().BeFalse();
    }
}
