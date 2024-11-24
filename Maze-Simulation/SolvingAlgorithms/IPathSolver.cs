using Maze_Simulation.Generation;

namespace Maze_Simulation.SolvingAlgorithms
{
    /// <summary>
    /// Interface for pathfinding algorithms used to solve mazes.
    /// </summary>
    public interface IPathSolver
    {
        /// <summary>
        /// List of processed cells and their associated costs during pathfinding.
        /// </summary>
        public IEnumerable<(Cell Cell, double Cost)> ProcessedCells { get; }

        /// <summary>
        /// Event triggered whenever the list of processed cells is updated.
        /// </summary>
        public event Action<IEnumerable<(Cell Cell, double Cost)>>? ProcessedCellsUpdated;

        /// <summary>
        /// Initializes the algorithm with the maze's cells.
        /// </summary>
        /// <param name="cells">The cells of the maze.</param>
        public void InitSolver(Cell[,] cells);

        /// <summary>
        /// Starts the algorithm to solve the maze.
        /// </summary>
        /// <param name="visualize">Indicates whether the solving process should be visualized.</param>
        /// <returns>A list of cells representing the found path, or <c>null</c> if no path was found.</returns>
        public Task<List<Cell>> StartSolver(bool visualize);
    }
}
