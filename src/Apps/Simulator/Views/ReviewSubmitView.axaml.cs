using Avalonia.Controls;
using OpenExamSuite.Simulator.ViewModels;

namespace OpenExamSuite.Simulator.Views;

public partial class ReviewSubmitView : UserControl
{
    private const double WideBreakpoint = 1100;

    public ReviewSubmitView()
    {
        InitializeComponent();
        SizeChanged += (_, e) =>
        {
            if (DataContext is ReviewSubmitViewModel viewModel)
                viewModel.IsWide = e.NewSize.Width >= WideBreakpoint;
        };
    }
}
