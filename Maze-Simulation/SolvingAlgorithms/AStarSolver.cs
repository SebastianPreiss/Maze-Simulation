using Maze_Simulation.Generation;

namespace Maze_Simulation.SolvingAlgorithms
{
    public class AStarSolver : IPathSolver
    {
        public event Action<IEnumerable<(Cell Cell, double Cost)>>? ProcessedCellsUpdated;

        private readonly List<(Cell Cell, double Cost)> _processedCells = [];
        public IEnumerable<(Cell Cell, double Cost)> ProcessedCells => _processedCells;
        private Cell[,]? _cells;
        private Cell? _start;
        private Cell? _target;

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
        /// Starts the A* pathfinding algorithm to find the shortest path from the start cell to the target cell.
        /// </summary>
        /// <returns>A list of cells representing the path from the start to the target. Returns null if no path can be found.</returns>
        public async Task<List<Cell>> StartSolver(bool visualize)
        {
            if (_cells == null || _start == null || _target == null) return null;

            // Open set: a list of cells that need to be evaluated, starting with the start cell
            var openSet = new List<Cell> { _start };

            // Dictionary to keep track of the most efficient previous cell for each visited cell
            var cameFrom = new Dictionary<Cell, Cell>();

            // Dictionary to store the cost of the cheapest path from the start to each cell
            var gScore = _cells.Cast<Cell>().ToDictionary(c => c, c => double.MaxValue);
            gScore[_start] = 0;

            // Dictionary to store the estimated total cost (gScore + heuristic) for each cell
            var fScore = _cells.Cast<Cell>().ToDictionary(c => c, c => double.MaxValue);
            fScore[_start] = Heuristic(_start, _target);

            while (openSet.Any())
            {
                // Select the cell with the lowest fScore (best estimated total cost)
                var current = openSet.OrderBy(c => fScore[c]).First();

                // Visualization: process and display the current cell
                if (visualize)
                {
                    _processedCells.Add((current, gScore[current]));
                    ProcessedCellsUpdated?.Invoke(_processedCells);
                    await Task.Delay(50);
                }

                if (current == _target)
                {
                    _processedCells.Clear();
                    return ReconstructPath(cameFrom, current);
                }

                openSet.Remove(current);

                foreach (var neighbor in GetNeighbors(current))
                {
                    // Tentative gScore: cost to move to the neighbor through the current cell
                    var tentativeGScore = gScore[current] + 1;

                    // If this path to the neighbor is not better, skip it
                    if (!(tentativeGScore < gScore[neighbor])) continue;

                    // Update the path and scores for the neighbor
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, _target);

                    // If the neighbor is not already in the open set, add it
                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }

            return null;
        }


        private static List<Cell>? ReconstructPath(IReadOnlyDictionary<Cell, Cell> cameFrom, Cell current)
        {
            var totalPath = new List<Cell> { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Add(current);
            }
            totalPath.Reverse();
            return totalPath;
        }

        private static double Heuristic(Cell a, Cell b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        private IEnumerable<Cell> GetNeighbors(Cell cell)
        {
            var directions = new Dictionary<Move, (int dx, int dy)>
            {
                { Move.Top, (0, 1) },
                { Move.Right, (1, 0) },
                { Move.Bottom, (0, -1) },
                { Move.Left, (-1, 0) }
            };

            foreach (var direction in directions)
            {
                var x = cell.X + direction.Value.dx;
                var y = cell.Y + direction.Value.dy;

                if (x < 0 || x >= _cells.GetLength(0) || y < 0 || y >= _cells.GetLength(1)) continue;
                if (!HasWall(cell, direction.Key))
                {
                    yield return _cells[x, y];
                }
            }
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
