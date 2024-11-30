using Maze_Simulation.Shared;
using System.Diagnostics.CodeAnalysis;

namespace Maze_Simulation.Generation;

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
        _track.Push(_board[0, 0]);

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
            ConnectCells(ref current, ref next, move);

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
            var current = _board[x, y];
            var direction = directions[_random.Next(directions.Length)];

            if (TryMove(current, direction, out var next))
            {
                ConnectCells(ref current, ref next, direction);
            }
        }
    }

    private bool TryGetNextMove(Cell cell, out Direction direction, [NotNullWhen(true)] out Cell? next)
    {
        direction = Direction.None;
        next = default;
        if (_collapsed.GetValueOrDefault(cell, false)) return false;

        var available = GetWalls(cell).ToList();
        while (available.Any())
        {
            var index = _random.Next(available.Count);
            direction = available[index];
            available.RemoveAt(index);

            if (!TryMove(cell, direction, out next)) continue;
            if (!_collapsed.GetValueOrDefault(next, false) && !_visited.GetValueOrDefault(next, false)) return true;
        }
        return false;
    }

    private bool TryMove(Cell cell, Direction direction, [NotNullWhen(true)] out Cell? next)
    {
        next = default;
        var (newX, newY) = GetNewPosition(cell, direction);
        if (newX >= _board.Width || newX < 0) return false;
        if (newY >= _board.Height || newY < 0) return false;
        next = _board[newX, newY];
        return true;
    }

    private static IEnumerable<Direction> GetWalls(Cell cell)
    {
        if (cell[Direction.Top]) yield return Direction.Top;
        if (cell[Direction.Right]) yield return Direction.Right;
        if (cell[Direction.Bottom]) yield return Direction.Bottom;
        if (cell[Direction.Left]) yield return Direction.Left;
    }

    private static void ConnectCells(ref Cell a, ref Cell b, Direction connection)
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

    private static (int x, int y) GetNewPosition(Cell current, Direction move)
    {
        var (offsetX, offsetY) = move.GetOffset();
        var newX = current.X + offsetX;
        var newY = current.Y + offsetY;
        return (newX, newY);
    }
}
