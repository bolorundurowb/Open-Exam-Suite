using System.Collections.Immutable;

namespace OpenExamSuite.Simulator.Session.Models;

/// <summary>
/// Represents a candidate's answer selection for a single question.
/// This replaces the untyped object[] used in the legacy UI.
/// Progress count only advances when State is Answered.
/// </summary>
public readonly record struct AnswerSelection
{
    public AnswerState State { get; init; }

    /// <summary>
    /// For single-choice: the selected option letter. Valid only when State == Answered.
    /// </summary>
    public char? SingleChoice { get; init; }

    /// <summary>
    /// For multiple-choice: the selected option letters. Valid only when State == Answered.
    /// </summary>
    public ImmutableArray<char> MultipleChoices { get; init; }

    /// <summary>
    /// Whether the explanation has been revealed (Practice mode Check Answer).
    /// </summary>
    public bool ExplanationRevealed { get; init; }

    /// <summary>
    /// Whether the selected choices are locked after checking a Practice answer.
    /// Flagging remains available while the choices are locked.
    /// </summary>
    public bool IsLocked { get; init; }

    /// <summary>
    /// Whether this question is flagged for review.
    /// </summary>
    public bool IsFlagged { get; init; }

    private AnswerSelection(AnswerState state, char? singleChoice = null, ImmutableArray<char> multipleChoices = default, bool explanationRevealed = false, bool isLocked = false, bool isFlagged = false)
    {
        State = state;
        SingleChoice = singleChoice;
        MultipleChoices = multipleChoices.IsDefault ? ImmutableArray<char>.Empty : multipleChoices;
        ExplanationRevealed = explanationRevealed;
        IsLocked = isLocked;
        IsFlagged = isFlagged;
    }

    /// <summary>
    /// Creates an unanswered selection.
    /// </summary>
    public static AnswerSelection Unanswered(bool isFlagged = false) =>
        new(AnswerState.Unanswered, isFlagged: isFlagged);

    /// <summary>
    /// Creates a single-choice answered selection.
    /// </summary>
    public static AnswerSelection Answered(char choice, bool isFlagged = false) =>
        new(AnswerState.Answered, singleChoice: choice, isFlagged: isFlagged);

    /// <summary>
    /// Creates a multiple-choice answered selection.
    /// </summary>
    public static AnswerSelection Answered(ImmutableArray<char> choices, bool isFlagged = false) =>
        new(AnswerState.Answered, multipleChoices: choices, isFlagged: isFlagged);

    /// <summary>
    /// Creates a selection with explanation revealed (Practice mode after Check Answer).
    /// </summary>
    public static AnswerSelection WithExplanationRevealed(AnswerSelection selection) =>
        selection with { ExplanationRevealed = true, IsLocked = true };

    /// <summary>
    /// Creates a selection toggled flagged state.
    /// </summary>
    public static AnswerSelection WithFlagToggled(AnswerSelection selection) =>
        selection with { IsFlagged = !selection.IsFlagged };

    /// <summary>
    /// Returns true if the question has been answered (progress should count it).
    /// </summary>
    public readonly bool IsAnswered => State == AnswerState.Answered;

    /// <summary>
    /// Returns true if the question is unanswered.
    /// </summary>
    public readonly bool IsUnanswered => State == AnswerState.Unanswered;

    /// <summary>
    /// Gets the selected choices as an array for grading compatibility.
    /// </summary>
    public readonly char[] ToCharArray()
    {
        if (!IsAnswered)
            return Array.Empty<char>();

        if (SingleChoice.HasValue)
            return [SingleChoice.Value];

        return MultipleChoices.ToArray();
    }

    /// <summary>
    /// Gets the single selected choice for single-answer questions.
    /// </summary>
    public readonly char GetSingleChoice() => SingleChoice ?? '\0';

    /// <summary>
    /// Creates an AnswerSelection from a char array (for migration/compatibility).
    /// </summary>
    public static AnswerSelection FromCharArray(char[]? array, bool isFlagged = false)
    {
        if (array == null || array.Length == 0)
            return Unanswered(isFlagged);

        if (array.Length == 1)
            return Answered(array[0], isFlagged);

        return Answered(array.ToImmutableArray(), isFlagged);
    }
}

public enum AnswerState
{
    /// <summary>
    /// The candidate has not provided an answer.
    /// </summary>
    Unanswered,

    /// <summary>
    /// The candidate has provided an answer.
    /// </summary>
    Answered
}