using Maze_Simulation.Generation;
using Maze_Simulation.Model;
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
                DrawMaze();
            };

            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        /// <summary>
        /// Draws the maze on the canvas based on the current state of the view model.
        /// Clears previous drawings and calculates cell sizes and offsets.
        /// </summary>
        private void DrawMaze()
        {
            if (_viewModel.Cells == null) return;

            MazeCanvas.Children.Clear();

            var drawingVisual = new DrawingVisual();
            using (var dc = drawingVisual.RenderOpen())
            {
                _viewModel.CalcCellSize((int)MazeCanvas.ActualWidth, (int)MazeCanvas.ActualHeight);
                _viewModel.CalcOffset((int)MazeCanvas.ActualWidth, (int)MazeCanvas.ActualHeight);

                for (var i = 0; i < _viewModel.Cells.GetLength(0); i++)
                {
                    for (var j = 0; j < _viewModel.Cells.GetLength(1); j++)
                    {
                        var cell = _viewModel.Cells[i, j];
                        var x = i * _viewModel.CellSize + _viewModel.OffsetX;
                        var y = j * _viewModel.CellSize + _viewModel.OffsetY;

                        if (cell.Walls[Cell.Bottom])
                            dc.DrawLine(new Pen(Brushes.Black, 1), new Point(x, y), new Point(x + _viewModel.CellSize, y));
                        if (cell.Walls[Cell.Right])
                            dc.DrawLine(new Pen(Brushes.Black, 1), new Point(x + _viewModel.CellSize, y), new Point(x + _viewModel.CellSize, y + _viewModel.CellSize));
                        if (cell.Walls[Cell.Top])
                            dc.DrawLine(new Pen(Brushes.Black, 1), new Point(x, y + _viewModel.CellSize), new Point(x + _viewModel.CellSize, y + _viewModel.CellSize));
                        if (cell.Walls[Cell.Left])
                            dc.DrawLine(new Pen(Brushes.Black, 1), new Point(x, y), new Point(x, y + _viewModel.CellSize));

                        if (_viewModel.SolvedPath != null && _viewModel.SolvedPath.Contains(cell) && cell is { IsStart: false, IsTarget: false })
                        {

                            var pathX = cell.X * _viewModel.CellSize + _viewModel.OffsetX;
                            var pathY = cell.Y * _viewModel.CellSize + _viewModel.OffsetY;
                            var rectangleSize = _viewModel.CellSize * 0.4;

                            var pathColor = CalcColor(cell);

                            dc.DrawRectangle(
                                new SolidColorBrush(pathColor),
                                new Pen(new SolidColorBrush(pathColor), 1),
                                new Rect(
                                    pathX + (_viewModel.CellSize - rectangleSize) / 2,
                                    pathY + (_viewModel.CellSize - rectangleSize) / 2,
                                    rectangleSize,
                                    rectangleSize
                                )
                            );
                        }


                        if (cell.IsStart)
                        {
                            dc.DrawEllipse(
                                Brushes.Blue, null,
                                new Point(
                                    x + _viewModel.CellSize / 2,
                                    y + _viewModel.CellSize / 2
                                ),
                                _viewModel.CellSize * _viewModel.MinPadding / 2,
                                _viewModel.CellSize * _viewModel.MinPadding / 2
                            );
                        }

                        if (cell.IsTarget)
                        {
                            dc.DrawEllipse(
                                Brushes.Red, null,
                                new Point(
                                    x + _viewModel.CellSize / 2,
                                    y + _viewModel.CellSize / 2
                                ),
                                _viewModel.CellSize * _viewModel.MinPadding / 2,
                                _viewModel.CellSize * _viewModel.MinPadding / 2
                            );
                        }
                    }
                }
            }

            var drawingImage = new DrawingImage(drawingVisual.Drawing);

            var image = new Image
            {
                Source = drawingImage,
                Width = _viewModel.Cells.GetLength(0) * _viewModel.CellSize,
                Height = _viewModel.Cells.GetLength(1) * _viewModel.CellSize
            };

            var offsetX = (MazeCanvas.ActualWidth - image.Width) / 2;
            var offsetY = (MazeCanvas.ActualHeight - image.Height) / 2;

            Canvas.SetLeft(image, offsetX);
            Canvas.SetTop(image, offsetY);

            MazeCanvas.Children.Add(image);
        }

        /// <summary>
        /// Calculates the color for a given cell based on its position in the solution path.
        /// The color gradient transitions from blue (start point) to red (end point).
        /// </summary>
        /// <param name="cell">The cell for which the color is to be calculated.</param>
        /// <returns>The calculated color as a <see cref="Color"/>, representing the gradient from blue to red.</returns>
        private Color CalcColor(Cell cell)
        {
            var cellIndex = _viewModel.SolvedPath.IndexOf(cell);
            var totalSteps = _viewModel.SolvedPath.Count;

            var red = (byte)(255 * cellIndex / (double)totalSteps);
            var blue = (byte)(255 * (1 - cellIndex / (double)totalSteps));

            return Color.FromArgb(128, red, 0, blue);
        }

        /// <summary>
        /// Handles the click event for generating a new maze. Resets the solved path and generates a new board.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void OnGenerateClicked(object sender, RoutedEventArgs e)
        {
            _viewModel.ResetSolvedPath();
            _viewModel.GenerateBoard(Seed.Text, (int)WidthSlider.Value, (int)HeightSlider.Value);
            DrawMaze();
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
            var position = e.GetPosition(MazeCanvas);
            _viewModel.SelectActionOnCell(position);
            DrawMaze();
        }

        /// <summary>
        /// Starts the selected maze-solving algorithm asynchronously. Resets the solved path before starting.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="routedEventArgs">The event data.</param>
        private async void OnStartAlgorithm(object sender, RoutedEventArgs routedEventArgs)
        {
            _viewModel.ResetSolvedPath();
            var index = AlgorithmComboBox.SelectedIndex;
            await _viewModel.StartAlgorithm(index);
            DrawMaze();
        }
    }
}