﻿using Maze_Simulation.Generation;

namespace Maze_Simulation.SolvingAlgorithms
{
    internal interface IPathSolver
    {
        public List<Cell> StartSolver();
    }
}