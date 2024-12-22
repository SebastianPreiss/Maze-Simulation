# Source Code for the Visualisation Diagram

This is the source code for the visualisation diagram.  
The diagram was generated using [Mermaid](https://mermaid.live/edit).

To visualize the diagram, you can paste the following code into the Mermaid live editor or any Mermaid-compatible Markdown viewer.

```mermaid
classDiagram
direction LR
%% Interfaces

class IBoardStrategy {
    <<interface>>
    +int Seed
    +Board Generate(int width, int height, bool multiPath)
}

class IPathSolver {
    <<interface>>
    +Solve? Solve(Board board, Position start, Position target)
}

%% Classes

class DrawSettings {
    +Spacing Spacing
    +Pens Pens
    +Highlights Highlights
}

class Pens {
    +Pen Wall
}

class Highlight {
    +Brush Fill
    +Size Size
}

class Highlights {
    +Highlight Start
    +Highlight Target
}

class Spacing {
    +Size CellSize
    +Size Offset
    +Size SolveSize
    +Size ProcessingSize
    +static Spacing GetDrawingSpacing(Board board, FrameworkElement element)
}

class MainViewModel {
    +Board Board
    +Position Start
    +Position Target
    +Solve Solve
    +List~IBoardStrategy~ Generators
    +List~IPathSolver~ Solvers
    +DispatcherTimer Timer
    +int ToVisualize
    +bool IsRunning
    +delegate BoardEvent(Board board)
    +event BoardEvent OnBoardChanged
    +string Duration
    +void GenerateBoard(string seed, int width, int height, bool multiPath)
    +void SelectActionOnCell(Position position, MouseButtonEventArgs mouseButton)
    +void SolveBoard(int index)
    +void StartVisualisation()
    +void StopVisualisation()
}

class MainWindow {
    +MainViewModel _viewModel
    +Image _maze
    +void UpdateSettings(Board board)
    +void Draw()
    +void DrawBoard(Board board)
    +static void DrawHighlight(Board board, Position position, DrawingContext dc, Highlight settings)
    +static void DrawBoard(Board board, DrawingContext dc)
    +void DrawSolve(Board board, Solve solve, DrawingContext dc)
    +void DrawSolveProcess(Board board, Solve solve, DrawingContext dc)
    +static void DrawSolvePath(Board board, Solve solve, DrawingContext dc)
}

class Board {
    -Cell[,] _cells
    +int Width
    +int Height
    +Board(int width, int height)
    +Cell this[Position position]
    +void FillBlank()
    +IEnumerator<Cell> GetEnumerator()
}

class Position {
    +int X
    +int Y
}

class Solve {
    +IList<Direction> Steps
    +Position Start
    +Position Target
    +Queue<Cell> ProcessingOrder
    +IDictionary<Cell, double> CellValue
}

%% Relations
DrawSettings --> Pens : "Uses"
DrawSettings --> Highlights : "Contains"
DrawSettings --> Spacing : "Contains"
Highlights --> Highlight : "Contains"

MainViewModel --> Board : "Has"
MainViewModel --> Position : "Contains"
MainViewModel --> Solve : "Uses"
MainViewModel --> IBoardStrategy : "Generates"
MainViewModel --> IPathSolver : "Solves"

MainWindow --> MainViewModel : "Displays"
MainWindow --> Board : "Manipulates"
MainWindow --> Position : "Manipulates"
MainWindow --> Solve : "Manipulates"
```
