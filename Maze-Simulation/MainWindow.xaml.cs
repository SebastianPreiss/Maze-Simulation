using Maze_Simulation.Model;
using Maze_Simulation.Shared;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Maze_Simulation
{
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

            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.ProcessedCells))
                {
                    Draw();
                }
            };

        }

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
            var settings = new DrawSettings(spacing, pens);

            var drawingVisualisation = new DrawingVisual();
            using (var dc = drawingVisualisation.RenderOpen())
            {
                DrawBoard(board, dc, settings);
                //_viewModel.DrawSolvedPath(dc);

                //// Draw processed cells
                //if (VisuSolver.IsChecked.Value && _viewModel.Solver != null)
                //{
                //    _viewModel.DrawProcessedCells(dc, _viewModel.Solver.ProcessedCells);
                //}
            }

            var drawingImage = new DrawingImage(drawingVisualisation.Drawing);

            var image = new Image
            {
                Source = drawingImage,
                Width = board.Width * settings.Spacing.CellSize.X,
                Height = board.Height * settings.Spacing.CellSize.Y
            };

            Canvas.SetLeft(image, spacing.Offset.X);
            Canvas.SetTop(image, spacing.Offset.Y);

            DrawingCanvas.Children.Add(image);
        }

        /// <summary>
        /// Draws the basic structure of the board on the given canvas using the specified DrawingContext.
        /// </summary>
        /// <param name="dc">The DrawingContext used to render the board elements.</param>
        private static void DrawBoard(Board board, DrawingContext dc, DrawSettings settings)
        {
            Direction[] directions = [Direction.Top, Direction.Right, Direction.Bottom, Direction.Left];
            for (var i = 0; i < board.Width; i++)
            {
                for (var j = 0; j < board.Height; j++)
                {
                    var current = board[i, j];

                    var boardHeight = board.Height * settings.Spacing.CellSize.Y;

                    var x = i * settings.Spacing.CellSize.X + settings.Spacing.Offset.X;
                    var y = boardHeight - (j * settings.Spacing.CellSize.Y + settings.Spacing.Offset.Y);
                    var offset = new Vector(x, y);

                    foreach (var direction in directions)
                    {
                        if (!current[direction]) continue;
                        var (start, end) = GetLine(direction, settings.Spacing.CellSize);

                        // int drawing mode
                        //var s = Point.Add(start, offset);
                        //var e = Point.Add(end, offset);
                        //dc.DrawLine(settings.Pens.Wall, new Point((int)s.X, (int)s.Y), new Point((int)e.X, (int)e.Y));


                        dc.DrawLine(settings.Pens.Wall, Point.Add(start, offset), Point.Add(end, offset));
                    }

                    //if (cell.IsStart)
                    //{
                    //    dc.DrawEllipse(
                    //        Brushes.Blue, null,
                    //        new Point(
                    //            x + CellSize / 2,
                    //            y + CellSize / 2
                    //        ),
                    //        CellSize * MinPadding / 2,
                    //        CellSize * MinPadding / 2
                    //    );
                    //}

                    //if (cell.IsTarget)
                    //{
                    //    dc.DrawEllipse(
                    //        Brushes.Red, null,
                    //        new Point(
                    //            x + CellSize / 2,
                    //            y + CellSize / 2
                    //        ),
                    //        CellSize * MinPadding / 2,
                    //        CellSize * MinPadding / 2
                    //    );
                    //}
                }
            }
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
            if (e.LeftButton != MouseButtonState.Pressed) return;
            var position = e.GetPosition(DrawingCanvas);
            //_viewModel.SelectActionOnCell(position);
            Draw();
        }

        /// <summary>
        /// Starts the selected maze-solving algorithm asynchronously. Resets the solved path before starting.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="routedEventArgs">The event data.</param>
        private async void OnStartAlgorithm(object sender, RoutedEventArgs routedEventArgs)
        {
            var visibility = VisuSolver.IsChecked.Value ? Visibility.Hidden : Visibility.Visible;
            DurationHead.Visibility = visibility;
            DurationContent.Visibility = visibility;

            //_viewModel.ResetSolvedPath();
            var index = AlgorithmComboBox.SelectedIndex;
            //await _viewModel.StartAlgorithm(index, VisuSolver.IsChecked.Value, (int)VisuSpeedSlider.Value);
            Draw();
        }


        private record DrawSettings(Spacing Spacing, Pens Pens);
        private record Pens(Pen Wall);
        private record Spacing(Size CellSize, Size Offset);
        private record Size(double X, double Y);

        private static Spacing GetDrawingSpacing(Board board, FrameworkElement element)
        {
            var width = element.ActualWidth;
            var height = element.ActualHeight;

            var cellWidth = (width * MinPadding) / board.Width;
            var cellHeight = (height * MinPadding) / board.Height;

            // ensure square shape of cell
            var cell = cellHeight < cellWidth ? cellHeight : cellWidth;
            var cellSize = new Size(cell, cell);

            var boardWidth = board.Width * cellSize.X;
            var boardHeight = board.Height * cellSize.Y;

            var offsetX = (width - boardWidth) / 2;
            var offsetY = (height - boardHeight) / 2;
            var offset = new Size(offsetX, offsetY);

            return new Spacing(cellSize, offset);
        }

        private static (Point x, Point y) GetLine(Direction wall, Size cellSize)
        {
            return wall switch
            {
                Direction.Top => (new Point(0, 0), new Point(cellSize.X, 0)),
                Direction.Right => (new Point(cellSize.X, 0), new Point(cellSize.X, cellSize.Y)),
                Direction.Bottom => (new Point(0, cellSize.Y), new Point(cellSize.X, cellSize.Y)),
                Direction.Left => (new Point(0, 0), new Point(0, cellSize.Y)),
                _ => throw new ArgumentException($"Invalid wall \"{wall}\"")
            };
        }
    }
}