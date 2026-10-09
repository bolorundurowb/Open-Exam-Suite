using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenExamSuite.Creator.Session.Models;
using OpenExamSuite.Shared;
using OpenExamSuite.Shared.Enums;
using OpenExamSuite.Shared.Utilities;

namespace OpenExamSuite.Creator.Session.Services;

/// <summary>
/// Document orchestration for the Avalonia Creator: load, save, dirty tracking, selection,
/// search, reorder, duplicate, undo coalescing, validation, and recovery-copy management.
/// The underlying <see cref="Exam"/> schema is unchanged; session IDs are maintained in memory.
/// </summary>
public sealed class CreatorDocument
{
    private const int MaxUndoStack = 50;
    private const int CoalesceMilliseconds = 400;

    private readonly Reader _reader;
    private readonly Writer _writer;
    private readonly ILogger<CreatorDocument> _logger;
    private readonly List<DocumentSnapshot> _undoStack = [];
    private readonly List<DocumentSnapshot> _redoStack = [];
    private readonly Dictionary<string, DocumentNode> _nodes = new(StringComparer.Ordinal);

    private Exam _exam = new();
    private string? _filePath;
    private string? _selectedNodeId;
    private bool _isDirty;
    private bool _isLegacy;
    private DateTime _lastTextEdit = DateTime.MinValue;
    private bool _coalescing;

    public CreatorDocument(Reader reader, Writer writer, ILogger<CreatorDocument>? logger = null)
    {
        _reader = reader;
        _writer = writer;
        _logger = logger ?? NullLogger<CreatorDocument>.Instance;
    }

    public event Action? Changed;
    public event Action<string?>? SelectionChanged;
    public event Action<IReadOnlyList<ValidationProblem>>? ProblemsChanged;

    public Exam Exam => _exam;

    public string? FilePath => _filePath;

    public bool IsDirty
    {
        get => _isDirty;
        private set
        {
            if (_isDirty == value)
                return;

            _isDirty = value;
            Changed?.Invoke();
        }
    }

    public bool IsLegacy => _isLegacy;

    public string? SelectedNodeId
    {
        get => _selectedNodeId;
        set
        {
            if (_selectedNodeId == value)
                return;

            _selectedNodeId = value;
            SelectionChanged?.Invoke(value);
        }
    }

    public IReadOnlyDictionary<string, DocumentNode> Nodes => _nodes;

    public bool CanUndo => _undoStack.Count > 0;

    public bool CanRedo => _redoStack.Count > 0;

    public IReadOnlyList<ValidationProblem> Problems { get; private set; } = Array.Empty<ValidationProblem>();

    public DocumentLoadResult NewDocument()
    {
        _exam = new Exam
        {
            Properties = new Properties
            {
                Title = "Untitled Exam",
                Code = string.Empty,
                Instructions = string.Empty,
                Passmark = 700,
                TimeLimit = 60,
                HideAnswers = false
            }
        };
        _filePath = null;
        _isLegacy = false;
        RebuildNodes();
        ClearUndoRedo();
        IsDirty = false;
        SelectedNodeId = _nodes.Values.First(n => n.Type == NodeType.Exam).Id;
        Revalidate();
        return DocumentLoadResult.Ok(string.Empty, false);
    }

    public DocumentLoadResult Load(string filePath)
    {
        var result = _reader.FromOefFile(filePath);
        if (!result.Success || result.Exam == null)
            return DocumentLoadResult.Fail(result.Error, filePath);

        _exam = result.Exam;
        _filePath = filePath;
        _isLegacy = result.IsLegacy;
        RebuildNodes();
        ClearUndoRedo();
        IsDirty = false;

        var firstSection = _nodes.Values.FirstOrDefault(n => n.Type == NodeType.Section);
        SelectedNodeId = firstSection?.Id ?? _nodes.Values.First(n => n.Type == NodeType.Exam).Id;

        Revalidate();
        _logger.LogInformation("Loaded exam '{Title}' from '{FilePath}'.", _exam.Properties.Title, filePath);
        return DocumentLoadResult.Ok(filePath, result.IsLegacy);
    }

    public DocumentLoadResult LoadJson(string filePath)
    {
        var result = _reader.FromJsonFile(filePath);
        if (!result.Success || result.Exam == null)
            return DocumentLoadResult.Fail(result.Error, filePath);

        _exam = result.Exam;
        _filePath = Path.ChangeExtension(filePath, ".oef");
        _isLegacy = false;
        RebuildNodes();
        ClearUndoRedo();
        IsDirty = true;

        var firstSection = _nodes.Values.FirstOrDefault(n => n.Type == NodeType.Section);
        SelectedNodeId = firstSection?.Id ?? _nodes.Values.First(n => n.Type == NodeType.Exam).Id;

        Revalidate();
        return DocumentLoadResult.Ok(_filePath, false);
    }

