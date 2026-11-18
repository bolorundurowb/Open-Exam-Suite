using OmniAssert;
using OpenExamSuite.Simulator.Views.Controls;
using Xunit;

namespace OpenExamSuite.Simulator.Tests;

public class FillingWrapPanelTests
{
    [Theory]
    [InlineData(1112, 360, 16, 3, 360)]
    [InlineData(1111, 360, 16, 2, 547.5)]
    [InlineData(800, 360, 16, 2, 392)]
    [InlineData(735, 360, 16, 1, 735)]
    [InlineData(200, 360, 16, 1, 200)]
    public void Resolve_FillsTheRowAndWrapsWhenAnotherMinimumCardDoesNotFit(
        double availableWidth, double minItemWidth, double spacing, int columns, double itemWidth)
    {
        var resolved = FillingWrapPanel.Resolve(availableWidth, minItemWidth, spacing);

        resolved.Columns.Must().Be(columns);
        resolved.ItemWidth.Must().BeApproximately(itemWidth, 0.001);
    }

    [Fact]
    public void Resolve_UsesTheMinimumWidthWhenTheAvailableWidthIsUnbounded()
    {
        var resolved = FillingWrapPanel.Resolve(double.PositiveInfinity, 360, 16);

        resolved.Columns.Must().Be(1);
        resolved.ItemWidth.Must().Be(360);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-20)]
    public void Resolve_YieldsNothingWhenThereIsNoWidth(double availableWidth)
    {
        var resolved = FillingWrapPanel.Resolve(availableWidth, 360, 16);

        resolved.Columns.Must().Be(1);
        resolved.ItemWidth.Must().Be(0);
    }
}
