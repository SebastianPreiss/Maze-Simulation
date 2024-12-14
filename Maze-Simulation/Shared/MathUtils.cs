namespace Maze_Simulation.Shared;

/// <summary>
/// Provides utility methods for mathematical operations.
/// </summary>
public static class MathUtils
{
    /// <summary>
    /// Maps a value from one range to another.
    /// </summary>
    /// <param name="val">The value to be mapped.</param>
    /// <param name="min">The minimum value of the target range.</param>
    /// <param name="max">The maximum value of the target range.</param>
    /// <returns>The mapped value in the target range.</returns>
    public static double Range(double val, double min, double max) => min + val * (max - min);
}