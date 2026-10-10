using OpenExamSuite.Simulator.Engine.HostPorts;

namespace OpenExamSuite.Simulator.Services;

/// <summary>
/// Printing goes through PDF: the document is written to a temporary file and opened in the
/// system PDF viewer, which owns the print dialog on every platform.
/// </summary>
public sealed class PdfPrintService : IPrintService
{
    private readonly IAppPaths _paths;
    private readonly IUriLauncher _launcher;

    public PdfPrintService(IAppPaths paths, IUriLauncher launcher)
    {
        _paths = paths;
        _launcher = launcher;
    }

    public async Task<bool> PrintAsync(string documentTitle, Stream content, string contentType = "application/pdf")
    {
        var path = Path.Combine(_paths.TempDirectory, $"{SafeName(documentTitle)}-{Guid.NewGuid():N}.pdf");
        Directory.CreateDirectory(_paths.TempDirectory);
        await using (var file = File.Create(path))
        {
            await content.CopyToAsync(file);
        }

        return await _launcher.OpenFileAsync(path);
    }

    public Task<bool> PrintPreviewAsync(string documentTitle, Stream content, string contentType = "application/pdf") =>
        PrintAsync(documentTitle, content, contentType);

    public async Task<bool> ExportToPdfAsync(string filePath, Stream content, string contentType = "application/pdf")
    {
        await using var file = File.Create(filePath);
        await content.CopyToAsync(file);
        return true;
    }

    private static string SafeName(string title)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var cleaned = new string(title.Select(c => invalid.Contains(c) ? '_' : c).ToArray()).Trim();
        return cleaned.Length == 0 ? "results" : cleaned;
    }
}