    public DocumentSaveResult Save(string? filePath = null)
    {
        var target = filePath ?? _filePath;
        if (string.IsNullOrWhiteSpace(target))
            return DocumentSaveResult.Fail(ExamIoError.WriteFailed);

        var result = _writer.ToOef(_exam, target);
        if (!result.Success)
            return DocumentSaveResult.Fail(result.Error);

        _filePath = target;
        _isLegacy = false;
        IsDirty = false;
        _logger.LogInformation("Saved exam '{Title}' to '{FilePath}'.", _exam.Properties.Title, target);
        return DocumentSaveResult.Ok(target);
    }

    public DocumentSaveResult SaveJson(string filePath)
    {
        var result = _writer.ToJson(_exam, filePath);
        if (!result.Success)
            return DocumentSaveResult.Fail(result.Error);

        return DocumentSaveResult.Ok(filePath);
    }

    public DocumentSaveResult SaveXml(string filePath)
    {
        var result = _writer.ToXml(_exam, filePath);
        if (!result.Success)
            return DocumentSaveResult.Fail(result.Error);

        return DocumentSaveResult.Ok(filePath);
    }

    public DocumentSaveResult SavePdf(string filePath)
    {
        var result = _writer.ToPdf(_exam, filePath);
        if (!result.Success)
            return DocumentSaveResult.Fail(result.Error);

        return DocumentSaveResult.Ok(filePath);
    }

    public void MarkDirty()
    {
        IsDirty = true;
    }

    // ---- Node creation -----------------------------------------------------------------------

    public DocumentNode AddSection(string? title = null)
    {
        PushUndo("Add section");
        var section = new Section { Title = title ?? NewSectionName() };
        _exam.Sections.Add(section);
        var sectionIndex = _exam.Sections.Count - 1;
        var node = new DocumentNode(Guid.NewGuid().ToString("N"), NodeType.Section) { Section = section };
        _nodes[node.Id] = node;
        RenumberQuestions();
        Revalidate();
        SelectedNodeId = node.Id;
        return node;
    }

    public DocumentNode AddQuestion(string sectionId)
    {
        if (!_nodes.TryGetValue(sectionId, out var sectionNode) || sectionNode.Section == null)
            throw new ArgumentException("Section not found", nameof(sectionId));

        PushUndo("Add question");
        var question = new Question
        {
            No = sectionNode.Section.Questions.Count + 1,
            Text = string.Empty,
            Options = CreateDefaultOptions()
        };
        sectionNode.Section.Questions.Add(question);
        var node = new DocumentNode(Guid.NewGuid().ToString("N"), NodeType.Question, sectionNode.Id) { Question = question };
        _nodes[node.Id] = node;
        Revalidate();
        SelectedNodeId = node.Id;
        return node;
    }

    public void DuplicateNode(string nodeId)
    {
        if (!_nodes.TryGetValue(nodeId, out var node))
            return;

        switch (node.Type)
        {
            case NodeType.Section when node.Section != null:
                PushUndo("Duplicate section");
                var clonedSection = CloneSection(node.Section);
                var insertIndex = _exam.Sections.IndexOf(node.Section) + 1;
                _exam.Sections.Insert(insertIndex, clonedSection);
                RebuildNodes();
                RenumberQuestions();
                Revalidate();
                break;

            case NodeType.Question when node.Question != null && node.ParentId != null
                && _nodes.TryGetValue(node.ParentId, out var parentNode) && parentNode.Section != null:
                PushUndo("Duplicate question");
                var clonedQuestion = CloneQuestion(node.Question);
                var qIndex = parentNode.Section.Questions.IndexOf(node.Question) + 1;
                parentNode.Section.Questions.Insert(qIndex, clonedQuestion);
                RebuildNodes();
                RenumberQuestions();
                Revalidate();
                SelectedNodeId = _nodes.Values.Last(n => n.Question == clonedQuestion).Id;
                break;
        }
    }

