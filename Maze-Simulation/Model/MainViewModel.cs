namespace Maze_Simulation.Model;

using Generation;
using Shared;
using SolvingAlgorithms;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

/// <summary>
/// Represents the main view model for managing the maze generation, pathfinding, 
/// and UI interactions in the application.
/// </summary>
public class MainViewModel : INotifyPropertyChanged
{
    //Maze properties
    public Board? Board { get; private set; }

    public List<IBoardStrategy> Generators { get; } = [];
    public List<IPathSolver> Solvers { get; } = [];

    //Pathfinding properties
    public List<Cell>? SolvedPath { get; private set; }
    public IPathSolver? Solver;

    private List<(Cell Cell, double Cost)> _processedCells;
    public List<(Cell Cell, double Cost)> ProcessedCells
    {
        get => _processedCells;
        set
        {
            _processedCells = value;
            OnPropertyChanged(nameof(ProcessedCells));
        }
    }
    private bool _isPathSolved;

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
        Solvers.Add(new BfsSolver());
        Solvers.Add(new HandOnWallSolver { UseLeftHand = true });
        Solvers.Add(new HandOnWallSolver { UseLeftHand = false });

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
    }

    ///// <summary>
    ///// Draws the solved path of the maze onto the given DrawingContext.
    ///// </summary>
    ///// <param name="dc">The DrawingContext used to render the solved path.</param>
    //public void DrawSolvedPath(DrawingContext dc)
    //{
    //    if (Board == null || SolvedPath == null) return;
    //    var rectangleSize = CellSize * 0.4;

    //    for (var i = 0; i < Board.Cells.GetLength(0); i++)
    //    {
    //        for (var j = 0; j < Board.Cells.GetLength(1); j++)
    //        {
    //            var cell = Board.Cells[i, j];

    //            if (SolvedPath == null || !SolvedPath.Contains(cell) ||
    //                cell is not { IsStart: false, IsTarget: false }) continue;

    //            var pathX = cell.X * CellSize + OffsetX;
    //            var pathY = cell.Y * CellSize + OffsetY;

    //            var cellIndex = SolvedPath.IndexOf(cell);
    //            var totalSteps = SolvedPath.Count;

    //            var pathColor = Color.FromArgb(128,
    //                (byte)(255 * cellIndex / (double)totalSteps),
    //                0,
    //                (byte)(255 * (1 - cellIndex / (double)totalSteps)));

    //            dc.DrawRectangle(
    //                new SolidColorBrush(pathColor),
    //                new Pen(new SolidColorBrush(pathColor), 1),
    //                new Rect(
    //                    pathX + (CellSize - rectangleSize) / 2,
    //                    pathY + (CellSize - rectangleSize) / 2,
    //                    rectangleSize,
    //                    rectangleSize
    //                )
    //            );
    //        }
    //    }
    //}

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


    ///// <summary>
    ///// Handles user actions on a selected cell in the board, allowing the user to set a start or target cell.
    ///// </summary>
    ///// <param name="position">The position of the mouse click on the canvas.</param>
    //public void SelectActionOnCell(Point position)
    //{
    //    var relativeX = position.X - OffsetX;
    //    var relativeY = position.Y - OffsetY;

    //    var row = (int)(relativeX / CellSize);
    //    var column = (int)(relativeY / CellSize);

    //    if (Board == null || row < 0 || row >= Board.Cells.GetLength(0) || column < 0 ||
    //        column >= Board.Cells.GetLength(1)) return;

    //    var clickedCell = Board.Cells[row, column];

    //    var dialog = new CellActionDialog();
    //    if (dialog.ShowDialog() != true) return;
    //    switch (dialog.SelectedAction)
    //    {
    //        case "Start":
    //            if (clickedCell.IsTarget)
    //            {
    //                MessageBox.Show("Cannot set start on target cell", "Error", MessageBoxButton.OK,
    //                    MessageBoxImage.Error);
    //                break;
    //            }

    //            foreach (var cell in Board.Cells)
    //            {
    //                cell.IsStart = false;
    //            }

    //            clickedCell.IsStart = true;
    //            clickedCell.IsTarget = false;
    //            break;

    //        case "Target":
    //            if (clickedCell.IsStart)
    //            {
    //                MessageBox.Show("Cannot set target on start cell", "Error", MessageBoxButton.OK,
    //                    MessageBoxImage.Error);
    //                break;
    //            }

    //            foreach (var cell in Board.Cells)
    //            {
    //                cell.IsTarget = false;
    //            }

    //            clickedCell.IsStart = false;
    //            clickedCell.IsTarget = true;
    //            break;

    //        case "Cancel":
    //            break;
    //    }
    //}

    ///// <summary>
    ///// Starts the selected pathfinding algorithm (A*, HandOnWall or BFS) and calculates the solved path.
    ///// </summary>
    ///// <param name="index">The index of the algorithm to use (0 for A*, 1 for HandOnWall(left-handed), 2 for HandOnWall(right-handed) , 3 for Bfs).</param>
    ///// <param name="visualize">Specifies if the algorithm's steps should be visualized.</param>
    //public async Task StartAlgorithm(int index, bool visualize, int visualizationSpeed)
    //{
    //    if (Board == null)
    //    {
    //        MessageBox.Show("No Maze defined", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    //        return;
    //    }

    //    Solver = index switch
    //    {
    //        0 => _aStarSolver,
    //        1 => new HandOnWallSolver { UseLeftHand = true },
    //        2 => new HandOnWallSolver { UseLeftHand = false },
    //        3 => _bfsSolver,
    //        _ => null                     // Default
    //    };

    //    Solver.ProcessedCellsUpdated += OnProcessedCellsUpdated;

    //    if (Solver == null)
    //    {
    //        MessageBox.Show("No valid algorithm", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    //        return;
    //    }

    //    try
    //    {
    //        _isPathSolved = false;
    //        _stopwatch.Restart();

    //        UpdateDurationInBackground();

    //        Solver.InitSolver(Board.Cells);

    //        SolvedPath = (await Solver.StartSolver(visualize, visualizationSpeed)).ToList();

    //        if (SolvedPath == null)
    //        {
    //            MessageBox.Show("Unable to find Path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    //        }
    //        else _isPathSolved = true;

    //    }
    //    catch (Exception ex)
    //    {
    //        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    //    }
    //    finally
    //    {
    //        _stopwatch.Stop();
    //        Duration = _stopwatch.Elapsed.ToString(@"mm\:ss\.fff");
    //    }
    //}

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