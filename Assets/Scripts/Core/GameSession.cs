/// <summary>
/// Single entry point for starting a gameplay run. Ensures session state,
/// lives, difficulty, and question history are reset consistently before
/// the GAME scene loads.
/// </summary>
public static class GameSession
{
    /// <summary>
    /// Resets all per-run state and loads the game scene.
    /// Every path into gameplay must call this instead of LoadGame directly.
    /// </summary>
    public static void BeginRun()
    {
        GameState.Init();
        Question.ClearRecentHistory();
        QuestionHistoryDisplay.ClearHistory();

        if (LivesSystem.Instance != null)
            LivesSystem.Instance.ResetLives();

        if (DifficultyPresets.Instance != null)
            DifficultyPresets.Instance.ApplyDifficulty();

        if (TimeAttackMode.IsTimeAttack() && TimeAttackMode.Instance != null)
            TimeAttackMode.Instance.StartTimer();

        if (GameManager.instance != null)
            GameManager.instance.LoadGame();
        else
            UnityEngine.Debug.LogError("GameSession.BeginRun: GameManager.instance is null.");
    }
}