    public void DeleteNode(string nodeId)
    {
        if (!_nodes.TryGetValue(nodeId, out var node))
            return;

        PushUndo("Delete");

        switch (node.Type)
        {
            case NodeType.Section when node.Section != null:
                _exam.Sections.Remove(node.Section);
                break;

            case NodeType.Question when node.Question != null && node.ParentId != null
                && _nodes.TryGetValue(node.ParentId, out var parentNode) && parentNode.Section != null:
                parentNode.Section.Questions.Remove(node.Question);
                break;
        }

        RebuildNodes();
        RenumberQuestions();
        Revalidate();

        var fallback = _nodes.Values.FirstOrDefault(n => n.Type == NodeType.Exam)?.Id;
        if (SelectedNodeId == nodeId || !_nodes.ContainsKey(SelectedNodeId ?? string.Empty))
            SelectedNodeId = fallback;
    }

    // ---- Reorder -----------------------------------------------------------------------------

    public void MoveQuestion(string questionId, string targetSectionId, int targetIndex)
    {
        if (!_nodes.TryGetValue(questionId, out var qNode) || qNode.Question == null
            || qNode.ParentId == null
            || !_nodes.TryGetValue(targetSectionId, out var targetSectionNode) || targetSectionNode.Section == null)
            return;

        if (!_nodes.TryGetValue(qNode.ParentId, out var sourceSectionNode) || sourceSectionNode.Section == null)
            return;

        PushUndo("Move question");
        var question = qNode.Question;
        sourceSectionNode.Section.Questions.Remove(question);
        var clampedIndex = Math.Clamp(targetIndex, 0, targetSectionNode.Section.Questions.Count);
        targetSectionNode.Section.Questions.Insert(clampedIndex, question);
        RebuildNodes();
        RenumberQuestions();
        Revalidate();
        SelectedNodeId = questionId;
    }

    public void ReorderQuestion(string questionId, int newIndex)
    {
        if (!_nodes.TryGetValue(questionId, out var qNode) || qNode.Question == null
            || qNode.ParentId == null
            || !_nodes.TryGetValue(qNode.ParentId, out var sectionNode) || sectionNode.Section == null)
            return;

        PushUndo("Reorder question");
        var question = qNode.Question;
        sectionNode.Section.Questions.Remove(question);
        var clampedIndex = Math.Clamp(newIndex, 0, sectionNode.Section.Questions.Count);
        sectionNode.Section.Questions.Insert(clampedIndex, question);
        RebuildNodes();
        RenumberQuestions();
        Revalidate();
        SelectedNodeId = questionId;
    }

    public void ReorderSection(string sectionId, int newIndex)
    {
        if (!_nodes.TryGetValue(sectionId, out var sectionNode) || sectionNode.Section == null)
            return;

        PushUndo("Reorder section");
        var section = sectionNode.Section;
        _exam.Sections.Remove(section);
        var clampedIndex = Math.Clamp(newIndex, 0, _exam.Sections.Count);
        _exam.Sections.Insert(clampedIndex, section);
        RebuildNodes();
        Revalidate();
        SelectedNodeId = sectionId;
    }

    public void ReorderOption(string questionId, string optionId, int newIndex)
    {
        if (!_nodes.TryGetValue(questionId, out var qNode) || qNode.Question == null)
            return;

        var option = qNode.Question.Options.FirstOrDefault(o => o.Alphabet.ToString() == optionId);
        if (option == null)
            return;

        PushUndo("Reorder option");
        qNode.Question.Options.Remove(option);
        var clampedIndex = Math.Clamp(newIndex, 0, qNode.Question.Options.Count);
        qNode.Question.Options.Insert(clampedIndex, option);
        ReassignOptionLetters(qNode.Question);
        Revalidate();
        SelectedNodeId = questionId;
    }

    // ---- Editing ------------------------------------------------------------------------------

    public void UpdateExamProperties(Properties properties)
    {
        PushUndo("Update properties");
        _exam.Properties = properties;
        Revalidate();
    }

    public void UpdateSectionName(string sectionId, string name)
    {
        if (!_nodes.TryGetValue(sectionId, out var node) || node.Section == null)
            return;

        PushUndo("Rename section");
        node.Section.Title = name;
        Revalidate();
    }

    public void UpdateQuestionText(string questionId, string text)
    {
        if (!_nodes.TryGetValue(questionId, out var node) || node.Question == null)
            return;

        CoalesceEdit("Update question text", () => node.Question!.Text = text);
    }

    public void UpdateQuestionImage(string questionId, byte[]? imageData)
    {
        if (!_nodes.TryGetValue(questionId, out var node) || node.Question == null)
            return;

        PushUndo("Update image");
        node.Question.ImageData = imageData;
        Revalidate();
    }

