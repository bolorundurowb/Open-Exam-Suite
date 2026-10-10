using Avalonia;
using Avalonia.Headless;
using OmniAssert;
using Xunit;

namespace OpenExamSuite.App.Headless.Tests;

/// <summary>
/// The Creator app starts on the headless platform, so the XAML, styles and fonts load without a display.
/// </summary>
public class CreatorHeadlessTests
{
    [Fact]
    public void Creator_Starts_Headlessly()
    {
        using var session = HeadlessUnitTestSession.StartNew(typeof(CreatorHeadlessEntry));
        session.Dispatch(() =>
        {
            Application.Current.Must().NotBeNull();
            Application.Current!.GetType().Must().Be(typeof(OpenExamSuite.Creator.App));
        }, CancellationToken.None);
    }
}
