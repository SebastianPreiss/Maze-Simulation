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
        /// Generates a new maze using a specified maze generation strategy. 
        /// Optionally allows the creation of multiple paths within the maze.
        /// </summary>
        /// <param name="multiPath">
        /// A boolean indicating whether to create multiple paths in the maze. 
        /// If set to <c>true</c>, additional random connections will be added to the generated maze.
        /// </param>

        public void GenerateMaze(bool multiPath)
        {
            _strategy = new MazeGenerator(ref Cells, _seed, multiPath);
            _strategy.Reset();
            _strategy.SetStartingPoint(1, 2);
            _strategy.Generate();
        }
    }
}
