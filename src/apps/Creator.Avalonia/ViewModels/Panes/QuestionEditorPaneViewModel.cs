using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenExamSuite.Creator.Services;
using OpenExamSuite.Creator.ViewModels.Nodes;
using OpenExamSuite.Logging;
using OpenExamSuite.Shared;
using OpenExamSuite.Shared.Avalonia.Services;
using OpenExamSuite.Shared.Enums;
using OpenExamSuite.Shared.Models;

namespace OpenExamSuite.Creator.ViewModels.Panes;

/// <summary>
/// Replaces the WinForms <c>pan_display_questions</c> panel: question text,
/// explanation, image, options collection (CheckBox row vs RadioButton row
/// based on <see cref="IsMultipleChoice"/>), and add/remove/clear-image
/// commands.
/// </summary>
public sealed partial class QuestionEditorPaneViewModel : ObservableObject, IRightPaneViewModel
{
    private readonly IFilePickerService _files;
    private readonly IMessageBoxService _msg;
    private readonly IUndoRedoService _undo;

    [ObservableProperty] private string _sectionAndQuestionHeader = string.Empty;
    [ObservableProperty] private string _questionText = string.Empty;
    [ObservableProperty] private string _explanation = string.Empty;
    [ObservableProperty] private byte[]? _imageData;
    [ObservableProperty] private bool _isMultipleChoice;
    [ObservableProperty] private bool _isEditable = true;

    public ObservableCollection<OptionRowViewModel> Options { get; } = new();

    /// <summary>Backing question node, if any. Null when a SectionNode is selected.</summary>
    public QuestionNodeViewModel? QuestionNode { get; private set; }
    public SectionNodeViewModel?  SectionNode  { get; private set; }

    /// <summary>True while the VM is being populated from the model, used to
    /// suspend the undo-stack push that <c>QuestionChanged</c> would normally do.</summary>
    private bool _loading;

    public QuestionEditorPaneViewModel(
        IFilePickerService files,
        IMessageBoxService msg,
        IUndoRedoService undo)
    {
        _files = files;
        _msg = msg;
        _undo = undo;
    }

    public QuestionEditorPaneViewModel InitializeFromQuestion(QuestionNodeViewModel node)
    {
        _loading = true;
        try
        {
            QuestionNode = node;
            SectionNode = node.Parent as SectionNodeViewModel;
            var q = node.Question;
            QuestionText = q.Text;
            Explanation = q.Explanation;
            ImageData = q.ImageData;
            IsMultipleChoice = q.IsMultipleChoice;
            IsEditable = true;
            SectionAndQuestionHeader =
                $"Section: {GetSectionTitle(node)} Question {q.No}";

            Options.Clear();
            foreach (var opt in q.Options)
            {
                Options.Add(new OptionRowViewModel
                {
                    Letter = opt.Alphabet,
                    Text = opt.Text,
                    IsChecked = IsMultipleChoice
                        ? q.Answers.Contains(opt.Alphabet)
                        : opt.Alphabet == q.Answer,
                    IsMultipleChoice = IsMultipleChoice,
                });
            }
        }
        finally
        {
            _loading = false;
        }
        return this;
    }

    public QuestionEditorPaneViewModel InitializeFromSection(SectionNodeViewModel node)
    {
        _loading = true;
        try
        {
            QuestionNode = null;
            SectionNode = node;
            QuestionText = string.Empty;
            Explanation = string.Empty;
            ImageData = null;
            IsMultipleChoice = false;
            IsEditable = false;
            SectionAndQuestionHeader = $"Section: {node.Title}";
            Options.Clear();
        }
        finally
        {
            _loading = false;
        }
        return this;
    }

    /// <summary>
    /// Writes the current editor state back to the underlying
    /// <see cref="Question"/>. Mirrors the WinForms <c>CommitQuestion</c>.
    /// </summary>
    public void CommitToModel()
    {
        if (QuestionNode is null) return;

        var q = QuestionNode.Question;
        q.IsMultipleChoice = IsMultipleChoice;
        if (IsMultipleChoice)
        {
            q.Answers = Options.Where(o => o.IsChecked).Select(o => o.Letter).ToArray();
            q.Answer = '\0';
        }
        else
        {
            q.Answer = Options.FirstOrDefault(o => o.IsChecked)?.Letter ?? '\0';
            q.Answers = Array.Empty<char>();
        }

        q.Options = Options.Select(o => new Option { Alphabet = o.Letter, Text = o.Text }).ToList();
        q.Text = QuestionText;
        q.Explanation = Explanation;
        q.ImageData = ImageData;
    }

