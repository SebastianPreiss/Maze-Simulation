namespace Maze_Simulation.SolvingAlgorithms;

using Shared;

/// <summary>
/// Interface for pathfinding algorithms used to solve mazes.
/// </summary>
public interface IPathSolver
{
    Solve? Solve(Board board, Position start, Position target);
}
