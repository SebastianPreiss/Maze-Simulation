using Maze_Simulation.Generation;

namespace Maze_Simulation.SolvingAlgorithms
{
    /// <summary>
    /// Implements the "Hand on Wall" algorithm to solve a maze.
    /// </summary>
    public class HandOnWallSolver : IPathSolver
    {
        public event Action<IEnumerable<(Cell Cell, double Cost)>>? ProcessedCellsUpdated;
        public bool UseLeftHand { get; set; } = false;

        private readonly List<(Cell Cell, double Cost)> _processedCells = [];
        public IEnumerable<(Cell Cell, double Cost)> ProcessedCells => _processedCells;
        private Cell[,]? _cells;
        private Cell? _start;
        private Cell? _target;
        private Move _currentDirection;

        /// <summary>
        /// Initializes the solver with the maze cells.
        /// </summary>
        /// <param name="cells">Cells of the maze.</param>
        public void InitSolver(Cell[,] cells)
        {
            _cells = cells;
            _start = _cells.Cast<Cell>().First(c => c.IsStart);
            _target = _cells.Cast<Cell>().First(c => c.IsTarget);
        }

        /// <summary>
        /// Starts the "Hand on wall"-Algorithmus.
        /// </summary>
        /// <returns>A list of cells representing the path from the start to the target. Returns null if no path can be found.</returns>
        public async Task<List<Cell>> StartSolver(bool visualize)
        {
            if (_cells == null || _start == null || _target == null) return null;

            var path = new List<Cell> { _start };
            var current = _start;
            _currentDirection = UseLeftHand ? Move.Right : Move.Left;

            while (current != _target)
            {
                // Try to turn right first
                var newDirection = UseLeftHand ? TurnLeft(_currentDirection) : TurnRight(_currentDirection);

                if (CanMove(current, newDirection))
                {
                    // If we can move, update the direction and move
                    _currentDirection = newDirection;
                    current = MoveTo(current, _currentDirection);
                }
                else if (CanMove(current, _currentDirection))
                {
                    // Otherwise, continue straight
                    current = MoveTo(current, _currentDirection);
                }
                else
                {
                    // Otherwise, turn left
                    _currentDirection = TurnLeft(_currentDirection);
                }

                path.Add(current);

                if (path.Count > _cells.GetLength(0) * _cells.GetLength(1)) return null;


                if (!visualize) continue;
                await Task.Delay(50);
                _processedCells.Add((current, 0));
                ProcessedCellsUpdated?.Invoke(_processedCells);
            }
            _processedCells.Clear();
            return path;
        }

        private bool CanMove(Cell cell, Move direction)
        {
            if (direction == Move.None) return false;

            var (dx, dy) = GetDirectionOffset(direction);
            var newX = cell.X + dx;
            var newY = cell.Y + dy;

            // Check boundaries
            if (newX < 0 || newX >= _cells.GetLength(0) || newY < 0 || newY >= _cells.GetLength(1))
                return false;

            // Check for walls
            return !HasWall(cell, direction);
        }

        private Cell MoveTo(Cell cell, Move direction)
        {
            var (dx, dy) = GetDirectionOffset(direction);
            return _cells[cell.X + dx, cell.Y + dy];
        }

        private static Move TurnRight(Move current)
        {
            return current switch
            {
                Move.Top => Move.Right,
                Move.Right => Move.Bottom,
                Move.Bottom => Move.Left,
                Move.Left => Move.Top,
                _ => Move.None
            };
        }

        private static Move TurnLeft(Move current)
        {
            return current switch
            {
                Move.Top => Move.Left,
                Move.Left => Move.Bottom,
                Move.Bottom => Move.Right,
                Move.Right => Move.Top,
                _ => Move.None
            };
        }

        private static (int dx, int dy) GetDirectionOffset(Move direction)
        {
            return direction switch
            {
                Move.Top => (0, 1),
                Move.Right => (1, 0),
                Move.Bottom => (0, -1),
                Move.Left => (-1, 0),
                _ => (0, 0)
            };
        }

        private static bool HasWall(Cell cell, Move direction)
        {
            return direction switch
            {
                Move.Top => cell.Walls[Cell.Top],
                Move.Right => cell.Walls[Cell.Right],
                Move.Bottom => cell.Walls[Cell.Bottom],
                Move.Left => cell.Walls[Cell.Left],
                _ => true
            };
        }
    }
}
