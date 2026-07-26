using UnityEngine;
using UnityEngine.UI;
using MathRunner.Core;

public static class GameState
{
    private static bool gameRunning = false;
    private static bool gameOver = false;
    private static bool questionExists = false;
    private static int score = 0;
    private static float characterSpeed = GameConstants.DEFAULT_SPEED;
    private static int questionsAnsweredThisGame = 0;
    private static int correctAnswersThisGame = 0;
    private static float gameStartTime = 0f;

    public static event System.Action OnNewHighScore;
    public static event System.Action<int> OnScoreChanged;

    public static void Init()
    {
        gameRunning = false;
        gameOver = false;
        score = 0;
        characterSpeed = GameConstants.DEFAULT_SPEED;
        questionsAnsweredThisGame = 0;
        correctAnswersThisGame = 0;
        SetQuestionExists(false);
    }

    public static void StartGame()
    {
        gameRunning = true;
        gameOver = false;
        score = 0;
        questionsAnsweredThisGame = 0;
        correctAnswersThisGame = 0;
        gameStartTime = Time.time;
    }

    public static int GetQuestionsAnsweredThisGame() => questionsAnsweredThisGame;
    public static int GetCorrectAnswersThisGame() => correctAnswersThisGame;
    public static float GetGameDuration() => Time.time - gameStartTime;

    public static float GetAccuracyThisGame()
    {
        if (questionsAnsweredThisGame == 0) return 0f;
        return (float)correctAnswersThisGame / questionsAnsweredThisGame * 100f;
    }

    public static void RecordAnswer(bool correct)
    {
        questionsAnsweredThisGame++;
        if (correct) correctAnswersThisGame++;
    }

    public static void ResetAdCount()
    {
        PlayerPrefs.SetInt("adCount", 2);
    }

    public static void GameOver()
    {
        gameRunning = false;
        gameOver = true;
    }

    public static void SetScore(int s)
    {
        score = s;
        OnScoreChanged?.Invoke(score);
    }

    public static void AddScore(int points)
    {
        score += points;
        OnScoreChanged?.Invoke(score);
    }

    public static int GetScore()
    {
        return score;
    }

    public static void SaveScore()
    {
        PlayerPrefs.SetInt(GameConstants.PREF_SCORE, score);
    }

    public static int GetScoreFromSave()
    {
        return PlayerPrefs.GetInt(GameConstants.PREF_SCORE);
    }

    public static void SetRunning(bool running)
    {
        gameRunning = running;
    }

    public static bool IsRunning()
    {
        return gameRunning;
    }

    public static void SetGameOver(bool over)
    {
        gameOver = over;
    }

    public static bool IsGameOver()
    {
        return gameOver;
    }

    public static void DecreaseAdCount()
    {
        int adCount = GetAdCount();

        PlayerPrefs.SetInt("adCount", --adCount);
    }

    public static int GetAdCount()
    {
        return PlayerPrefs.GetInt("adCount");
    }

    public static void SetCharacterSpeed(float speed)
    {
        characterSpeed = speed;
    }

    public static float GetCharacterSpeed()
    {
        return characterSpeed;
    }

    public static void SetCharacter(string character)
    {
        PlayerPrefs.SetString(GameConstants.PREF_CHARACTER, character);
    }

    public static string GetCharacter()
    {
        return PlayerPrefs.GetString(GameConstants.PREF_CHARACTER);
    }

    public static int GetPlayCount()
    {
        return PlayerPrefs.GetInt(GameConstants.PREF_GAMES_PLAYED);
    }

    public static void SetPlayCount(int playCount)
    {
        PlayerPrefs.SetInt(GameConstants.PREF_GAMES_PLAYED, playCount);
    }

    public static void IncrementPlayCount()
    {
        int count = PlayerPrefs.GetInt(GameConstants.PREF_GAMES_PLAYED);
        PlayerPrefs.SetInt(GameConstants.PREF_GAMES_PLAYED, ++count);
    }

    public static void SetQuestionType(string questionType)
    {
        PlayerPrefs.SetString(GameConstants.PREF_MODE, questionType);
    }

    public static string GetQuestionType()
    {
        return PlayerPrefs.GetString(GameConstants.PREF_MODE);
    }

    public static void SetQuestionExists(bool exists)
    {
        questionExists = exists;
    }

    public static bool GetQuestionExists()
    {
        return questionExists;
    }

