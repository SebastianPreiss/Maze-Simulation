This is the sourcecode for the generation img.
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

class IBoardStrategy {
    +void Reset()
    +void SetStartingPoint(int x, int y)
    +void Generate()
}

class MazeGenerator {
    <<implements>> IBoardStrategy
    -readonly Cell[,]? _cells
    -readonly Stack~Cell~ _track
    -readonly Random _random
    -readonly bool _multiPath

    +MazeGenerator(ref Cell[,]? cells, int seed = 42, bool multiPath = false)
    +void Reset()
    +void SetStartingPoint(int x, int y)
    +void Generate()
}

class BoardControl {
    +int Width
    +int Height
    +Cell[,]? Cells
    -readonly int _seed
    -IBoardStrategy _strategy

    +BoardControl(int width, int height, int seed)
    +void GenerateMaze()
}

class BoardUtils {
    <<static>>
    +void FillBlank(Cell[,]? cells)
    +void Reset(Cell[,]? cells)
}

Move <-- Cell : uses
MazeGenerator --> Cell : manages
MazeGenerator --> BoardUtils : uses
BoardControl --> MazeGenerator : uses
BoardControl --> IBoardStrategy : strategy pattern
MazeGenerator --|> IBoardStrategy : implements
BoardUtils --> Cell : operates on
```
