using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using OpenExamSuite.Simulator.ViewModels;

namespace OpenExamSuite.Simulator.Views;

/// <summary>
/// Exam and practice screen. Handles the keyboard shortcuts:
/// A to Z select an option, Left and Right move, F flags, P pauses, Enter goes next, Ctrl+Enter opens review.
/// </summary>
public partial class ExamView : UserControl
{
    private const double WideBreakpoint = 1100;

    private TopLevel? _topLevel;
    private ExamViewModel? _viewModel;

    public ExamView()
    {
        InitializeComponent();
        SizeChanged += (_, e) =>
        {
            if (DataContext is ExamViewModel viewModel)
                viewModel.IsWide = e.NewSize.Width >= WideBreakpoint;
        };
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        // Shortcuts are caught at the window so they still work when the focused option was just replaced.
        _topLevel = TopLevel.GetTopLevel(this);
        _topLevel?.AddHandler(KeyDownEvent, OnPreviewKeyDown, RoutingStrategies.Tunnel);
        Focus();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _topLevel?.RemoveHandler(KeyDownEvent, OnPreviewKeyDown);
        _topLevel = null;
        Detach();
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        Detach();
        _viewModel = DataContext as ExamViewModel;
        if (_viewModel != null)
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void Detach()
    {
        if (_viewModel != null)
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;

        _viewModel = null;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ExamViewModel.IsPaused) && _viewModel is { IsPaused: true })
            Dispatcher.UIThread.Post(() => ResumeButton.Focus());
    }

    private void OnPreviewKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not ExamViewModel viewModel || !IsEffectivelyVisible)
            return;

        // Never steal keys from text entry or from another window such as a dialog.
        if (e.Source is TextBox || TopLevel.GetTopLevel(this) is not Window { IsActive: true })
            return;

        var modifiers = e.KeyModifiers;

        if (viewModel.IsPaused)
        {
            if (e.Key == Key.P && modifiers == KeyModifiers.None)
            {
                _ = viewModel.ResumeAsync();
                e.Handled = true;
            }

            return;
        }

        switch (e.Key)
        {
            case Key.Enter when modifiers == KeyModifiers.Control:
                _ = viewModel.ReviewAsync();
                e.Handled = true;
                return;
            case Key.Enter when modifiers == KeyModifiers.None:
                _ = viewModel.NextAsync();
                e.Handled = true;
                return;
            case Key.Left when modifiers == KeyModifiers.None:
                _ = viewModel.PreviousAsync();
                e.Handled = true;
                return;
            case Key.Right when modifiers == KeyModifiers.None:
                _ = viewModel.NextAsync();
                e.Handled = true;
                return;
        }

        if (e.Key is < Key.A or > Key.Z || (modifiers & ~KeyModifiers.Shift) != KeyModifiers.None)
            return;

        var letter = (char)('A' + (e.Key - Key.A));
        var shifted = modifiers == KeyModifiers.Shift;

        // F and P are shortcuts. An option lettered F or P is chosen with Shift+F or Shift+P.
        if (!shifted && letter == 'F')
        {
            _ = viewModel.ToggleFlagAsync();
        }
        else if (!shifted && letter == 'P')
        {
            _ = viewModel.PauseAsync();
        }
        else
        {
            _ = viewModel.SelectLetterAsync(letter);
        }

        e.Handled = true;
    }
}
