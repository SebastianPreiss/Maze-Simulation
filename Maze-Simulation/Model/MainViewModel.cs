using Maze_Simulation.Generation;
using System.Windows;

namespace Maze_Simulation.Model
{
    public class MainViewModel
    {
        public Cell[,]? Cells;
        public int CellSize = 16;
        public int OffsetX;
        public int OffsetY;

        public double MinPadding = 0.8;


        public void GenerateBoard(string Seed, int mazeWidth, int mazeHeight)
        {
            int.TryParse(Seed, out var seed);
            var board = new BoardControl(mazeWidth, mazeHeight, seed);
            board.GenerateMaze();
            Cells = board.Cells;
        }

        public void CalcCellSize(int canvasWidth, int canvasHeight)
        {
            if (Cells == null) return;
            var width = canvasWidth * MinPadding;
            var height = canvasHeight * MinPadding;

            var cellWidth = width / Cells.GetLength(0);
            var cellHeight = height / Cells.GetLength(1);

            CellSize = (int)(cellHeight < cellWidth ? cellHeight : cellWidth);
        }

        public void CalcOffset(int canvasWidth, int canvasHeight)
        {
            if (Cells == null) return;
            var mazeWidth = Cells.GetLength(0) * CellSize;
            var mazeHeight = Cells.GetLength(1) * CellSize;
            OffsetX = (canvasWidth - mazeWidth) / 2;
            OffsetY = (canvasHeight - mazeHeight) / 2;
        }

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

        public void StartAlgorithm(int index)
        {

            MessageBox.Show("This will be implemented soon");

            switch (index)
            {
                // A*

                case 0:
                    break;

                // Dijkstra
                case 1:
                    break;
            }
        }
    }
}
