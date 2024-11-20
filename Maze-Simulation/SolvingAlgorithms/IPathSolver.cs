using Maze_Simulation.Generation;

namespace Maze_Simulation.SolvingAlgorithms
{
    internal interface IPathSolver
    {
        public void InitSolver(Cell[,] cells);
        public Task<List<Cell>?> StartSolver();
    }
}
