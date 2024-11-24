using Maze_Simulation.Generation;
using Maze_Simulation.SolvingAlgorithms;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Maze_Simulation.Model
{
    public class MainViewModel : INotifyPropertyChanged
    {
        //Maze properties
        public Cell[,]? Cells;

        //Drawing properties
        public int CellSize = 16;
        public int OffsetX;
        public int OffsetY;
        public double MinPadding = 0.9;

        //Pathfinding properties
        public List<Cell>? SolvedPath { get; private set; }
        public IPathSolver? Solver;
        private readonly AStarSolver _aStarSolver;

        private List<(Cell Cell, double Cost)> _processedCells;
        public List<(Cell Cell, double Cost)> ProcessedCells
        {
            get => _processedCells;
            set
            {
                _processedCells = value;
                OnPropertyChanged(nameof(ProcessedCells));
            }
        }
        private bool _isPathSolved;

        //UI properties
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly Stopwatch _stopwatch;
        private string _duration;

        public string Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            _stopwatch = new Stopwatch();
            _aStarSolver = new AStarSolver();
            Duration = "0:0.0";
        }

        /// <summary>
        /// Generates a new maze board with the specified dimensions and seed value.
        /// </summary>
        /// <param name="Seed">The seed used for random generation of the maze.</param>
        /// <param name="mazeWidth">The width of the maze.</param>
        /// <param name="mazeHeight">The height of the maze.</param>
        public void GenerateBoard(string Seed, int mazeWidth, int mazeHeight, bool multiPath)
        {
            int.TryParse(Seed, out var seed);
            var board = new BoardControl(mazeWidth, mazeHeight, seed);
            board.GenerateMaze(multiPath);
            Cells = board.Cells;
        }

        /// <summary>
        /// Draws the basic structure of the maze on the given canvas using the specified DrawingContext.
        /// Calculates cell sizes and offsets based on the canvas dimensions.
        /// Renders walls, start point, and target point for each cell in the maze grid.
        /// </summary>
        /// <param name="MazeCanvas">The canvas where the maze is drawn.</param>
        /// <param name="dc">The DrawingContext used to render the maze elements.</param
        public void DramBasicBoard(Canvas MazeCanvas, DrawingContext dc)
        {
            if (Cells == null) return;

            CalcCellSize((int)MazeCanvas.ActualWidth, (int)MazeCanvas.ActualHeight);
            CalcOffset((int)MazeCanvas.ActualWidth, (int)MazeCanvas.ActualHeight);

            for (var i = 0; i < Cells.GetLength(0); i++)
            {
                for (var j = 0; j < Cells.GetLength(1); j++)
                {
                    var cell = Cells[i, j];
                    var x = i * CellSize + OffsetX;
                    var y = j * CellSize + OffsetY;

                    if (cell.Walls[Cell.Bottom])
                        dc.DrawLine(new Pen(Brushes.Black, 1), new Point(x, y), new Point(x + CellSize, y));
                    if (cell.Walls[Cell.Right])
                        dc.DrawLine(new Pen(Brushes.Black, 1), new Point(x + CellSize, y), new Point(x + CellSize, y + CellSize));
                    if (cell.Walls[Cell.Top])
                        dc.DrawLine(new Pen(Brushes.Black, 1), new Point(x, y + CellSize), new Point(x + CellSize, y + CellSize));
                    if (cell.Walls[Cell.Left])
                        dc.DrawLine(new Pen(Brushes.Black, 1), new Point(x, y), new Point(x, y + CellSize));

                    if (cell.IsStart)
                    {
                        dc.DrawEllipse(
                            Brushes.Blue, null,
                            new Point(
                                x + CellSize / 2,
                                y + CellSize / 2
                            ),
                            CellSize * MinPadding / 2,
                            CellSize * MinPadding / 2
                        );
                    }

                    if (cell.IsTarget)
                    {
                        dc.DrawEllipse(
                            Brushes.Red, null,
                            new Point(
                                x + CellSize / 2,
                                y + CellSize / 2
                            ),
                            CellSize * MinPadding / 2,
                            CellSize * MinPadding / 2
                        );
                    }
                }

            }
        }

        /// <summary>
        /// Draws the solved path of the maze onto the given DrawingContext.
        /// Highlights cells that are part of the solution path, excluding the start and target cells.
        /// Each path cell is drawn as a colored rectangle, with its color determined dynamically.
        /// </summary>
        /// <param name="dc">The DrawingContext used to render the solved path.</param>
        public void DrawSolvedPath(DrawingContext dc)
        {

            for (var i = 0; i < Cells.GetLength(0); i++)
            {
                for (var j = 0; j < Cells.GetLength(1); j++)
                {
                    var cell = Cells[i, j];
                    var x = i * CellSize + OffsetX;
                    var y = j * CellSize + OffsetY;


                    if (SolvedPath != null && SolvedPath.Contains(cell) && cell is { IsStart: false, IsTarget: false })
                    {

                        var pathX = cell.X * CellSize + OffsetX;
                        var pathY = cell.Y * CellSize + OffsetY;
                        var rectangleSize = CellSize * 0.4;

                        var pathColor = CalcColor(cell);

                        dc.DrawRectangle(
                            new SolidColorBrush(pathColor),
                            new Pen(new SolidColorBrush(pathColor), 1),
                            new Rect(
                                pathX + (CellSize - rectangleSize) / 2,
                                pathY + (CellSize - rectangleSize) / 2,
                                rectangleSize,
                                rectangleSize
                            )
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Draws the processed cells of the maze during the execution of the pathfinding algorithm.
        /// The processed cells are represented as semi-transparent rectangles, scaled to fit within each maze cell.
        /// If the path has already been solved, this method skips drawing the processed cells.
        /// </summary>
        /// <param name="dc">The drawing context used for rendering the processed cells.</param>
        /// <param name="processedCells">
        /// A collection of tuples containing the processed cells and their associated cost values.
        /// The cost values are used to represent the processing state of the algorithm.
        /// </param>
        public void DrawProcessedCells(DrawingContext dc, IEnumerable<(Cell Cell, double Cost)> processedCells)
        {
            if (_isPathSolved) return;
            var maxCost = processedCells.Max(c => c.Cost);
            var minCost = processedCells.Min(c => c.Cost);
            var rectangleSize = CellSize * 0.4;

            foreach (var (cell, cost) in processedCells)
            {
                var normalizedCost = (cost - minCost) / (maxCost - minCost);
                var color = Color.FromArgb(
                    128,
                    (byte)(255 * normalizedCost),
                    (byte)(255 * (1 - normalizedCost)),
                    0
                );

                var pathX = cell.X * CellSize + OffsetX;
                var pathY = cell.Y * CellSize + OffsetY;

                dc.DrawRectangle(
                    new SolidColorBrush(color),
                    new Pen(new SolidColorBrush(color), 1),
                    new Rect(
                        pathX + (CellSize - rectangleSize) / 2,
                        pathY + (CellSize - rectangleSize) / 2,
                        rectangleSize,
                        rectangleSize
                    )
                );
            }
        }

        /// <summary>
        /// Calculates the color for a given cell based on its position in the solution path.
        /// The color gradient transitions from blue (start point) to red (end point).
        /// </summary>
        /// <param name="cell">The cell for which the color is to be calculated.</param>
        /// <returns>The calculated color as a <see cref="Color"/>, representing the gradient from blue to red.</returns>
        private Color CalcColor(Cell cell)
        {
            var cellIndex = SolvedPath.IndexOf(cell);
            var totalSteps = SolvedPath.Count;

            var red = (byte)(255 * cellIndex / (double)totalSteps);
            var blue = (byte)(255 * (1 - cellIndex / (double)totalSteps));

            return Color.FromArgb(128, red, 0, blue);
        }

        /// <summary>
        /// Calculates the size of each cell based on the given canvas dimensions and a minimum padding factor.
        /// </summary>
        /// <param name="canvasWidth">The width of the canvas to draw the maze.</param>
        /// <param name="canvasHeight">The height of the canvas to draw the maze.</param>
        private void CalcCellSize(int canvasWidth, int canvasHeight)
        {
            if (Cells == null) return;
            var width = canvasWidth * MinPadding;
            var height = canvasHeight * MinPadding;

            var cellWidth = width / Cells.GetLength(0);
            var cellHeight = height / Cells.GetLength(1);

            CellSize = (int)(cellHeight < cellWidth ? cellHeight : cellWidth);
        }

        /// <summary>
        /// Calculates the offset to center the maze within the given canvas dimensions.
        /// </summary>
        /// <param name="canvasWidth">The width of the canvas to draw the maze.</param>
        /// <param name="canvasHeight">The height of the canvas to draw the maze.</param>
        public void CalcOffset(int canvasWidth, int canvasHeight)
        {
            if (Cells == null) return;
            var mazeWidth = Cells.GetLength(0) * CellSize;
            var mazeHeight = Cells.GetLength(1) * CellSize;
            OffsetX = (canvasWidth - mazeWidth) / 2;
            OffsetY = (canvasHeight - mazeHeight) / 2;
        }

        /// <summary>
        /// Handles user actions on a selected cell in the maze, allowing the user to set a start or target cell.
        /// </summary>
        /// <param name="position">The position of the mouse click on the canvas.</param>
        public void SelectActionOnCell(Point position)
        {
            var relativeX = position.X - OffsetX;
            var relativeY = position.Y - OffsetY;

            var row = (int)(relativeX / CellSize);
            var column = (int)(relativeY / CellSize);

            if (Cells == null || row < 0 || row >= Cells.GetLength(0) || column < 0 ||
                column >= Cells.GetLength(1)) return;

            var clickedCell = Cells[row, column];
            var dialog = new CellActionDialog();
            if (dialog.ShowDialog() != true) return;
            switch (dialog.SelectedAction)
            {
                case "Start":
                    if (clickedCell.IsTarget)
                    {
                        MessageBox.Show("Cannot set start on target cell", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        break;
                    }

                    foreach (var cell in Cells)
                    {
                        cell.IsStart = false;
                    }

                    clickedCell.IsStart = true;
                    clickedCell.IsTarget = false;
                    break;

                case "Target":
                    if (clickedCell.IsStart)
                    {
                        MessageBox.Show("Cannot set target on start cell", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        break;
                    }

                    foreach (var cell in Cells)
                    {
                        cell.IsTarget = false;
                    }

                    clickedCell.IsStart = false;
                    clickedCell.IsTarget = true;
                    break;

                case "Cancel":
                    break;
            }
        }

        /// <summary>
        /// Starts the selected pathfinding algorithm (A* or Dijkstra) and calculates the solved path.
        /// Displays an error if no maze is defined or if the selected algorithm is invalid.
        /// </summary>
        /// <param name="index">The index of the algorithm to use (0 for A*, 1 for Dijkstra).</param>
        public async Task StartAlgorithm(int index, bool visualize)
        {
            if (Cells == null)
            {
                MessageBox.Show("No Maze defined", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Solver = index switch
            {
                0 => _aStarSolver,
                1 => new HandOnWallSolver { UseLeftHand = true },
                2 => new HandOnWallSolver { UseLeftHand = false },
                _ => null                     // Default
            };

            Solver.ProcessedCellsUpdated += OnProcessedCellsUpdated;

            if (Solver == null)
            {
                MessageBox.Show("No valid algorithm", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                _isPathSolved = false;
                _stopwatch.Restart();

                UpdateDurationInBackground();

                Solver.InitSolver(Cells);

                SolvedPath = await Solver.StartSolver(visualize);

                if (SolvedPath == null)
                {
                    MessageBox.Show("Unable to find Path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else _isPathSolved = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _stopwatch.Stop();
                Duration = _stopwatch.Elapsed.ToString(@"mm\:ss\.fff");
            }
        }

        /// <summary>
        /// Updates the list of processed cells and notifies the UI about the change.
        /// This method is typically called when the set of processed cells in the pathfinding algorithm is updated.
        /// </summary>
        /// <param name="processedCells">The updated list of processed cells, each associated with its cost value.</param>
        private void OnProcessedCellsUpdated(IEnumerable<(Cell Cell, double Cost)> processedCells)
        {
            ProcessedCells = processedCells.ToList();
            OnPropertyChanged(nameof(ProcessedCells));
        }

        /// <summary>
        /// Updates the duration of the pathfinding algorithm in the background while the stopwatch is running.
        /// </summary>
        private async Task UpdateDurationInBackground()
        {
            while (_stopwatch.IsRunning)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Duration = _stopwatch.Elapsed.ToString(@"mm\:ss\.fff");
                });
                await Task.Delay(500);
            }
        }

        /// <summary>
        /// Resets the solved path and duration, clearing any previously calculated paths.
        /// </summary>
        public void ResetSolvedPath()
        {
            if (Cells == null) return;
            SolvedPath?.Clear();
            _stopwatch.Reset();
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}