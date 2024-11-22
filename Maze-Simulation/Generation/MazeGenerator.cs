namespace Maze_Simulation.Generation
{
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
        private readonly Cell[,]? _cells;
        private readonly Stack<Cell> _track = new();
        private readonly Random _random;

        /// <summary>
        /// Initializes a new instance of the <see cref="MazeGenerator"/> class.
        /// This class uses a depth-first search algorithm to generate a maze and optionally adds multiple paths.
        /// </summary>
        /// <param name="cells">A reference to the 2D array of <see cref="Cell"/> objects representing the maze.</param>
        /// <param name="seed">The seed for the random number generator, ensuring reproducibility of the maze structure. Default is 42.</param>
        /// <param name="multiPath">
        /// A boolean indicating whether to create multiple paths in the maze. 
        /// If set to <c>true</c>, additional random connections between cells will be created.
        /// Default is <c>false</c>.
        /// </param>

        public MazeGenerator(ref Cell[,]? cells, int seed = 42, bool multiPath = false)
        {
            _cells = cells;
            _random = new Random(seed);
            _multiPath = multiPath;
        }

        /// <summary>
        /// Resets the maze to its initial state, clearing any modifications made during generation.
        /// </summary>
        public void Reset()
        {
            _cells.Reset();
        }

        /// <summary>
        /// Sets the starting point for maze generation at the specified coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the starting point.</param>
        /// <param name="y">The y-coordinate of the starting point.</param>

        public void SetStartingPoint(int x, int y)
        {
            _track.Clear();
            _track.Push(_cells[x, y]);
        }

        /// <summary>
        /// Generates the maze using a depth-first search algorithm. 
        /// It modifies the walls of the cells to create paths, based on the random moves chosen from the available options.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if an invalid move is encountered during generation.</exception>
        public void Generate()
        {
            while (_track.Any())
            {
                var current = _track.Peek();
                current.IsVisited = true;
                if (!TryGetNextMove(current, out var move))
                {
                    _track.Pop();
                    continue;
                }
                var (x, y) = GetNewPosition(current, move);
                var nextCell = _cells[x, y];
                _track.Push(nextCell);
                switch (move)
                {
                    case Move.Top:
                        current.Walls[Cell.Top] = false;
                        nextCell.Walls[Cell.Bottom] = false;
                        nextCell.Available.Remove(Move.Bottom);
                        break;
                    case Move.Right:
                        current.Walls[Cell.Right] = false;
                        nextCell.Walls[Cell.Left] = false;
                        nextCell.Available.Remove(Move.Left);
                        break;
                    case Move.Bottom:
                        current.Walls[Cell.Bottom] = false;
                        nextCell.Walls[Cell.Top] = false;
                        nextCell.Available.Remove(Move.Top);
                        break;
                    case Move.Left:
                        current.Walls[Cell.Left] = false;
                        nextCell.Walls[Cell.Right] = false;
                        nextCell.Available.Remove(Move.Right);
                        break;
                    case Move.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            if (_multiPath) AddRandomConnections();
        }

        /// <summary>
        /// Adds random connections to create additional paths in the maze.
        /// </summary>
        private void AddRandomConnections()
        {
            if (_cells is null) return;
            var numberOfCells = _cells.GetLength(0) * _cells.GetLength(1);
            var numberOfConnections = numberOfCells / 5;
            for (var i = 0; i < numberOfConnections; i++)
            {
                var x = _random.Next(_cells.GetLength(0));
                var y = _random.Next(_cells.GetLength(1));

                var cell = _cells[x, y];
                var moves = Enum.GetValues(typeof(Move)).Cast<Move>().Where(m => m != Move.None).ToList();
                var randomMove = moves[_random.Next(moves.Count)];
                var (newX, newY) = GetNewPosition(cell, randomMove);

                if (newX >= 0 && newX < _cells.GetLength(0) && newY >= 0 && newY < _cells.GetLength(1))
                {
                    var neighbor = _cells[newX, newY];
                    switch (randomMove)
                    {
                        case Move.Top:
                            cell.Walls[Cell.Top] = false;
                            neighbor.Walls[Cell.Bottom] = false;
                            break;
                        case Move.Right:
                            cell.Walls[Cell.Right] = false;
                            neighbor.Walls[Cell.Left] = false;
                            break;
                        case Move.Bottom:
                            cell.Walls[Cell.Bottom] = false;
                            neighbor.Walls[Cell.Top] = false;
                            break;
                        case Move.Left:
                            cell.Walls[Cell.Left] = false;
                            neighbor.Walls[Cell.Right] = false;
                            break;
                        case Move.None:
                            break;
                        default:
                            throw new ArgumentException($"Invalid move \"{randomMove}\"");
                    }
                }
            }
        }

        private bool TryGetNextMove(Cell cell, out Move move)
        {
            move = Move.None;
            if (cell.IsCollapsed) return false;
            while (!cell.IsCollapsed)
            {
                var moveIndex = _random.Next(cell.Available.Count());
                move = cell.Available[moveIndex];
                cell.Available.RemoveAt(moveIndex);
                var (newX, newY) = GetNewPosition(cell, move);
                if (newX >= _cells.GetLength(0) || newX < 0) continue;
                if (newY >= _cells.GetLength(1) || newY < 0) continue;
                if (!_cells[newX, newY].IsCollapsed && !_cells[newX, newY].IsVisited) return true;
            }
            return false;
        }

        private static (int x, int y) GetNewPosition(Cell current, Move move)
        {
            var (offsetX, offsetY) = GetOffset(move);
            var newX = current.X + offsetX;
            var newY = current.Y + offsetY;
            return (newX, newY);
        }

        private static (int x, int y) GetOffset(Move move)
        {
            return move switch
            {
                Move.Top => (0, 1),
                Move.Right => (1, 0),
                Move.Bottom => (0, -1),
                Move.Left => (-1, 0),
                _ => throw new ArgumentException($"Invalid move \"{move}\"")
            };
        }
    }
}
