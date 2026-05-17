using System;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;

namespace OpenExamSuite.Shared.Avalonia.Converters;

/// <summary>
/// Converts PNG-encoded <c>byte[]</c> from the cross-platform domain model
/// (<c>Question.ImageData</c>) into an <see cref="Avalonia.Media.Imaging.Bitmap"/>
/// suitable for binding to <c>Image.Source</c>. The class name is kept for
/// continuity with the original migration plan, but no <c>System.Drawing</c>
/// is involved any more.
/// </summary>
public sealed class BitmapToAvaloniaImageConverter : IValueConverter
{
    public static readonly BitmapToAvaloniaImageConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not byte[] bytes || bytes.Length == 0)
            return null;

        try
        {
            using var ms = new MemoryStream(bytes, writable: false);
            return new Bitmap(ms);
        }
        catch
        {
            return null;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
