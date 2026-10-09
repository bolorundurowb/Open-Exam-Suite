using Microsoft.Extensions.DependencyInjection;
using OpenExamSuite.Simulator.Services;

namespace OpenExamSuite.Simulator;

/// <summary>
/// Start-up state handed from <see cref="Program"/> to the Avalonia application.
/// </summary>
public static class AppHost
{
    public static IServiceProvider? Services { get; set; }

    /// <summary>Exam paths from the command line.</summary>
    public static IReadOnlyList<string> StartupPaths { get; set; } = [];

    /// <summary>Receives files forwarded by later launches. Null when the pipe could not be created.</summary>
    public static SingleInstanceCoordinator? Coordinator { get; set; }

    public static T Get<T>() where T : notnull => Services!.GetRequiredService<T>();
}
