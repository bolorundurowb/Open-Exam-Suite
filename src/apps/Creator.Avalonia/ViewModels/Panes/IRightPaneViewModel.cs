namespace OpenExamSuite.Creator.ViewModels.Panes;

/// <summary>
/// Marker interface for the three concrete right-pane VMs (Splash,
/// ExamProperties, QuestionEditor) that swap in <c>MainWindow</c>'s
/// <c>ContentControl</c>. Replaces the WinForms
/// <c>Controls.Add/Remove</c> dance on <c>splitContainer2.Panel2</c>.
/// </summary>
public interface IRightPaneViewModel { }
