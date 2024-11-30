using Maze_Simulation.Generation;

namespace Maze_Simulation.Playground
{
    public class Board
    {
        public Cell[,]? Cells;

        public void InitBoard(int width, int height)
        {
            Cells = new Cell[width, height];
            Cells.FillBlank();
        }

        public void GenerateMaze(bool multiPath, int seed)
        {
            var mazeGenerator = new MazeGenerator(ref Cells, seed, multiPath);
            mazeGenerator.Reset();
            mazeGenerator.SetStartingPoint(1, 2);
            mazeGenerator.Generate();
        }
    }
}
