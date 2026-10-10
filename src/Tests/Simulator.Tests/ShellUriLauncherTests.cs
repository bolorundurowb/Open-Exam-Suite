using System.Threading.Tasks;
using OmniAssert;
using OpenExamSuite.Simulator.Services;
using Xunit;

namespace OpenExamSuite.Simulator.Tests;

public class ShellUriLauncherTests
{
    private static ShellUriLauncher Launcher() => new();

    [Theory]
    [InlineData("file:///etc/passwd")]
    [InlineData("javascript:alert(1)")]
    [InlineData("data:text/html,<script>alert(1)</script>")]
    [InlineData("vbscript:msgbox(1)")]
    [InlineData("mailto:someone@example.com")]
    [InlineData("ftp://example.com/file")]
    [InlineData("ms-settings:privacy")]
    [InlineData(@"C:\Windows\System32\calc.exe")]
    [InlineData("https://")]
    public async Task LaunchUriAsync_RefusesAnythingThatIsNotHttpOrHttps(string uri)
    {
        var launched = await Launcher().LaunchUriAsync(uri);

        launched.Must().BeFalse();
    }

    [Theory]
    [InlineData(@"C:\exams\payload.exe")]
    [InlineData(@"C:\exams\payload.EXE")]
    [InlineData(@"C:\exams\script.ps1")]
    [InlineData(@"C:\exams\script.bat")]
    [InlineData(@"C:\exams\script.cmd")]
    [InlineData(@"C:\exams\script.vbs")]
    [InlineData(@"C:\exams\shortcut.lnk")]
    [InlineData("/home/user/payload.sh")]
    [InlineData("/home/user/app.desktop")]
    public async Task OpenFileAsync_RefusesExecutableExtensions(string filePath)
    {
        var opened = await Launcher().OpenFileAsync(filePath);

        opened.Must().BeFalse();
    }
}
