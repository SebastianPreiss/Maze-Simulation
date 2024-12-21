using Maze_Simulation.Shared;

namespace Maze_Simulation.SolvingAlgorithms;

/// <summary>
/// Implements the Breadth-First Search (BFS) algorithm to solve a maze.
/// BFS explores all possible paths in a breadth-first manner, guaranteeing the shortest path
/// in an unweighted grid or maze. The algorithm uses a queue to manage paths and avoids revisiting cells.
/// </summary>
public class BfsSolver : IPathSolver
{
    private Queue<Cell> _processedCells = [];
    private Dictionary<Cell, double> _score = [];


    public Solve? Solve(Board board, Position start, Position target)
    {
        var startCell = board[start];
        var targetCell = board[target];

        // Queue for BFS: Each entry contains the current cell and its path so far
        var queue = new Queue<List<Cell>>();
        queue.Enqueue([startCell]);

        _processedCells = [];
        _score = [];

        while (queue.Any())
        {
            // Get the next path to explore
            var cells = queue.Dequeue();
            var current = cells.Last();

            if (_processedCells.Contains(current)) continue;

            _processedCells.Enqueue(current);
            _score[current] = cells.Count;

            if (current == targetCell)
            {
                var path = ReconstructPath(cells).ToList();
                return new Solve(path, start, target, _processedCells, _score);
            }

            // Explore neighbors
            foreach (var (_, position) in BoardUtils.GetAvailableDirections(board, current, new BoardUtils.NoWallStrategy(), new BoardUtils.FirstNextStrategy()))
            {
                var neighbor = board[position];

                if (cells.Contains(neighbor)) continue;

                var newPath = new List<Cell>(cells) { neighbor };

                queue.Enqueue(newPath);
            }
        }

        // no solution found!
        return null;
    }

    private static IEnumerable<Direction> ReconstructPath(IList<Cell> path)
    {
        foreach (var cell in path)
        {
            if (path.IndexOf(cell) == path.Count - 1) break;
            yield return GetDirection(cell, path[path.IndexOf(cell) + 1]);
        }
    }

    // TODO: Move this method to a shared utility class (also used in A-star)
    private static Direction GetDirection(Cell start, Cell end)
    {
        if (start.X > end.X) return Direction.Left;
        if (start.X < end.X) return Direction.Right;
        if (start.Y > end.Y) return Direction.Bottom;
        if (start.Y < end.Y) return Direction.Top;
        return Direction.None;
    }
}