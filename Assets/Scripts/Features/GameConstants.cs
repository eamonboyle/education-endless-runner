using UnityEngine;

/// <summary>
/// Shared gameplay constants used across multiple systems.
/// Lane X-positions match the values used by QuestionGeneration and QuestionGenerator.
/// </summary>
public static class GameConstants
{
    /// <summary>X position of the left lane.</summary>
    public const float LEFT_LANE = -1.586f;

    /// <summary>X position of the center lane.</summary>
    public const float CENTER_LANE = 0f;

    /// <summary>X position of the right lane.</summary>
    public const float RIGHT_LANE = 1.586f;

    /// <summary>Y height at which question boxes and collectibles are placed.</summary>
    public const float BOX_HEIGHT = 1.3f;

    /// <summary>Base points awarded for a correct answer.</summary>
    public const int BASE_CORRECT_POINTS = 10;
}
