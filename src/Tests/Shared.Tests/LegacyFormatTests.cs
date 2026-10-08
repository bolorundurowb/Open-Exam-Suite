using OpenExamSuite.Shared.Utilities;
using OmniAssert;
using Xunit;

namespace OpenExamSuite.Shared.Tests;

public class LegacyFormatTests
{
    private readonly Reader _reader = new();
    private readonly Writer _writer = new();

    private static string LegacyFixturePath =>
        Path.Combine(AppContext.BaseDirectory, "Fixtures", "BasicScience.legacy.oef");

    private static string SamplesDirectory =>
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "..", "samples"));

    [Fact]
    public void FromOefFile_LegacySample_ReadsWithoutWriting()
    {
        var before = File.ReadAllBytes(LegacyFixturePath);

        var result = _reader.FromOefFile(LegacyFixturePath);

        result.Success.Must().BeTrue();
        result.IsLegacy.Must().BeTrue();

        var exam = result.Exam;
        exam.Must().NotBeNull();
        exam!.Properties.Title.Must().Be("Basic Science Primary 2A");
        exam.NumberOfQuestions.Must().Be(8);

        // Reading a legacy payload must never rewrite the source path.
        File.ReadAllBytes(LegacyFixturePath).SequenceEqual(before).Must().BeTrue();
    }

    [Fact]
    public void FromOefFile_LegacySample_ImageMemberIsNull_NoBytesDropped()
    {
        // Inventory finding (2026-10-08): the legacy NRBF payload stores question images as a
        // System.Drawing.Bitmap in '<Image>k__BackingField'. The bundled legacy sample carries a
        // null image on every question, so the mapper has no bytes to preserve. This test pins
        // that finding: if a legacy payload ever ships with non-null image data, this must be
        // revisited so the bytes are not dropped.
        var result = _reader.FromOefFile(LegacyFixturePath);

        result.Success.Must().BeTrue();
        result.Exam!.Sections
            .SelectMany(s => s.Questions)
            .All(q => q.ImageData == null)
            .Must().BeTrue();
    }

    [Fact]
    public void FromOefFile_ReadOnlyLegacySample_OpensWithoutError()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"oes-readonly-{Guid.NewGuid():N}");
        var tempFile = Path.Combine(tempDir, "sample.oef");
        Directory.CreateDirectory(tempDir);
        try
        {
            File.Copy(LegacyFixturePath, tempFile);
            File.SetAttributes(tempFile, File.GetAttributes(tempFile) | FileAttributes.ReadOnly);

            var result = _reader.FromOefFile(tempFile);

            result.Success.Must().BeTrue();
            result.IsLegacy.Must().BeTrue();
        }
        finally
        {
            File.SetAttributes(tempFile, File.GetAttributes(tempFile) & ~FileAttributes.ReadOnly);
            if (File.Exists(tempFile))
                File.Delete(tempFile);
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir);
        }
    }

    [Fact]
    public void FromOef_Stream_ReadsProtobufWithoutSideEffect()
    {
        var exam = new Exam
        {
            Properties = new Properties { Title = "Streamed", Code = "S1" }
        };
        exam.AddQuestion("S", new Question { Text = "Q?" });

        using var stream = new MemoryStream();
        _writer.ToOef(exam, stream).Success.Must().BeTrue();

        stream.Position = 0;
        var result = _reader.FromOef(stream);

        result.Success.Must().BeTrue();
        result.IsLegacy.Must().BeFalse();
        result.Exam!.Properties.Title.Must().Be("Streamed");
    }

    [Fact]
    public void FromOef_Stream_LegacyBytes_ReportLegacyFlag()
    {
        var bytes = File.ReadAllBytes(LegacyFixturePath);
        using var stream = new MemoryStream(bytes);

        var result = _reader.FromOef(stream);

        result.Success.Must().BeTrue();
        result.IsLegacy.Must().BeTrue();
        result.Exam!.NumberOfQuestions.Must().Be(8);
    }

    [Fact]
    public void BundledSamples_AreProtobuf()
    {
        var sampleFiles = new[]
        {
            Path.Combine(SamplesDirectory, "Basic Science.oef"),
            Path.Combine(SamplesDirectory, "GMAT Sample.oef")
        };

        foreach (var sample in sampleFiles)
        {
            File.Exists(sample).Must().BeTrue();

            var result = _reader.FromOefFile(sample);

            result.Success.Must().BeTrue();
            result.IsLegacy.Must().BeFalse();
        }
    }
}
