namespace Maze_Simulation.Shared;
/// <summary>
/// Provides utility methods for initializing and resetting a 2D array of cells.
/// </summary>
public static class BoardUtils
{
    public static Position GetNewPosition(Cell current, Direction move)
    {
        var (offsetX, offsetY) = move.GetOffset();
        var newX = current.X + offsetX;
        var newY = current.Y + offsetY;
        return new(newX, newY);
    }

    public static bool TryMove(Board board, Cell cell, Direction direction, out Position nextPos)
    {
        nextPos = GetNewPosition(cell, direction);
        if (nextPos.X >= board.Width || nextPos.X < 0) return false;
        if (nextPos.Y >= board.Height || nextPos.Y < 0) return false;
        return true;
    }

    public static IEnumerable<(Direction, Position)> GetAvailableDirections(Board board, Cell cell, IAvailableStrategy availableStrategy, INextStrategy nextStrategy)
    {
        var available = availableStrategy.GetAvailable(cell).ToList();
        while (available.Any())
        {
            var index = nextStrategy.GetNextIndex(available.Count);
            var direction = available[index];
            available.RemoveAt(index);

            if (TryMove(board, cell, direction, out var position)) yield return (direction, position);
        }
    }

    public interface IAvailableStrategy
    {
        public IEnumerable<Direction> GetAvailable(Cell cell);
    }

    public class WallStrategy : IAvailableStrategy
    {
        public IEnumerable<Direction> GetAvailable(Cell cell)
        {
            if (cell[Direction.Top]) yield return Direction.Top;
            if (cell[Direction.Right]) yield return Direction.Right;
            if (cell[Direction.Bottom]) yield return Direction.Bottom;
            if (cell[Direction.Left]) yield return Direction.Left;
        }
    }

    public class NoWallStrategy : IAvailableStrategy
    {
        public IEnumerable<Direction> GetAvailable(Cell cell)
        {
            if (!cell[Direction.Top]) yield return Direction.Top;
            if (!cell[Direction.Right]) yield return Direction.Right;
            if (!cell[Direction.Bottom]) yield return Direction.Bottom;
            if (!cell[Direction.Left]) yield return Direction.Left;
        }
    }

    public interface INextStrategy
    {
        public int GetNextIndex(int available);
    }

    public class FirstNextStrategy : INextStrategy
    {
        public int GetNextIndex(int available)
        {
            return 0;
        }
    }

    public class RandomNextStrategy(Random random) : INextStrategy
    {
        public int GetNextIndex(int available)
        {
            return random.Next(available);
        }
    }
}