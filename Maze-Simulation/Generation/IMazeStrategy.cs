namespace Maze_Simulation.Generation
{
    /// <summary>
    /// Defines the contract for a maze generation strategy.
    /// Implementing classes should provide specific algorithms for maze generation.
    /// </summary>
    public interface IMazeStrategy
    {
        /// <summary>
        /// Resets the maze to its initial state, clearing any modifications made during generation.
        /// </summary>
        void Reset();

        /// <summary>
        /// Sets the starting point for maze generation at the specified coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the starting point.</param>
        /// <param name="y">The y-coordinate of the starting point.</param>
        void SetStartingPoint(int x, int y);

        /// <summary>
        /// Generates the maze based on the implemented algorithm.
        /// </summary>
        void Generate();
    }

}
