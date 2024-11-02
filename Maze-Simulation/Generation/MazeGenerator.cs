namespace Maze_Simulation.Generation
{
    /// <summary>
    /// A Maze generator using Depth First Search algorithm
    /// </summary>
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

        public void Reset()
        {
            _cells.Reset();
        }

        public void SetStartingPoint(int x, int y)
        {
            _track.Clear();
            _track.Push(_cells[x, y]);
        }

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
            if (move == Move.Top) return (0, 1);
            if (move == Move.Right) return (1, 0);
            if (move == Move.Bottom) return (0, -1);
            if (move == Move.Left) return (-1, 0);
            throw new ArgumentException(@$"Invalid move ""{{move}}""");
        }
    }
}
