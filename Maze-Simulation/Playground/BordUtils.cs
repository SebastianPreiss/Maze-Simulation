namespace Maze_Simulation.Playground
{
    /// <summary>
    /// Provides utility methods for initializing and resetting a 2D array of cells.
    /// </summary>
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

        /// <summary>
        /// Retrieves the neighboring cells of a given cell that do not have a wall in the specified direction.
        /// </summary>
        /// <param name="cells">The two-dimensional array representing the grid of cells.</param>
        /// <param name="cell">The cell for which neighbors should be retrieved.</param>
        /// <returns>An enumeration of neighboring cells without a wall in the respective direction.</returns>
        public static IEnumerable<Cell> GetNeighbors(this Cell[,] cells, Cell cell)
        {
            var directions = new Dictionary<Move, (int dx, int dy)>
            {
                { Move.Top, (0, 1) },
                { Move.Right, (1, 0) },
                { Move.Bottom, (0, -1) },
                { Move.Left, (-1, 0) }
            };

            foreach (var direction in directions)
            {
                var x = cell.X + direction.Value.dx;
                var y = cell.Y + direction.Value.dy;

                if (x < 0 || x >= cells.GetLength(0) || y < 0 || y >= cells.GetLength(1)) continue;
                if (!cell.HasWall(direction.Key))
                {
                    yield return cells[x, y];
                }
            }
        }

        /// <summary>
        /// Checks if a given cell has a wall in a specific direction.
        /// </summary>
        /// <param name="cell">The cell to check.</param>
        /// <param name="move">The direction to check for a wall.</param>
        /// <returns>True if there is a wall in the specified direction; otherwise, false.</returns>
        public static bool HasWall(this Cell cell, Move move) => cell.Walls[(int)move];

        /// <summary>
        /// Returns the offset (x, y) for a specific movement direction.
        /// </summary>
        /// <param name="move">The movement direction for which to calculate the offset.</param>
        /// <returns>A tuple containing the offset values (x, y) corresponding to the specified movement direction.</returns>

        public static (int x, int y) GetOffset(this Move move)
        {
            return move switch
            {
                Move.Top => (0, 1),
                Move.Right => (1, 0),
                Move.Bottom => (0, -1),
                Move.Left => (-1, 0),
                _ => (0, 0)
            };
        }
    }
}
