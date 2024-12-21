using Maze_Simulation.Shared;
using System.Diagnostics.CodeAnalysis;

namespace Maze_Simulation.SolvingAlgorithms;

/// <summary>
/// Implements the "Hand on Wall" algorithm to solve a maze.
/// </summary>
public class HandOnWallSolver(bool leftHanded) : IPathSolver
{
    private Queue<Cell> _processedCells = [];
    private Dictionary<Cell, double> _score = [];
    private Dictionary<Cell, bool> _visited = [];

    public Solve? Solve(Board board, Position start, Position target)
    {
        var startCell = board[start];
        var targetCell = board[target];

        var directions = new Stack<Direction>();
        var path = new Stack<Cell>();
        path.Push(startCell);

        _processedCells = [];
        _score = [];
        _visited = [];

        while (path.Count > 0)
        {
            var current = path.Peek();
            _visited[current] = true;
            _processedCells.Enqueue(current);
            _score[current] = 1;


            if (current == targetCell)
            {
                return new Solve(directions.Reverse().ToList(), start, target, _processedCells, _score);
            }

            if (!TryGetMove(board, current, out var move))
            {
                // backtrack when no moves are available
                path.Pop();
                directions.TryPop(out var _);
                continue;
            }

            var (direction, next) = move.Value;
            path.Push(next);
            directions.Push(direction);
        }
        // no solution found!
        return null;
    }

    private bool TryGetMove(Board board, Cell cell, [NotNullWhen(true)] out (Direction, Cell)? move)
    {
        move = null;
        BoardUtils.IAvailableStrategy strategy = leftHanded ? new LeftHandStrategy() : new RightHandStrategy();
        var moves = BoardUtils.GetAvailableDirections(board, cell, strategy, new BoardUtils.FirstNextStrategy()).ToList();

        foreach (var (direction, _) in moves)
        {
            if (!BoardUtils.TryMove(board, cell, direction, out var nextPos)) continue;
            var next = board[nextPos];
            if (_visited.GetValueOrDefault(next, false)) continue;
            move = (direction, next);
            return true;
        }
        return false;
    }

    class RightHandStrategy : BoardUtils.IAvailableStrategy
    {
        public IEnumerable<Direction> GetAvailable(Cell cell)
        {
            Direction[] toCheck = [Direction.Top, Direction.Right, Direction.Bottom, Direction.Left, Direction.Bottom, Direction.Right, Direction.Top];
            var available = new List<Direction>();

            foreach (var direction in toCheck)
            {
                var expectedWall = Rotate(direction);
                if (cell[direction]) continue;
                if (!(cell[expectedWall] || available.Contains(expectedWall))) continue;
                available.Add(direction);
            }
            return available;
        }
    }

    class LeftHandStrategy : BoardUtils.IAvailableStrategy
    {
        public IEnumerable<Direction> GetAvailable(Cell cell)
        {
            Direction[] toCheck = [Direction.Top, Direction.Left, Direction.Bottom, Direction.Right, Direction.Bottom, Direction.Left, Direction.Top];
            var available = new List<Direction>();

            foreach (var direction in toCheck)
            {
                var expectedWall = Rotate(direction, false);
                if (cell[direction]) continue;
                if (!(cell[expectedWall] || available.Contains(expectedWall))) continue;
                available.Add(direction);
            }
            return available;
        }
    }

    private static Direction Rotate(Direction direction, bool clockwise = true)
    {
        return direction switch
        {
            Direction.Top => clockwise ? Direction.Right : Direction.Left,
            Direction.Right => clockwise ? Direction.Bottom : Direction.Top,
            Direction.Bottom => clockwise ? Direction.Left : Direction.Right,
            Direction.Left => clockwise ? Direction.Top : Direction.Bottom,
            _ => Direction.None
        };
    }
}
