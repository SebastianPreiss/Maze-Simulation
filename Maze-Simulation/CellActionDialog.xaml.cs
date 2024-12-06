namespace Maze_Simulation;

using Maze_Simulation.Model;
using System.Windows;

/// <summary>
/// Interaction logic for the Cell Action Dialog, allowing the user to select an action 
/// for a cell in the maze (Start, Target, or Cancel).
/// </summary>
public partial class CellActionDialog : Window
{
    public CellAction SelectedAction { get; private set; }

    public CellActionDialog()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles the click event for the Start button. Sets the selected action to "Start" 
    /// and closes the dialog with a successful result.
    /// </summary>
    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        SelectedAction = CellAction.Start;
        DialogResult = true;
    }

    /// <summary>
    /// Handles the click event for the Target button. Sets the selected action to "Target" 
    /// and closes the dialog with a successful result.
    /// </summary>
    private void TargetButton_Click(object sender, RoutedEventArgs e)
    {
        SelectedAction = CellAction.Target;
        DialogResult = true;
    }

    /// <summary>
    /// Handles the click event for the Cancel button. Sets the selected action to "Cancel" 
    /// and closes the dialog with a failure result.
    /// </summary>
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        SelectedAction = CellAction.Cancel;
        DialogResult = false;
    }
}
