using Avalonia.Platform;

namespace OpenExamSuite.Creator.Views.Dialogs;

internal static class BundledDocuments
{
    public static string Read(string assetName)
    {
        var uri = new Uri($"avares://OpenExamSuite.Creator/Assets/{assetName}");
        using var stream = AssetLoader.Open(uri);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
