namespace Maze_Simulation.Generation
{
    /// <summary>
    /// Represents a maze generator that utilizes the Depth First Search algorithm to create a maze. 
    /// The generator modifies the walls of the cells to establish paths based on random moves selected 
    /// from available options, starting from a specified starting point.
    /// </summary>
    /// <remarks>
    /// This class implements the <see cref="IMazeStrategy"/> interface and requires a 2D array of cells 
    /// and a seed value for random number generation to initialize the maze generation process.
    /// </remarks>

    public class MazeGenerator : IMazeStrategy
    {
        private readonly Cell[,]? _cells;
        private readonly Stack<Cell> _track = new();
        private readonly Random _random;

        public MazeGenerator(ref Cell[,]? cells, int seed = 42)
        {
            _cells = cells;
            _random = new Random(seed);
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
                _ => throw new ArgumentException(@$"Invalid move ""{{move}}""")
            };
        }
    }
}
