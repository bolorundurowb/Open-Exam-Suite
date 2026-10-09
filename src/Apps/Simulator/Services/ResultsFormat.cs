using System.Globalization;

namespace OpenExamSuite.Simulator.Services;

/// <summary>
/// Shared number and time formatting so every screen prints scores and clocks the same way.
/// </summary>
public static class ResultsFormat
{
    /// <summary>mm:ss, or h:mm:ss when an hour or longer.</summary>
    public static string Duration(TimeSpan value)
    {
        if (value < TimeSpan.Zero)
            value = TimeSpan.Zero;

        var totalSeconds = (int)Math.Ceiling(value.TotalSeconds);
        var hours = totalSeconds / 3600;
        var minutes = totalSeconds % 3600 / 60;
        var seconds = totalSeconds % 60;
        return hours > 0
            ? string.Create(CultureInfo.InvariantCulture, $"{hours}:{minutes:00}:{seconds:00}")
            : string.Create(CultureInfo.InvariantCulture, $"{minutes:00}:{seconds:00}");
    }

    public static string Percent(double value) =>
        string.Format(CultureInfo.CurrentCulture, "{0:0.#}%", value);

    public static string ShortDate(DateTime utcOrLocal) =>
        (utcOrLocal.Kind == DateTimeKind.Utc ? utcOrLocal.ToLocalTime() : utcOrLocal)
            .ToString("d", CultureInfo.CurrentCulture);
}
