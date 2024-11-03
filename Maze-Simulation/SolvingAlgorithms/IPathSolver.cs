using Maze_Simulation.Generation;

namespace Maze_Simulation.SolvingAlgorithms
{
    interface IPathSolver
    {
        public List<Cell> FindPath();
    }
}
