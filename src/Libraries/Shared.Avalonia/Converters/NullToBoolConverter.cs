using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace OpenExamSuite.Shared.Avalonia.Converters;

/// <summary>
/// Returns <c>true</c> when the bound value is not null, otherwise <c>false</c>.
/// Pass <c>"invert"</c> as the parameter to flip the result.
/// </summary>
public sealed class NullToBoolConverter : IValueConverter
{
    public static readonly NullToBoolConverter Instance = new();
    public static readonly NullToBoolConverter Inverse  = new() { Invert = true };

    public bool Invert { get; init; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var hasValue = value is not null;
        if (value is string s) hasValue = !string.IsNullOrEmpty(s);
        return Invert ? !hasValue : hasValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
