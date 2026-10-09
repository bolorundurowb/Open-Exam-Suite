using System;
using OmniAssert;
using OpenExamSuite.Simulator.Services;
using Xunit;

namespace OpenExamSuite.Simulator.Tests;

public class ResultsFormatTests
{
    [Theory]
    [InlineData(0, "00:00")]
    [InlineData(6, "00:06")]
    [InlineData(65, "01:05")]
    [InlineData(3600, "1:00:00")]
    [InlineData(5405, "1:30:05")]
    public void Duration_PrintsMinutesAndSeconds(int seconds, string expected)
    {
        ResultsFormat.Duration(TimeSpan.FromSeconds(seconds)).Must().Be(expected);
    }

    [Fact]
    public void Duration_NeverShowsNegativeTime()
    {
        ResultsFormat.Duration(TimeSpan.FromSeconds(-5)).Must().Be("00:00");
    }
}
