namespace Maze_Simulation.Shared;

public static class MathUtils
{
    public static double Range(double val, double min, double max) => min + val * (max - min);
}