using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenExamSuite.Creator.Services;
using OpenExamSuite.Shared;

namespace OpenExamSuite.Creator.ViewModels;

public sealed partial class QuestionEditorViewModel : ViewModelBase
{
    private readonly ShellServices _shell;
    private readonly string _questionId;

    public QuestionEditorViewModel(ShellServices shell, string questionId)
    {
        _shell = shell;
        _questionId = questionId;
        Reload();
    }

    [ObservableProperty] private string _text = string.Empty;
    [ObservableProperty] private string _explanation = string.Empty;
    [ObservableProperty] private bool _isMultipleChoice;
    [ObservableProperty] private Bitmap? _image;
    [ObservableProperty] private bool _hasImage;

    public ObservableCollection<OptionViewModel> Options { get; } = [];

    [RelayCommand]
    private async Task AddImageAsync()
    {
        var path = await _shell.Dialogs.PickOpenFileAsync("Select image", new FilePickerFileType("Image")
        {
            Patterns = ["*.png", "*.jpg", "*.jpeg", "*.gif", "*.bmp"],
            MimeTypes = ["image/png", "image/jpeg"]
        });

        if (string.IsNullOrEmpty(path))
            return;

        var bytes = await File.ReadAllBytesAsync(path);
        _shell.Document.UpdateQuestionImage(_questionId, bytes);
        ReloadImage(bytes);
    }

    [RelayCommand]
    private void RemoveImage()
    {
        _shell.Document.UpdateQuestionImage(_questionId, null);
        Image = null;
        HasImage = false;
    }

    [RelayCommand]
    private void AddOption()
    {
        _shell.Document.AddOption(_questionId);
        ReloadOptions();
    }

    [RelayCommand]
    private void ToggleMultipleChoice()
    {
        _shell.Document.SetQuestionMultipleChoice(_questionId, !IsMultipleChoice);
    }

    partial void OnTextChanged(string value) => _shell.Document.UpdateQuestionText(_questionId, value);
    partial void OnExplanationChanged(string value) => _shell.Document.UpdateQuestionExplanation(_questionId, value);

    partial void OnIsMultipleChoiceChanged(bool value)
    {
        _shell.Document.SetQuestionMultipleChoice(_questionId, value);
        ReloadOptions();
    }

    internal void Reload()
    {
        if (!_shell.Document.Nodes.TryGetValue(_questionId, out var node) || node.Question == null)
            return;

        var question = node.Question;
        Text = question.Text;
        Explanation = question.Explanation;
        IsMultipleChoice = question.IsMultipleChoice;
        ReloadImage(question.ImageData);
        ReloadOptions();
    }

    private void ReloadOptions()
    {
        Options.Clear();
        if (!_shell.Document.Nodes.TryGetValue(_questionId, out var node) || node.Question == null)
            return;

        var answers = node.Question.Answers.ToHashSet();
        foreach (var option in node.Question.Options)
        {
            Options.Add(new OptionViewModel(_shell, _questionId, option, answers.Contains(option.Alphabet)));
        }
    }

    private void ReloadImage(byte[]? data)
    {
        if (data == null || data.Length == 0)
        {
            Image = null;
            HasImage = false;
            return;
        }

        try
        {
            using var stream = new MemoryStream(data);
            Image = new Bitmap(stream);
            HasImage = true;
        }
        catch
        {
            Image = null;
            HasImage = false;
        }
    }
}

public sealed partial class OptionViewModel : ViewModelBase
{
    private readonly ShellServices _shell;
    private readonly string _questionId;
    private readonly Option _option;

    public OptionViewModel(ShellServices shell, string questionId, Option option, bool isCorrect)
    {
        _shell = shell;
        _questionId = questionId;
        _option = option;
        _text = option.Text;
        _isCorrect = isCorrect;
    }

    public string Letter => _option.Alphabet.ToString();

    [ObservableProperty] private string _text;
    [ObservableProperty] private bool _isCorrect;

    partial void OnTextChanged(string value) => _shell.Document.UpdateOptionText(_questionId, Letter, value);

    partial void OnIsCorrectChanged(bool value) => _shell.Document.SetOptionCorrect(_questionId, Letter, value);
}
