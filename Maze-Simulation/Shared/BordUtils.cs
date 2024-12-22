namespace Maze_Simulation.Shared;
/// <summary>
/// Provides utility methods for initializing and resetting a 2D array of cells.
/// </summary>
public static class BoardUtils
{
    /// <summary>
    /// Calculates a new position based on the current position and a specified move direction.
    /// </summary>
    /// <param name="current">The current cell position.</param>
    /// <param name="move">The direction to move in.</param>
    /// <returns>The new calculated position.</returns>
    public static Position GetNewPosition(Cell current, Direction move)
    {
        var (offsetX, offsetY) = move.GetOffset();
        var newX = current.X + offsetX;
        var newY = current.Y + offsetY;
        return new(newX, newY);
    }

    /// <summary>
    /// Tries to move to a new position based on the current cell and direction. 
    /// Checks whether the new position is within the boundaries of the board.
    /// </summary>
    /// <param name="board">The board on which the move is attempted.</param>
    /// <param name="cell">The current cell position.</param>
    /// <param name="direction">The direction in which to move.</param>
    /// <param name="nextPos">The calculated position to move to if the move is valid.</param>
    /// <returns>True if the move is valid (within the board boundaries), otherwise false.</returns>
    public static bool TryMove(Board board, Cell cell, Direction direction, out Position nextPos)
    {
        nextPos = GetNewPosition(cell, direction);
        if (nextPos.X >= board.Width || nextPos.X < 0) return false;
        if (nextPos.Y >= board.Height || nextPos.Y < 0) return false;
        return true;
    }

    /// <summary>
    /// Gets a list of available directions that a cell can move in, based on the strategy 
    /// for available directions and the strategy for selecting the next direction.
    /// </summary>
    /// <param name="board">The board to check the available directions on.</param>
    /// <param name="cell">The cell for which to check available directions.</param>
    /// <param name="availableStrategy">The strategy for determining which directions are available for movement.</param>
    /// <param name="nextStrategy">The strategy for selecting which direction to take next from the available ones.</param>
    /// <returns>An enumerable collection of directions and their corresponding positions.</returns>
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

    /// <summary>
    /// Defines a strategy for determining which directions are available for movement from a given cell.
    /// </summary>
    public interface IAvailableStrategy
    {
        /// <summary>
        /// Gets the available directions for the specified cell.
        /// </summary>
        /// <param name="cell">The cell from which available directions are determined.</param>
        /// <returns>A collection of directions that are available for movement.</returns>
        public IEnumerable<Direction> GetAvailable(Cell cell);
    }

    /// <summary>
    /// A strategy for getting directions where walls exist in the specified cell.
    /// </summary>
    public class WallStrategy : IAvailableStrategy
    {
        /// <summary>
        /// Gets the directions where walls are present in the specified cell.
        /// </summary>
        /// <param name="cell">The cell to check for walls.</param>
        /// <returns>An enumerable collection of directions where walls are present.</returns>
        public IEnumerable<Direction> GetAvailable(Cell cell)
        {
            if (cell[Direction.Top]) yield return Direction.Top;
            if (cell[Direction.Right]) yield return Direction.Right;
            if (cell[Direction.Bottom]) yield return Direction.Bottom;
            if (cell[Direction.Left]) yield return Direction.Left;
        }
    }

    /// <summary>
    /// A strategy for getting directions where no walls exist in the specified cell.
    /// </summary>
    public class NoWallStrategy : IAvailableStrategy
    {
        /// <summary>
        /// Gets the directions where no walls are present in the specified cell.
        /// </summary>
        /// <param name="cell">The cell to check for available directions.</param>
        /// <returns>An enumerable collection of directions where no walls are present.</returns>
        public IEnumerable<Direction> GetAvailable(Cell cell)
        {
            if (!cell[Direction.Top]) yield return Direction.Top;
            if (!cell[Direction.Right]) yield return Direction.Right;
            if (!cell[Direction.Bottom]) yield return Direction.Bottom;
            if (!cell[Direction.Left]) yield return Direction.Left;
        }
    }

    /// <summary>
    /// Defines a strategy for selecting the next direction from a list of available directions.
    /// </summary>
    public interface INextStrategy
    {
        /// <summary>
        /// Gets the index of the next direction to move to from the available directions.
        /// </summary>
        /// <param name="available">The number of available directions.</param>
        /// <returns>The index of the next direction to choose.</returns>
        public int GetNextIndex(int available);
    }

    /// <summary>
    /// A strategy that always chooses the first available direction from the list.
    /// </summary>
    public class FirstNextStrategy : INextStrategy
    {
        /// <summary>
        /// Always returns the first available direction (index 0).
        /// </summary>
        /// <param name="available">The number of available directions.</param>
        /// <returns>Always returns 0, the first direction.</returns>
        public int GetNextIndex(int available)
        {
            return 0;
        }
    }

    /// <summary>
    /// A strategy that selects a random direction from the list of available directions.
    /// </summary>
    public class RandomNextStrategy(Random random) : INextStrategy
    {
        /// <summary>
        /// Returns a random index for the available directions list.
        /// </summary>
        /// <param name="available">The number of available directions.</param>
        /// <returns>A randomly chosen index from the available directions.</returns>
        public int GetNextIndex(int available)
        {
            return random.Next(available);
        }
    }
}