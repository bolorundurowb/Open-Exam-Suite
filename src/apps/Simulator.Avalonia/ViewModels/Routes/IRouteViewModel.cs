namespace OpenExamSuite.Simulator.ViewModels.Routes;

/// <summary>
/// Marker for ViewModels that participate in the Simulator's main-window
/// content swap (Home → ExamSettings → Assessment → ScoreSheet). Replaces the
/// WinForms <c>Hide()</c> + <c>ShowDialog()</c> chain.
/// </summary>
public interface IRouteViewModel { }
