namespace Maze_Simulation.SolvingAlgorithms;

using Shared;

/// <summary>
/// Defines the contract for pathfinding algorithms used to solve mazes.
/// Any pathfinding algorithm (e.g., A*, BFS, DFS) must implement this interface.
/// </summary>
public interface IPathSolver
{
    /// <summary>
    /// Solves the maze using the specified pathfinding algorithm.
    /// </summary>
    /// <param name="board">The maze board to be solved.</param>
    /// <param name="start">The starting position in the maze.</param>
    /// <param name="target">The target position to reach in the maze.</param>
    /// <returns>A <see cref="Solve"/> object containing the path from start to target, or null if no path is found.</returns>
    Solve? Solve(Board board, Position start, Position target);
}
