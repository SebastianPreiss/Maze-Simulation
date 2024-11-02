namespace Maze_Simulation.Generation
{
    public static class BoardUtils
    {
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
            cells[0, 0].IsPlayer = true;
        }

        public static void Reset(this Cell[,]? cells) => cells.FillBlank();
    }
}
