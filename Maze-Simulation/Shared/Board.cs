namespace Maze_Simulation.Shared;

public class Board
{
    private readonly Cell[,] _cells;

    public Cell? Start { get; set; }
    public Cell? Target { get; set; }
    public int Width => _cells.GetLength(0);
    public int Height => _cells.GetLength(1);

    public Board(int width, int height)
    {
        _cells = new Cell[width, height];
        FillBlank();
    }

    public Cell this[int x, int y] => _cells[x, y];

    /// <summary>
    /// Initializes an empty board.
    /// </summary>
    public void FillBlank()
    {
        for (var i = 0; i < Width; i++)
        {
            for (var j = 0; j < Height; j++)
            {
                _cells[i, j] = new Cell(i, j);
            }
        }
    }
}