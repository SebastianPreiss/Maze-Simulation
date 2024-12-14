namespace Maze_Simulation;

using Model;
using Shared;
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
    private readonly DispatcherTimer _resizeTimer;
    private readonly MainViewModel _viewModel;
    private static DrawSettings _settings = default!;
    private Image _maze;

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
        _viewModel.Timer.Tick += (s, e) => Draw();
        _viewModel.OnBoardChanged += UpdateSettings;
        _viewModel.OnBoardChanged += DrawBoard;
        DataContext = _viewModel;
        //_viewModel.GenerateBoard("42", 32, 32, true);
    }

    /// <summary>
    /// Updates the drawing settings based on the given board.
    /// This includes setting spacing, pens, and highlights for drawing.
    /// </summary>
    private void UpdateSettings(Board board)
    {
        var spacing = Spacing.GetDrawingSpacing(board, DrawingCanvas);
        var pens = new Pens(new Pen(Brushes.Black, 3));
        var highlights = new Highlights(new Highlight(Brushes.Blue, spacing.SolveSize), new Highlight(Brushes.Red, spacing.SolveSize));
        _settings = new DrawSettings(spacing, pens, highlights);
    }

    /// <summary>
    /// Draws the maze on the canvas based on the current state of the view model.
    /// Clears previous drawings and calculates cell sizes and offsets.
    /// </summary>
    private void Draw()
    {
        if (_viewModel.Board is not Board board) return;

        DrawingCanvas.Children.Clear();

        var drawingVisualisation = new DrawingVisual();
        using (var dc = drawingVisualisation.RenderOpen())
        {
            if (_viewModel.Solve is Solve solve)
            {
                DrawSolve(board, solve, dc);
            }
            if (_viewModel.Start is Position start)
            {
                DrawHighlight(board, start, dc, _settings.Highlights.Start);
            }
            if (_viewModel.Target is Position target)
            {
                DrawHighlight(board, target, dc, _settings.Highlights.Target);
            }

            // Do stuff for fix padding ^^
            var pen = new Pen(Brushes.Transparent, 3);
            var offset = GetOffset(board, new Position(0, 0), _settings.Spacing);

            var (s, end) = GetLine(Direction.Bottom, _settings.Spacing.CellSize);
            dc.DrawLine(pen, Point.Add(s, offset), Point.Add(end, offset));
            (s, end) = GetLine(Direction.Left, _settings.Spacing.CellSize);
            dc.DrawLine(pen, Point.Add(s, offset), Point.Add(end, offset));

            offset = GetOffset(board, new Position(board.Width - 1, board.Height - 1), _settings.Spacing);

            (s, end) = GetLine(Direction.Top, _settings.Spacing.CellSize);
            dc.DrawLine(pen, Point.Add(s, offset), Point.Add(end, offset));
            (s, end) = GetLine(Direction.Right, _settings.Spacing.CellSize);
            dc.DrawLine(pen, Point.Add(s, offset), Point.Add(end, offset));
        }

        var drawingImage = new DrawingImage(drawingVisualisation.Drawing);

        var image = new Image
        {
            Source = drawingImage,
            Width = board.Width * _settings.Spacing.CellSize.Width,
            Height = board.Height * _settings.Spacing.CellSize.Height
        };

        Canvas.SetLeft(image, _settings.Spacing.Offset.Width);
        Canvas.SetTop(image, _settings.Spacing.Offset.Height);
        DrawingCanvas.Children.Add(image);

        Canvas.SetLeft(_maze, _settings.Spacing.Offset.Width);
        Canvas.SetTop(_maze, _settings.Spacing.Offset.Height);
        DrawingCanvas.Children.Add(_maze);
    }

    /// <summary>
    /// Draws the board on the canvas. This method will be triggered when the board is updated.
    /// </summary>
    private void DrawBoard(Board board)
    {
        var drawingVisualisation = new DrawingVisual();
        using (var dc = drawingVisualisation.RenderOpen())
        {
            DrawBoard(board, dc);
        }

        var drawingImage = new DrawingImage(drawingVisualisation.Drawing);

        _maze = new Image
        {
            Source = drawingImage,
            Width = board.Width * _settings.Spacing.CellSize.Width,
            Height = board.Height * _settings.Spacing.CellSize.Height
        };
        Draw();
    }

    /// <summary>
    /// Draws the highlighted position (Start or Target) on the canvas.
    /// </summary>
    private static void DrawHighlight(Board board, Position position, DrawingContext dc, Highlight settings)
    {
        var center = GetDrawPoint(board, position, new(0, 0));
        dc.DrawEllipse(settings.Fill, null, center, settings.Size.Width, settings.Size.Height);
    }

    /// <summary>
    /// Draws the basic structure of the board on the given canvas using the specified DrawingContext.
    /// </summary>
    private static void DrawBoard(Board board, DrawingContext dc)
    {
        Direction[] directions = [Direction.Top, Direction.Right, Direction.Bottom, Direction.Left];
        for (var i = 0; i < board.Width; i++)
        {
            for (var j = 0; j < board.Height; j++)
            {
                var position = new Position(i, j);
                var current = board[position];

                var offset = GetOffset(board, position, _settings.Spacing);

                foreach (var direction in directions)
                {
                    if (!current[direction]) continue;
                    var (start, end) = GetLine(direction, _settings.Spacing.CellSize);
                    dc.DrawLine(_settings.Pens.Wall, Point.Add(start, offset), Point.Add(end, offset));
                }
            }
        }
    }

    /// <summary>
    /// Draws the solved path of the maze onto the given DrawingContext.
    /// </summary>
    private void DrawSolve(Board board, Solve solve, DrawingContext dc)
    {
        if (_viewModel.IsRunning && (VisuSolver.IsChecked ?? false))
        {
            DrawSolveProcess(board, solve, dc);
        }
        else
        {
            DrawSolvePath(board, solve, dc);
        }
    }

    /// <summary>
    /// Visualizes the solving process step by step.
    /// </summary>
    private void DrawSolveProcess(Board board, Solve solve, DrawingContext dc)
    {
        var cells = solve.ProcessingOrder;
        var values = solve.CellValue;
        var maxCost = values.Values.Max();
        var minCost = values.Values.Min();

        foreach (var cell in cells.Take(_viewModel.ToVisualize))
        {
            if (cell == board[_viewModel.Start ?? new Position()]) continue;
            var point = GetDrawPoint(board, cell, _settings.Spacing.ProcessingSize);

            var normalizedValue = (solve.CellValue[cell] - minCost) / (maxCost - minCost);

            var color = Color.FromArgb(128,
                (byte)(255 * normalizedValue),
                (byte)(255 * (1 - normalizedValue)),
                0);

            dc.DrawRectangle(
                null,
                new Pen(new SolidColorBrush(color), 6),
                new Rect(point, _settings.Spacing.ProcessingSize)
            );
        }
    }

    /// <summary>
    /// Draws the final solved path after the algorithm has completed.
    /// </summary>
    private static void DrawSolvePath(Board board, Solve solve, DrawingContext dc)
    {
        var current = board[solve.Start];
        var target = board[solve.Target];
        var counter = 1;
        var total = solve.Steps.Count;

        foreach (var step in solve.Steps)
        {
            counter++;
            var position = BoardUtils.GetNewPosition(current, step);

            // draw current
            var point = GetDrawPoint(board, position, _settings.Spacing.SolveSize);

            var doneness = (double)counter / total;
            var pathColor = Color.FromArgb(128,
                (byte)MathUtils.Range(doneness, 0, 255),
                0,
                (byte)MathUtils.Range(doneness, 255, 0));

            dc.DrawRectangle(
                new SolidColorBrush(pathColor),
                null,
                new Rect(point, _settings.Spacing.SolveSize)
            );

            // step to next and set as current
            var next = board[position];
            if (Equals(next, target)) break;
            current = next;
        }
    }

    /// <summary>
    /// Returns the drawing point for the specified board cell and its offset.
    /// </summary>
    private static Point GetDrawPoint(Board board, Cell cell, Size? size = default)
    {
        return GetDrawPoint(board, new Position(cell.X, cell.Y), size);
    }

    /// <summary>
    /// Returns the drawing point for the specified position in the board and its offset.
    /// </summary>
    private static Point GetDrawPoint(Board board, Position position, Size? size = default)
    {
        var offset = GetOffset(board, position, _settings.Spacing);
        var cellSize = new Point(0, 0);
        if (size is Size s)
        {
            cellSize = new Point((_settings.Spacing.CellSize.Width - s.Width) / 2d, (_settings.Spacing.CellSize.Height - s.Height) / 2d);
        }
        return Point.Add(cellSize, offset);
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
        _viewModel.GenerateBoard(Seed.Text, (int)WidthSlider.Value, (int)HeightSlider.Value, MultiPath.IsChecked ?? false);
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

        var spacing = Spacing.GetDrawingSpacing(board, DrawingCanvas);

        var point = e.GetPosition(DrawingCanvas);

        var boardHeight = board.Height * spacing.CellSize.Height;

        var relativeX = point.X - spacing.Offset.Width;
        var relativeY = boardHeight - (point.Y - spacing.Offset.Height);

        var x = (int)Math.Round(relativeX / spacing.CellSize.Width, MidpointRounding.ToZero);
        var y = (int)Math.Round(relativeY / spacing.CellSize.Height, MidpointRounding.ToZero);

        x = Math.Min(Math.Max(0, x), board.Width - 1);
        y = Math.Min(Math.Max(0, y), board.Height - 1);

        var position = new Position(x, y);

        _viewModel.SelectActionOnCell(position, e);
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

    private void VisuSpeedSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_viewModel?.Timer is DispatcherTimer timer)
        {
            timer.Interval = TimeSpan.FromMilliseconds(VisuSpeedSlider.Value);
        }
    }
}