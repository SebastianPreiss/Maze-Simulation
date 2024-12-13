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
    private Queue<Cell> _processedCells = [];
    private Dictionary<Cell, double> _score = [];

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

        _processedCells = [];
        _score = [];

        while (openSet.Any())
        {
            // Select the cell with the lowest fScore (best estimated total cost)
            var current = openSet.OrderBy(c => _fScore[c]).First();

            // Add the current cell to the processed cells
            _processedCells.Enqueue(current);
            _score[current] = _gScore[current];

            if (current == targetCell)
            {
                var path = ReconstructPath(cameFrom, current).ToList();
                return new Solve(path, start, target, _processedCells, _score);
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

    private static IEnumerable<Direction> ReconstructPath(IReadOnlyDictionary<Cell, Cell> moves, Cell target)
    {
        foreach (var (current, next) in ReconstructFromTarget(moves, target).Reverse())
        {
            yield return GetDirection(current, next);
        }
    }

    private static IEnumerable<(Cell previous, Cell current)> ReconstructFromTarget(IReadOnlyDictionary<Cell, Cell> moves, Cell target)
    {
        var current = target;
        while (moves.ContainsKey(current))
        {
            var previous = moves[current];
            yield return (previous, current);
            current = previous;
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