using Maze_Simulation.Generation;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Maze_Simulation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Cell[,]? _cells;
        private int _cellSize = 16;
        private int _offsetX;
        private int _offsetY;
        private readonly DispatcherTimer _resizeTimer;

        private const double MinPadding = 0.8;

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
        }

        private void GenerateBoard()
        {
            int.TryParse(Seed.Text, out var seed);
            var board = new BoardControl((int)WidthSlider.Value, (int)HeightSlider.Value, seed);
            board.GenerateMaze();
            _cells = board.Cells;
        }

        private void CalcCellSize()
        {
            if (_cells == null) return;
            var width = MazeCanvas.ActualWidth * MinPadding;
            var height = MazeCanvas.ActualHeight * MinPadding;

            var cellWidth = width / _cells.GetLength(0);
            var cellHeight = height / _cells.GetLength(1);

            _cellSize = (int)(cellHeight < cellWidth ? cellHeight : cellWidth);
        }

        private void CalcOffset()
        {
            if (_cells == null) return;
            var mazeWidth = _cells.GetLength(0) * _cellSize;
            var mazeHeight = _cells.GetLength(1) * _cellSize;
            _offsetX = (int)((MazeCanvas.ActualWidth - mazeWidth) / 2);
            _offsetY = (int)((MazeCanvas.ActualHeight - mazeHeight) / 2);
        }

        private void DrawMaze()
        {
            if (_cells == null) return;
            MazeCanvas.Children.Clear();

            CalcCellSize();
            CalcOffset();

            for (var i = 0; i < _cells.GetLength(0); i++)
            {
                for (var j = 0; j < _cells.GetLength(1); j++)
                {
                    var cell = _cells[i, j];
                    var x = i * _cellSize + _offsetX;
                    var y = j * _cellSize + _offsetY;
                    if (cell.Walls[Cell.Bottom])
                    {
                        MazeCanvas.Children.Add(new Line
                        {
                            X1 = x,
                            Y1 = y,
                            X2 = x + _cellSize,
                            Y2 = y,
                            Stroke = Brushes.Black
                        });
                    }
                    if (cell.Walls[Cell.Right])
                    {
                        MazeCanvas.Children.Add(new Line
                        {
                            X1 = x + _cellSize,
                            Y1 = y,
                            X2 = x + _cellSize,
                            Y2 = y + _cellSize,
                            Stroke = Brushes.Black
                        });
                    }
                    if (cell.Walls[Cell.Top])
                    {
                        MazeCanvas.Children.Add(new Line
                        {
                            X1 = x,
                            Y1 = y + _cellSize,
                            X2 = x + _cellSize,
                            Y2 = y + _cellSize,
                            Stroke = Brushes.Black
                        });
                    }
                    if (cell.Walls[Cell.Left])
                    {
                        MazeCanvas.Children.Add(new Line
                        {
                            X1 = x,
                            Y1 = y,
                            X2 = x,
                            Y2 = y + _cellSize,
                            Stroke = Brushes.Black
                        });
                    }
                    if (cell.IsPlayer)
                    {
                        MazeCanvas.Children.Add(new Ellipse
                        {
                            Width = _cellSize * MinPadding,
                            Height = _cellSize * MinPadding,
                            Fill = Brushes.Blue,
                            Margin = new Thickness(x + _cellSize * (1 - MinPadding) / 2, y + _cellSize * (1 - MinPadding) / 2, 0, 0)
                        });
                    }
                    if (cell.IsTarget)
                    {
                        MazeCanvas.Children.Add(new Ellipse
                        {
                            Width = _cellSize * MinPadding,
                            Height = _cellSize * MinPadding,
                            Fill = Brushes.Red,
                            Margin = new Thickness(x + _cellSize * (1 - MinPadding) / 2, y + _cellSize * (1 - MinPadding) / 2, 0, 0)
                        });
                    }
                }
            }
        }

        private void OnGenerateClicked(object sender, RoutedEventArgs e)
        {
            GenerateBoard();
            DrawMaze();
        }

        private void WindowResized(object sender, SizeChangedEventArgs e)
        {
            _resizeTimer.Stop();
            _resizeTimer.Start();
        }
    }
}