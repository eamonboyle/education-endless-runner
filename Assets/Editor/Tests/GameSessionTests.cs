using NUnit.Framework;
using UnityEngine;
using MathRunner.Core;

namespace MathRunner.Tests
{
    [TestFixture]
    public class GameSessionTests
    {
        [SetUp]
        public void SetUp()
        {
            GameState.Init();
            Question.ClearRecentHistory();
            PlayerPrefs.DeleteKey(GameConstants.PREF_DIFFICULTY);
            PlayerPrefs.DeleteKey(GameConstants.PREF_TIME_ATTACK);
            ChallengeCodeSystem.SetActiveChallenge("");
        }

        [TearDown]
        public void TearDown()
        {
            if (LivesSystem.Instance != null)
                Object.DestroyImmediate(LivesSystem.Instance.gameObject);
            if (DifficultyPresets.Instance != null)
                Object.DestroyImmediate(DifficultyPresets.Instance.gameObject);
            if (TimeAttackMode.Instance != null)
                Object.DestroyImmediate(TimeAttackMode.Instance.gameObject);
            ChallengeCodeSystem.SetActiveChallenge("");
        }

        [Test]
        public void ClearRecentHistory_AllowsNewQuestions()
        {
            _ = new Question();
            Assert.DoesNotThrow(() => Question.ClearRecentHistory());

            var q = new Question();
            Assert.IsFalse(string.IsNullOrEmpty(q.Text));
            Assert.AreEqual(3, q.Numbers.Count);
            Assert.Contains(q.Answer, q.Numbers);
        }

        [Test]
        public void Init_ThenApplyDifficulty_SetsMediumSpeed()
        {
            var go = new GameObject("[DifficultyPresets_Test]");
            go.AddComponent<DifficultyPresets>();

            GameState.Init();
            Assert.AreEqual(GameConstants.DEFAULT_SPEED, GameState.GetCharacterSpeed(), 0.001f);

            DifficultyPresets.SetDifficulty(DifficultyLevel.Medium);
            DifficultyPresets.Instance.ApplyDifficulty();

            Assert.AreEqual(GameConstants.DEFAULT_SPEED * 1.0f, GameState.GetCharacterSpeed(), 0.001f);
        }

        [Test]
        public void ApplyDifficulty_Easy_ScalesSpeed()
        {
            var go = new GameObject("[DifficultyPresets_Test]");
            go.AddComponent<DifficultyPresets>();

            GameState.Init();
            DifficultyPresets.SetDifficulty(DifficultyLevel.Easy);
            DifficultyPresets.Instance.ApplyDifficulty();

            Assert.AreEqual(GameConstants.DEFAULT_SPEED * 0.7f, GameState.GetCharacterSpeed(), 0.001f);
        }

        [Test]
        public void ApplyDifficulty_Hard_ScalesSpeed()
        {
            var go = new GameObject("[DifficultyPresets_Test]");
            go.AddComponent<DifficultyPresets>();

            GameState.Init();
            DifficultyPresets.SetDifficulty(DifficultyLevel.Hard);
            DifficultyPresets.Instance.ApplyDifficulty();

            Assert.AreEqual(GameConstants.DEFAULT_SPEED * 1.3f, GameState.GetCharacterSpeed(), 0.001f);
        }

        [Test]
        public void LivesSystem_ResetLives_RestoresMaxForMedium()
        {
            var go = new GameObject("[LivesSystem_Test]");
            var lives = go.AddComponent<LivesSystem>();

            lives.ResetLives();
            Assert.AreEqual(3, lives.GetLives());
            Assert.AreEqual(3, lives.GetMaxLives());

            lives.LoseLife();
            Assert.AreEqual(2, lives.GetLives());

            lives.ResetLives();
            Assert.AreEqual(3, lives.GetLives());
        }

        [Test]
        public void LivesSystem_LoseLife_ReturnsFalseWhenExhausted()
        {
            PlayerPrefs.SetInt(GameConstants.PREF_DIFFICULTY, (int)DifficultyLevel.Hard);

            var go = new GameObject("[LivesSystem_Test]");
            var lives = go.AddComponent<LivesSystem>();
            lives.ResetLives();

            Assert.AreEqual(1, lives.GetLives());
            Assert.IsFalse(lives.LoseLife());
            Assert.AreEqual(0, lives.GetLives());
        }

        [Test]
        public void BeginRun_ResetsScoreAndFlags_WithoutGameManager()
        {
            GameState.SetScore(999);
            GameState.SetRunning(true);
            GameState.SetGameOver(true);
            _ = new Question();

            // BeginRun logs an error when GameManager is missing, but still
            // resets session state before attempting to load the scene.
            GameSession.BeginRun();

            Assert.AreEqual(0, GameState.GetScore());
            Assert.IsFalse(GameState.IsRunning());
            Assert.IsFalse(GameState.IsGameOver());
        }

        [Test]
        public void BeginRun_AppliesActiveChallengeSettings_WithoutGameManager()
        {
            string code = ChallengeCodeSystem.GenerateCode("division", (int)DifficultyLevel.Hard);
            ChallengeCodeSystem.SetActiveChallenge(code);
            GameState.SetQuestionType("addition");
            DifficultyPresets.SetDifficulty(DifficultyLevel.Easy);
            TimeAttackMode.SetTimeAttack(true);
            PlayerPrefs.SetInt(GameConstants.PREF_CAMPAIGN_ACTIVE, 1);

            GameSession.BeginRun();

            Assert.AreEqual("division", GameState.GetQuestionType());
            Assert.AreEqual(DifficultyLevel.Hard, DifficultyPresets.GetDifficulty());
            Assert.IsFalse(TimeAttackMode.IsTimeAttack());
            Assert.AreEqual(0, PlayerPrefs.GetInt(GameConstants.PREF_CAMPAIGN_ACTIVE));
        }
    }
}
