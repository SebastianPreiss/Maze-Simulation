namespace Maze_Simulation;

using Model;
using Shared;
using SolvingAlgorithms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

/// <summary>
/// Main window for the Maze application, responsible for displaying the maze and handling user interactions.
/// </summary>
public partial class MainWindow : Window
{
    // Drawing properties
    private const double MinPadding = 0.9;

    private readonly DispatcherTimer _resizeTimer;
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _resizeTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(1)
        };
        _resizeTimer.Tick += (s, e) =>
        {
            _resizeTimer.Stop();
            Draw();
        };

        _viewModel = new MainViewModel();
        DataContext = _viewModel;

        //_viewModel.PropertyChanged += (sender, args) =>
        //{
        //    if (args.PropertyName == nameof(_viewModel.ProcessedCells))
        //    {
        //        Draw();
        //    }
        //};

    }

    // TODO: Fix performance issues for large mazes
    /// <summary>
    /// Draws the maze on the canvas based on the current state of the view model.
    /// Clears previous drawings and calculates cell sizes and offsets.
    /// </summary>
    private void Draw()
    {
        if (_viewModel.Board is not Board board) return;

        DrawingCanvas.Children.Clear();
        var spacing = GetDrawingSpacing(board, DrawingCanvas);
        var pens = new Pens(new Pen(Brushes.Black, 3));
        var highlights = new Highlights(new Highlight(Brushes.Blue, spacing.SolveSize), new Highlight(Brushes.Red, spacing.SolveSize));
        var settings = new DrawSettings(spacing, pens, highlights);

        var drawingVisualisation = new DrawingVisual();
        using (var dc = drawingVisualisation.RenderOpen())
        {
            DrawBoard(board, dc, settings);
            if (_viewModel.Solve is Solve solve)
            {
                DrawSolve(board, solve, dc, settings);
            }
            if (_viewModel.Start is Position start)
            {
                DrawHighlight(board, start, dc, settings.Highlights.Start, settings.Spacing);
            }
            if (_viewModel.Target is Position target)
            {
                DrawHighlight(board, target, dc, settings.Highlights.Target, settings.Spacing);
            }
        }

        var drawingImage = new DrawingImage(drawingVisualisation.Drawing);

        var image = new Image
        {
            Source = drawingImage,
            Width = board.Width * settings.Spacing.CellSize.Width,
            Height = board.Height * settings.Spacing.CellSize.Height
        };

        Canvas.SetLeft(image, spacing.Offset.Width);
        Canvas.SetTop(image, spacing.Offset.Height);

        DrawingCanvas.Children.Add(image);
    }

    private static void DrawHighlight(Board board, Position position, DrawingContext dc, Highlight settings, Spacing spacing)
    {
        var offset = GetOffset(board, position, spacing);
        var halfCellSize = new Point(spacing.CellSize.Width / 2, spacing.CellSize.Height / 2);
        var center = Point.Add(halfCellSize, offset);

        dc.DrawEllipse(settings.Fill, null, center, settings.Size.Width, settings.Size.Height);
    }

    /// <summary>
    /// Draws the basic structure of the board on the given canvas using the specified DrawingContext.
    /// </summary>
    private static void DrawBoard(Board board, DrawingContext dc, DrawSettings settings)
    {
        Direction[] directions = [Direction.Top, Direction.Right, Direction.Bottom, Direction.Left];
        for (var i = 0; i < board.Width; i++)
        {
            for (var j = 0; j < board.Height; j++)
            {
                var position = new Position(i, j);
                var current = board[position];

                var offset = GetOffset(board, position, settings.Spacing);

                foreach (var direction in directions)
                {
                    if (!current[direction]) continue;
                    var (start, end) = GetLine(direction, settings.Spacing.CellSize);
                    dc.DrawLine(settings.Pens.Wall, Point.Add(start, offset), Point.Add(end, offset));
                }
            }
        }
    }

    /// <summary>
    /// Draws the solved path of the maze onto the given DrawingContext.
    /// </summary>
    private static void DrawSolve(Board board, Solve solve, DrawingContext dc, DrawSettings settings)
    {
        var current = board[solve.Start];
        var target = board[solve.Target];
        var counter = 0;
        var total = solve.Steps.Count;

        foreach (var step in solve.Steps)
        {
            counter++;
            var position = BoardUtils.GetNewPosition(current, step);

            // draw current
            var offset = GetOffset(board, position, settings.Spacing);
            var cellSize = new Point(settings.Spacing.CellSize.Width / 2 - settings.Spacing.SolveSize.Width / 2, settings.Spacing.CellSize.Height / 2 - settings.Spacing.SolveSize.Height / 2);
            var point = Point.Add(cellSize, offset);

            var pathColor = Color.FromArgb(128,
                (byte)(255 * counter / total),
                0,
                (byte)(255 * (1 - counter / total)));

            dc.DrawRectangle(
                new SolidColorBrush(pathColor),
                null,
                new Rect(point, settings.Spacing.SolveSize)
            );

            // step to next and set as current
            var next = board[position];
            if (Equals(next, target)) return;
            current = next;
        }
    }

    private static Vector GetOffset(Board board, Position position, Spacing spacing)
    {
        var boardHeight = board.Height * spacing.CellSize.Height;
        var x = position.X * spacing.CellSize.Width + spacing.Offset.Width;
        var y = boardHeight - (position.Y * spacing.CellSize.Height + spacing.Offset.Height);
        return new Vector(x, y);
    }

    /// <summary>
    /// Handles the click event for generating a new maze. Resets the solved path and generates a new board.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnGenerateClicked(object sender, RoutedEventArgs e)
    {
        //_viewModel.ResetSolvedPath();
        _viewModel.GenerateBoard(Seed.Text, (int)WidthSlider.Value, (int)HeightSlider.Value, MultiPath.IsChecked.Value);
        Draw();
    }

    /// <summary>
    /// Handles the size changed event for the window. Triggers the redraw of the maze after resizing.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void WindowResized(object sender, SizeChangedEventArgs e)
    {
        _resizeTimer.Stop();
        _resizeTimer.Start();
    }

    /// <summary>
    /// Handles the mouse down event on the maze canvas. Executes the selected action on the clicked cell.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void MazeCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (_viewModel.Board is not Board board) return;

        DrawingCanvas.Children.Clear();
        var spacing = GetDrawingSpacing(board, DrawingCanvas);

        if (e.LeftButton != MouseButtonState.Pressed) return;
        var point = e.GetPosition(DrawingCanvas);

        var boardHeight = board.Height * spacing.CellSize.Height;

        var relativeX = point.X - spacing.Offset.Width;
        var relativeY = boardHeight - (point.Y - spacing.Offset.Height);

        var x = (int)Math.Round(relativeX / spacing.CellSize.Width, MidpointRounding.ToZero);
        var y = (int)Math.Round(relativeY / spacing.CellSize.Height, MidpointRounding.ToZero);

        x = Math.Min(Math.Max(0, x), board.Width - 1);
        y = Math.Min(Math.Max(0, y), board.Height - 1);

        var position = new Position(x, y);
        // position
        _viewModel.SelectActionOnCell(position);
        Draw();
    }

    /// <summary>
    /// Starts the selected maze-solving algorithm asynchronously. Resets the solved path before starting.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="routedEventArgs">The event data.</param>
    private void OnStartAlgorithm(object sender, RoutedEventArgs routedEventArgs)
    {
        var index = AlgorithmComboBox.SelectedIndex;
        _viewModel.SolveBoard(index);
        Draw();
    }

    private record DrawSettings(Spacing Spacing, Pens Pens, Highlights Highlights);
    private record Pens(Pen Wall);
    private record Highlight(Brush Fill, Size Size);
    private record Highlights(Highlight Start, Highlight Target);
    private record Spacing(Size CellSize, Size Offset, Size SolveSize);

    private static Spacing GetDrawingSpacing(Board board, FrameworkElement element)
    {
        var width = element.ActualWidth;
        var height = element.ActualHeight;

        var cellWidth = (width * MinPadding) / board.Width;
        var cellHeight = (height * MinPadding) / board.Height;

        // ensure square shape of cell
        var cell = cellHeight < cellWidth ? cellHeight : cellWidth;
        var cellSize = new Size(cell, cell);

        var solveSize = new Size(cell * 0.4, cell * 0.4);

        var boardWidth = board.Width * cellSize.Width;
        var boardHeight = board.Height * cellSize.Height;

        var offsetX = (width - boardWidth) / 2;
        var offsetY = (height - boardHeight) / 2;
        var offset = new Size(offsetX, offsetY);


        return new Spacing(cellSize, offset, solveSize);
    }

    private static (Point x, Point y) GetLine(Direction wall, Size cellSize)
    {
        return wall switch
        {
            Direction.Top => (new Point(0, 0), new Point(cellSize.Width, 0)),
            Direction.Right => (new Point(cellSize.Width, 0), new Point(cellSize.Width, cellSize.Height)),
            Direction.Bottom => (new Point(0, cellSize.Height), new Point(cellSize.Width, cellSize.Height)),
            Direction.Left => (new Point(0, 0), new Point(0, cellSize.Height)),
            _ => throw new ArgumentException($"Invalid wall \"{wall}\"")
        };
    }
}