This is the sourcecode for the view and models img.
The img was generated wit [mermaid](https://mermaid.live/edit).

```
classDiagram
direction LR
    class MainWindow {
        +MainWindow()
        -DrawMaze()
        -OnGenerateClicked(sender, e)
        -WindowResized(sender, e)
        -MazeCanvas_OnMouseDown(sender, e)
        -OnStartAlgorithm(sender, routedEventArgs)
    }

    class MainViewModel {
        +int CellSize
        +int OffsetX
        +int OffsetY
        +double MinPadding
        +Cell[,]? Cells
        +List~Cell~? SolvedPath
        +IPathSolver? Solver
        +List~(Cell, double)~ ProcessedCells
        +string Duration
        +GenerateBoard(seed, mazeWidth, mazeHeight, multiPath)
        +DramBasicBoard(canvas, dc)
        +DrawSolvedPath(dc)
        +DrawProcessedCells(dc, processedCells)
        +SelectActionOnCell(position)
        +StartAlgorithm(index, visualize)
        +ResetSolvedPath()
    }

    class IPathSolver {
        <<interface>>
        +InitSolver(cells)
        +Task~IEnumerable~ StartSolver(visualize)
        +event ProcessedCellsUpdated
    }

    class AStarSolver {
        +InitSolver(cells)
        +Task~IEnumerable~ StartSolver(visualize)
    }

    class HandOnWallSolver {
        +bool UseLeftHand
        +InitSolver(cells)
        +Task~IEnumerable~ StartSolver(visualize)
    }

    class BfsSolver {
        +InitSolver(cells)
        +Task~IEnumerable~ StartSolver(visualize)
    }

    class CellActionDialog {
        +string SelectedAction
        +CellActionDialog()
        -StartButton_Click(sender, e)
        -TargetButton_Click(sender, e)
        -CancelButton_Click(sender, e)
    }

    MainWindow --> MainViewModel : "uses"
    MainViewModel --> IPathSolver : "Solver"
    IPathSolver <|-- AStarSolver : "implements"
    IPathSolver <|-- HandOnWallSolver : "implements"
    IPathSolver <|-- BfsSolver : "implements"
    MainWindow --> CellActionDialog : "opens"
```
