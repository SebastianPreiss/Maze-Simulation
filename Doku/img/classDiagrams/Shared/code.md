# Source Code for the Shared Folder Diagram

This is the source code for the shared folder diagram.  
The diagram was generated using [Mermaid](https://mermaid.live/edit).

To visualize the diagram, you can paste the following code into the Mermaid live editor or any Mermaid-compatible Markdown viewer.

```mermaid
classDiagram
direction LR

%% Main classes
class Board {
    -Cell[,] _cells
    +int Width
    +int Height
    +Board(int width, int height)
    +Cell this[Position position]
    +void FillBlank()
    +IEnumerator<Cell> GetEnumerator()
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

class Solve {
    +IList<Direction> Steps
    +Position Start
    +Position Target
    +Queue<Cell> ProcessingOrder
    +IDictionary<Cell, double> CellValue
}

class Direction {
    <<enum>>
    None
    Top
    Right
    Bottom
    Left
}

%% Utility classes
class BoardUtils {
    +static Position GetNewPosition(Cell current, Direction move)
    +static bool TryMove(Board board, Cell cell, Direction direction, out Position nextPos)
    +static IEnumerable<(Direction, Position)> GetAvailableDirections(Board board, Cell cell, IAvailableStrategy availableStrategy, INextStrategy nextStrategy)
}

class MathUtils {
    +static double Range(double val, double min, double max)
}

class DirectionExtension {
    +static (int x, int y) GetOffset(Direction direction)
}

%% Interfaces and strategies
class IAvailableStrategy {
    +IEnumerable<Direction> GetAvailable(Cell cell)
}

class INextStrategy {
    +int GetNextIndex(int available)
}

class WallStrategy {
    +IEnumerable<Direction> GetAvailable(Cell cell)
}

class NoWallStrategy {
    +IEnumerable<Direction> GetAvailable(Cell cell)
}

class FirstNextStrategy {
    +int GetNextIndex(int available)
}

class RandomNextStrategy {
    +int GetNextIndex(int available)
}

%% Relationships
Board *-- Cell : "contains"
Board *-- Position : "uses"
Solve --> Cell : "processes"
Solve --> Direction : "steps"
Cell --> Position : "has a"
Cell --> Direction : "walls"
BoardUtils --> Board : "depends on"
BoardUtils --> IAvailableStrategy : "uses"
BoardUtils --> INextStrategy : "uses"
WallStrategy ..|> IAvailableStrategy : "implements"
NoWallStrategy ..|> IAvailableStrategy : "implements"
FirstNextStrategy ..|> INextStrategy : "implements"
RandomNextStrategy ..|> INextStrategy : "implements"
DirectionExtension --> Direction : "extends"
Position --> MathUtils : "uses"
```
