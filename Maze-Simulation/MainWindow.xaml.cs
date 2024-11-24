using Maze_Simulation.Model;
using Maze_Simulation.SolvingAlgorithms;
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

            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.ProcessedCells))
                {
                    DrawMaze();
                }
            };

        }

        /// <summary>
        /// Draws the maze on the canvas based on the current state of the view model.
        /// Clears previous drawings and calculates cell sizes and offsets.
        /// </summary>
        private void DrawMaze()
        {
            if (_viewModel.Cells == null) return;

            MazeCanvas.Children.Clear();

            var drawingVisualisation = new DrawingVisual();
            using (var dc = drawingVisualisation.RenderOpen())
            {
                _viewModel.DramBasicBoard(MazeCanvas, dc);
                _viewModel.DrawSolvedPath(dc);

                // Draw processed cells
                if (_viewModel.Solver is AStarSolver aStarSolver && VisuSolver.IsChecked.Value)
                {
                    _viewModel.DrawProcessedCells(dc, aStarSolver.ProcessedCells);
                }
            }

            var drawingImage = new DrawingImage(drawingVisualisation.Drawing);

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
        /// Handles the click event for generating a new maze. Resets the solved path and generates a new board.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void OnGenerateClicked(object sender, RoutedEventArgs e)
        {
            _viewModel.ResetSolvedPath();
            _viewModel.GenerateBoard(Seed.Text, (int)WidthSlider.Value, (int)HeightSlider.Value, MultiPath.IsChecked.Value);
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
            var visibility = VisuSolver.IsChecked.Value ? Visibility.Hidden : Visibility.Visible;
            DurationHead.Visibility = visibility;
            DurationContent.Visibility = visibility;

            _viewModel.ResetSolvedPath();
            var index = AlgorithmComboBox.SelectedIndex;
            await _viewModel.StartAlgorithm(index, VisuSolver.IsChecked.Value);
            DrawMaze();
        }
    }
}