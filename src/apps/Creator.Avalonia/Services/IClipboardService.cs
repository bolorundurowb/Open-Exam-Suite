using System.Threading.Tasks;

namespace OpenExamSuite.Creator.Services;

public interface IClipboardService
{
    Task<string?> GetTextAsync();
    Task SetTextAsync(string text);
}
