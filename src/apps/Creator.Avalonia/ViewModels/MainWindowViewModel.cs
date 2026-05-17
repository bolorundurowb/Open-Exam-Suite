using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OpenExamSuite.Creator.Services;
using OpenExamSuite.Creator.ViewModels.Nodes;
using OpenExamSuite.Creator.ViewModels.Panes;
using OpenExamSuite.Creator.Views;
using OpenExamSuite.Creator.Views.Dialogs;
using OpenExamSuite.Shared;
using OpenExamSuite.Shared.Avalonia.Dialogs;
using OpenExamSuite.Shared.Avalonia.Services;
using OpenExamSuite.Shared.Enums;
using OpenExamSuite.Shared.Models;
using OpenExamSuite.Shared.Utilities;

namespace OpenExamSuite.Creator.ViewModels;

public sealed partial class MainWindowViewModel : ObservableObject
{
    private readonly IServiceProvider   _sp;
    private readonly IFilePickerService _files;
    private readonly IMessageBoxService _msg;
    private readonly IOpenUrlService    _urls;
    private readonly IUndoRedoService   _undo;
    private readonly IExamHistoryService _history;
    private readonly IPrintService      _print;
    private readonly IClipboardService  _clipboard;

    [ObservableProperty] private Exam? _exam;
    [ObservableProperty] private string? _currentExamFile;
    [ObservableProperty] private bool _isDirty;
    [ObservableProperty] private NodeViewModel? _selectedNode;
    [ObservableProperty] private IRightPaneViewModel? _currentRightPane;
    [ObservableProperty] private PrintScope _lastPrintScope = PrintScope.AllQuestions;

    public ObservableCollection<NodeViewModel> Nodes { get; } = new();

    public bool HasExam      => Exam is not null;
    public bool CanNewSection => HasExam;
    public bool CanNewQuestion => SelectedNode is SectionNodeViewModel or QuestionNodeViewModel;
    public bool CanPrint     => HasExam;
    public bool CanExport    => HasExam;
    public bool CanClose     => HasExam;
    public bool CanUndo      => _undo.CanUndo;
    public bool CanRedo      => _undo.CanRedo;
    public bool CanEditOrCopy => SelectedNode is QuestionNodeViewModel;

    public MainWindowViewModel(
        IServiceProvider sp,
        IFilePickerService files,
        IMessageBoxService msg,
        IOpenUrlService urls,
        IUndoRedoService undo,
        IExamHistoryService history,
        IPrintService print,
        IClipboardService clipboard)
    {
        _sp = sp;
        _files = files;
        _msg = msg;
        _urls = urls;
        _undo = undo;
        _history = history;
        _print = print;
        _clipboard = clipboard;

        _undo.Changed += (_, _) =>
        {
            UndoCommand.NotifyCanExecuteChanged();
            RedoCommand.NotifyCanExecuteChanged();
        };

        ShowSplash();
    }

    // ---------------------------------------------------------------------
    // Construction helpers
    // ---------------------------------------------------------------------

    private SplashPaneViewModel BuildSplash()
    {
        var splash = _sp.GetRequiredService<SplashPaneViewModel>();
        splash.OpenRequested = OpenAsync; // history click → open
        return splash;
    }

    private void ShowSplash()
    {
        CurrentRightPane = BuildSplash();
    }

    private void HookProps(ExamPropertiesPaneViewModel props)
    {
        props.Saved += (_, p) => OnPropertiesSaved(p);
    }

    private void OnPropertiesSaved(Properties p)
    {
        if (Exam is null)
        {
            Exam = new Exam { Properties = p };
            Nodes.Clear();
            Nodes.Add(new ExamNodeViewModel(p));
        }
        else
        {
            Exam.Properties = p;
            // Refresh root node display name.
            if (Nodes.FirstOrDefault() is ExamNodeViewModel root)
                root.Properties = p;
            else
                Nodes.Insert(0, new ExamNodeViewModel(p));
        }

        IsDirty = true;
        NotifyCommandStates();
    }

