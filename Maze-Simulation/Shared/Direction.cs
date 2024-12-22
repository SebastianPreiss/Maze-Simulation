namespace Maze_Simulation.Shared;

/// <summary>
/// Represents possible movement directions within a maze.
/// </summary>
public enum Direction
{
    None,
    Top,
    Right,
    Bottom,
    Left
}

public static class DirectionExtension
{
    /// <summary>
    /// Returns the offset (x, y) for a specific movement direction.
    /// </summary>
    /// <param name="direction">The movement direction for which to calculate the offset.</param>
    /// <returns>A tuple containing the offset values (x, y) corresponding to the specified movement direction.</returns>

    public static (int x, int y) GetOffset(this Direction direction)
    {
        return direction switch
        {
            Direction.Top => (0, 1),
            Direction.Right => (1, 0),
            Direction.Bottom => (0, -1),
            Direction.Left => (-1, 0),
            _ => (0, 0)
        };
    }
}