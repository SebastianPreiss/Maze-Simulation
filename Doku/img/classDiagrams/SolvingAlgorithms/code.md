# Source Code for the Solving Algorithm Folder Diagram

This is the source code for the solving algorithm folder diagram.  
The diagram was generated using [Mermaid](https://mermaid.live/edit).

To visualize the diagram, you can paste the following code into the Mermaid live editor or any Mermaid-compatible Markdown viewer.

```mermaid
classDiagram
direction TB

%% Interface
class IPathSolver {
    <<interface>>
    +Solve? Solve(Board board, Position start, Position target)
}

%% Classes
class AStarSolver {
    -Dictionary<Cell, double> _gScore
    -Dictionary<Cell, double> _fScore
    -Queue<Cell> _processedCells
    -Dictionary<Cell, double> _score
    +Solve? Solve(Board board, Position start, Position target)
}

class BfsSolver {
    -Queue<Cell> _processedCells
    -Dictionary<Cell, double> _score
    +Solve? Solve(Board board, Position start, Position target)
}

class HandOnWallSolver {
    -bool leftHanded
    -Queue<Cell> _processedCells
    -Dictionary<Cell, double> _score
    -Dictionary<Cell, bool> _visited
    +Solve? Solve(Board board, Position start, Position target)
}

class RightHandStrategy {
    +IEnumerable<Direction> GetAvailable(Cell cell)
}

class LeftHandStrategy {
    +IEnumerable<Direction> GetAvailable(Cell cell)
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

class Board {
    -Cell[,] _cells
    +int Width
    +int Height
    +Board(int width, int height)
    +Cell this[Position position]
    +void FillBlank()
    +IEnumerator<Cell> GetEnumerator()
}

%% Utils
class BoardUtils {
    +static Position GetNewPosition(Cell current, Direction move)
    +static bool TryMove(Board board, Cell cell, Direction direction, out Position nextPos)
    +static IEnumerable<(Direction, Position)> GetAvailableDirections(Board board, Cell cell, IAvailableStrategy availableStrategy, INextStrategy nextStrategy)
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
IPathSolver <|.. AStarSolver : "implements"
IPathSolver <|.. BfsSolver : "implements"
IPathSolver <|.. HandOnWallSolver : "implements"

HandOnWallSolver --> RightHandStrategy : "uses"
HandOnWallSolver --> LeftHandStrategy : "uses"

AStarSolver ..> BoardUtils : "depends on"
BfsSolver ..> BoardUtils : "depends on"
HandOnWallSolver ..> BoardUtils : "depends on"

%% Dependencies
IPathSolver ..> Solve : "uses"
IPathSolver ..> Board : "uses"
IPathSolver ..> Position : "uses"
IPathSolver ..> Cell : "uses"
IPathSolver ..> Direction : "uses"

```