    public void UpdateQuestionExplanation(string questionId, string explanation)
    {
        if (!_nodes.TryGetValue(questionId, out var node) || node.Question == null)
            return;

        CoalesceEdit("Update explanation", () => node.Question!.Explanation = explanation);
    }

    public void SetQuestionMultipleChoice(string questionId, bool isMultipleChoice)
    {
        if (!_nodes.TryGetValue(questionId, out var node) || node.Question == null)
            return;

        PushUndo("Toggle multiple choice");
        node.Question.IsMultipleChoice = isMultipleChoice;
        if (!isMultipleChoice)
        {
            var firstCorrect = node.Question.Answers.FirstOrDefault();
            node.Question.Answers = firstCorrect == '\0' ? [] : [firstCorrect];
        }

        Revalidate();
    }

    public void AddOption(string questionId)
    {
        if (!_nodes.TryGetValue(questionId, out var node) || node.Question == null)
            return;

        PushUndo("Add option");
        var nextLetter = node.Question.Options.Count == 0
            ? 'A'
            : (char)(node.Question.Options.Max(o => o.Alphabet) + 1);
        node.Question.Options.Add(new Option { Alphabet = nextLetter, Text = string.Empty });
        Revalidate();
    }

    public void UpdateOptionText(string questionId, string optionId, string text)
    {
        if (!_nodes.TryGetValue(questionId, out var node) || node.Question == null)
            return;

        var option = node.Question.Options.FirstOrDefault(o => o.Alphabet.ToString() == optionId);
        if (option == null)
            return;

        CoalesceEdit("Update option text", () => option.Text = text);
    }

    public void SetOptionCorrect(string questionId, string optionId, bool correct)
    {
        if (!_nodes.TryGetValue(questionId, out var node) || node.Question == null)
            return;

        var option = node.Question.Options.FirstOrDefault(o => o.Alphabet.ToString() == optionId);
        if (option == null)
            return;

        PushUndo("Set correct option");
        var answers = node.Question.Answers.ToList();
        if (correct)
        {
            if (node.Question.IsMultipleChoice)
            {
                if (!answers.Contains(option.Alphabet))
                    answers.Add(option.Alphabet);
            }
            else
            {
                answers = [option.Alphabet];
            }
        }
        else
        {
            answers.Remove(option.Alphabet);
        }

        node.Question.Answers = answers.ToArray();
        if (!node.Question.IsMultipleChoice && answers.Count > 0)
            node.Question.Answer = answers[0];
        else if (answers.Count == 0)
            node.Question.Answer = '\0';

        Revalidate();
    }

    public void DeleteOption(string questionId, string optionId)
    {
        if (!_nodes.TryGetValue(questionId, out var node) || node.Question == null)
            return;

        var option = node.Question.Options.FirstOrDefault(o => o.Alphabet.ToString() == optionId);
        if (option == null)
            return;

        PushUndo("Delete option");
        node.Question.Options.Remove(option);
        ReassignOptionLetters(node.Question);
        var answers = node.Question.Answers.Where(a => node.Question.Options.Any(o => o.Alphabet == a)).ToArray();
        node.Question.Answers = answers;
        node.Question.Answer = answers.FirstOrDefault();
        Revalidate();
    }

    // ---- Undo / Redo -------------------------------------------------------------------------

    public void Undo()
    {
        if (_undoStack.Count == 0)
            return;

        var current = CaptureSnapshot();
        var snapshot = _undoStack[^1];
        _undoStack.RemoveAt(_undoStack.Count - 1);
        _redoStack.Add(current);
        RestoreSnapshot(snapshot);
    }

    public void Redo()
    {
        if (_redoStack.Count == 0)
            return;

        var current = CaptureSnapshot();
        var snapshot = _redoStack[^1];
        _redoStack.RemoveAt(_redoStack.Count - 1);
        _undoStack.Add(current);
        RestoreSnapshot(snapshot);
    }

    // ---- Search -------------------------------------------------------------------------------

    public IReadOnlyList<DocumentNode> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return _nodes.Values.ToList();

        var lower = query.ToLowerInvariant();
        var matchedQuestions = _nodes.Values
            .Where(n => n.Type == NodeType.Question
                && n.Question != null
                && (n.Question.Text.Contains(query, StringComparison.OrdinalIgnoreCase)
                    || n.Question.Options.Any(o => o.Text.Contains(query, StringComparison.OrdinalIgnoreCase))))
            .ToList();

