namespace Maze_Simulation.SolvingAlgorithms;

using Shared;

/// <summary>
/// Implements the A* algorithm to solve mazes. A* guarantees finding the shortest path
/// by combining the actual path cost (gScore) and an estimated cost (heuristic).
/// </summary>
public class AStarSolver : IPathSolver
{
    private Dictionary<Cell, double> _gScore = [];
    private Dictionary<Cell, double> _fScore = [];

    public event Action<IEnumerable<(Cell Cell, double Cost)>>? ProcessedCellsUpdated;

    private readonly List<(Cell Cell, double Cost)> _processedCells = [];
    //public IEnumerable<(Cell Cell, double Cost)> ProcessedCells => _processedCells;
    //private Cell[,]? _cells;
    //private Cell? _start;
    //private Cell? _target;

    ///// <summary>
    ///// Initializes the solver with the maze cells.
    ///// </summary>
    ///// <param name="cells">Cells of the maze.</param>
    //public void InitSolver(Cell[,] cells)
    //{
    //    _cells = cells;
    //    //_start = _cells.Cast<Cell>().First(c => c.IsStart);
    //    //_target = _cells.Cast<Cell>().First(c => c.IsTarget);
    //    _start = _cells[0, 0];
    //    _target = _cells[cells.GetLength(0) - 1, cells.GetLength(1) - 1];
    //}

    /// <summary>
    /// Starts the A* pathfinding algorithm to find the shortest path from the start cell to the target cell.
    /// </summary>
    /// <returns>A list of cells representing the path from the start to the target. Returns null if no path can be found.</returns>
    public Solve? Solve(Board board, Position start, Position target)
    {
        var startCell = board[start];
        var targetCell = board[target];

        // Open set: a list of cells that need to be evaluated, starting with the start cell
        var openSet = new List<Cell> { startCell };

        // Dictionary to keep track of the most efficient previous cell for each visited cell
        var cameFrom = new Dictionary<Cell, Cell>();

        // Dictionary to store the cost of the cheapest path from the start to each cell
        _gScore = board.ToDictionary(c => c, c => double.MaxValue);
        _gScore[startCell] = 0;

        // Dictionary to store the estimated total cost (gScore + heuristic) for each cell
        _fScore = board.ToDictionary(c => c, c => double.MaxValue);
        _fScore[startCell] = Heuristic(startCell, targetCell);

        while (openSet.Any())
        {
            // Select the cell with the lowest fScore (best estimated total cost)
            var current = openSet.OrderBy(c => _fScore[c]).First();

            //// Visualization: process and display the current cell
            //if (visualize)
            //{
            //    _processedCells.Add((current, gScore[current]));
            //    ProcessedCellsUpdated?.Invoke(_processedCells);
            //    await Task.Delay(visualizationSpeed);
            //}

            if (current == targetCell)
            {
                _processedCells.Clear();
                var path = ReconstructPath(cameFrom);
                return new Solve(path.ToList(), start, target);
            }

            openSet.Remove(current);

            foreach (var (_, position) in BoardUtils.GetAvailableDirections(board, current, new BoardUtils.NoWallStrategy(), new BoardUtils.FirstNextStrategy()))
            {
                var neighbor = board[position];

                // Tentative gScore: cost to move to the neighbor through the current cell
                var tentativeGScore = _gScore[current] + 1;

                // If this path to the neighbor is not better, skip it
                if (!(tentativeGScore < _gScore[neighbor])) continue;

                // Update the path and scores for the neighbor
                cameFrom[neighbor] = current;
                _gScore[neighbor] = tentativeGScore;
                _fScore[neighbor] = _gScore[neighbor] + Heuristic(neighbor, targetCell);

                // If the neighbor is not already in the open set, add it
                if (!openSet.Contains(neighbor)) openSet.Add(neighbor);
            }
        }

        // no solution found!
        return null;
    }

    private static IEnumerable<Direction> ReconstructPath(IReadOnlyDictionary<Cell, Cell> cameFrom)
    {
        foreach (var (start, end) in cameFrom)
        {
            yield return GetDirection(start, end);
        }
    }

    private static double Heuristic(Cell a, Cell b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }

    private static Direction GetDirection(Cell start, Cell end)
    {
        if (start.X > end.X) return Direction.Left;
        if (start.X < end.X) return Direction.Right;
        if (start.Y > end.Y) return Direction.Bottom;
        if (start.Y < end.Y) return Direction.Top;
        return Direction.None;
    }
}