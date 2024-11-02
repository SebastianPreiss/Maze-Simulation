namespace Maze_Simulation.Generation
{
    public interface IMazeStrategy
    {
        void Reset();
        void SetStartingPoint(int x, int y);
        void Generate();
    }
}
