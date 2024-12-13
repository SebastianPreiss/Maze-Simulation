using Maze_Simulation.Shared;
using System.Windows;
using System.Windows.Media;

namespace Maze_Simulation.Model;

public record DrawSettings(Spacing Spacing, Pens Pens, Highlights Highlights);
public record Pens(Pen Wall);
public record Highlight(Brush Fill, Size Size);
public record Highlights(Highlight Start, Highlight Target);
public record Spacing(Size CellSize, Size Offset, Size SolveSize, Size ProcessingSize)
{
    private const double MinPadding = 0.9;
    private const double SolvePadding = 0.4;
    private const double ProcessingPadding = 0.6;

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

