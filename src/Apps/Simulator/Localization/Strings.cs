using System.Globalization;
using System.Resources;

namespace OpenExamSuite.Simulator.Localization;

/// <summary>
/// Single access point for user-visible text. Every string lives in <c>Resources/Strings.resx</c>
/// so translations can be added as <c>Strings.xx.resx</c> without touching code.
/// Layouts must allow roughly 30% longer text than the English source.
/// </summary>
public static class Strings
{
    private const string BaseName = "OpenExamSuite.Simulator.Resources.Strings";

    private static readonly ResourceManager Manager = new(BaseName, typeof(Strings).Assembly);

    /// <summary>Overrides the UI culture. Null follows the operating system.</summary>
    public static CultureInfo? Culture { get; set; }

    public static string Get(string key)
    {
        var value = Manager.GetString(key, Culture ?? CultureInfo.CurrentUICulture);
        return value ?? $"[{key}]";
    }

    public static string Format(string key, params object[] args) =>
        string.Format(Culture ?? CultureInfo.CurrentUICulture, Get(key), args);

    /// <summary>
    /// Picks <c>{key}_One</c> or <c>{key}_Other</c> and formats it with the count as argument 0.
    /// </summary>
    public static string Plural(string key, int count, params object[] extraArgs)
    {
        var args = new object[extraArgs.Length + 1];
        args[0] = count;
        Array.Copy(extraArgs, 0, args, 1, extraArgs.Length);
        return Format(count == 1 ? key + "_One" : key + "_Other", args);
    }
}
