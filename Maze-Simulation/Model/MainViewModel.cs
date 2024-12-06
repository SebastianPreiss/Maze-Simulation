namespace Maze_Simulation.Model;

using Generation;
using Shared;
using SolvingAlgorithms;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

/// <summary>
/// Represents the main view model for managing the maze generation, pathfinding, 
/// and UI interactions in the application.
/// </summary>
public class MainViewModel : INotifyPropertyChanged
{
    //Maze properties
    public Board? Board { get; private set; }
    public Position? Start { get; private set; }
    public Position? Target { get; private set; }
    public Solve? Solve { get; private set; }

    public List<IBoardStrategy> Generators { get; } = [];
    public List<IPathSolver> Solvers { get; } = [];

    //UI properties
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly Stopwatch _stopwatch;
    private string _duration;

    public string Duration
    {
        get => _duration;
        set
        {
            _duration = value;
            OnPropertyChanged();
        }
    }

    public MainViewModel()
    {
        _stopwatch = new Stopwatch();

        Generators.Add(new MazeGenerator());

        Solvers.Add(new AStarSolver());
        //Solvers.Add(new BfsSolver());
        //Solvers.Add(new HandOnWallSolver { UseLeftHand = true });
        //Solvers.Add(new HandOnWallSolver { UseLeftHand = false });

        Duration = "0:0.0";

        // For debugging
        //Board = new Board(2, 2);
        //Board.FillBlank();

        //Board[0, 0][Direction.Right] = false;
        //Board[1, 0][Direction.Left] = false;

        //Board[1, 1][Direction.Bottom] = false;
        //Board[1, 0][Direction.Top] = false;

        //Board[1, 1][Direction.Left] = false;
        //Board[0, 1][Direction.Right] = false;

        //Board[0, 0][Direction.Top] = false;
        //Board[0, 1][Direction.Bottom] = false;
    }

    /// <summary>
    /// Generates a new maze board with the specified dimensions and seed value.
    /// </summary>
    /// <param name="seed">The seed used for random generation of the board.</param>
    /// <param name="width">The width of the board.</param>
    /// <param name="height">The height of the board.</param>
    /// <param name="multiPath">Specifies if multiple paths should be created.</param>
    public void GenerateBoard(string seed, int width, int height, bool multiPath)
    {
        if (!int.TryParse(seed, out var parsedSeed)) throw new ArgumentException("The provided seed value is not a valid integer.", nameof(seed));

        // Select the first generator in the list 
        var generator = Generators[0];
        generator.Seed = parsedSeed;
        Board = generator.Generate(width, height);
        Target = new Position(2, 2);
        Start = new Position(5, 5);
        if (Start is Position s && Target is Position t)
        {
            Solve = new Solve([Direction.Top, Direction.Top], s, t);
            //Solve = new Solve([Direction.Right, Direction.Right], s, t);
            Solve = new Solve([Direction.Bottom, Direction.Bottom], s, t);
            //Solve = new Solve([Direction.Left, Direction.Left], s, t);
        }
    }


    ///// <summary>
    ///// Draws the processed cells of the maze during the execution of the pathfinding algorithm.
    ///// </summary>
    ///// <param name="dc">The DrawingContext used for rendering the processed cells.</param>
    ///// <param name="processedCells">
    ///// A collection of tuples containing the processed cells and their associated cost values.
    ///// </param>
    //public void DrawProcessedCells(DrawingContext dc, IEnumerable<(Cell Cell, double Cost)> processedCells)
    //{
    //    if (_isPathSolved) return;
    //    var maxCost = processedCells.Max(c => c.Cost);
    //    var minCost = processedCells.Min(c => c.Cost);
    //    var rectangleSize = CellSize * 0.4;

    //    foreach (var (cell, cost) in processedCells)
    //    {
    //        var normalizedCost = (cost - minCost) / (maxCost - minCost);
    //        var color = Color.FromArgb(
    //            128,
    //            (byte)(255 * normalizedCost),
    //            (byte)(255 * (1 - normalizedCost)),
    //            0
    //        );

    //        var pathX = cell.X * CellSize + OffsetX;
    //        var pathY = cell.Y * CellSize + OffsetY;

    //        dc.DrawRectangle(
    //            new SolidColorBrush(color),
    //            new Pen(new SolidColorBrush(color), 1),
    //            new Rect(
    //                pathX + (CellSize - rectangleSize) / 2,
    //                pathY + (CellSize - rectangleSize) / 2,
    //                rectangleSize,
    //                rectangleSize
    //            )
    //        );
    //    }
    //}


    /// <summary>
    /// Handles user actions on a selected cell in the board, allowing the user to set a start or target cell.
    /// </summary>
    /// <param name="position">The position of the mouse click on the canvas.</param>
    public void SelectActionOnCell(Position position)
    {
        var dialog = new CellActionDialog();
        var result = dialog.ShowDialog() ?? false;
        if (!result) return;

        switch (dialog.SelectedAction)
        {
            case CellAction.Start:
                if (Equals(Target, position))
                {
                    MessageBox.Show("Start and Target should not be the same cell!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Start = position;
                return;

            case CellAction.Target:
                if (Equals(Start, position))
                {
                    MessageBox.Show("Start and Target should not be the same cell!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Target = position;
                return;

            case CellAction.Cancel:
            default: return;
        }
    }

    /// <summary>
    /// Starts the selected pathfinding algorithm (A*, HandOnWall or BFS) and calculates the solved path.
    /// </summary>
    /// <param name="index">The index of the algorithm to use (0 for A*, 1 for HandOnWall(left-handed), 2 for HandOnWall(right-handed) , 3 for Bfs).</param>
    /// <param name="visualize">Specifies if the algorithm's steps should be visualized.</param>
    public void SolveBoard(int index)
    {
        if (Board is not Board board)
        {
            MessageBox.Show("No Maze defined", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (Solvers[index] is not IPathSolver solver)
        {
            MessageBox.Show("No valid algorithm", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (Start is not Position start)
        {
            MessageBox.Show("Start not set!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (Target is not Position target)
        {
            MessageBox.Show("Target not set!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        _stopwatch.Restart();
        Solve = solver.Solve(board, start, target);
        _stopwatch.Stop();
        Duration = _stopwatch.Elapsed.ToString(@"mm\:ss\.fff");
    }

    ///// <summary>
    ///// Resets the solved path and duration, clearing any previously calculated paths.
    ///// </summary>
    //public void ResetSolvedPath()
    //{
    //    if (Board == null) return;
    //    SolvedPath?.Clear();
    //    _stopwatch.Reset();
    //}

    //private void OnProcessedCellsUpdated(IEnumerable<(Cell Cell, double Cost)> processedCells)
    //{
    //    ProcessedCells = processedCells.ToList();
    //    OnPropertyChanged(nameof(ProcessedCells));
    //}

    //private async Task UpdateDurationInBackground()
    //{
    //    while (_stopwatch.IsRunning)
    //    {
    //        Application.Current.Dispatcher.Invoke(() =>
    //        {
    //            Duration = _stopwatch.Elapsed.ToString(@"mm\:ss\.fff");
    //        });
    //        await Task.Delay(50);
    //    }
    //}

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}