namespace Maze_Simulation.Generation
{
    public static class BoardUtils
    {
        /// <summary>
        /// Initializes a 2D array of cells, setting each cell's position and available moves. 
        /// The starting cell is set in the top-left corner and the target cell in the bottom-right corner by default.
        /// </summary>
        /// <param name="cells">The 2D array of cells to be initialized.</param>
        public static void FillBlank(this Cell[,]? cells)
        {
            for (var i = 0; i < cells.GetLength(0); i++)
            {
                for (var j = 0; j < cells.GetLength(1); j++)
                {
                    cells[i, j] = new Cell(i, j) { Available = new() { Move.Top, Move.Right, Move.Bottom, Move.Left } };
                }
            }
            cells[cells.GetLength(0) - 1, cells.GetLength(1) - 1].IsTarget = true;
            cells[0, 0].IsStart = true;
        }

        /// <summary>
        /// Resets the cells to their initial state by filling them with new cell instances and setting 
        /// the start and target cells to their respective positions.
        /// </summary>
        /// <param name="cells">The 2D array of cells to reset.</param>
        public static void Reset(this Cell[,]? cells) => cells.FillBlank();
    }
}
