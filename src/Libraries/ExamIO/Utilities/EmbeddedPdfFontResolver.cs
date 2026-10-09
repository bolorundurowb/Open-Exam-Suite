using System.Reflection;
using PdfSharp.Fonts;

namespace OpenExamSuite.Shared.Utilities;

internal sealed class EmbeddedPdfFontResolver : IFontResolver
{
    private const string FamilyName = "IBM Plex Sans";
    private const string RegularFaceName = "IBMPlexSans-Regular";
    private const string BoldFaceName = "IBMPlexSans-Bold";
    private const string ItalicFaceName = "IBMPlexSans-Italic";
    private const string RegularResourceName = "OpenExamSuite.Shared.Resources.fonts.IBMPlexSans-Regular.ttf";
    private const string BoldResourceName = "OpenExamSuite.Shared.Resources.fonts.IBMPlexSans-Bold.ttf";
    private const string ItalicResourceName = "OpenExamSuite.Shared.Resources.fonts.IBMPlexSans-Italic.ttf";

    private static readonly Lazy<byte[]> RegularFaceData = new(() => LoadFontBytes(RegularResourceName));
    private static readonly Lazy<byte[]> BoldFaceData = new(() => LoadFontBytes(BoldResourceName));
    private static readonly Lazy<byte[]> ItalicFaceData = new(() => LoadFontBytes(ItalicResourceName));

    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        if (!string.Equals(familyName, FamilyName, StringComparison.OrdinalIgnoreCase))
            return PlatformFontResolver.ResolveTypeface(familyName, isBold, isItalic);

        var face = isItalic ? ItalicFaceName : isBold ? BoldFaceName : RegularFaceName;
        return new FontResolverInfo(face);
    }

    public byte[]? GetFont(string faceName)
    {
        return faceName switch
        {
            RegularFaceName => RegularFaceData.Value,
            BoldFaceName => BoldFaceData.Value,
            ItalicFaceName => ItalicFaceData.Value,
            _ => null
        };
    }

    private static byte[] LoadFontBytes(string resourceName)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)
                           ?? throw new InvalidOperationException($"Embedded font resource not found: {resourceName}");
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        return memory.ToArray();
    }
}