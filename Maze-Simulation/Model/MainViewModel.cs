namespace Maze_Simulation.Model;

using Generation;
using Shared;
using SolvingAlgorithms;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

/// <summary>
/// Represents the main view model for managing the maze generation, pathfinding, 
/// and UI interactions in the application.
/// </summary>
public class MainViewModel : INotifyPropertyChanged
{
    public const int DEFAULT_SPEED_MS = 100;

    // Maze properties
    public Board? Board { get; private set; }
    public Position? Start { get; private set; }
    public Position? Target { get; private set; }
    public Solve? Solve { get; private set; }

    public List<IBoardStrategy> Generators { get; } = [];
    public List<IPathSolver> Solvers { get; } = [];

    public DispatcherTimer Timer { get; private set; }
    public int ToVisualize { get; private set; } = 0;
    public bool IsRunning => ToVisualize < Solve?.ProcessingOrder.Count;

    public delegate void BoardEvent(Board board);
    public event BoardEvent OnBoardChanged;

    // UI properties
    public event PropertyChangedEventHandler? PropertyChanged;

    // Visualizing the duration of a solver
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

    /// <summary>
    /// Initializes the MainViewModel, setting up the maze generation and solving algorithms,
    /// as well as the default timer settings for visualizing the solution process.
    /// </summary>
    public MainViewModel()
    {
        _stopwatch = new Stopwatch();

        Generators.Add(new MazeGenerator());

        Solvers.Add(new AStarSolver());
        Solvers.Add(new BfsSolver());
        Solvers.Add(new HandOnWallSolver(true));
        Solvers.Add(new HandOnWallSolver(false));

        Duration = "0:0.0";

        Timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(DEFAULT_SPEED_MS)
        };
        Timer.Tick += (s, e) =>
        {
            ToVisualize++;
            if (!IsRunning) Timer.Stop();
        };
    }

    /// <summary>
    /// Generates a new maze board with the specified dimensions and seed value.
    /// The default start and target positions are initialized after the board generation.
    /// </summary>
    /// <param name="seed">The seed used for random generation of the board.</param>
    /// <param name="width">The width of the maze board.</param>
    /// <param name="height">The height of the maze board.</param>
    /// <param name="multiPath">Indicates whether multiple paths should be generated.</param>
    public void GenerateBoard(string seed, int width, int height, bool multiPath)
    {
        if (!int.TryParse(seed, out var parsedSeed)) throw new ArgumentException("The provided seed value is not a valid integer.", nameof(seed));

        // Select the first generator in the list 
        var generator = Generators[0];
        generator.Seed = parsedSeed;
        Board = generator.Generate(width, height, multiPath);
        StopVisualisation();
        OnBoardChanged.Invoke(Board);
        Target = new Position(2, 2);
        Start = new Position(width - 2, height - 2);
    }

    /// <summary>
    /// Handles user interactions with the maze board to set the start and target cells.
    /// Left-clicking on a cell sets the start position, while right-clicking sets the target position.
    /// If the start and target are the same cell, an error message is displayed.
    /// </summary>
    /// <param name="position">The position of the cell the user clicked on the board.</param>
    /// <param name="mouseButton">The mouse button that triggered the event (left or right).</param>
    public void SelectActionOnCell(Position position, MouseButtonEventArgs mouseButton)
    {
        switch (mouseButton.ChangedButton)
        {
            case MouseButton.Left:
                if (Equals(Target, position))
                {
                    MessageBox.Show("Start and Target should not be the same cell!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Start = position;
                return;

            case MouseButton.Right:
                if (Equals(Start, position))
                {
                    MessageBox.Show("Start and Target should not be the same cell!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Target = position;
                return;

            default: return;
        }
    }

    /// <summary>
    /// Starts the selected pathfinding algorithm and solves the maze.
    /// The algorithm to be used is selected by the given index.
    /// </summary>
    /// <param name="index">The index of the algorithm to use (0 for A*, 1 for HandOnWall(left-handed), 2 for HandOnWall(right-handed) , 3 for Bfs).</param>
    public void SolveBoard(int index)
    {
        if (Board is not Board board)
        {
            MessageBox.Show("No Maze defined", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        if (Solvers[index] is not IPathSolver solver)
        {
            MessageBox.Show("No valid algorithm", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        _stopwatch.Restart();
        Solve = solver.Solve(board, start, target);
        _stopwatch.Stop();

        if (Solve is null)
        {
            MessageBox.Show("No path found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        StartVisualisation();
        Duration = _stopwatch.Elapsed.ToString(@"mm\:ss\.fff");
    }

    private void StartVisualisation()
    {
        Timer.Start();
        ToVisualize = 0;
    }

    private void StopVisualisation()
    {
        Timer.Stop();
        ToVisualize = 0;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}