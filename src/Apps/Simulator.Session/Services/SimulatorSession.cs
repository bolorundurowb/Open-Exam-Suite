using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenExamSuite.Simulator.Session.HostPorts;
using OpenExamSuite.Simulator.Session.Models;
using OpenExamSuite.Simulator.Session.States;
using OpenExamSuite.Shared;
using OpenExamSuite.Shared.Enums;
using OpenExamSuite.Shared.Interfaces;
using OpenExamSuite.Shared.Models;
using OpenExamSuite.Shared.Services;
using OpenExamSuite.Shared.Utilities;
using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Interfaces;
using OpenExamSuite.Storage.Models;

namespace OpenExamSuite.Simulator.Session.Services;

/// <summary>
/// UI-neutral session layer for Simulator taking flow.
/// Orchestrates state transitions and business logic without any UI framework references.
/// </summary>
public sealed class SimulatorSession : ISimulatorSession, IDisposable
{
    private readonly IExamLibraryService _library;
    private readonly IAppSettingsService _settings;
    private readonly Reader _reader;
    private readonly IExamComposer _composer;
    private readonly IScorer _scorer;
    private readonly ExamFileLoader _examFileLoader;
    private readonly IAppPaths _appPaths;
    private readonly IFileSystem _fileSystem;
    private readonly IPrompts _prompts;
    private readonly IUriLauncher _uriLauncher;
    private readonly IPrintService _printService;
    private readonly IToastService _toastService;
    private readonly ILogger<SimulatorSession> _logger;
    private readonly TimeProvider _timeProvider;
    private readonly Writer _writer;

    private const string SamplesSeededKey = "Simulator.SamplesSeeded";

    private ISessionState _currentState = new LibraryState([], [], null, null, false);
    private AttemptState? _attemptState;
    private Exam? _currentExam;
    private string _currentExamFilePath = string.Empty;
    private PreExamSettings? _preExamSettings;
    private SelectionResult? _missedQuestionSelection;
    private ResultsState? _resultsBeforeReview;
    private DateTimeOffset _attemptStartTime;
    private TimeSpan _accumulatedPausedTime;
    private DateTimeOffset? _pauseStartTime;
    private DateTimeOffset? _systemSuspendStartTime;
    private int _randomSeed;
    private Random? _random;

    public event Action<ISessionState>? StateChanged;

    public ISessionState CurrentState => _currentState;

    public SimulatorSession(
        IExamLibraryService library,
        IAppSettingsService settings,
        Reader reader,
        IExamComposer composer,
        IScorer scorer,
        ExamFileLoader examFileLoader,
        IAppPaths appPaths,
        IFileSystem fileSystem,
        IPrompts prompts,
        IUriLauncher uriLauncher,
        IPrintService printService,
        IToastService toastService,
        ILogger<SimulatorSession> logger,
        TimeProvider? timeProvider = null,
        Writer? writer = null)
    {
        _library = library;
        _settings = settings;
        _reader = reader;
        _composer = composer;
        _scorer = scorer;
        _examFileLoader = examFileLoader;
        _appPaths = appPaths;
        _fileSystem = fileSystem;
        _prompts = prompts;
        _uriLauncher = uriLauncher;
        _printService = printService;
        _toastService = toastService;
        _logger = logger;
        _timeProvider = timeProvider ?? TimeProvider.System;
        _writer = writer ?? new Writer();
    }

    public async Task InitializeAsync()
    {
        try
        {
            // Seed the samples on first launch only, so a sample the user removed stays removed.
            if (_settings.Get(SamplesSeededKey, AppSettingsType.Other)?.Value != "1")
            {
                var samplePaths = await _fileSystem.GetFilesAsync(_appPaths.BundledSamplesRoot, "*.oef");
                foreach (var samplePath in samplePaths)
                {
                    _library.AddExam(
                        ExamCatalog.Simulator,
                        samplePath,
                        Path.GetFileNameWithoutExtension(samplePath));
                }

                if (samplePaths.Length > 0)
                    _settings.Set(new AppSetting { Key = SamplesSeededKey, Value = "1" }, AppSettingsType.Other);
            }
        }
        catch (Exception ex)
        {
            // An absent or read-only bundle must not prevent the Library from opening.
            _logger.LogWarning(ex, "Could not seed bundled samples from '{SamplesRoot}'.", _appPaths.BundledSamplesRoot);
        }

        await LoadLibraryAsync();
    }

    public Task RefreshLibraryAsync() => LoadLibraryAsync();