        var matchedSections = _nodes.Values
            .Where(n => n.Type == NodeType.Section
                && n.Section != null
                && n.Section.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var examNode = _nodes.Values.First(n => n.Type == NodeType.Exam);
        var result = new List<DocumentNode> { examNode };
        result.AddRange(matchedSections);
        result.AddRange(matchedQuestions);
        return result;
    }

    // ---- Validation ----------------------------------------------------------------------------

    public void Revalidate()
    {
        var problems = new List<ValidationProblem>();
        var sectionTitles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var sectionNode in _nodes.Values.Where(n => n.Type == NodeType.Section))
        {
            if (sectionNode.Section == null)
                continue;

            if (!sectionTitles.Add(sectionNode.Section.Title))
            {
                problems.Add(new ValidationProblem(
                    ProblemKind.DuplicateSectionName,
                    sectionNode.Id,
                    $"Duplicate section name: {sectionNode.Section.Title}"));
            }

            foreach (var qNode in _nodes.Values
                .Where(n => n.Type == NodeType.Question && n.ParentId == sectionNode.Id))
            {
                if (qNode.Question == null)
                    continue;

                if (string.IsNullOrWhiteSpace(qNode.Question.Text))
                {
                    problems.Add(new ValidationProblem(
                        ProblemKind.EmptyQuestionText,
                        qNode.Id,
                        "Empty question text"));
                }

                if (qNode.Question.Options.Count < 2)
                {
                    problems.Add(new ValidationProblem(
                        ProblemKind.FewerThanTwoOptions,
                        qNode.Id,
                        "Fewer than 2 options"));
                }

                if (qNode.Question.Answers.Length == 0 || qNode.Question.Answers.All(a => a == '\0'))
                {
                    problems.Add(new ValidationProblem(
                        ProblemKind.NoCorrectAnswer,
                        qNode.Id,
                        "No correct answer"));
                }
            }
        }

