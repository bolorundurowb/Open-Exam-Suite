using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using OpenExamSuite.Shared.Enums;
using OpenExamSuite.Shared.Interfaces;
using OpenExamSuite.Shared.Models;
using OpenExamSuite.Shared.Services;
using OpenExamSuite.Shared.Utilities;
using OpenExamSuite.Simulator.Session.HostPorts;
using OpenExamSuite.Simulator.Session.Models;
using OpenExamSuite.Simulator.Session.Services;
using OpenExamSuite.Simulator.Session.States;
using OpenExamSuite.Storage.Enums;
using OpenExamSuite.Storage.Interfaces;
using OpenExamSuite.Storage.Models;

namespace OpenExamSuite.Simulator.Session.Tests;

/// <summary>
/// Shared test fixture for building a configured <see cref="SimulatorSession"/>.
/// </summary>
public sealed class SimulatorSessionTestFixture : IDisposable
{
    private readonly string _tempDirectory;
    private readonly List<string> _createdFiles = [];

    public SimulatorSessionTestFixture()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"oes-session-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDirectory);
        Library.Setup(x => x.GetAttempts(It.IsAny<string>())).Returns(new List<Storage.Models.ExamAttempt>());
    }

    public FakeTimeProvider TimeProvider { get; } = new();

    public Mock<IExamLibraryService> Library { get; } = new();

    public Mock<IAppSettingsService> Settings { get; } = new();

    public Mock<IAppPaths> AppPaths { get; } = new();

    public Mock<IFileSystem> FileSystem { get; } = new();

    public Mock<IPrompts> Prompts { get; } = new();

    public Mock<IUriLauncher> UriLauncher { get; } = new();

    public Mock<IPrintService> PrintService { get; } = new();

    public Mock<IToastService> ToastService { get; } = new();

    public ExamComposer Composer { get; } = new();

    public Scorer Scorer { get; } = new();

    public Reader Reader { get; } = new();

    public ExamFileLoader ExamFileLoader { get; } = new();

    public SimulatorSession CreateSession()
    {
        return new SimulatorSession(
            Library.Object,
            Settings.Object,
            Reader,
            Composer,
            Scorer,
            ExamFileLoader,
            AppPaths.Object,
            FileSystem.Object,
            Prompts.Object,
            UriLauncher.Object,
            PrintService.Object,
            ToastService.Object,
            NullLogger<SimulatorSession>.Instance,
            TimeProvider);
    }

    public string SaveExam(Exam exam)
    {
        var path = Path.Combine(_tempDirectory, $"{Guid.NewGuid():N}.oef");
        new Writer().ToOef(exam, path);
        _createdFiles.Add(path);
        return path;
    }

    public static Exam CreateExam(bool hideAnswers = false, int questionsPerSection = 3)
    {
        var exam = new Exam
        {
            Properties = new Properties
            {
                Title = "Test Exam",
                Code = "TEST",
                Passmark = 700,
                TimeLimit = 10,
                Instructions = "Answer all questions.",
                HideAnswers = hideAnswers
            }
        };

        var sectionA = new Section { Title = "Section A" };
        for (int i = 0; i < questionsPerSection; i++)
        {
            sectionA.Questions.Add(CreateSingleChoiceQuestion(i + 1, 'A'));
        }

        var sectionB = new Section { Title = "Section B" };
        for (int i = 0; i < questionsPerSection; i++)
        {
            sectionB.Questions.Add(CreateSingleChoiceQuestion(i + 1 + questionsPerSection, 'B'));
        }

        exam.Sections.Add(sectionA);
        exam.Sections.Add(sectionB);
        return exam;
    }

    public static Exam CreateExamWithMultipleChoice()
    {
        var exam = new Exam
        {
            Properties = new Properties
            {
                Title = "Multiple Choice Exam",
                Code = "MC",
                Passmark = 700,
                TimeLimit = 10,
                Instructions = "Select all that apply."
            }
        };

        var section = new Section { Title = "Section A" };
        section.Questions.Add(new Question
        {
            No = 1,
            Text = "Pick A and B",
            IsMultipleChoice = true,
            Answers = ['A', 'B'],
            Options =
            [
                new Option { Alphabet = 'A', Text = "First" },
                new Option { Alphabet = 'B', Text = "Second" },
                new Option { Alphabet = 'C', Text = "Third" }
            ],
            Explanation = "A and B are correct."
        });

        exam.Sections.Add(section);
        return exam;
    }

    private static Question CreateSingleChoiceQuestion(int no, char answer)
    {
        return new Question
        {
            No = no,
            Text = $"Question {no}",
            Answer = answer,
            IsMultipleChoice = false,
            Options =
            [
                new Option { Alphabet = 'A', Text = "Alpha" },
                new Option { Alphabet = 'B', Text = "Bravo" },
                new Option { Alphabet = 'C', Text = "Charlie" }
            ],
            Explanation = $"The correct answer is {answer}."
        };
    }

    public static PreExamSettings PracticeSettings() => new() { Mode = ExamMode.Practice };

    public static PreExamSettings ExamSettings(int? overrideMinutes = null) => new()
    {
        Mode = ExamMode.Exam,
        TimerOverrideMinutes = overrideMinutes ?? 0
    };

    public void Dispose()
    {
        foreach (var file in _createdFiles.Where(File.Exists))
        {
            try { File.Delete(file); } catch { /* best effort */ }
        }

        try { Directory.Delete(_tempDirectory, recursive: true); } catch { /* best effort */ }
    }
}
