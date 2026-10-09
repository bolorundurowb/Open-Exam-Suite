using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using OpenExamSuite.Creator.Session.Models;
using OpenExamSuite.Creator.ViewModels;

namespace OpenExamSuite.Creator.Views;

public partial class WorkspaceView : UserControl
{
    private const string NodePrefix = "oes-outline-node:";
    private static readonly DataFormat<string> NodeFormat = DataFormat.Text;

    private Point? _pressPoint;
    private OutlineNodeViewModel? _pressNode;
    private bool _dragging;

    public WorkspaceView()
    {
        InitializeComponent();
        AddHandler(PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
        AddHandler(PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel);
        AddHandler(PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        if (e.Source is not Visual source || source.FindAncestorOfType<TreeView>() != OutlineTree)
            return;

        if (source.FindAncestorOfType<Button>() is { } button && button.Classes.Contains("outline-chevron"))
            return;

        var node = FindOutlineNode(source);
        if (node == null || node.Type == NodeType.Exam)
            return;

        _pressNode = node;
        _pressPoint = e.GetPosition(this);
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _pressNode = null;
        _pressPoint = null;
    }

    private async void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_dragging || _pressNode == null || _pressPoint == null)
            return;

        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        var delta = e.GetPosition(this) - _pressPoint.Value;
        if (Math.Abs(delta.X) < 6 && Math.Abs(delta.Y) < 6)
            return;

        var node = _pressNode;
        _pressNode = null;
        _pressPoint = null;
        _dragging = true;
        try
        {
            var transfer = new DataTransfer();
            transfer.Add(DataTransferItem.Create(NodeFormat, NodePrefix + node.Id));
            await DragDrop.DoDragDropAsync(e, transfer, DragDropEffects.Move);
        }
        finally
        {
            _dragging = false;
        }
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = TryReadNodeId(e.DataTransfer, out _) ? DragDropEffects.Move : DragDropEffects.None;
        e.Handled = true;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (DataContext is WorkspaceViewModel workspace
            && TryReadNodeId(e.DataTransfer, out var sourceId)
            && e.Source is Visual source)
        {
            var target = FindOutlineNode(source);
            if (target != null)
                workspace.DropNode(sourceId, target.Id);
        }

        e.Handled = true;
    }

    private static bool TryReadNodeId(IDataTransfer? transfer, out string nodeId)
    {
        nodeId = string.Empty;
        var text = transfer?.TryGetText();
        if (string.IsNullOrEmpty(text) || !text.StartsWith(NodePrefix, StringComparison.Ordinal))
            return false;

        nodeId = text[NodePrefix.Length..];
        return nodeId.Length > 0;
    }

    private static OutlineNodeViewModel? FindOutlineNode(Visual? source)
    {
        while (source != null)
        {
            if (source is StyledElement { DataContext: OutlineNodeViewModel node })
                return node;

            source = source.GetVisualParent();
        }

        return null;
    }
}
