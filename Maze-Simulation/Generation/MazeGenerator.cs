namespace Maze_Simulation.Generation;

using Maze_Simulation.Shared;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents a maze generator that utilizes the Depth First Search algorithm to create a maze. 
/// The generator modifies the walls of the cells to establish paths based on random moves selected 
/// from available options, starting from a specified starting point.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IBoardStrategy"/> interface and requires a 2D array of cells 
/// and a seed value for random number generation to initialize the maze generation process.
/// </remarks>
public class MazeGenerator : IBoardStrategy
{
    private readonly bool _multiPath;
    private readonly Stack<Cell> _track = new();
    private readonly Dictionary<Cell, bool> _visited = [];
    private readonly Dictionary<Cell, bool> _collapsed = [];

    private Board _board = default!;
    private Random _random = new();

    public int Seed { set => _random = new Random(value); }

    /// <summary>
    /// Initializes a new instance of the <see cref="MazeGenerator"/> class.
    /// This class uses a depth-first search algorithm to generate a maze and optionally adds multiple paths.
    /// </summary>
    /// <param name="seed">The seed for the random number generator, ensuring reproducibility of the maze structure. Default is 42.</param>
    /// <param name="multiPath">
    /// A boolean indicating whether to create multiple paths in the maze. 
    /// If set to <c>true</c>, additional random connections between cells will be created.
    /// Default is <c>false</c>.
    /// </param>
    public MazeGenerator(bool multiPath = false)
    {
        _multiPath = multiPath;
    }

    /// <summary>
    /// Generates the maze using a depth-first search algorithm. 
    /// It modifies the walls of the cells to create paths, based on the random moves chosen from the available options.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if an invalid direction is encountered during generation.</exception>
    public Board Generate(int width, int height)
    {
        _board = new Board(width, height);

        //init starting point(might be done by a strategy)
        var start = new Position(0, 0);
        _track.Push(_board[start]);

        while (_track.Count > 0)
        {
            var current = _track.Peek();
            _visited[current] = true;
            if (!TryGetNextMove(current, out var move, out var next))
            {
                _collapsed[current] = true;
                _track.Pop();
                continue;
            }
            _track.Push(next);
            ConnectCells(current, next, move);
        }
        if (_multiPath) AddRandomConnections();
        return _board;
    }

    /// <summary>
    /// Adds random connections to create additional paths in the maze.
    /// </summary>
    private void AddRandomConnections()
    {
        const int MagicNumber = 5;

        var numberOfCells = _board.Width * _board.Height;
        var numberOfConnections = numberOfCells / MagicNumber;
        Direction[] directions = [Direction.Top, Direction.Right, Direction.Bottom, Direction.Left];

        for (var i = 0; i < numberOfConnections; i++)
        {
            var x = _random.Next(_board.Width);
            var y = _random.Next(_board.Height);
            var current = _board[new Position(x, y)];
            var direction = directions[_random.Next(directions.Length)];

            if (BoardUtils.TryMove(_board, current, direction, out var next))
            {
                ConnectCells(current, _board[next], direction);
            }
        }
    }

    private bool TryGetNextMove(Cell cell, out Direction direction, [NotNullWhen(true)] out Cell? next)
    {
        direction = Direction.None;
        next = default;
        if (_collapsed.GetValueOrDefault(cell, false)) return false;
        foreach (var (dir, position) in BoardUtils.GetAvailableDirections(_board, cell, new BoardUtils.WallStrategy(), new BoardUtils.RandomNextStrategy(_random)))
        {
            next = _board[position];
            direction = dir;
            if (!_collapsed.GetValueOrDefault(next, false) && !_visited.GetValueOrDefault(next, false)) return true;
        }
        return false;
    }

    private static void ConnectCells(Cell a, Cell b, Direction connection)
    {
        var opening = GetOpposite(connection);

        a[connection] = false;
        b[opening] = false;
    }

    private static Direction GetOpposite(Direction connection)
    {
        return connection switch
        {
            Direction.Top => Direction.Bottom,
            Direction.Right => Direction.Left,
            Direction.Bottom => Direction.Top,
            Direction.Left => Direction.Right,
            _ => throw new ArgumentException($"Invalid connection \"{connection}\"")
        };
    }
}
