using System;
using System.IO;
using System.Linq;
using OmniAssert;
using OpenExamSuite.Storage;
using Xunit;

namespace OpenExamSuite.Simulator.Session.Tests;

public class BundledSamplesTests : IDisposable
{
    private readonly string _baseDirectory;

    public BundledSamplesTests()
    {
        _baseDirectory = Path.Combine(Path.GetTempPath(), $"oes-samples-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_baseDirectory);
    }

    public void Dispose()
    {
        try { Directory.Delete(_baseDirectory, recursive: true); } catch { /* best effort */ }
    }

    [Fact]
    public void Resolve_WindowsLinuxInstalledLayout_FindsSamplesBesideAppDirectory()
    {
        var appDir = Path.Combine(_baseDirectory, "Simulator");
        var samplesDir = Path.Combine(_baseDirectory, "Samples");
        Directory.CreateDirectory(appDir);
        Directory.CreateDirectory(samplesDir);
        File.WriteAllText(Path.Combine(samplesDir, BundledSamples.FileNames[0]), "sample");

        var result = BundledSamples.Resolve(appDir);

        result.Count.Must().Be(1);
        result[0].Must().Be(Path.Combine(samplesDir, BundledSamples.FileNames[0]));
    }

    [Fact]
    public void Resolve_MacOSAppBundle_FindsSamplesInContentsResources()
    {
        var appBundle = Path.Combine(_baseDirectory, "OpenExamSuite.app");
        var macOsDir = Path.Combine(appBundle, "Contents", "MacOS");
        var resourcesDir = Path.Combine(appBundle, "Contents", "Resources");
        Directory.CreateDirectory(macOsDir);
        Directory.CreateDirectory(resourcesDir);
        File.WriteAllText(Path.Combine(resourcesDir, BundledSamples.FileNames[0]), "sample");

        var result = BundledSamples.Resolve(macOsDir);

        result.Count.Must().Be(1);
        result[0].Must().Be(Path.Combine(resourcesDir, BundledSamples.FileNames[0]));
    }

    [Fact]
    public void Resolve_NoSamples_ReturnsEmpty()
    {
        var result = BundledSamples.Resolve(_baseDirectory);

        result.Must().BeEmpty();
    }
}
