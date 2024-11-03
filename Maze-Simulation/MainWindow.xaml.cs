using Maze_Simulation.Generation;
using Maze_Simulation.Model;
using System.Windows;
using System.Windows.Input;
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

        private void DrawMaze()
        {
            if (_viewModel.Cells == null) return;
            MazeCanvas.Children.Clear();

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
                    {
                        MazeCanvas.Children.Add(new Line
                        {
                            X1 = x,
                            Y1 = y,
                            X2 = x + _viewModel.CellSize,
                            Y2 = y,
                            Stroke = Brushes.Black
                        });
                    }

                    if (cell.Walls[Cell.Right])
                    {
                        MazeCanvas.Children.Add(new Line
                        {
                            X1 = x + _viewModel.CellSize,
                            Y1 = y,
                            X2 = x + _viewModel.CellSize,
                            Y2 = y + _viewModel.CellSize,
                            Stroke = Brushes.Black
                        });
                    }

                    if (cell.Walls[Cell.Top])
                    {
                        MazeCanvas.Children.Add(new Line
                        {
                            X1 = x,
                            Y1 = y + _viewModel.CellSize,
                            X2 = x + _viewModel.CellSize,
                            Y2 = y + _viewModel.CellSize,
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
                            Y2 = y + _viewModel.CellSize,
                            Stroke = Brushes.Black
                        });
                    }

                    if (_viewModel.SolvedPath != null)
                    {
                        foreach (var c in _viewModel.SolvedPath.Where(c => c is { IsTarget: false, IsStart: false }))
                        {
                            var pathX = c.X * _viewModel.CellSize + _viewModel.OffsetX;
                            var pathY = c.Y * _viewModel.CellSize + _viewModel.OffsetY;
                            var rectangleSize = _viewModel.CellSize * 0.4;
                            MazeCanvas.Children.Add(new Rectangle
                            {
                                Width = rectangleSize,
                                Height = rectangleSize,
                                Stroke = Brushes.Yellow,
                                StrokeThickness = 1,
                                Fill = new SolidColorBrush(Color.FromArgb(128, 255, 255, 0)),
                                Margin = new Thickness(
                                    pathX + (_viewModel.CellSize - rectangleSize) / 2,
                                    pathY + (_viewModel.CellSize - rectangleSize) / 2,
                                    0,
                                    0
                                )
                            });
                        }
                    }

                    if (cell.IsStart)
                    {
                        MazeCanvas.Children.Add(new Ellipse
                        {
                            Width = _viewModel.CellSize * _viewModel.MinPadding,
                            Height = _viewModel.CellSize * _viewModel.MinPadding,
                            Fill = Brushes.Blue,
                            Margin = new Thickness(x + _viewModel.CellSize * (1 - _viewModel.MinPadding) / 2,
                                y + _viewModel.CellSize * (1 - _viewModel.MinPadding) / 2, 0, 0)
                        });
                    }

                    if (cell.IsTarget)
                    {
                        MazeCanvas.Children.Add(new Ellipse
                        {
                            Width = _viewModel.CellSize * _viewModel.MinPadding,
                            Height = _viewModel.CellSize * _viewModel.MinPadding,
                            Fill = Brushes.Red,
                            Margin = new Thickness(x + _viewModel.CellSize * (1 - _viewModel.MinPadding) / 2,
                                y + _viewModel.CellSize * (1 - _viewModel.MinPadding) / 2, 0, 0)
                        });
                    }
                }
            }
        }

        private void OnGenerateClicked(object sender, RoutedEventArgs e)
        {
            _viewModel.ResetPathSolver();
            _viewModel.GenerateBoard(Seed.Text, (int)WidthSlider.Value, (int)HeightSlider.Value);
            DrawMaze();
        }

        private void WindowResized(object sender, SizeChangedEventArgs e)
        {
            _resizeTimer.Stop();
            _resizeTimer.Start();
        }

        private void MazeCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            var position = e.GetPosition(MazeCanvas);
            _viewModel.SelectActionOnCell(position);
            DrawMaze();
        }

        private async void OnStartAlgorithm(object sender, RoutedEventArgs routedEventArgs)
        {
            _viewModel.ResetPathSolver();
            var index = AlgorithmComboBox.SelectedIndex;
            await Task.Run(() => _viewModel.StartAlgorithm(index));
            DrawMaze();
        }
    }
}