    public async Task<LibraryActionResult> AddExamAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !await _fileSystem.ExistsAsync(filePath))
            return new LibraryActionResult(LibraryActionStatus.FileNotFound);

        if (!IsOefPath(filePath))
            return new LibraryActionResult(LibraryActionStatus.UnsupportedFormat);

        var load = _examFileLoader.TryLoad(filePath);
        if (!load.Success || load.Exam == null)
            return new LibraryActionResult(LibraryActionStatus.CorruptFile);

        var targetPath = filePath;
        var alreadyThere = _library.GetExams(ExamCatalog.Simulator)
            .Any(e => PathsEqual(e.FilePath, targetPath));
        if (alreadyThere)
            return new LibraryActionResult(LibraryActionStatus.AlreadyInLibrary, targetPath);

        _library.AddExam(ExamCatalog.Simulator, targetPath, Path.GetFileNameWithoutExtension(targetPath));
        await LoadLibraryAsync();
        return new LibraryActionResult(LibraryActionStatus.Success, targetPath);
    }

    public async Task RemoveExamAsync(string filePath)
    {
        _library.RemoveExam(ExamCatalog.Simulator, filePath);
        await LoadLibraryAsync();
    }

    public async Task<LibraryActionResult> DuplicateExamAsync(string filePath)
    {
        if (!await _fileSystem.ExistsAsync(filePath))
            return new LibraryActionResult(LibraryActionStatus.FileNotFound);

        var directory = Path.GetDirectoryName(filePath) ?? _appPaths.DocumentsDirectory;
        var name = Path.GetFileNameWithoutExtension(filePath);
        var extension = Path.GetExtension(filePath);

        try
        {
            var copyPath = GetUniquePath(directory, $"{name} (copy)", extension);
            await using (var source = await _fileSystem.OpenReadAsync(filePath))
            await using (var target = await _fileSystem.OpenWriteAsync(copyPath))
            {
                await source.CopyToAsync(target);
            }

            _library.AddExam(ExamCatalog.Simulator, copyPath, Path.GetFileNameWithoutExtension(copyPath));
            await LoadLibraryAsync();
            return new LibraryActionResult(LibraryActionStatus.Success, copyPath);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            _logger.LogWarning(ex, "Could not duplicate '{FilePath}'.", filePath);
            return new LibraryActionResult(LibraryActionStatus.WriteFailed);
        }
    }

    public async Task<LibraryActionResult> RelocateExamAsync(string missingFilePath, string newFilePath)
    {
        if (!IsOefPath(newFilePath))
            return new LibraryActionResult(LibraryActionStatus.UnsupportedFormat);

        var load = _examFileLoader.TryLoad(newFilePath);
        if (load.Error == ExamFileLoadError.FileNotFound)
            return new LibraryActionResult(LibraryActionStatus.FileNotFound);
        if (!load.Success)
            return new LibraryActionResult(LibraryActionStatus.CorruptFile);

        _library.RemoveExam(ExamCatalog.Simulator, missingFilePath);
        _library.AddExam(ExamCatalog.Simulator, newFilePath, Path.GetFileNameWithoutExtension(newFilePath));
        await LoadLibraryAsync();
        return new LibraryActionResult(LibraryActionStatus.Success, newFilePath);
    }

    public Task<ExamFileProperties?> GetExamPropertiesAsync(string filePath)
    {
        var info = new FileInfo(filePath);
        if (!info.Exists)
            return Task.FromResult<ExamFileProperties?>(null);

        var load = _examFileLoader.TryLoad(filePath);
        if (!load.Success || load.Exam == null)
            return Task.FromResult<ExamFileProperties?>(null);

        var exam = load.Exam;
        var isLegacy = string.Equals(info.Extension, ".oef", StringComparison.OrdinalIgnoreCase)
                       && _reader.FromOefFile(filePath).IsLegacy;

        return Task.FromResult<ExamFileProperties?>(new ExamFileProperties(
            filePath,
            exam.Properties.Title,
            exam.Properties.Code,
            exam.Properties.Version,
            info.Length,
            info.LastWriteTime,
            exam.NumberOfQuestions,
            exam.Sections.Count,
            exam.Properties.TimeLimit,
            ToPercent(exam.Properties.Passmark),
            exam.Properties.HideAnswers,
            isLegacy));
    }

    private static bool IsOefPath(string filePath) =>
        string.Equals(Path.GetExtension(filePath), ".oef", StringComparison.OrdinalIgnoreCase);

    private static bool PathsEqual(string left, string right) =>
        string.Equals(
            Path.GetFullPath(left),
            Path.GetFullPath(right),
            OperatingSystem.IsWindows() || OperatingSystem.IsMacOS()
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal);

    private static string GetUniquePath(string directory, string baseName, string extension)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var safe = new string(baseName.Select(c => invalid.Contains(c) ? '_' : c).ToArray()).Trim();
        if (safe.Length == 0)
            safe = "Exam";

        var candidate = Path.Combine(directory, safe + extension);
        for (var i = 2; File.Exists(candidate); i++)
            candidate = Path.Combine(directory, $"{safe} {i}{extension}");

        return candidate;
    }

    private async Task LoadLibraryAsync()
    {
        TransitionTo(new LibraryState([], [], null, null, true));

        var exams = _library.GetExams(ExamCatalog.Simulator);
        var examCards = new List<ExamCard>();
        var recentAttempts = new List<AttemptSummary>();

        foreach (var entry in exams)
        {
            if (!IsOefPath(entry.FilePath))
            {
                examCards.Add(new ExamCard(
                    entry.FilePath,
                    entry.Name,
                    string.Empty,
                    0,
                    0,
                    0,
                    0,
                    null,
                    null,
                    null,
                    false,
                    true));
                continue;
            }

            var loadResult = _examFileLoader.TryLoad(entry.FilePath);
            if (loadResult.Success && loadResult.Exam != null)
            {
                var exam = loadResult.Exam;
                var attempts = _library.GetAttempts(entry.FilePath);
                var lastAttempt = attempts.OrderByDescending(a => a.TakenAt).FirstOrDefault();

                examCards.Add(new ExamCard(
                    entry.FilePath,
                    exam.Properties.Title,
                    exam.Properties.Code,
                    exam.NumberOfQuestions,
                    exam.Sections.Count,
                    exam.Properties.TimeLimit,
                    ToPercent(exam.Properties.Passmark),
                    lastAttempt?.TakenAt,
                    lastAttempt != null ? lastAttempt.Correct * 100 / lastAttempt.Total : null,
                    lastAttempt != null ? lastAttempt.Passed : null,
                    false,
                    false));
            }
            else
            {
                examCards.Add(new ExamCard(
                    entry.FilePath,
                    entry.Name,
                    string.Empty,
                    0,
                    0,
                    0,
                    0,
                    null,
                    null,
                    null,
                    loadResult.Error == ExamFileLoadError.FileNotFound,
                    loadResult.Error != ExamFileLoadError.FileNotFound && loadResult.Error != ExamFileLoadError.None));
            }
        }

        // Get recent attempts across all exams
        var allAttempts = examCards
            .Where(c => !c.IsMissing && !c.IsCorrupt)
            .SelectMany(c => _library.GetAttempts(c.FilePath)
                .Select(a => new AttemptSummary(
                    c.FilePath,
                    c.Title,
                    c.Code,
                    a.TakenAt,
                    TimeSpan.FromSeconds(a.TimeUsedSeconds),
                    a.Correct * 100 / a.Total,
                    a.Score,
                    a.Passed)))
            .OrderByDescending(a => a.Date)
            .Take(10)
            .ToList();

        TransitionTo(new LibraryState(examCards, allAttempts, null, null, false));
    }

    public async Task<PreExamState> LoadExamAsync(string examFilePath)
    {
        if (!IsOefPath(examFilePath))
            throw new ExamLoadException(ExamFileLoadError.UnknownOrCorrupt, "Only .oef exam files can be opened.", unsupportedFormat: true);

        var loadResult = _examFileLoader.TryLoad(examFilePath);
        if (!loadResult.Success || loadResult.Exam == null)
        {
            var errorMsg = loadResult.Error switch
            {
                ExamFileLoadError.FileNotFound => "The selected exam file no longer exists.",
                ExamFileLoadError.EmptyOrInvalidJson => "The JSON file is empty or invalid.",
                ExamFileLoadError.EmptyOrInvalidXml => "The XML file is empty or invalid.",
                ExamFileLoadError.InvalidXml => "The XML file is invalid.",
                _ => "The exam file is corrupt or in an unsupported format."
            };
            // The host decides how to present the failure (localised), so no prompt is shown here.
            throw new ExamLoadException(loadResult.Error, errorMsg);
        }

        _currentExam = loadResult.Exam;
        _currentExamFilePath = examFilePath;

        // Add to library history
        _library.AddExam(ExamCatalog.Simulator, examFilePath, Path.GetFileNameWithoutExtension(examFilePath));

        // Load last candidate name from settings
        var lastCandidate = _settings.Get("Simulator.LastCandidateName", AppSettingsType.Other)?.Value ?? string.Empty;

        var settings = new PreExamSettings
        {
            Mode = _currentExam.Properties.HideAnswers ? ExamMode.Exam : ExamMode.Practice,
            CandidateName = lastCandidate,
            QuestionSet = QuestionSetMode.AllQuestions
        };

        var examSummary = CreateExamSummary(_currentExam);
        var (startDisabled, reason) = ValidatePreExamSettings(settings, examSummary);

        _preExamSettings = settings;
        return TransitionTo(new PreExamState(examSummary, settings, startDisabled, reason));
    }

    private ExamSummary CreateExamSummary(Exam exam)
    {
        var sectionCounts = exam.Sections.ToDictionary(s => s.Title, s => s.Questions.Count);
        return new ExamSummary(
            exam.Properties.Title,
            exam.Properties.Code,
            exam.Properties.Instructions,
            exam.NumberOfQuestions,
            exam.Sections.Count,
            exam.Properties.TimeLimit,
            ToPercent(exam.Properties.Passmark),
            exam.Properties.HideAnswers,
            sectionCounts);
    }

    private (bool disabled, string? reason) ValidatePreExamSettings(PreExamSettings settings, ExamSummary exam)
    {
        if (settings.WouldYieldZeroQuestions(exam))
        {
            return (true, "This selection would contain zero questions. Please choose a different set.");
        }

        if (settings.Mode == ExamMode.Practice && exam.HideAnswers)
        {
            return (true, "The author has hidden answers for this exam. Practice mode is unavailable.");
        }

        return (false, null);
    }

    public async Task UpdatePreExamSettingsAsync(PreExamSettings settings)
    {
        if (_preExamSettings == null || _currentExam == null)
            throw new InvalidOperationException("No exam loaded.");

        if (settings.QuestionSet != QuestionSetMode.MissedQuestions)
            _missedQuestionSelection = null;

        _preExamSettings = settings with { };
        var examSummary = CreateExamSummary(_currentExam);
        var (startDisabled, reason) = ValidatePreExamSettings(settings, examSummary);

        TransitionTo(new PreExamState(examSummary, settings, startDisabled, reason));
    }

    public async Task<AttemptState> StartAttemptAsync()
    {
        if (_preExamSettings == null || _currentExam == null)
            throw new InvalidOperationException("Pre-exam settings not configured.");

        var (startDisabled, startDisabledReason) = ValidatePreExamSettings(
            _preExamSettings,
            CreateExamSummary(_currentExam));
        if (startDisabled)
            throw new InvalidOperationException(startDisabledReason ?? "The attempt cannot be started with these settings.");

        // Save candidate name for next time
        _settings.Set(new AppSetting { Key = "Simulator.LastCandidateName", Value = _preExamSettings.CandidateName }, AppSettingsType.Other);

        // Resolve the attempt seed before composition so Random N and both shuffle
        // operations are deterministic for a non-zero configured seed.
        _randomSeed = _preExamSettings.RandomSeed > 0 ? _preExamSettings.RandomSeed : Random.Shared.Next(1, int.MaxValue);
        _random = new Random(_randomSeed);

        // Compose the question set.
        var selection = ComposeQuestionSet(_currentExam, _preExamSettings);
        var questions = selection.Questions;
        var sections = selection.Sections;

        if (questions.Count == 0)
            throw new InvalidOperationException("No questions available for this selection.");

        if (_preExamSettings.ShuffleQuestions)
            ShuffleInPlace(questions, _random);

        // Renumber questions sequentially
        for (int i = 0; i < questions.Count; i++)
            questions[i].No = i + 1;

        // Initialize answer selections
        var answers = new Dictionary<int, AnswerSelection>();
        for (int i = 0; i < questions.Count; i++)
            answers[i] = AnswerSelection.Unanswered();

        // Setup timer
        int timeLimitMinutes = _preExamSettings.Mode == ExamMode.Exam
            ? (_preExamSettings.TimerOverrideMinutes > 0 ? _preExamSettings.TimerOverrideMinutes : _currentExam.Properties.TimeLimit)
            : 0;

        var timeRemaining = TimeSpan.FromMinutes(timeLimitMinutes);
        _attemptStartTime = _timeProvider.GetUtcNow();
        _accumulatedPausedTime = TimeSpan.Zero;

        // Shuffle options if requested. The answer map must be captured before alphabets are reassigned.
        if (_preExamSettings.ShuffleOptions)
        {
            foreach (var q in questions)
            {
                var originalAlphabets = q.Options.Select(o => o.Alphabet).ToList();
                var shuffled = q.Options.OrderBy(_ => _random!.Next()).ToList();

                // Build map from original alphabet to its new position.
                var answerMap = new Dictionary<char, char>();
                for (int i = 0; i < shuffled.Count; i++)
                {
                    answerMap[shuffled[i].Alphabet] = originalAlphabets[i];
                    shuffled[i] = new Option { Alphabet = originalAlphabets[i], Text = shuffled[i].Text };
                }

                q.Options = shuffled;

                if (q.IsMultipleChoice)
                {
                    q.Answers = q.Answers.Select(a => answerMap[a]).ToArray();
                }
                else
                {
                    if (answerMap.TryGetValue(q.Answer, out var newAnswer))
                        q.Answer = newAnswer;
                }
            }
        }

        _attemptState = new AttemptState(
            _currentExam,
            _preExamSettings,
            questions,
            sections,
            0,
            answers,
            timeRemaining,
            _preExamSettings.Mode == ExamMode.Exam,
            false,
            false,
            0,
            0,
            questions.Count);

        StartTimer();
        return TransitionTo(_attemptState);
    }

    /// <summary>
    /// Pass marks are stored on the 0-1000 scaled-score scale. The UI-facing models use percent.
    /// </summary>
    private static double ToPercent(double scaledPassMark) => scaledPassMark / 10d;

    private static void ShuffleInPlace<T>(IList<T> items, Random random)
    {
        for (var i = items.Count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (items[i], items[j]) = (items[j], items[i]);
        }
    }

    private SelectionResult ComposeQuestionSet(Exam exam, PreExamSettings settings)
    {
        if (settings.QuestionSet == QuestionSetMode.SelectedSections)
        {
            var selectedSections = exam.Sections.Where(s => settings.SelectedSectionTitles.Contains(s.Title)).ToList();
            return _composer.SelectSections(exam, selectedSections);
        }
        else if (settings.QuestionSet == QuestionSetMode.RandomN)
        {
            return _composer.SelectRandomQuestions(exam, settings.RandomQuestionCount, _randomSeed);
        }
        else if (settings.QuestionSet == QuestionSetMode.MissedQuestions)
        {
            if (_missedQuestionSelection != null)
                return CloneSelection(_missedQuestionSelection);

            return ComposeMissedQuestions(exam, settings.MissedQuestionIndices);
        }
        else
        {
            // All questions
            var allSections = exam.Sections.Select(CloneSection).ToList();
            var allQuestions = allSections.SelectMany(s => s.Questions).ToList();
            return new SelectionResult(allSections, allQuestions);
        }
    }

    private static SelectionResult ComposeMissedQuestions(Exam exam, ImmutableArray<int> missedIndices)
    {
        var missedSet = new HashSet<int>(missedIndices);
        var allQuestions = exam.Sections.SelectMany(s => s.Questions).ToList();
        var selectedQuestions = missedIndices
            .Where(i => i >= 0 && i < allQuestions.Count)
            .Select(i => CloneQuestion(allQuestions[i]))
            .ToList();

        var index = 0;
        var selectedSections = new List<Section>();
        foreach (var section in exam.Sections)
        {
            var sectionQuestions = new List<Question>();
            foreach (var _ in section.Questions)
            {
                if (missedSet.Contains(index))
                    sectionQuestions.Add(CloneQuestion(allQuestions[index]));
                index++;
            }

            if (sectionQuestions.Count > 0)
                selectedSections.Add(new Section { Title = section.Title, Questions = sectionQuestions });
        }

        return new SelectionResult(selectedSections, selectedQuestions);
    }

    private static SelectionResult CloneSelection(SelectionResult selection)
    {
        var sections = selection.Sections.Select(CloneSection).ToList();
        return new SelectionResult(sections, sections.SelectMany(s => s.Questions).ToList());
    }

    private static Section CloneSection(Section section)
    {
        return new Section
        {
            Title = section.Title,
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
            Answers = question.Answers?.ToArray() ?? [],
            Options = question.Options.Select(o => new Option { Alphabet = o.Alphabet, Text = o.Text }).ToList(),
            Explanation = question.Explanation
        };
    }

    private ITimer? _timer;

    private void StartTimer()
    {
        if (_preExamSettings?.Mode != ExamMode.Exam || _attemptState == null)
            return;

        _timer?.Dispose();
        _timer = _timeProvider.CreateTimer(
            _ => UpdateTimer(),
            null,
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1));
    }

    private void UpdateTimer()
    {
        if (_attemptState == null || _preExamSettings?.Mode != ExamMode.Exam)
            return;

        var now = _timeProvider.GetUtcNow();
        var elapsed = now - _attemptStartTime - _accumulatedPausedTime;
        var timeLimit = TimeSpan.FromMinutes(_attemptState.Settings.Mode == ExamMode.Exam
            ? (_attemptState.Settings.TimerOverrideMinutes > 0 ? _attemptState.Settings.TimerOverrideMinutes : _attemptState.Exam.Properties.TimeLimit)
            : 0);

        var remaining = timeLimit - elapsed;
        if (remaining <= TimeSpan.Zero)
        {
            _timer?.Dispose();
            _timer = null;
            _ = HandleTimeUpAsync();
            return;
        }

        var isLowTime = remaining <= TimeSpan.FromMinutes(5);
        var isCriticalTime = remaining <= TimeSpan.FromMinutes(1);

        _attemptState = _attemptState with
        {
            TimeRemaining = remaining,
            IsLowTimeWarning = isLowTime,
            IsCriticalTimeWarning = isCriticalTime
        };

        // The clock keeps running while the candidate reviews, but a tick must never drag the
        // visible state back to the attempt view.
        switch (_currentState)
        {
            case AttemptState:
                TransitionTo(_attemptState);
                break;
            case ReviewAndSubmitState review:
                TransitionTo(review with { Attempt = _attemptState });
                break;
        }
    }

    public async Task NavigateToQuestionAsync(int questionIndex)
    {
        if (_attemptState == null || questionIndex < 0 || questionIndex >= _attemptState.Questions.Count)
            return;

        _attemptState = _attemptState with { CurrentQuestionIndex = questionIndex };
        TransitionTo(_attemptState);
    }

    public async Task NavigateNextAsync()
    {
        if (_attemptState == null)
            return;

        var nextIndex = _attemptState.CurrentQuestionIndex + 1;
        if (nextIndex < _attemptState.Questions.Count)
        {
            await NavigateToQuestionAsync(nextIndex);
        }
    }

    public async Task NavigatePreviousAsync()
    {
        if (_attemptState == null)
            return;

        var prevIndex = _attemptState.CurrentQuestionIndex - 1;
        if (prevIndex >= 0)
        {
            await NavigateToQuestionAsync(prevIndex);
        }
    }

    public async Task AnswerQuestionAsync(AnswerSelection selection)
    {
        if (_attemptState == null)
            return;

        var currentIndex = _attemptState.CurrentQuestionIndex;
        var wasAnswered = _attemptState.Answers.TryGetValue(currentIndex, out var oldSelection) && oldSelection.IsAnswered;
        if (oldSelection.IsLocked)
        {
            if (!HasSameChoices(oldSelection, selection))
                throw new InvalidOperationException("This Practice answer is locked after checking.");

            selection = selection with
            {
                ExplanationRevealed = true,
                IsLocked = true
            };
        }

        var isNowAnswered = selection.IsAnswered;

        var newAnswers = _attemptState.Answers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        newAnswers[currentIndex] = selection;

        int answeredDelta = (isNowAnswered ? 1 : 0) - (wasAnswered ? 1 : 0);
        int flaggedDelta = (selection.IsFlagged ? 1 : 0) - (oldSelection.IsFlagged ? 1 : 0);

        _attemptState = _attemptState with
        {
            Answers = newAnswers,
            AnsweredCount = _attemptState.AnsweredCount + answeredDelta,
            FlaggedCount = _attemptState.FlaggedCount + flaggedDelta,
            UnansweredCount = _attemptState.Questions.Count - (_attemptState.AnsweredCount + answeredDelta)
        };

        TransitionTo(_attemptState);
    }

    private static bool HasSameChoices(AnswerSelection left, AnswerSelection right) =>
        left.State == right.State
        && left.SingleChoice == right.SingleChoice
        && left.MultipleChoices.SequenceEqual(right.MultipleChoices);

    public async Task ToggleFlagAsync()
    {
        if (_attemptState == null)
            return;

        var currentIndex = _attemptState.CurrentQuestionIndex;
        var currentSelection = _attemptState.Answers.TryGetValue(currentIndex, out var sel) ? sel : AnswerSelection.Unanswered();
        var newSelection = currentSelection with { IsFlagged = !currentSelection.IsFlagged };

        await AnswerQuestionAsync(newSelection);
    }

    public async Task<AnswerSelection> CheckAnswerAsync()
    {
        if (_attemptState == null || _preExamSettings?.Mode != ExamMode.Practice)
            throw new InvalidOperationException("Check answer is only available in Practice mode.");

        var currentIndex = _attemptState.CurrentQuestionIndex;
        var currentSelection = _attemptState.Answers.TryGetValue(currentIndex, out var sel) ? sel : AnswerSelection.Unanswered();

        if (!currentSelection.IsAnswered)
            throw new InvalidOperationException("Cannot check answer for unanswered question.");

        if (currentSelection.IsLocked)
            return currentSelection;

        var newSelection = AnswerSelection.WithExplanationRevealed(currentSelection);
        await AnswerQuestionAsync(newSelection);

        return newSelection;
    }

    public async Task PauseAsync()
    {
        if (_attemptState == null || _preExamSettings?.Mode != ExamMode.Exam)
            return;

        _timer?.Dispose();
        _timer = null;
        _pauseStartTime = _timeProvider.GetUtcNow();

        var pausedState = new PausedState(_attemptState);
        TransitionTo(pausedState);
    }

    public async Task ResumeAsync()
    {
        if (_currentState.Kind != SessionStateKind.Paused || _attemptState == null)
            return;

        if (_pauseStartTime.HasValue)
        {
            var pauseDuration = _timeProvider.GetUtcNow() - _pauseStartTime.Value;
            _accumulatedPausedTime += pauseDuration;
            _pauseStartTime = null;
        }

        StartTimer();
        TransitionTo(_attemptState);
    }

    public Task NotifySystemSuspendingAsync()
    {
        if (_attemptState == null || _preExamSettings?.Mode != ExamMode.Exam || _systemSuspendStartTime.HasValue)
            return Task.CompletedTask;

        _timer?.Dispose();
        _timer = null;
        _systemSuspendStartTime = _timeProvider.GetUtcNow();
        return Task.CompletedTask;
    }

    public Task NotifySystemResumedAsync()
    {
        if (_attemptState == null || !_systemSuspendStartTime.HasValue)
            return Task.CompletedTask;

        _accumulatedPausedTime += _timeProvider.GetUtcNow() - _systemSuspendStartTime.Value;
        _systemSuspendStartTime = null;

        if (_currentState.Kind == SessionStateKind.Attempt)
            StartTimer();

        return Task.CompletedTask;
    }

    public async Task EnterReviewAndSubmitAsync()
    {
        if (_attemptState == null)
            return;

        var unansweredIndices = _attemptState.Answers
            .Where(kvp => !kvp.Value.IsAnswered)
            .Select(kvp => kvp.Key)
            .ToList();

        var flaggedIndices = _attemptState.Answers
            .Where(kvp => kvp.Value.IsFlagged)
            .Select(kvp => kvp.Key)
            .ToList();

        var reviewState = new ReviewAndSubmitState(_attemptState, unansweredIndices, flaggedIndices, false);
        TransitionTo(reviewState);
    }

    public async Task ReturnToAttemptAsync()
    {
        if (_currentState.Kind != SessionStateKind.ReviewAndSubmit || _attemptState == null)
            return;

        TransitionTo(_attemptState);
    }

    public async Task<ResultsState> SubmitAsync()
    {
        if (_attemptState == null || _preExamSettings == null || _currentExam == null)
            throw new InvalidOperationException("No active attempt.");

        _timer?.Dispose();
        _timer = null;

        var gradingDetails = BuildGradingDetails(_attemptState);
        var userAnswers = gradingDetails.Select(d => d.GradingAnswer).ToArray();

        var grading = _scorer.Grade(userAnswers, _attemptState.Questions.ToList(), _attemptState.Sections.ToList());
        var normalizedScore = _scorer.ComputeNormalizedScore(grading.NumberOfCorrectAnswers, _attemptState.Questions.Count);
        var passed = _scorer.IsPassed(normalizedScore, _currentExam.Properties.Passmark);

        var elapsed = _timeProvider.GetUtcNow() - _attemptStartTime - _accumulatedPausedTime;

        // Get previous attempt for comparison.
        var previousAttempts = _library.GetAttempts(_currentExamFilePath);
        var lastAttempt = previousAttempts.OrderByDescending(a => a.TakenAt).FirstOrDefault();

        var resultsState = new ResultsState(
            _currentExam,
            _preExamSettings,
            _preExamSettings.CandidateName,
            DateTime.UtcNow,
            elapsed,
            _attemptState.Questions.Count,
            grading.NumberOfCorrectAnswers,
            (double)grading.NumberOfCorrectAnswers / _attemptState.Questions.Count * 100,
            normalizedScore,
            ToPercent(_currentExam.Properties.Passmark),
            passed,
            grading.ResultSpread,
            lastAttempt != null ? lastAttempt.Correct * 100 / lastAttempt.Total : null,
            lastAttempt?.Score,
            lastAttempt?.TakenAt,
            false,
            _attemptState.Questions,
            _attemptState.Sections,
            _attemptState.Answers,
            gradingDetails);

        // Save attempt.
        var attempt = new ExamAttempt
        {
            ExamFilePath = _currentExamFilePath,
            CandidateName = _preExamSettings.CandidateName,
            Score = normalizedScore,
            Passed = passed,
            Correct = grading.NumberOfCorrectAnswers,
            Total = _attemptState.Questions.Count,
            TimeUsedSeconds = elapsed.TotalSeconds,
            TakenAt = DateTime.UtcNow
        };
        _library.SaveAttempt(attempt);

        _attemptState = null;
        return TransitionTo(resultsState);
    }

    private static IReadOnlyList<GradingDetail> BuildGradingDetails(AttemptState attempt)
    {
        var hideAnswers = attempt.Exam.Properties.HideAnswers;
        var details = new List<GradingDetail>();

        for (int i = 0; i < attempt.Questions.Count; i++)
        {
            var question = attempt.Questions[i];
            var selection = attempt.Answers.TryGetValue(i, out var sel) ? sel : AnswerSelection.Unanswered();
            var userAnswer = selection.ToCharArray();

            object? gradingAnswer = userAnswer.Length == 0 ? null :
                question.IsMultipleChoice ? userAnswer :
                userAnswer.Length == 1 ? userAnswer[0] : null;

            var isCorrect = gradingAnswer != null && Grader.Grade([gradingAnswer], new List<Question> { question }, attempt.Sections.ToList()).NumberOfCorrectAnswers == 1;

            details.Add(new GradingDetail(
                i,
                question,
                selection,
                gradingAnswer,
                isCorrect,
                hideAnswers ? null : (question.IsMultipleChoice ? null : question.Answer),
                hideAnswers ? null : (question.IsMultipleChoice ? question.Answers : null),
                hideAnswers ? null : question.Explanation,
                hideAnswers));
        }

        return details;
    }

    public async Task HandleTimeUpAsync()
    {
        if (_attemptState == null)
            return;

        _timer?.Dispose();
        _timer = null;

        // Auto-transition to review and submit.
        await EnterReviewAndSubmitAsync();

        // Start auto-submit countdown (6 seconds as per design).
        var timeUpState = new TimeUpState(_attemptState, 6);
        TransitionTo(timeUpState);

        // Countdown using a timer so FakeTimeProvider can drive it in tests.
        var countdown = 6;
        var countdownTimer = _timeProvider.CreateTimer(
            _ =>
            {
                countdown--;
                timeUpState = timeUpState with { AutoSubmitCountdownSeconds = Math.Max(0, countdown) };
                TransitionTo(timeUpState);

                if (countdown <= 0)
                {
                    _timer?.Dispose();
                    _timer = null;
                    _ = SubmitAfterTimeUpAsync();
                }
            },
            null,
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1));

        // Keep a reference so the timer is not GC'd before it fires.
        _timer = countdownTimer;
    }

    private async Task SubmitAfterTimeUpAsync()
    {
        try
        {
            await SubmitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Automatic submission failed after time expired.");
        }
    }

    public async Task<AnswerReviewState> EnterAnswerReviewAsync(AnswerReviewFilter filter = AnswerReviewFilter.All)
    {
        if (_currentState is not ResultsState resultsState)
            throw new InvalidOperationException("Not in results state.");

        var questions = resultsState.PresentedQuestions ?? resultsState.Exam.Sections.SelectMany(s => s.Questions).ToList();
        var sections = resultsState.PresentedSections ?? resultsState.Exam.Sections;
        var answers = resultsState.Answers ?? new Dictionary<int, AnswerSelection>();
        var gradingDetails = resultsState.GradingDetails ?? [];

        _resultsBeforeReview = resultsState;
        var reviewState = new AnswerReviewState(
            resultsState.Exam,
            resultsState.Settings,
            questions,
            sections,
            answers,
            gradingDetails,
            filter,
            0);

        return TransitionTo(reviewState);
    }

    public Task ReturnToResultsAsync()
    {
        if (_currentState is AnswerReviewState && _resultsBeforeReview != null)
            TransitionTo(_resultsBeforeReview);

        return Task.CompletedTask;
    }

    public async Task NavigateReviewAsync(int direction)
    {
        if (_currentState is not AnswerReviewState reviewState)
            return;

        var newIndex = reviewState.CurrentReviewIndex + direction;
        if (newIndex >= 0 && newIndex < reviewState.FilteredCount)
        {
            reviewState = reviewState with { CurrentReviewIndex = newIndex };
            TransitionTo(reviewState);
        }
    }

    public async Task SetReviewFilterAsync(AnswerReviewFilter filter)
    {
        if (_currentState is not AnswerReviewState reviewState)
            return;

        reviewState = reviewState with { Filter = filter, CurrentReviewIndex = 0 };
        TransitionTo(reviewState);
    }

    public async Task<PreExamState> RetakeAsync()
    {
        if (_currentExam == null || _preExamSettings == null)
            throw new InvalidOperationException("No exam to retake.");

        // Return to pre-exam sheet with same settings
        // If Random was selected, generate new seed
        var newSettings = _preExamSettings.QuestionSet == QuestionSetMode.RandomN
            ? _preExamSettings with { RandomSeed = 0 }
            : _preExamSettings;

        _preExamSettings = newSettings;
        var examSummary = CreateExamSummary(_currentExam);
        var (startDisabled, reason) = ValidatePreExamSettings(newSettings, examSummary);

        return TransitionTo(new PreExamState(examSummary, newSettings, startDisabled, reason));
    }

    public async Task ReturnToLibraryAsync()
    {
        _timer?.Dispose();
        _timer = null;
        _attemptState = null;
        _currentExam = null;
        _preExamSettings = null;
        _missedQuestionSelection = null;
        _resultsBeforeReview = null;
        await LoadLibraryAsync();
    }

    public Task<byte[]> ExportResultsPdfAsync()
    {
        if (_currentState is not ResultsState results)
            throw new InvalidOperationException("Results are only available after an attempt is submitted.");

        using var stream = new MemoryStream();
        var written = _writer.ToResultsPdf(BuildReport(results), stream, ResultsLabels);
        if (!written.Success)
            throw new IOException("The results report could not be generated.");

        return Task.FromResult(stream.ToArray());
    }

    public async Task PrintResultsAsync()
    {
        if (_currentState is not ResultsState results)
            return;

        var bytes = await ExportResultsPdfAsync();
        using var stream = new MemoryStream(bytes);
        await _printService.PrintAsync(results.Exam.Properties.Title, stream);
    }

    /// <summary>
    /// Captions used on the exported report. The host may replace these with localised text.
    /// </summary>
    public ResultsReportLabels ResultsLabels { get; set; } = new();

    private static ResultsReport BuildReport(ResultsState results)
    {
        var details = results.GradingDetails ?? [];
        var questions = details
            .Select(d => new ResultsReportQuestion(
                d.QuestionIndex + 1,
                d.IsCorrect
                    ? ResultsReportOutcome.Correct
                    : d.IsAnswered ? ResultsReportOutcome.Wrong : ResultsReportOutcome.Unanswered,
                d.Question.Text))
            .ToList();

        var limit = results.Settings.Mode == ExamMode.Exam
            ? (results.Settings.TimerOverrideMinutes > 0
                ? results.Settings.TimerOverrideMinutes
                : results.Exam.Properties.TimeLimit)
            : 0;

        return new ResultsReport(
            results.Exam.Properties.Title,
            results.Exam.Properties.Code,
            results.CandidateName,
            results.CompletedAt,
            results.ElapsedTime,
            limit > 0 ? TimeSpan.FromMinutes(limit) : null,
            results.TotalQuestions,
            results.CorrectAnswers,
            results.PercentScore,
            results.ScaledScore,
            (int)Math.Round(results.PassMarkPercent * 10),
            results.Passed,
            results.SectionBreakdown,
            questions);
    }

    public async Task<PreExamState> PracticeMissedAgainAsync()
    {
        if (_currentState is not AnswerReviewState reviewState || _currentExam == null || _preExamSettings == null)
            throw new InvalidOperationException("Not in answer review state.");

        var missedIndices = reviewState.GradingDetails
            .Where(d => !d.IsCorrect)
            .Select(d => d.QuestionIndex)
            .Distinct()
            .ToImmutableArray();

        var missedQuestions = reviewState.GradingDetails
            .Where(d => !d.IsCorrect)
            .Select(d => d.Question)
            .ToHashSet(ReferenceEqualityComparer.Instance);
        var missedSections = reviewState.Sections
            .Select(section => new Section
            {
                Title = section.Title,
                Questions = section.Questions
                    .Where(missedQuestions.Contains)
                    .Select(CloneQuestion)
                    .ToList()
            })
            .Where(section => section.Questions.Count > 0)
            .ToList();
        _missedQuestionSelection = new SelectionResult(
            missedSections,
            missedSections.SelectMany(section => section.Questions).ToList());

        var newSettings = _preExamSettings with
        {
            Mode = ExamMode.Practice,
            QuestionSet = QuestionSetMode.MissedQuestions,
            MissedQuestionIndices = missedIndices,
            RandomSeed = 0,
            ShuffleQuestions = false,
            ShuffleOptions = false
        };

        _preExamSettings = newSettings;
        var examSummary = CreateExamSummary(_currentExam);
        var (startDisabled, reason) = ValidatePreExamSettings(newSettings, examSummary);

        return TransitionTo(new PreExamState(examSummary, newSettings, startDisabled, reason));
    }

    private T TransitionTo<T>(T newState) where T : ISessionState
    {
        _currentState = newState;
        StateChanged?.Invoke(newState);
        return newState;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}