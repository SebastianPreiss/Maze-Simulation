﻿using Maze_Simulation.Generation;

namespace Maze_Simulation.SolvingAlgorithms
{
    public interface IPathSolver
    {
        public event Action<IEnumerable<(Cell Cell, double Cost)>>? ProcessedCellsUpdated;
        public void InitSolver(Cell[,] cells);
        public Task<List<Cell>> StartSolver(bool visualize);
    }
}
