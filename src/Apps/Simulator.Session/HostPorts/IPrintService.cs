using System.Threading.Tasks;

namespace OpenExamSuite.Simulator.Session.HostPorts;

/// <summary>
/// Printing and PDF export. The host implements this.
/// </summary>
public interface IPrintService
{
    /// <summary>
    /// Shows a print dialog and prints the document.
    /// </summary>
    Task<bool> PrintAsync(string documentTitle, Stream content, string contentType = "application/pdf");

    /// <summary>
    /// Shows a print preview dialog.
    /// </summary>
    Task<bool> PrintPreviewAsync(string documentTitle, Stream content, string contentType = "application/pdf");

    /// <summary>
    /// Exports content to a PDF file.
    /// </summary>
    Task<bool> ExportToPdfAsync(string filePath, Stream content, string contentType = "application/pdf");
}