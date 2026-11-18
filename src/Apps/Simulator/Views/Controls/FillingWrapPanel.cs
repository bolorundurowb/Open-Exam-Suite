using Avalonia;
using Avalonia.Controls;

namespace OpenExamSuite.Simulator.Views.Controls;

/// <summary>
/// Wraps children into rows that use the full width. Every child in a row is as wide as the others
/// and as tall as the tallest child in that row. A width below <see cref="MinItemWidth"/> still
/// yields one column of that width, so a single card fills a narrow library.
/// </summary>
public sealed class FillingWrapPanel : Panel
{
    public static readonly StyledProperty<double> MinItemWidthProperty =
        AvaloniaProperty.Register<FillingWrapPanel, double>(nameof(MinItemWidth), 360);

    public static readonly StyledProperty<double> HorizontalSpacingProperty =
        AvaloniaProperty.Register<FillingWrapPanel, double>(nameof(HorizontalSpacing));

    public static readonly StyledProperty<double> VerticalSpacingProperty =
        AvaloniaProperty.Register<FillingWrapPanel, double>(nameof(VerticalSpacing));

    private double _measuredItemWidth = double.NaN;

    static FillingWrapPanel()
    {
        AffectsMeasure<FillingWrapPanel>(MinItemWidthProperty, HorizontalSpacingProperty, VerticalSpacingProperty);
    }

    public double MinItemWidth
    {
        get => GetValue(MinItemWidthProperty);
        set => SetValue(MinItemWidthProperty, value);
    }

    public double HorizontalSpacing
    {
        get => GetValue(HorizontalSpacingProperty);
        set => SetValue(HorizontalSpacingProperty, value);
    }

    public double VerticalSpacing
    {
        get => GetValue(VerticalSpacingProperty);
        set => SetValue(VerticalSpacingProperty, value);
    }

    /// <summary>
    /// How many columns fit in <paramref name="availableWidth"/>, and how wide each one is so the
    /// row fills that width. Extra space is shared. It is not left over beside the last column.
    /// </summary>
    internal static (int Columns, double ItemWidth) Resolve(double availableWidth, double minItemWidth, double spacing)
    {
        if (double.IsNaN(availableWidth) || availableWidth <= 0)
            return (1, 0);

        var gap = spacing > 0 ? spacing : 0;
        var minimum = minItemWidth > 0 ? minItemWidth : 0;

        if (double.IsInfinity(availableWidth))
            return (1, minimum);

        var columns = 1;
        if (minimum > 0)
        {
            var fitted = (int)Math.Floor((availableWidth + gap) / (minimum + gap));
            if (fitted > 1)
                columns = fitted;
        }

        var width = columns == 1
            ? availableWidth
            : (availableWidth - gap * (columns - 1)) / columns;

        return (columns, width > 0 ? width : 0);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var (columns, itemWidth) = Resolve(availableSize.Width, MinItemWidth, HorizontalSpacing);
        _measuredItemWidth = itemWidth;
        MeasureChildren(itemWidth);

        var width = double.IsInfinity(availableSize.Width) ? itemWidth : availableSize.Width;
        return new Size(width, StackHeight(columns));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var (columns, itemWidth) = Resolve(finalSize.Width, MinItemWidth, HorizontalSpacing);
        if (double.IsNaN(_measuredItemWidth) || Math.Abs(_measuredItemWidth - itemWidth) > 0.5)
        {
            MeasureChildren(itemWidth);
            _measuredItemWidth = itemWidth;
        }

        var gapX = HorizontalSpacing > 0 ? HorizontalSpacing : 0;
        var gapY = VerticalSpacing > 0 ? VerticalSpacing : 0;
        var visible = VisibleChildren();
        double y = 0;

        for (var index = 0; index < visible.Count;)
        {
            var end = Math.Min(index + columns, visible.Count);
            var rowHeight = 0d;
            for (var i = index; i < end; i++)
                rowHeight = Math.Max(rowHeight, visible[i].DesiredSize.Height);

            double x = 0;
            for (var i = index; i < end; i++)
            {
                visible[i].Arrange(new Rect(x, y, itemWidth, rowHeight));
                x += itemWidth + gapX;
            }

            y += rowHeight;
            if (end < visible.Count)
                y += gapY;
            index = end;
        }

        foreach (var child in Children)
        {
            if (!child.IsVisible)
                child.Arrange(default);
        }

        return finalSize;
    }

    private void MeasureChildren(double itemWidth)
    {
        var constraint = new Size(itemWidth, double.PositiveInfinity);
        foreach (var child in Children)
            child.Measure(child.IsVisible ? constraint : default);
    }

    private double StackHeight(int columns)
    {
        var visible = VisibleChildren();
        if (visible.Count == 0 || columns < 1)
            return 0;

        var gapY = VerticalSpacing > 0 ? VerticalSpacing : 0;
        double height = 0;
        var rows = 0;
        for (var index = 0; index < visible.Count; index += columns)
        {
            var end = Math.Min(index + columns, visible.Count);
            var rowHeight = 0d;
            for (var i = index; i < end; i++)
                rowHeight = Math.Max(rowHeight, visible[i].DesiredSize.Height);
            height += rowHeight;
            rows++;
        }

        if (rows > 1)
            height += gapY * (rows - 1);
        return height;
    }

    private List<Control> VisibleChildren()
    {
        var visible = new List<Control>();
        foreach (var child in Children)
        {
            if (child.IsVisible)
                visible.Add(child);
        }

        return visible;
    }
}
