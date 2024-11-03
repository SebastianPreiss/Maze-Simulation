using Maze_Simulation.Generation;
using Maze_Simulation.SolvingAlgorithms;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using Timer = System.Timers.Timer;

namespace Maze_Simulation.Model
{
    public class MainViewModel
    {
        public Cell[,]? Cells;
        public int CellSize = 16;
        public int OffsetX;
        public int OffsetY;
        public double MinPadding = 0.8;
        public List<Cell>? SolvedPath { get; private set; }

        private readonly Timer _timer;
        private readonly Stopwatch _stopwatch;
        public event PropertyChangedEventHandler PropertyChanged;

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
            _timer = new Timer(100);
            _timer.Elapsed += (s, e) => UpdateDuration();
            _stopwatch = new Stopwatch();
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
                        MessageBox.Show("Cannot set start on target cell", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        MessageBox.Show("Cannot set target on start cell", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
        public async void StartAlgorithm(int index)
        {
            IPathSolver? solver = null;
            if (Cells == null)
            {
                MessageBox.Show("No Maze defined", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            switch (index)
            {
                // A*
                case 0:
                    solver = new AStarSolver(Cells);
                    break;

                // Dijkstra
                case 1:
                    break;

                default:
                    MessageBox.Show("No valid algorithm", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
            if (solver == null) return;

            _stopwatch.Start();
            _timer.Start();

            SolvedPath = await Task.Run(() => solver.StartSolver());
            if (SolvedPath == null) MessageBox.Show("Unable to find Path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);


            _timer.Stop();
            _stopwatch.Stop();

            Application.Current.Dispatcher.Invoke(() =>
            {
                Duration = $"{_stopwatch.Elapsed.Milliseconds}ms";
            });
        }

        /// <summary>
        /// Resets the solved path and duration, clearing any previously calculated paths.
        /// </summary>
        public void ResetSolvedPath()
        {
            if (Cells == null) return;
            SolvedPath?.Clear();
            Duration = "0ms";
            _stopwatch.Reset();
        }

        private void UpdateDuration()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Duration = $"{_stopwatch.Elapsed.Milliseconds}ms";
            });
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
