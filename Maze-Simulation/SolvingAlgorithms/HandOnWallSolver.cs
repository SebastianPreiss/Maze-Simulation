using Maze_Simulation.Shared;

namespace Maze_Simulation.SolvingAlgorithms
{
    /// <summary>
    /// Implements the "Hand on Wall" algorithm to solve a maze.
    /// </summary>
    public class HandOnWallSolver //: IPathSolver
    {
        public event Action<IEnumerable<(Cell Cell, double Cost)>>? ProcessedCellsUpdated;
        public bool UseLeftHand { get; set; } = false;

        private readonly List<(Cell Cell, double Cost)> _processedCells = [];
        public IEnumerable<(Cell Cell, double Cost)> ProcessedCells => _processedCells;
        private Cell[,]? _cells;
        private Cell? _start;
        private Cell? _target;
        private Direction _currentDirection;

        /// <summary>
        /// Initializes the solver with the maze cells.
        /// </summary>
        /// <param name="cells">Cells of the maze.</param>
        public void InitSolver(Cell[,] cells)
        {
            _cells = cells;
            //_start = _cells.Cast<Cell>().First(c => c.IsStart);
            //_target = _cells.Cast<Cell>().First(c => c.IsTarget);
            _start = _cells[0, 0];
            _target = _cells[cells.GetLength(0) - 1, cells.GetLength(1) - 1];
        }

        /// <summary>
        /// Starts the "Hand on wall"-Algorithms.
        /// </summary>
        /// <returns>A list of cells representing the path from the start to the target. Returns null if no path can be found.</returns>
        public async Task<IEnumerable<Cell>> StartSolver(bool visualize, int visualizationSpeed)
        {
            if (_cells == null || _start == null || _target == null) return null;

            var path = new List<Cell> { _start };
            var current = _start;
            _currentDirection = UseLeftHand ? Direction.Right : Direction.Left;

            while (current != _target)
            {
                // Try to turn right first
                var newDirection = Turn(_currentDirection);

                if (CanMove(current, newDirection))
                {
                    // If we can direction, update the direction and direction
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
                    _currentDirection = Turn(_currentDirection);
                }

                path.Add(current);

                if (path.Count > _cells.GetLength(0) * _cells.GetLength(1)) return null;


                if (!visualize) continue;
                await Task.Delay(visualizationSpeed);
                _processedCells.Add((current, 0));
                ProcessedCellsUpdated?.Invoke(_processedCells);
            }
            _processedCells.Clear();
            return path;
        }

        private bool CanMove(Cell cell, Direction direction)
        {
            if (direction == Direction.None) return false;

            var (dx, dy) = direction.GetOffset();
            var newX = cell.X + dx;
            var newY = cell.Y + dy;

            // Check boundaries
            if (newX < 0 || newX >= _cells.GetLength(0) || newY < 0 || newY >= _cells.GetLength(1))
                return false;

            // Check for walls
            //return !cell.HasWall(direction);
            return false;
        }

        private Cell MoveTo(Cell cell, Direction direction)
        {
            var (dx, dy) = direction.GetOffset();
            return _cells[cell.X + dx, cell.Y + dy];
        }

        private Direction Turn(Direction current)
        {
            return current switch
            {
                Direction.Top => UseLeftHand ? Direction.Left : Direction.Right,
                Direction.Left => UseLeftHand ? Direction.Bottom : Direction.Top,
                Direction.Bottom => UseLeftHand ? Direction.Right : Direction.Left,
                Direction.Right => UseLeftHand ? Direction.Top : Direction.Bottom,
                _ => Direction.None
            };
        }
    }
}
