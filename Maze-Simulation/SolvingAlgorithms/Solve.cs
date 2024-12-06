namespace Maze_Simulation.SolvingAlgorithms;

using Shared;

public record Solve(IList<Direction> Steps, Position Start, Position Target);
