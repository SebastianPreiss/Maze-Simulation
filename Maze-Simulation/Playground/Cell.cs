namespace Maze_Simulation.Playground
{
    /// <summary>
    /// Represents a single cell in the board, including its position, walls, and state.
    /// </summary>
    public class Cell
    {
        public const int Top = 0;
        public const int Right = 1;
        public const int Bottom = 2;
        public const int Left = 3;

        /// <summary>
        /// 0-> Top; 1->Right; 2->Bottom; 3-> Left
        /// </summary>
        public bool[] Walls = [true, true, true, true];
        public List<Move> Available = [];
        public bool IsVisited;
        public int X;
        public int Y;
        public bool IsCollapsed => !Available.Any();
        public bool IsTarget;
        public bool IsStart;

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