        Problems = problems;
        ProblemsChanged?.Invoke(problems);
        Changed?.Invoke();
    }

    // ---- Recovery copy ------------------------------------------------------------------------

    public void WriteRecoveryCopy(string recoveryPath)
    {
        if (!IsDirty && File.Exists(recoveryPath))
            return;

        try
        {
            _writer.ToOef(_exam, recoveryPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write recovery copy to '{RecoveryPath}'.", recoveryPath);
        }
    }

    public bool HasRecoveryCopy(string recoveryPath) =>
        File.Exists(recoveryPath) && new FileInfo(recoveryPath).Length > 0;

    public DateTime? RecoveryCopyTimestamp(string recoveryPath) =>
        File.Exists(recoveryPath) ? File.GetLastWriteTimeUtc(recoveryPath) : null;

    // ---- Helpers -------------------------------------------------------------------------------

    private void RebuildNodes(IReadOnlyDictionary<NodeIdentity, string>? identityMap = null)
    {
        _nodes.Clear();
        var examNode = new DocumentNode(GetId(new NodeIdentity(NodeType.Exam, -1, -1), identityMap), NodeType.Exam);
        _nodes[examNode.Id] = examNode;

        for (var sectionIndex = 0; sectionIndex < _exam.Sections.Count; sectionIndex++)
        {
            var section = _exam.Sections[sectionIndex];
            var sectionId = GetId(new NodeIdentity(NodeType.Section, sectionIndex, -1), identityMap);
            var sectionNode = new DocumentNode(sectionId, NodeType.Section) { Section = section };
            _nodes[sectionId] = sectionNode;

            for (var questionIndex = 0; questionIndex < section.Questions.Count; questionIndex++)
            {
                var question = section.Questions[questionIndex];
                var questionId = GetId(new NodeIdentity(NodeType.Question, sectionIndex, questionIndex), identityMap);
                var questionNode = new DocumentNode(questionId, NodeType.Question, sectionId) { Question = question };
                _nodes[questionId] = questionNode;
            }
        }
    }

    private static string GetId(NodeIdentity identity, IReadOnlyDictionary<NodeIdentity, string>? map) =>
        map != null && map.TryGetValue(identity, out var id) ? id : Guid.NewGuid().ToString("N");

    private IReadOnlyDictionary<NodeIdentity, string> CaptureNodeIdentities() =>
        _nodes.Values.ToDictionary(
            n => new NodeIdentity(n.Type, SectionIndex(n), QuestionIndex(n)),
            n => n.Id);

    private int SectionIndex(DocumentNode node)
    {
        if (node.Type == NodeType.Exam)
            return -1;

        if (node.Section != null)
            return _exam.Sections.IndexOf(node.Section);

        if (node.Question != null && node.ParentId != null && _nodes.TryGetValue(node.ParentId, out var parent))
            return SectionIndex(parent);

        return -1;
    }

    private static int QuestionIndex(DocumentNode node) =>
        node.Question != null && node.ParentId != null ? node.Question.No - 1 : -1;

    private void RenumberQuestions()
    {
        foreach (var section in _exam.Sections)
        {
            for (var i = 0; i < section.Questions.Count; i++)
                section.Questions[i].No = i + 1;
        }
    }

    private void ReassignOptionLetters(Question question)
    {
        var letter = 'A';
        foreach (var option in question.Options)
            option.Alphabet = letter++;
    }

    private static List<Option> CreateDefaultOptions() =>
        [new() { Alphabet = 'A', Text = string.Empty }, new() { Alphabet = 'B', Text = string.Empty }];

    private string NewSectionName()
    {
        var index = 1;
        var names = new HashSet<string>(_exam.Sections.Select(s => s.Title), StringComparer.OrdinalIgnoreCase);
        while (names.Contains($"Section {index}"))
            index++;

        return $"Section {index}";
    }

    private static Section CloneSection(Section section)
    {
        return new Section
        {
            Title = $"{section.Title} (Copy)",
            Questions = section.Questions.Select(CloneQuestion).ToList()
        };
    }

    private static Question CloneQuestion(Question question)
    {
        return new Question
        {
            No = question.No,
            Text = question.Text,
            ImageData = question.ImageData,
            Answer = question.Answer,
            IsMultipleChoice = question.IsMultipleChoice,
            Answers = question.Answers.ToArray(),
            Options = question.Options.Select(o => new Option { Alphabet = o.Alphabet, Text = o.Text }).ToList(),
            Explanation = question.Explanation
        };
    }

    private void PushUndo(string label)
    {
        _redoStack.Clear();
        _undoStack.Add(CaptureSnapshot(label));
        TrimUndoStack();
        IsDirty = true;
        _coalescing = false;
    }

    private void CoalesceEdit(string label, Action apply)
    {
        var now = DateTime.UtcNow;
        var sinceLast = now - _lastTextEdit;
        _lastTextEdit = now;

        if (!_coalescing || sinceLast.TotalMilliseconds > CoalesceMilliseconds)
        {
            PushUndo(label);
            _coalescing = true;
        }

        apply();
        Revalidate();
    }

    private void TrimUndoStack()
    {
        while (_undoStack.Count > MaxUndoStack)
            _undoStack.RemoveAt(0);
    }

    private DocumentSnapshot CaptureSnapshot(string? label = null)
    {
        return new DocumentSnapshot(CloneExam(_exam), CaptureNodeIdentities(), _selectedNodeId, label);
    }

    private void RestoreSnapshot(DocumentSnapshot snapshot)
    {
        _exam = snapshot.Exam;
        RebuildNodes(snapshot.Identities);
        RenumberQuestions();
        _selectedNodeId = snapshot.SelectedNodeId;
        if (_selectedNodeId != null && !_nodes.ContainsKey(_selectedNodeId))
            _selectedNodeId = _nodes.Values.FirstOrDefault(n => n.Type == NodeType.Exam)?.Id;

        Revalidate();
        IsDirty = true;
        SelectionChanged?.Invoke(_selectedNodeId);
    }

    private void ClearUndoRedo()
    {
        _undoStack.Clear();
        _redoStack.Clear();
    }

    private static Exam CloneExam(Exam exam)
    {
        return new Exam
        {
            Properties = new Properties
            {
                Title = exam.Properties.Title,
                Code = exam.Properties.Code,
                Version = exam.Properties.Version,
                Passmark = exam.Properties.Passmark,
                TimeLimit = exam.Properties.TimeLimit,
                Instructions = exam.Properties.Instructions,
                HideAnswers = exam.Properties.HideAnswers
            },
            Sections = exam.Sections.Select(CloneSectionExact).ToList()
        };
    }

    private static Section CloneSectionExact(Section section)
    {
        return new Section
        {
            Title = section.Title,
            Questions = section.Questions.Select(CloneQuestion).ToList()
        };
    }

    private sealed record NodeIdentity(NodeType Type, int SectionIndex, int QuestionIndex);

    private sealed record DocumentSnapshot(Exam Exam, IReadOnlyDictionary<NodeIdentity, string> Identities, string? SelectedNodeId, string? Label);
}