    public static void SetHighScore()
    {
        int s = GetHighScore();

        if (score > s)
        {
            PlayerPrefs.SetInt(GameConstants.PREF_HIGH_SCORE_PREFIX + GetQuestionType(), score);
            OnNewHighScore?.Invoke();
        }
    }

    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt(GameConstants.PREF_HIGH_SCORE_PREFIX + GetQuestionType());
    }

    public static int GetHighScore(string mode)
    {
        return PlayerPrefs.GetInt(GameConstants.PREF_HIGH_SCORE_PREFIX + mode);
    }

    public static bool IsNewHighScore()
    {
        return score > GetHighScore();
    }

    public static void SetFirstLoad()
    {
        PlayerPrefs.SetInt(GameConstants.PREF_FIRST_LOAD, 1);
    }

    public static bool IsFirstLoad()
    {
        if (PlayerPrefs.GetInt(GameConstants.PREF_FIRST_LOAD) == 0)
        {
            SettingState.SetSound(true);
            SettingState.SetGraphics("Medium");
            return true;
        }

        return false;
    }

    public static void ShowGameUI()
    {
        SetCanvasEnabled("InGameUI", true);
        SetCanvasEnabled("GameOverUI", false);
        SetCanvasEnabled("PauseUI", false);
    }

    public static void ShowPauseUI()
    {
        SetRunning(false);
        QuestionBoxShow(false);

        var player = GameObject.Find("PlayerObject");
        if (player != null)
        {
            var animator = player.GetComponent<Animator>();
            if (animator != null)
                animator.SetBool("isRunning", false);
        }

        SetCanvasEnabled("InGameUI", false);
        SetCanvasEnabled("GameOverUI", false);
        SetCanvasEnabled("PauseUI", true);

        var questionText = GameObject.Find("QuestionText");
        if (questionText != null) questionText.SetActive(false);
    }

    public static void QuestionBoxShow(bool show)
    {
        foreach (GameObject box in GameObject.FindGameObjectsWithTag("QuestionBox"))
        {
            if (box == null) continue;

            var meshRenderer = box.GetComponent<MeshRenderer>();
            if (meshRenderer != null) meshRenderer.enabled = show;

            if (box.transform.childCount > 0)
            {
                var childRenderer = box.transform.GetChild(0).GetComponent<MeshRenderer>();
                if (childRenderer != null) childRenderer.enabled = show;
            }
        }
    }

    public static void ShowGameOverUI()
    {
        PrefsFlush.Flush();

        QuestionBoxShow(false);
        SetRunning(false);
        SetGameOver(true);
        SetQuestionExists(false);
        SetHighScore();

        var highScoreText = GameObject.Find("HighScoreAmount");
        if (highScoreText != null)
        {
            var text = highScoreText.GetComponent<Text>();
            if (text != null) text.text = GetHighScore().ToString();
        }

        var currentScoreText = GameObject.Find("CurrentScoreAmount");
        if (currentScoreText != null)
        {
            var text = currentScoreText.GetComponent<Text>();
            if (text != null) text.text = GetScore().ToString();
        }

        SetCanvasEnabled("InGameUI", false);
        SetCanvasEnabled("PauseUI", false);
        SetCanvasEnabled("GameOverUI", true);

        var questionText = GameObject.Find("QuestionText");
        if (questionText != null) questionText.SetActive(false);
    }

    public static void ShowTutorialGameOver()
    {
        SetRunning(false);
        SetCanvasEnabled("TutorialUI", false);
        SetCanvasEnabled("TutorialGameOverUI", true);

        var questionText = GameObject.Find("QuestionText");
        if (questionText != null) questionText.SetActive(false);
    }

    public static void ShowTutorialCompleteUI()
    {
        SetRunning(false);
        SetCanvasEnabled("TutorialUI", false);
        SetCanvasEnabled("TutorialGameOverUI", false);
        SetCanvasEnabled("TutorialCompleteUI", true);

        var questionText = GameObject.Find("QuestionText");
        if (questionText != null) questionText.SetActive(false);

        var player = GameObject.Find("PlayerObject");
        if (player != null)
        {
            var animator = player.GetComponent<Animator>();
            animator.SetBool("isRunning", false);
            animator.SetBool("dancing", true);
        }

        SetFirstLoad();
    }

    private static void SetCanvasEnabled(string objectName, bool enabled)
    {
        var obj = GameObject.Find(objectName);
        if (obj != null)
        {
            var canvas = obj.GetComponent<Canvas>();
            if (canvas != null) canvas.enabled = enabled;
        }
    }
}