    // ---------------------------------------------------------------------
    // Right-pane selection logic — replaces WinForms AfterSelect
    // ---------------------------------------------------------------------

    partial void OnSelectedNodeChanged(NodeViewModel? value)
    {
        // Commit any pending question edits before switching panes.
        if (CurrentRightPane is QuestionEditorPaneViewModel editor)
            editor.CommitToModel();

        switch (value)
        {
            case ExamNodeViewModel exam:
                var props = _sp.GetRequiredService<ExamPropertiesPaneViewModel>().Initialize(exam);
                HookProps(props);
                CurrentRightPane = props;
                break;

            case SectionNodeViewModel section:
                CurrentRightPane = _sp.GetRequiredService<QuestionEditorPaneViewModel>().InitializeFromSection(section);
                break;

            case QuestionNodeViewModel question:
                CurrentRightPane = _sp.GetRequiredService<QuestionEditorPaneViewModel>().InitializeFromQuestion(question);
                break;

            case null:
                ShowSplash();
                break;
        }

        NotifyCommandStates();
    }

    partial void OnExamChanged(Exam? value)
    {
        NotifyCommandStates();
    }

    partial void OnIsDirtyChanged(bool value)
    {
        NotifyCommandStates();
    }

    private void NotifyCommandStates()
    {
        OnPropertyChanged(nameof(HasExam));
        OnPropertyChanged(nameof(CanNewSection));
        OnPropertyChanged(nameof(CanNewQuestion));
        OnPropertyChanged(nameof(CanPrint));
        OnPropertyChanged(nameof(CanExport));
        OnPropertyChanged(nameof(CanClose));
        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));
        OnPropertyChanged(nameof(CanEditOrCopy));

        SaveCommand.NotifyCanExecuteChanged();
        SaveAsCommand.NotifyCanExecuteChanged();
        ExportAsJsonCommand.NotifyCanExecuteChanged();
        ExportAsXmlCommand.NotifyCanExecuteChanged();
        ExportAsPdfCommand.NotifyCanExecuteChanged();
        PrintCommand.NotifyCanExecuteChanged();
        PrintPreviewCommand.NotifyCanExecuteChanged();
        CloseExamCommand.NotifyCanExecuteChanged();
        NewSectionCommand.NotifyCanExecuteChanged();
        NewQuestionCommand.NotifyCanExecuteChanged();
        UndoCommand.NotifyCanExecuteChanged();
        RedoCommand.NotifyCanExecuteChanged();
        CutCommand.NotifyCanExecuteChanged();
        CopyCommand.NotifyCanExecuteChanged();
        PasteCommand.NotifyCanExecuteChanged();
        DeleteQuestionCommand.NotifyCanExecuteChanged();
        EditSectionCommand.NotifyCanExecuteChanged();
    }

    // ---------------------------------------------------------------------
    // File: New / Open / Save / Save As / Close / Import / Export / Exit
    // ---------------------------------------------------------------------

    [RelayCommand]
    private async Task NewAsync()
    {
        if (!await ConfirmDiscardChangesAsync()) return;

        Exam = new Exam();
        CurrentExamFile = null;
        Nodes.Clear();
        _undo.Reset();

        var props = _sp.GetRequiredService<ExamPropertiesPaneViewModel>().InitializeNew();
        HookProps(props);
        CurrentRightPane = props;
        IsDirty = false;
    }

    [RelayCommand]
    private async Task OpenPickAsync()
    {
        if (!await ConfirmDiscardChangesAsync()) return;

        var path = await _files.PickOpenFileAsync(
            "Open Exam", new[] { new FileFilter("OEF Files", "*.oef") });
        if (string.IsNullOrEmpty(path)) return;

        await OpenAsync(path);
    }

    public async Task OpenAsync(string filePath)
    {
        var load = ExamFileLoader.TryLoad(filePath);

        if (!string.IsNullOrEmpty(load.ErrorMessage))
        {
            await _msg.ShowAsync(load.ErrorMessage!, "Error",
                MessageBoxButtons.Ok, MessageBoxIconKind.Error);
            return;
        }

        if (!load.Success || load.Exam is null)
        {
            await _msg.ShowAsync(
                "Sorry, the exam selected is either old or corrupt. If it is an old exam, please upgrade it with the upgrade tool at:\nhttps://sourceforge.net/projects/exam-upgrade-tool/",
                "Error", MessageBoxButtons.Ok, MessageBoxIconKind.Error);
            return;
        }

        Exam = load.Exam;
        var ext = Path.GetExtension(filePath)?.ToLowerInvariant();
        CurrentExamFile = ext is ".json" or ".xml" ? null : filePath;

        RebuildTreeFromExam();
        _undo.Reset();

        if (!string.IsNullOrEmpty(load.PathForHistory))
            _history.Add(load.PathForHistory);

        // Select the root and show properties.
        if (Nodes.FirstOrDefault() is ExamNodeViewModel root)
            SelectedNode = root;

        IsDirty = false;
    }

    private void RebuildTreeFromExam()
    {
        Nodes.Clear();
        if (Exam is null) return;

        var examNode = new ExamNodeViewModel(Exam.Properties);
        Nodes.Add(examNode);
        foreach (var section in Exam.Sections)
        {
            var sNode = new SectionNodeViewModel(section.Title);
            foreach (var q in section.Questions)
                sNode.Children.Add(new QuestionNodeViewModel(q));
            examNode.Children.Add(sNode);
        }
    }

    [RelayCommand(CanExecute = nameof(HasExam))]
    private async Task SaveAsync()
    {
        if (string.IsNullOrEmpty(CurrentExamFile))
        {
            await SaveAsAsync();
            return;
        }

        // Commit pending question edits to the model.
        if (CurrentRightPane is QuestionEditorPaneViewModel editor)
            editor.CommitToModel();

        SyncTreeToExam();

        var ok = Writer.ToOef(Exam!, CurrentExamFile);
        if (ok)
        {
            await _msg.ShowAsync("Exam has been successfully saved.", "Success",
                MessageBoxButtons.Ok, MessageBoxIconKind.Information);
            IsDirty = false;
        }
        else
        {
            await _msg.ShowAsync("Sorry, the exam could not be saved.", "Error",
                MessageBoxButtons.Ok, MessageBoxIconKind.Error);
        }

        if (!string.IsNullOrEmpty(CurrentExamFile))
            _history.Add(CurrentExamFile);
    }

    private void SyncTreeToExam()
    {
        if (Exam is null) return;
        if (Nodes.FirstOrDefault() is not ExamNodeViewModel examNode) return;

        Exam.Properties = examNode.Properties;
        Exam.Sections.Clear();
        foreach (var c in examNode.Children)
        {
            if (c is not SectionNodeViewModel sNode) continue;
            var s = new Section { Title = sNode.Title };
            foreach (var cq in sNode.Children)
                if (cq is QuestionNodeViewModel qNode)
                    s.Questions.Add(qNode.Question);
            Exam.Sections.Add(s);
        }
    }

    [RelayCommand(CanExecute = nameof(HasExam))]
    private async Task SaveAsAsync()
    {
        var path = await _files.PickSaveFileAsync(
            "Save Exam As",
            new[] { new FileFilter("OEF File", "*.oef") },
            suggestedFileName: Exam?.Properties.Title);
        if (string.IsNullOrEmpty(path))
        {
            await _msg.ShowAsync("Improper file name, Exam not saved!", "Error",
                MessageBoxButtons.Ok, MessageBoxIconKind.Error);
            return;
        }

        CurrentExamFile = path;
        await SaveAsync();
    }

    [RelayCommand]
    private async Task ImportFromJsonAsync()
    {
        if (!await ConfirmDiscardChangesAsync()) return;

        var path = await _files.PickOpenFileAsync(
            "Import JSON", new[] { new FileFilter("JSON Files", "*.json") });
        if (string.IsNullOrEmpty(path)) return;

        await OpenAsync(path);
    }

    [RelayCommand(CanExecute = nameof(HasExam))]
    private async Task ExportAsJsonAsync()
    {
        if (Exam is null) return;
        var path = await _files.PickSaveFileAsync(
            "Export as JSON",
            new[] { new FileFilter("JSON Files", "*.json") },
            suggestedFileName: Exam.Properties.Title);
        if (string.IsNullOrEmpty(path)) return;

        SyncTreeToExam();

        if (Writer.ToJson(Exam, path))
            await _msg.ShowAsync("JSON successfully exported.", "Export",
                MessageBoxButtons.Ok, MessageBoxIconKind.Information);
        else
            await _msg.ShowAsync("JSON file could not be exported.", "Export",
                MessageBoxButtons.Ok, MessageBoxIconKind.Error);
    }

    [RelayCommand(CanExecute = nameof(HasExam))]
    private async Task ExportAsXmlAsync()
    {
        if (Exam is null) return;
        var path = await _files.PickSaveFileAsync(
            "Export as XML",
            new[] { new FileFilter("XML Files", "*.xml") },
            suggestedFileName: Exam.Properties.Title);
        if (string.IsNullOrEmpty(path)) return;

        SyncTreeToExam();

        if (Writer.ToXml(Exam, path))
            await _msg.ShowAsync("XML successfully exported.", "Export",
                MessageBoxButtons.Ok, MessageBoxIconKind.Information);
        else
            await _msg.ShowAsync("XML could not be exported.", "Export",
                MessageBoxButtons.Ok, MessageBoxIconKind.Error);
    }

    [RelayCommand(CanExecute = nameof(HasExam))]
    private async Task ExportAsPdfAsync()
    {
        if (Exam is null) return;
        var path = await _files.PickSaveFileAsync(
            "Export as PDF",
            new[] { new FileFilter("PDF Files", "*.pdf") },
            suggestedFileName: Exam.Properties.Title);
        if (string.IsNullOrEmpty(path)) return;

        SyncTreeToExam();

        if (Writer.ToPdf(Exam, path))
            await _msg.ShowAsync("PDF successfully exported.", "Export",
                MessageBoxButtons.Ok, MessageBoxIconKind.Information);
        else
            await _msg.ShowAsync("PDF file could not be exported.", "Export",
                MessageBoxButtons.Ok, MessageBoxIconKind.Error);
    }

    [RelayCommand(CanExecute = nameof(HasExam))]
    private async Task PrintAsync()
    {
        if (Exam is null) return;

        var dlg = new PrintOptionsDialog(SelectedNode);
        var scope = await dlg.ShowDialog<PrintScope?>(MainWindow.Current!);
        if (scope is null) return;

        LastPrintScope = scope.Value;
        SyncTreeToExam();
        await _print.PrintAsync(Exam, SelectedNode, scope.Value);
    }

    [RelayCommand(CanExecute = nameof(HasExam))]
    private async Task PrintPreviewAsync()
    {
        if (Exam is null) return;

        var dlg = new PrintOptionsDialog(SelectedNode);
        var scope = await dlg.ShowDialog<PrintScope?>(MainWindow.Current!);
        if (scope is null) return;

        LastPrintScope = scope.Value;
        SyncTreeToExam();
        await _print.PreviewAsync(Exam, SelectedNode, scope.Value);
    }

    [RelayCommand(CanExecute = nameof(HasExam))]
    private async Task CloseExamAsync()
    {
        if (!await ConfirmDiscardChangesAsync()) return;

        Exam = null;
        CurrentExamFile = null;
        IsDirty = false;
        Nodes.Clear();
        _undo.Reset();
        SelectedNode = null;
        ShowSplash();
        if (CurrentRightPane is SplashPaneViewModel splash) splash.ReloadHistory();
    }

    [RelayCommand]
    private void Exit()
    {
        if (global::Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.Shutdown();
    }

    public async Task<bool> ConfirmDiscardChangesAsync()
    {
        if (!IsDirty) return true;

        var result = await _msg.ShowAsync(
            "There are unsaved changes in your project. Do you want to save the changes before closing it?",
            "Unsaved Changes",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIconKind.Warning);

        if (result == MessageBoxResult.Cancel) return false;
        if (result == MessageBoxResult.Yes) await SaveAsync();
        return true;
    }

    // ---------------------------------------------------------------------
    // Section / question creation
    // ---------------------------------------------------------------------

    [RelayCommand(CanExecute = nameof(CanNewSection))]
    private async Task NewSectionAsync()
    {
        var dlg = new AddSectionDialog();
        var title = await dlg.ShowDialog<string?>(MainWindow.Current!);
        if (string.IsNullOrWhiteSpace(title)) return;

        if (Nodes.FirstOrDefault() is not ExamNodeViewModel root) return;

        var section = new SectionNodeViewModel(title);
        root.Children.Add(section);
        SelectedNode = section;
        IsDirty = true;
    }

    [RelayCommand(CanExecute = nameof(CanNewQuestion))]
    private void NewQuestion()
    {
        SectionNodeViewModel? section = SelectedNode switch
        {
            SectionNodeViewModel s   => s,
            QuestionNodeViewModel q  => q.Parent as SectionNodeViewModel,
            _                        => null,
        };
        if (section is null) return;

        var question = new Question { No = section.Children.Count + 1 };
        var node = new QuestionNodeViewModel(question);
        section.Children.Add(node);
        SelectedNode = node;

        _undo.Push(new ChangeRepresentationObject
        {
            Action = ActionType.Add,
            Question = question,
            SectionTitle = section.Title,
        });

        IsDirty = true;
    }

    // ---------------------------------------------------------------------
    // Undo / Redo
    // ---------------------------------------------------------------------

    [RelayCommand(CanExecute = nameof(CanUndo))]
    private void Undo()
    {
        var u = _undo.Undo();
        if (u is null) return;
        ApplyChange(u, reverse: true);
    }

    [RelayCommand(CanExecute = nameof(CanRedo))]
    private void Redo()
    {
        var u = _undo.Redo();
        if (u is null) return;
        ApplyChange(u, reverse: false);
    }

    private void ApplyChange(ChangeRepresentationObject c, bool reverse)
    {
        if (Nodes.FirstOrDefault() is not ExamNodeViewModel root) return;
        var section = root.Children.OfType<SectionNodeViewModel>()
            .FirstOrDefault(s => s.Title == c.SectionTitle);
        if (c.Question is null) return;

        var action = reverse ? Invert(c.Action) : c.Action;

        switch (action)
        {
            case ActionType.Add:
                // Add question back / again.
                if (section is null)
                {
                    section = new SectionNodeViewModel(c.SectionTitle);
                    root.Children.Add(section);
                }

                var newNode = new QuestionNodeViewModel(c.Question);
                if (c.Question.No - 1 < section.Children.Count && c.Question.No > 0)
                    section.Children.Insert(c.Question.No - 1, newNode);
                else
                    section.Children.Add(newNode);
                section.RenumberQuestions();
                SelectedNode = newNode;
                IsDirty = true;
                break;

            case ActionType.Delete:
                if (section is null) return;
                var match = section.Children.OfType<QuestionNodeViewModel>()
                    .FirstOrDefault(qn => ReferenceEquals(qn.Question, c.Question)
                                       || qn.Question.No == c.Question.No);
                if (match is null) return;
                section.Children.Remove(match);
                section.RenumberQuestions();
                IsDirty = true;
                break;

            case ActionType.Modify:
                if (section is null) return;
                var target = section.Children.OfType<QuestionNodeViewModel>()
                    .FirstOrDefault(qn => qn.Question.No == c.Question.No);
                if (target is null) return;

                target.Question = c.Question;
                target.DisplayName = $"Question {c.Question.No}";

                // Refresh editor pane if it's showing this question.
                if (CurrentRightPane is QuestionEditorPaneViewModel editor &&
                    ReferenceEquals(editor.QuestionNode, target))
                {
                    editor.InitializeFromQuestion(target);
                }
                IsDirty = true;
                break;
        }
    }

    private static ActionType Invert(ActionType a) => a switch
    {
        ActionType.Add => ActionType.Delete,
        ActionType.Delete => ActionType.Add,
        _ => ActionType.Modify,
    };

    // ---------------------------------------------------------------------
    // Cut / Copy / Paste — delegated to focused TextBox via clipboard.
    // ---------------------------------------------------------------------

    [RelayCommand(CanExecute = nameof(CanEditOrCopy))]
    private async Task CutAsync()
    {
        var tb = MainWindow.Current?.GetFocusedTextBox();
        if (tb is null || string.IsNullOrEmpty(tb.SelectedText)) return;
        await _clipboard.SetTextAsync(tb.SelectedText);
        tb.SelectedText = string.Empty;
    }

    [RelayCommand(CanExecute = nameof(CanEditOrCopy))]
    private async Task CopyAsync()
    {
        var tb = MainWindow.Current?.GetFocusedTextBox();
        if (tb is null || string.IsNullOrEmpty(tb.SelectedText)) return;
        await _clipboard.SetTextAsync(tb.SelectedText);
    }

    [RelayCommand(CanExecute = nameof(CanEditOrCopy))]
    private async Task PasteAsync()
    {
        var tb = MainWindow.Current?.GetFocusedTextBox();
        if (tb is null) return;

        var text = await _clipboard.GetTextAsync();
        if (string.IsNullOrEmpty(text)) return;

        if (tb.SelectionEnd != tb.SelectionStart)
        {
            var ask = await _msg.ShowAsync(
                "Do you want to paste over current selection?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIconKind.Question);
            if (ask == MessageBoxResult.No)
            {
                tb.CaretIndex = Math.Max(tb.SelectionStart, tb.SelectionEnd);
                tb.SelectionStart = tb.SelectionEnd = tb.CaretIndex;
            }
        }

        tb.SelectedText = text;
    }

    // ---------------------------------------------------------------------
    // Context-menu commands (Edit Section, Delete Question)
    // ---------------------------------------------------------------------

    [RelayCommand(CanExecute = nameof(CanEditSection))]
    private async Task EditSectionAsync()
    {
        if (SelectedNode is not SectionNodeViewModel s) return;

        var dlg = new EditSectionDialog(s.Title);
        var newTitle = await dlg.ShowDialog<string?>(MainWindow.Current!);
        if (string.IsNullOrWhiteSpace(newTitle)) return;

        s.Title = newTitle;
        IsDirty = true;
    }

    private bool CanEditSection() => SelectedNode is SectionNodeViewModel;

    [RelayCommand(CanExecute = nameof(CanDeleteQuestion))]
    private void DeleteQuestion()
    {
        if (SelectedNode is not QuestionNodeViewModel q) return;
        if (q.Parent is not SectionNodeViewModel section) return;

        _undo.Push(new ChangeRepresentationObject
        {
            Action = ActionType.Delete,
            Question = q.Question,
            SectionTitle = section.Title,
        });

        section.Children.Remove(q);
        section.RenumberQuestions();
        IsDirty = true;
    }

    private bool CanDeleteQuestion() => SelectedNode is QuestionNodeViewModel;

    // ---------------------------------------------------------------------
    // Help / About / License
    // ---------------------------------------------------------------------

    [RelayCommand]
    private async Task HelpAsync()
        => await _urls.OpenUriAsync("https://bolorundurowb.github.io/Open-Exam-Suite");

    [RelayCommand]
    private async Task AboutAsync()
    {
        var dlg = new AboutDialog();
        await dlg.ShowDialog(MainWindow.Current!);
    }

    [RelayCommand]
    private async Task LicenseAsync()
    {
        var dlg = new LicenseDialog();
        await dlg.ShowDialog(MainWindow.Current!);
    }
}
