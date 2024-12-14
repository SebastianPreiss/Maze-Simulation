namespace Maze_Simulation.Shared;

/// <summary>
/// Represents the result of solving a maze, including the steps to take from start to target, 
/// the processing order of the cells, and the final values for each cell (e.g., cost or score).
/// </summary>
/// <param name="Steps">A list of directions representing the sequence of moves from the start position to the target position.</param>
/// <param name="Start">The starting position in the maze.</param>
/// <param name="Target">The target position in the maze.</param>
/// <param name="ProcessingOrder">A queue of cells representing the order in which cells were processed during the solving algorithm.</param>
/// <param name="CellValue">A dictionary mapping each cell to its final value, such as the cost to reach that cell during the solving process.</param>
public record Solve(IList<Direction> Steps, Position Start, Position Target, Queue<Cell> ProcessingOrder, IDictionary<Cell, double> CellValue);
