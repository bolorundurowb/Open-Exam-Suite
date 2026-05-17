using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace OpenExamSuite.Shared.Avalonia.Converters;

/// <summary>
/// Two-way converter useful for binding a <see cref="RadioButton.IsChecked"/>
/// to an enum-valued ViewModel property. The <c>ConverterParameter</c> carries
/// the enum value that the radio represents.
/// </summary>
public sealed class EnumToBoolConverter : IValueConverter
{
    public static readonly EnumToBoolConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null || parameter is null) return false;
        return value.Equals(parameter);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b && b && parameter is not null)
            return parameter;
        return BindingOperations.DoNothing;
    }
}
