This is the sourcecode for the solving img.
The img was generated wit [mermaid](https://mermaid.live/edit).

```
classDiagram
direction LR
class Move {
    <<enumeration>>
    None
    Top
    Right
    Bottom
    Left
}

class Cell {
    +const int Top = 0
    +const int Right = 1
    +const int Bottom = 2
    +const int Left = 3

    +bool[] Walls
    +List~Move~ Available
    +bool IsVisited
    +int X
    +int Y
    +bool IsCollapsed
    +bool IsTarget
    +bool IsStart

    +Cell(int x, int y)
}

class IPathSolver {
    +IEnumerable<(Cell Cell, double Cost)> ProcessedCells
    +event Action< IEnumerable<(Cell Cell, double Cost)>>?
    +void InitSolver(Cell[,] cells)
    +Task~IEnumerable~Cell~~ StartSolver(bool visualize)
}

class HandOnWallSolver {
    +bool UseLeftHand
    +void InitSolver(Cell[,] cells)
    +Task~IEnumerable~Cell~~ StartSolver(bool visualize)
    -Cell[,]? _cells
    -Cell? _start
    -Cell? _target
    -Move _currentDirection
}

class BfsSolver {
    +void InitSolver(Cell[,] cells)
    +Task~IEnumerable~Cell~~ StartSolver(bool visualize)
    -Cell[,]? _cells
    -Cell? _start
    -Cell? _target
}

class AStarSolver {
    +void InitSolver(Cell[,] cells)
    +Task~IEnumerable~Cell~~ StartSolver(bool visualize)
    -Cell[,]? _cells
    -Cell? _start
    -Cell? _target
}

IPathSolver <|-- HandOnWallSolver : implements
IPathSolver <|-- BfsSolver : implements
IPathSolver <|-- AStarSolver : implements
HandOnWallSolver --> Cell : operates on
BfsSolver --> Cell : operates on
AStarSolver --> Cell : operates on
Move <-- Cell : uses
Cell --> IPathSolver : analyzed by
```
