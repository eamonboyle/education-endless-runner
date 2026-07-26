# Project Overview

- **Game Title**: Math Runner
- **High-Level Concept**: An educational endless runner where players solve math equations by navigating into correct lanes (each lane represents a numeric answer) while dodging obstacles.
- **Players**: Single Player
- **Inspiration / Reference Games**: Temple Run, Subway Surfers, educational math games
- **Tone / Art Direction**: Bright, stylized, stylized low-poly art style
- **Target Platform**: StandaloneWindows64 (PC) and potentially Mobile (Android/iOS integration in code)
- **Screen Orientation / Resolution**: Landscape (1920x1080)
- **Render Pipeline**: Built-in Render Pipeline

# Game Mechanics

## Core Gameplay Loop
- Player runs forward automatically.
- Math questions are displayed at regular intervals.
- The path is divided into three lanes, with a question box in each lane representing an answer option (one correct, two incorrect).
- Player must swipe or move left/right to enter the lane with the correct answer before passing through the question boxes.
- Correct answers increase the score and allow the player to continue running. Incorrect answers trigger a fall/stumble animation and game-over state.

## Controls and Input Methods
- Keyboard (A/D or Left/Right Arrow Keys) or swipe gestures to shift between Left, Center, and Right lanes.
- Intuitive and responsive movement controls to support rapid lane changes.

# UI
- **In-Game HUD**: Displays the current math question at the top center, difficulty indicators, score, and swipe/movement hints for beginners.
- **Tutorial UI**: Displays simplified equations, swipe prompts, and clear instructions to help the player learn the mechanics.
- **Tutorial Game Over & Complete Screens**: Provide feedback upon success or failure during training.

# Key Asset & Context

The issue occurs in both the **Tutorial** scene and the **Game** scene. The root cause is inside the level generation system, specifically in how the existing floor pieces are tracked and deleted.

### Affected Script
- `Assets/Scripts/GameManagement/LevelGenerator.cs`

### Bug Explanation
1. During `Start()`, `LevelGenerator` fetches pre-existing floors in the scene using `GameObject.FindGameObjectsWithTag("Floor")`.
2. The order in which Unity returns these objects is **arbitrary** and not guaranteed to match their spatial order along the Z axis.
3. As the player runs forward, the `LevelGenerator` instantiates new floor tiles and adds them to `floorPieces`.
4. After every 2 tiles spawned (`floorCount == 2`), the generator attempts to destroy the oldest tile by accessing and destroying `floorPieces[0]`.
5. Since the list was populated in an arbitrary order, `floorPieces[0]` is often NOT the first tile behind the player, but rather a tile **directly in front** of the player (e.g., `Floor (2)` at `z = 90.44`).
6. When this tile is destroyed while the player is running towards it, the player steps into empty space and **falls through the sky**.

# Implementation Steps

## Step 1: Sort Initial Floor Tiles by Z Coordinate
- **Description**: Modify `LevelGenerator.Start()` to sort the initial array of found floor GameObjects by their Z coordinate before adding them to `floorPieces`. This ensures `floorPieces[0]` is always the floor piece with the smallest Z value (the furthest back, which the player has already run over).
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: No

### Code Modification details:
In `Assets/Scripts/GameManagement/LevelGenerator.cs`:

**Before**:
```csharp
    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");

        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        foreach (GameObject piece in floors)
        {
            floorPieces.Add(piece);
        }
    }
```

**After**:
```csharp
    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");

        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        
        // Sort floor pieces by their Z coordinate in ascending order
        System.Array.Sort(floors, (a, b) => a.transform.position.z.CompareTo(b.transform.position.z));

        foreach (GameObject piece in floors)
        {
            floorPieces.Add(piece);
        }
    }
```

# Verification & Testing

## Automated & Static Checks
1. Compile the code to ensure no assembly compilation errors are present.

## Manual Playtesting & Verification Steps
1. Open the **Tutorial** scene (`Assets/Scenes/Tutorial.unity`).
2. Enter **Play Mode** in the Editor.
3. Answer the first equation correctly.
4. Verify that the road does not disappear and that the player successfully reaches the second and third equations without falling through the sky.
5. Complete the tutorial and verify that the Tutorial Complete UI is shown successfully.
6. Open the **Game** scene (`Assets/Scenes/Game.unity`).
7. Enter **Play Mode** in the Editor.
8. Run indefinitely, answering multiple questions correctly.
9. Verify that the floor continues to generate ahead and clean up behind without deleting active/upcoming floor tiles.
