using System.Drawing.Imaging;

namespace OpenExamSuite.Shared.WinForms;

/// <summary>
/// Converts between persisted image bytes and WinForms <see cref="Bitmap"/> instances.
/// This keeps the <see cref="Core"/> model free of <see cref="System.Drawing"/> types.
/// </summary>
public static class WinFormsImageConverter
{
    public static Bitmap? ToBitmap(byte[]? imageData)
    {
        if (imageData == null || imageData.Length == 0)
            return null;

        using var ms = new MemoryStream(imageData);
        return new Bitmap(ms);
    }

    public static byte[]? ToByteArray(Bitmap? bitmap, ImageFormat? format = null)
    {
        if (bitmap == null)
            return null;

        using var ms = new MemoryStream();
        bitmap.Save(ms, format ?? ImageFormat.Png);
        return ms.ToArray();
    }
}