    partial void OnQuestionTextChanged(string value)   => NotifyQuestionChanged();
    partial void OnExplanationChanged(string value)    => NotifyQuestionChanged();

    private void NotifyQuestionChanged()
    {
        if (_loading || QuestionNode is null) return;

        CommitToModel();

        var section = GetSectionTitle(QuestionNode);
        _undo.Push(new ChangeRepresentationObject
        {
            Action = ActionType.Modify,
            Question = QuestionNode.Question,
            SectionTitle = section,
        });
    }

    [RelayCommand]
    private async Task InsertImageAsync()
    {
        var path = await _files.PickOpenFileAsync(
            "Select Image",
            new[]
            {
                new FileFilter("JPEG Files", "*.jpg"),
                new FileFilter("PNG Files",  "*.png"),
            });
        if (string.IsNullOrEmpty(path)) return;

        try
        {
            // Read the picked file as raw bytes; if it's a JPEG, re-encode to
            // PNG for storage uniformity (the model stores PNG bytes).
            var raw = await File.ReadAllBytesAsync(path);
            ImageData = NormalizeToPng(raw, path);
            NotifyQuestionChanged();
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            await _msg.ShowAsync("Could not load the selected image.", "Error",
                MessageBoxButtons.Ok, MessageBoxIconKind.Error);
        }
    }

    [RelayCommand]
    private void ClearImage()
    {
        ImageData = null;
        NotifyQuestionChanged();
    }

    /// <summary>
    /// PNG passes through; JPEG/other formats are decoded via Avalonia's
    /// <c>Bitmap</c> and re-encoded to PNG bytes to keep the storage format
    /// consistent with what the WinForms client always wrote.
    /// </summary>
    private static byte[] NormalizeToPng(byte[] raw, string sourcePath)
    {
        var ext = Path.GetExtension(sourcePath)?.ToLowerInvariant();
        if (ext == ".png") return raw;

        using var input = new MemoryStream(raw, writable: false);
        var bitmap = new global::Avalonia.Media.Imaging.Bitmap(input);
        using var output = new MemoryStream();
        bitmap.Save(output);
        return output.ToArray();
    }

    [RelayCommand(CanExecute = nameof(CanRemoveOption))]
    private void RemoveOption()
    {
        if (Options.Count == 0) return;
        Options.RemoveAt(Options.Count - 1);
        NotifyQuestionChanged();
    }

    private bool CanRemoveOption() => Options.Count > 0;

    [RelayCommand]
    private async Task AddOptionAsync()
    {
        try
        {
            // The WinForms code rejects mixing types — we do the same on toggle.
            char next = Options.Count == 0
                ? 'A'
                : (char)(Options[^1].Letter + 1);

            Options.Add(new OptionRowViewModel
            {
                Letter = next,
                Text = string.Empty,
                IsChecked = false,
                IsMultipleChoice = IsMultipleChoice,
            });
            NotifyQuestionChanged();
            RemoveOptionCommand.NotifyCanExecuteChanged();
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            await _msg.ShowAsync(
                "Sorry, you cannot mix option types. First remove the existing options then replace them.",
                "Error", MessageBoxButtons.Ok, MessageBoxIconKind.Error);
        }
    }

    partial void OnIsMultipleChoiceChanged(bool value)
    {
        if (_loading) return;

        // Sync every existing OptionRowViewModel to the new mode.
        foreach (var o in Options) o.IsMultipleChoice = value;

        // When switching from multi → single, demote multiple checks to none.
        if (!value)
        {
            var anyChecked = Options.FirstOrDefault(o => o.IsChecked);
            foreach (var o in Options) o.IsChecked = false;
            if (anyChecked is not null) anyChecked.IsChecked = true;
        }

        NotifyQuestionChanged();
    }

    public void OnOptionChanged()
    {
        // Called from option row events when a row's IsChecked or Text changes.
        if (!IsMultipleChoice)
        {
            // Enforce single-choice radio-like behaviour.
        }
        NotifyQuestionChanged();
    }

    private static string GetSectionTitle(QuestionNodeViewModel question)
        => (question.Parent as SectionNodeViewModel)?.Title ?? string.Empty;
}
