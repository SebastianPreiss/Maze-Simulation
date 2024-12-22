namespace Maze_Simulation.Generation;

using Shared;

/// <summary>
/// Defines the contract for the board generation strategy.
/// Implementing classes should provide specific algorithms for board generation.
/// </summary>
public interface IBoardStrategy
{
    int Seed { set; }

    /// <summary>
    /// Generates the board based on the implemented algorithm.
    /// </summary>
    Board Generate(int width, int height, bool multiPath);
}
