namespace Maze_Simulation.Shared;

using System.Collections;

/// <summary>
/// Represents a 2D grid of cells, providing methods to access and manipulate the cells in the grid.
/// Implements the <see cref="IEnumerable{Cell}"/> interface for iteration over the cells.
/// </summary>
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

    /// <summary>
    /// Indexer to access a specific cell at the given position.
    /// </summary>
    /// <param name="position">The position of the cell (X and Y coordinates).</param>
    /// <returns>The <see cref="Cell"/> at the specified position.</returns>
    public Cell this[Position position] => _cells[position.X, position.Y];

    /// <summary>
    /// Initializes all cells of the board with blank values.
    /// Each cell is initialized with its X and Y coordinates.
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