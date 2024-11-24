# Maze Generator and Solver

This project provides an application for generating and solving mazes. The mazes are created using the Depth First Search (DFS) algorithm, and various solving algorithms are implemented. Users can customize the maze size and the random seed for maze generation, allowing for the creation and testing of different maze configurations.

## Table of Contents

1. [Generation](#generation)
2. [Solving](#solving)
3. [Visualization](#visualization)
4. [Features](#features)
5. [How to Run](#how-to-run)
6. [Future Work](#future-work)

---

## Generation

![generation](Doku/img/classDiagrams/generation.svg "Generation")

The maze generation process is handled by the **`MazeGenerator`** class, which implements the **`IBoardStrategy`** interface. This allows the application to follow the Strategy Pattern, making it flexible for integrating different maze generation or other board game algorithms in the future.

### How It Works

1. **Initialization**:  
   The board is represented as a 2D array of `Cell` objects. Each `Cell` is initialized with all walls intact and possible moves in all directions (`Top`, `Right`, `Bottom`, `Left`).

   - The starting cell is set at the top-left corner.
   - The target cell is positioned at the bottom-right corner by default.

   Initialization is supported by static utility methods in the **`BoardUtils`** class, which provides reusable logic to set up and reset the board.

2. **Depth First Search Algorithm**:  
   The maze is generated using the DFS algorithm:

   - Starting from a specified cell, the algorithm selects a random direction (from available moves) and removes walls between adjacent cells to create a path.
   - It backtracks when no further moves are possible, ensuring every cell in the maze is visited.

3. **Customizable Parameters**:
   - **Seed**: The random seed allows users to recreate specific maze layouts.
   - **Size**: Users can define the width and height of the maze.
   - **MultiPath**: Users can define if there is more than one path.

### Key Components

- [**`Cell`**](/Maze-Simulation/Generation/Cell.cs): Represents a single unit of the board with walls, coordinates, and state information (e.g., visited, collapsed, start, or target).
- [**`IBoardStrategy`**](/Maze-Simulation/Generation/IBoardStrategy.cs): Defines the contract for generation strategies, allowing for different algorithms to be implemented and swapped dynamically. It includes methods for resetting the board, setting the starting point, and generating the maze.
- [**`MazeGenerator`**](/Maze-Simulation/Generation/MazeGenerator.cs): Implements the DFS algorithm, managing the cells and tracking visited paths using a stack.
- [**`BoardControl`**](/Maze-Simulation/Generation/BoardControl.cs): Manages the overall board dimensions, initialization, and strategy configuration.
- [**`BoardUtils`**](/Maze-Simulation/Generation/BoardUtils.cs): Contains static methods for initializing and resetting the board.

---

## Solving

![solving](Doku/img/classDiagrams/solving.svg "Solving")

The application provides several solving algorithms, each implemented as a class that conforms to the **`IPathSolver`** interface. These algorithms are designed to find a solution path from the start cell to the target cell in the generated maze.

### Algorithms

1. **A\* Algorithm**:

   - Implements a heuristic-based search to find the shortest path efficiently.
   - Prioritizes paths with the lowest cost (distance + heuristic).
   - Visualizes the process of expanding nodes.

2. **Breadth-First Search (BFS)**:

   - Explores all neighboring cells level by level.
   - Guarantees the shortest path in an unweighted maze.
   - Slower compared to A\* due to lack of heuristic guidance.

3. **Hand-on-Wall (Right/Left Hand Rule)**:

   - Simulates a simple "wall-following" approach, mimicking how a person might solve a maze.
   - May not find the shortest path but works in mazes with connected walls.

4. **Custom Solvers**:
   - Developers can add their own solvers by implementing the `IPathSolver` interface.

### Key Components

- [**`IPathSolver`**](/Maze-Simulation/SolvingAlgorithms/IPathSolver.cs): Defines the contract for all solving algorithms.
- [**`AStarSolver`**](/Maze-Simulation/SolvingAlgorithms/AStarSolver.cs): Implements the A\* algorithm.
- [**`BfsSolver`**](/Maze-Simulation/SolvingAlgorithms/BFSSolver.cs): Implements the BFS algorithm.
- [**`HandOnWallSolver`**](/Maze-Simulation/SolvingAlgorithms/HandOnWallSolver.cs): Implements the wall-following method.

---

## Visualization

![visualization](Doku/img/classDiagrams/visualization.svg "Visualization")

The application provides real-time visualization for maze solving processes, making it easier to understand the underlying algorithms.

### Features

- **Dynamic Cell Updates**:
  Each cell's state is updated live on the canvas as the algorithm progresses.
- **Color-Coding**:

  - **Visited Cells**: Highlighted during solving.
  - **Start and Target**: Clearly marked on the board.
  - **Solution Path**: Drawn in a different color after completion.

- **Playback Speed**:  
  Users can control the speed of the animation during solving.

### Components

- [**`MainViewModel`**](/Maze-Simulation/Model/MainViewModel.cs):  
  The ViewModel for the main view of the application. It manages the logic and data binding for the user interface, ensuring that user interactions are correctly translated into the business logic.

- [**`MainView`**](/Maze-Simulation/MainWindow.xaml.cs):  
  The main view that contains the user interface for the maze simulation. It handles the display of maze generation, solving, and visualization.

- [**`CellActionDialog`**](/Maze-Simulation/CellActionDialog.cs):  
  A dialog window that allows the user to select actions for individual cells in the maze, such as setting the start or target points.

---

## Features

- Customizable maze dimensions and seed for reproducible results.
- Support for multiple solving algorithms.
- Interactive cell selection for start and target positions.
- Visual debugging tools for exploring the algorithms step by step.

---

## How to Run

1. Clone the repository:
   ```bash
   git clone https://github.com/your-repo/maze-simulation.git
   cd maze-simulation
   ```
2. Open the solution file in Visual Studio:

- Open MazeSimulation.sln.

3. Build and run the application:

- Press F5 to start the application in debug mode.

4. Interact with the UI:

- Customize maze size, generation seed, and algorithms.
- Visualize maze solving.

## Future Work

- Add support for more generation algorithms (e.g., Prim’s or Kruskal’s algorithm).
- Extend the solver library with more advanced techniques.
- Improve performance for large mazes.
- Add export functionality for solved mazes as images or text.
- Develop a mobile version of the application.

Feel free to reach out for any questions or contributions!
