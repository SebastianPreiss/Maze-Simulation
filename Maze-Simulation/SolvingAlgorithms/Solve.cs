namespace Maze_Simulation.SolvingAlgorithms;

using Shared;

public record Solve(IList<Direction> Steps, Position Start, Position Target, Queue<Cell> ProcessingOrder, IDictionary<Cell, double> CellValue);
