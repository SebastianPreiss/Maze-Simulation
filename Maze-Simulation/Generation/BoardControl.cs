namespace Maze_Simulation.Generation
{
    public class BoardControl
    {
        public int Width;
        public int Height;
        public Cell[,]? Cells;
        private readonly int _seed;
        private IBoardStrategy _strategy;

        public BoardControl(int width, int height, int seed)
        {
            Width = width;
            Height = height;
            Cells = new Cell[Width, Height];
            _seed = seed;

        }

        /// <summary>
        /// Generates a new maze using an maze strategy.
        /// </summary>
        public void GenerateMaze()
        {
            _strategy = new MazeGenerator(ref Cells, _seed);
            _strategy.Reset();
            _strategy.SetStartingPoint(1, 2);
            _strategy.Generate();
        }
    }
}
