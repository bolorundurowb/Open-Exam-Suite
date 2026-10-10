using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using OpenExamSuite.Creator.Engine.Services;

namespace OpenExamSuite.Creator.Services;

public sealed class RecoveryOffer
{
    public required string Key { get; init; }

    public required string RecoveryPath { get; init; }

    public string? SourcePath { get; init; }

    public required DateTime TimestampUtc { get; init; }
}

/// <summary>
/// Autosave files under Application Data. Each copy has a sidecar naming the exam it belongs to
/// so a later launch can tell whether the copy is newer than the file on disk.
/// </summary>
public static class RecoveryCatalog
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

    public static string Folder => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "OpenExamSuite",
        "CreatorRecovery");

    public static string KeyForPath(string sourcePath) =>
        "file-" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(Path.GetFullPath(sourcePath)))).ToLowerInvariant();

    public static string KeyForUntitled(ref string? untitledKey)
    {
        untitledKey ??= Guid.NewGuid().ToString("N");
        return "untitled-" + untitledKey;
    }

    public static void Write(CreatorDocument document, string key, string? sourcePath)
    {
        if (!document.IsDirty)
            return;

        Directory.CreateDirectory(Folder);
        var recoveryPath = RecoveryPath(key);
        if (!document.WriteRecoveryCopy(recoveryPath))
            return;

        var meta = JsonSerializer.Serialize(new RecoveryMeta(sourcePath), JsonOptions);
        File.WriteAllText(MetaPath(key), meta);
    }

    public static IReadOnlyList<RecoveryOffer> FindNewer(CreatorDocument document)
    {
        if (!Directory.Exists(Folder))
            return [];

        var offers = new List<RecoveryOffer>();
        foreach (var recoveryPath in Directory.EnumerateFiles(Folder, "*.recovery.oef"))
        {
            var fileName = Path.GetFileName(recoveryPath);
            const string suffix = ".recovery.oef";
            if (!fileName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                continue;

            var key = fileName[..^suffix.Length];
            var sourcePath = ReadSourcePath(key);
            if (!document.IsRecoveryNewerThan(recoveryPath, sourcePath))
                continue;

            offers.Add(new RecoveryOffer
            {
                Key = key,
                RecoveryPath = recoveryPath,
                SourcePath = sourcePath,
                TimestampUtc = File.GetLastWriteTimeUtc(recoveryPath)
            });
        }

        return offers.OrderByDescending(offer => offer.TimestampUtc).ToList();
    }

    public static void Delete(string key)
    {
        TryDelete(RecoveryPath(key));
        TryDelete(MetaPath(key));
    }

    public static void RememberUntitledKey(string key, ref string? untitledKey)
    {
        const string prefix = "untitled-";
        if (key.StartsWith(prefix, StringComparison.Ordinal))
            untitledKey = key[prefix.Length..];
    }

    private static string? ReadSourcePath(string key)
    {
        var metaPath = MetaPath(key);
        if (!File.Exists(metaPath))
            return null;

        try
        {
            return JsonSerializer.Deserialize<RecoveryMeta>(File.ReadAllText(metaPath), JsonOptions)?.SourcePath;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string RecoveryPath(string key) => Path.Combine(Folder, key + ".recovery.oef");

    private static string MetaPath(string key) => Path.Combine(Folder, key + ".recovery.json");

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    private sealed record RecoveryMeta(string? SourcePath);
}
