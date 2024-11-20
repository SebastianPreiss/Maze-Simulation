using Maze_Simulation.Generation;
using Maze_Simulation.SolvingAlgorithms;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

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
        private readonly AStarSolver _aStarSolver;

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
            Duration = "0ms";
        }

        /// <summary>
        /// Generates a new maze board with the specified dimensions and seed value.
        /// </summary>
        /// <param name="Seed">The seed used for random generation of the maze.</param>
        /// <param name="mazeWidth">The width of the maze.</param>
        /// <param name="mazeHeight">The height of the maze.</param>
        public void GenerateBoard(string Seed, int mazeWidth, int mazeHeight)
        {
            int.TryParse(Seed, out var seed);
            var board = new BoardControl(mazeWidth, mazeHeight, seed);
            board.GenerateMaze();
            Cells = board.Cells;
        }

        /// <summary>
        /// Calculates the size of each cell based on the given canvas dimensions and a minimum padding factor.
        /// </summary>
        /// <param name="canvasWidth">The width of the canvas to draw the maze.</param>
        /// <param name="canvasHeight">The height of the canvas to draw the maze.</param>
        public void CalcCellSize(int canvasWidth, int canvasHeight)
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
        public async Task StartAlgorithm(int index)
        {
            if (Cells == null)
            {
                MessageBox.Show("No Maze defined", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            IPathSolver? solver = index switch
            {
                0 => _aStarSolver,            // A*
                1 => null,                    // lefthand 
                2 => null,                    // righthand
                _ => null                     // Default
            };

            if (solver == null)
            {
                MessageBox.Show("No valid algorithm", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                _stopwatch.Restart();

                UpdateDurationInBackground();

                solver.InitSolver(Cells);

                SolvedPath = await solver.StartSolver();

                if (SolvedPath == null)
                {
                    MessageBox.Show("Unable to find Path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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