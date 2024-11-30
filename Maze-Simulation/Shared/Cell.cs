namespace Maze_Simulation.Shared;

/// <summary>
/// Represents a single cell in the board, including its position, walls, and state.
/// </summary>
public class Cell(int x, int y)
{
    private readonly bool[] _walls = [true, true, true, true];

    public bool this[Direction wall]
    {
        get => _walls[GetIndex(wall)];
        set => _walls[GetIndex(wall)] = value;
    }

    public int X => x;
    public int Y => y;

    private static byte GetIndex(Direction direction)
    {
        return direction switch
        {
            Direction.Top => 0,
            Direction.Right => 1,
            Direction.Bottom => 2,
            Direction.Left => 3,
            _ => throw new ArgumentException($"Invalid direction \"{direction}\"")
        };
    }
}
