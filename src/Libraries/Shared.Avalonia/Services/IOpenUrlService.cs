using System;
using System.Threading.Tasks;

namespace OpenExamSuite.Shared.Avalonia.Services;

public interface IOpenUrlService
{
    Task<bool> OpenUriAsync(Uri uri);
    Task<bool> OpenUriAsync(string uri);
    Task<bool> OpenFileAsync(string localPath);
}
