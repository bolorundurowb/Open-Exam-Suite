using Microsoft.Extensions.DependencyInjection;

namespace OpenExamSuite.Creator;

public static class AppHost
{
    public static IServiceProvider? Services { get; set; }

    public static IReadOnlyList<string> StartupPaths { get; set; } = [];

    public static T Get<T>() where T : notnull => Services!.GetRequiredService<T>();
}
