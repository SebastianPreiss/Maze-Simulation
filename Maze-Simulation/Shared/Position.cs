namespace Maze_Simulation.Shared;

/// <summary>
/// Represents a position with X and Y coordinates, typically used for grid-based systems like a maze.
/// </summary>
public struct Position(int x, int y)
{
    public int X = x;
    public int Y = y;
}
