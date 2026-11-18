using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using OmniAssert;
using OpenExamSuite.Simulator.Views.Controls;
using Xunit;

namespace OpenExamSuite.App.Headless.Tests;

public class FillingWrapPanelTests
{
    [Fact]
    public void Row_StretchesShorterCardsToTheTallestAndFillsTheWidth()
    {
        using var session = HeadlessUnitTestSession.StartNew(typeof(SimulatorHeadlessEntry));
        session.Dispatch(() =>
        {
            var panel = new FillingWrapPanel
            {
                MinItemWidth = 360,
                HorizontalSpacing = 16,
                VerticalSpacing = 16,
                Children =
                {
                    new Border { Height = 80 },
                    new Border { Height = 140 },
                    new Border { Height = 90 }
                }
            };

            panel.Measure(new Size(1000, double.PositiveInfinity));
            panel.Arrange(new Rect(0, 0, 1000, panel.DesiredSize.Height));

            panel.DesiredSize.Height.Must().BeApproximately(246, 0.5);
            panel.Children[0].Bounds.Must().Be(new Rect(0, 0, 492, 140));
            panel.Children[1].Bounds.Must().Be(new Rect(508, 0, 492, 140));
            panel.Children[2].Bounds.Must().Be(new Rect(0, 156, 492, 90));
        }, CancellationToken.None);
    }
}
