using Maze_Simulation.Generation;

namespace Maze_Simulation.SolvingAlgorithms;

/// <summary>
/// Implements the Breadth-First Search (BFS) algorithm to solve a maze.
/// BFS explores all possible paths in a breadth-first manner, guaranteeing the shortest path
/// in an unweighted grid or maze. The algorithm uses a queue to manage paths and avoids revisiting cells.
/// </summary>
public class BfsSolver : IPathSolver
{
    public event Action<IEnumerable<(Cell Cell, double Cost)>>? ProcessedCellsUpdated;

    private readonly List<(Cell Cell, double Cost)> _processedCells = [];
    public IEnumerable<(Cell Cell, double Cost)> ProcessedCells => _processedCells;

    private Cell[,]? _cells;
    private Cell? _start;
    private Cell? _target;

    /// <summary>
    /// Initializes the BFS solver with the maze cells and identifies the start and target cells.
    /// </summary>
    /// <param name="cells">Two-dimensional array of maze cells.</param>
    public void InitSolver(Cell[,] cells)
    {
        _cells = cells;
        _start = _cells.Cast<Cell>().First(c => c.IsStart);
        _target = _cells.Cast<Cell>().First(c => c.IsTarget);
    }

    /// <summary>
    /// Executes the Breadth-First Search (BFS) algorithm to find the shortest path from the start cell to the target cell.
    /// </summary>
    /// <param name="visualize">If true, enables visualization of the algorithm's progress.</param>
    /// <returns>
    /// A list of cells representing the shortest path, or null if no path is found.
    /// </returns>
    public async Task<IEnumerable<Cell>> StartSolver(bool visualize, int visualizationSpeed)
    {
        if (_cells == null || _start == null || _target == null) return null;

        // Queue for BFS: Each entry contains the current cell and its path so far
        var queue = new Queue<List<Cell>>();
        queue.Enqueue(new List<Cell> { _start });

        // Visited set to avoid revisiting cells
        var visited = new HashSet<Cell> { _start };

        while (queue.Any())
        {
            // Get the next path to explore
            var path = queue.Dequeue();
            var current = path.Last();

            if (visualize)
            {
                _processedCells.Add((current, path.Count));
                ProcessedCellsUpdated?.Invoke(_processedCells);
                await Task.Delay(visualizationSpeed);
            }

            if (current == _target)
            {
                _processedCells.Clear();
                return path;
            }

            // Explore neighbors
            foreach (var neighbor in GetNeighbors(current))
            {
                if (visited.Contains(neighbor)) continue;
                visited.Add(neighbor);
                var newPath = new List<Cell>(path) { neighbor };
                queue.Enqueue(newPath);
            }
        }

        // If the queue is empty and no path is found, return null
        return null;
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