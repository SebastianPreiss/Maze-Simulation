namespace Maze_Simulation.Generation
{
    public class BoardControl
    {
        public int Width;
        public int Height;
        public readonly Cell[,]? Cells;
        private readonly IMazeStrategy _strategy;

        public BoardControl(int width, int height, int seed)
        {
            Width = width;
            Height = height;
            Cells = new Cell[Width, Height];
            _strategy = new MazeGenerator(ref Cells, seed);
            _strategy.Reset();
            _strategy.SetStartingPoint(1, 2);
        }

        public void GenerateMaze()
        {
            _strategy.Generate();
        }
    }
}
