using System.Windows;

namespace Maze_Simulation
{
    /// <summary>
    /// Interaktionslogik für CellActionDialog.xaml
    /// </summary>
    public partial class CellActionDialog : Window
    {
        public string SelectedAction { get; private set; }
        public CellActionDialog()
        {
            InitializeComponent();
        }


        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedAction = "Start";
            DialogResult = true;
        }

        private void TargetButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedAction = "Target";
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedAction = "Cancel";
            DialogResult = false;
        }
    }
}
