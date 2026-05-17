using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace OpenExamSuite.Shared.WinForms;

/// <summary>
/// Bridges the domain model's cross-platform <c>byte[] ImageData</c> shape
/// (PNG-encoded) and the WinForms <see cref="Bitmap"/> the
/// <see cref="System.Windows.Forms.PictureBox"/> expects.
/// </summary>
public static class QuestionImageExtensions
{
    /// <summary>Decodes <c>Question.ImageData</c> into a <see cref="Bitmap"/>, or returns null when empty.</summary>
    public static Bitmap? ToBitmap(this byte[]? imageData)
    {
        if (imageData is null || imageData.Length == 0) return null;
        try
        {
            using var ms = new MemoryStream(imageData, writable: false);
            return new Bitmap(ms);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>PNG-encodes a <see cref="Bitmap"/> into a byte[] for storage in <c>Question.ImageData</c>.</summary>
    public static byte[]? ToPngBytes(this Image? image)
    {
        if (image is null) return null;
        using var ms = new MemoryStream();
        image.Save(ms, ImageFormat.Png);
        return ms.ToArray();
    }
}
