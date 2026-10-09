using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace OpenExamSuite.Simulator.Views.Controls;

/// <summary>
/// A single horizontal bar showing a value from 0 to 100, optionally with a marker such as the pass mark.
/// Drawn directly so the app needs no chart package.
/// </summary>
public sealed class ScoreBar : Control
{
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<ScoreBar, double>(nameof(Value));

    public static readonly StyledProperty<double?> MarkProperty =
        AvaloniaProperty.Register<ScoreBar, double?>(nameof(Mark));

    public static readonly StyledProperty<IBrush?> TrackBrushProperty =
        AvaloniaProperty.Register<ScoreBar, IBrush?>(nameof(TrackBrush));

    public static readonly StyledProperty<IBrush?> FillBrushProperty =
        AvaloniaProperty.Register<ScoreBar, IBrush?>(nameof(FillBrush));

    public static readonly StyledProperty<IBrush?> MarkBrushProperty =
        AvaloniaProperty.Register<ScoreBar, IBrush?>(nameof(MarkBrush));

    public static readonly StyledProperty<double> BarHeightProperty =
        AvaloniaProperty.Register<ScoreBar, double>(nameof(BarHeight), 14);

    static ScoreBar()
    {
        AffectsRender<ScoreBar>(ValueProperty, MarkProperty, TrackBrushProperty, FillBrushProperty, MarkBrushProperty);
        AffectsMeasure<ScoreBar>(BarHeightProperty, MarkProperty);
    }

    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public double? Mark
    {
        get => GetValue(MarkProperty);
        set => SetValue(MarkProperty, value);
    }

    public IBrush? TrackBrush
    {
        get => GetValue(TrackBrushProperty);
        set => SetValue(TrackBrushProperty, value);
    }

    public IBrush? FillBrush
    {
        get => GetValue(FillBrushProperty);
        set => SetValue(FillBrushProperty, value);
    }

    public IBrush? MarkBrush
    {
        get => GetValue(MarkBrushProperty);
        set => SetValue(MarkBrushProperty, value);
    }

    public double BarHeight
    {
        get => GetValue(BarHeightProperty);
        set => SetValue(BarHeightProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var width = double.IsInfinity(availableSize.Width) ? 200 : availableSize.Width;
        var extra = Mark.HasValue ? 10 : 0;
        return new Size(width, BarHeight + extra);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var hasMark = Mark.HasValue;
        var top = hasMark ? 5d : 0d;
        var width = Bounds.Width;
        var radius = BarHeight / 2;
        var track = new Rect(0, top, width, BarHeight);

        if (TrackBrush != null)
            context.DrawRectangle(TrackBrush, null, track, radius, radius);

        var fraction = Math.Clamp(Value, 0, 100) / 100d;
        if (FillBrush != null && fraction > 0)
        {
            var fill = new Rect(0, top, Math.Max(BarHeight, width * fraction), BarHeight);
            context.DrawRectangle(FillBrush, null, fill, radius, radius);
        }

        if (hasMark && MarkBrush != null)
        {
            var x = Math.Clamp(Mark!.Value, 0, 100) / 100d * width;
            // A tall tick that sticks out above and below the bar so it is visible on top of the fill too.
            var tick = new Rect(Math.Clamp(x - 1.5, 0, Math.Max(0, width - 3)), 0, 3, BarHeight + 10);
            context.DrawRectangle(MarkBrush, null, tick);
        }
    }
}
