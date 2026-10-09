using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenExamSuite.Shared;

namespace OpenExamSuite.Creator.ViewModels;

public sealed partial class QuestionPreviewViewModel : ViewModelBase
{
    [ObservableProperty] private Bitmap? _image;

    public QuestionPreviewViewModel(Question question)
    {
        Question = question;
        Options = question.Options.Select(o => new PreviewOptionViewModel(o, question.Answers.Contains(o.Alphabet))).ToList();
        LoadImage(question.ImageData);
    }

    public Question Question { get; }

    public IReadOnlyList<PreviewOptionViewModel> Options { get; }

    public string Header => $"Question {Question.No}";

    private void LoadImage(byte[]? data)
    {
        if (data == null || data.Length == 0)
        {
            Image = null;
            return;
        }

        try
        {
            using var stream = new MemoryStream(data);
            Image = new Bitmap(stream);
        }
        catch
        {
            Image = null;
        }
    }
}

public sealed class PreviewOptionViewModel
{
    public PreviewOptionViewModel(Option option, bool isCorrect)
    {
        Letter = option.Alphabet.ToString();
        Text = option.Text;
        IsCorrect = isCorrect;
    }

    public string Letter { get; }
    public string Text { get; }
    public bool IsCorrect { get; }
}
