using System.Collections;

namespace Maze_Simulation.Shared;

public class Board : IEnumerable<Cell>
{
    private readonly Cell[,] _cells;

    public int Width => _cells.GetLength(0);
    public int Height => _cells.GetLength(1);

    public Board(int width, int height)
    {
        _cells = new Cell[width, height];
        FillBlank();
    }

    public Cell this[Position position] => _cells[position.X, position.Y];

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

    public IEnumerator<Cell> GetEnumerator()
    {
        return _cells.Cast<Cell>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}