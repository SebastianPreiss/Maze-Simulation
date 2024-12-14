using Maze_Simulation.Shared;
using System.Windows;
using System.Windows.Media;

namespace Maze_Simulation.Model;

/// <summary>
/// Represents the drawing settings for rendering the maze, including spacing, pen styles, and highlight options.
/// </summary>
public record DrawSettings(Spacing Spacing, Pens Pens, Highlights Highlights);

/// <summary>
/// Contains the pen settings for drawing maze walls.
/// </summary>
public record Pens(Pen Wall);

/// <summary>
/// Represents a highlight with a fill brush and size for specific maze elements like start or target.
/// </summary>
public record Highlight(Brush Fill, Size Size);

/// <summary>
/// Contains the highlight settings for the start and target positions in the maze.
/// </summary>
public record Highlights(Highlight Start, Highlight Target);

/// <summary>
/// Defines the spacing settings for the maze drawing, including cell size, offsets, and specific sizes for solving and processing the maze.
/// </summary>
public record Spacing(Size CellSize, Size Offset, Size SolveSize, Size ProcessingSize)
{
    private const double MinPadding = 0.9;
    private const double SolvePadding = 0.4;
    private const double ProcessingPadding = 0.6;

    /// <summary>
    /// Calculates the appropriate spacing settings for drawing the maze, ensuring that cells maintain a square shape.
    /// It adjusts the cell size based on the actual width and height of the drawing element.
    /// </summary>
    /// <param name="board">The board to be drawn.</param>
    /// <param name="element">The WPF element in which the maze will be drawn (e.g., a Canvas).</param>
    /// <returns>A Spacing object containing the calculated cell size, offsets, and specific sizes for solving and processing.</returns>
    public static Spacing GetDrawingSpacing(Board board, FrameworkElement element)
    {
        var width = element.ActualWidth;
        var height = element.ActualHeight;

        var cellWidth = (width * MinPadding) / board.Width;
        var cellHeight = (height * MinPadding) / board.Height;

        // ensure square shape of cell
        var cell = cellHeight < cellWidth ? cellHeight : cellWidth;
        var cellSize = new Size(cell, cell);

        var solveSize = new Size(cell * SolvePadding, cell * SolvePadding);
        var processingSize = new Size(cell * ProcessingPadding, cell * ProcessingPadding);

        var boardWidth = board.Width * cellSize.Width;
        var boardHeight = board.Height * cellSize.Height;

        var offsetX = (width - boardWidth) / 2;
        var offsetY = (height - boardHeight) / 2;
        var offset = new Size(offsetX, offsetY);

        return new Spacing(cellSize, offset, solveSize, processingSize);
    }
};

