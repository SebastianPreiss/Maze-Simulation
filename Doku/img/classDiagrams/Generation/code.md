# Source Code for the Generation Folder Diagram

This is the source code for the generation folder diagram.  
The diagram was generated using [Mermaid](https://mermaid.live/edit).

To visualize the diagram, paste the following code into the Mermaid live editor or any Mermaid-compatible Markdown viewer.

```mermaid
classDiagram
direction LR
%% Interfaces

class IBoardStrategy {
    <<interface>>
    +int Seed
    +Board Generate(int width, int height, bool multiPath)
}

%% Classes
class Board {
    -Cell[,] _cells
    +int Width
    +int Height
    +Board(int width, int height)
    +Cell this[Position position]
    +void FillBlank()
    +IEnumerator<Cell> GetEnumerator()
}

class BoardUtils {
    +Position GetNewPosition(Cell current, Direction move)
    +bool TryMove(Board board, Cell cell, Direction direction, out Position nextPos)
    +IEnumerable<(Direction, Position)> GetAvailableDirections(Board board, Cell cell, IAvailableStrategy availableStrategy, INextStrategy nextStrategy)
}

class Cell {
    -bool[] _walls
    +int X
    +int Y
    +bool this[Direction wall]
    +string ToString()
}

class Position {
    +int X
    +int Y
}

class MazeGenerator {
    -Stack<Cell> _track
    -Dictionary<Cell, bool> _visited
    -Dictionary<Cell, bool> _collapsed
    -Board _board
    -Random _random
    +int Seed
    +MazeGenerator()
    +Board Generate(int width, int height, bool multiPath)
}

%% Enum Direction
class Direction {
    <<enum>>
    None
    Top
    Right
    Bottom
    Left
}

%% Relationships
MazeGenerator ..> IBoardStrategy : "Implements"
MazeGenerator --> Board : "Uses"
MazeGenerator --> Cell : "Tracks"
MazeGenerator --> Direction : "Utilizes"
MazeGenerator --> Position : "Tracks"
MazeGenerator --> BoardUtils : "Uses"
Board ..> Cell : "Contains"
BoardUtils --> Position : "Calculates"
BoardUtils --> Direction : "Uses"
BoardUtils ..> Cell : "Processes"
Cell --> Direction : "Defines Walls"
```
