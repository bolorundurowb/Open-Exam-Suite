using System.Reflection;
using PdfSharp.Fonts;

namespace OpenExamSuite.Shared.Utilities;

internal sealed class EmbeddedNotoSansFontResolver : IFontResolver
{
    private const string FamilyName = "Noto Sans";
    private const string RegularFaceName = "NotoSans-Regular";
    private const string ItalicFaceName = "NotoSans-Italic";
    private const string RegularResourceName = "OpenExamSuite.ExamIO.Resources.fonts.NotoSans-VariableFont.ttf";
    private const string ItalicResourceName = "OpenExamSuite.ExamIO.Resources.fonts.NotoSans-VariableFont-Italic.ttf";

    private static readonly Lazy<byte[]> RegularFaceData = new(() => LoadFontBytes(RegularResourceName));
    private static readonly Lazy<byte[]> ItalicFaceData = new(() => LoadFontBytes(ItalicResourceName));

    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic) =>
        string.Equals(familyName, FamilyName, StringComparison.OrdinalIgnoreCase)
            ? new FontResolverInfo(isItalic ? ItalicFaceName : RegularFaceName)
            : PlatformFontResolver.ResolveTypeface(familyName, isBold, isItalic);

    public byte[]? GetFont(string faceName)
    {
        return faceName switch
        {
            RegularFaceName => RegularFaceData.Value,
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