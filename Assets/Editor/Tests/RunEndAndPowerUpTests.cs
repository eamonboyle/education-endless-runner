using MathRunner.Core;
using MathRunner.Data;
using NUnit.Framework;
using UnityEngine;

namespace MathRunner.Tests
{
    [TestFixture]
    public class RunEndPipelineTests
    {
        [SetUp]
        public void SetUp()
        {
            PlayerPrefs.DeleteAll();
            GameState.Init();
            GameState.SetQuestionType("addition");
            GameState.StartGame();
            GameState.AddScore(100);
            GameState.RecordAnswer(true);
            GameState.RecordAnswer(true);
            GameState.RecordAnswer(false);
            RunEndPipeline.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            PlayerPrefs.DeleteAll();
            if (PowerUpSystem.Instance != null)
                Object.DestroyImmediate(PowerUpSystem.Instance.gameObject);
            if (ComboSystem.Instance != null)
                Object.DestroyImmediate(ComboSystem.Instance.gameObject);
            if (LeaderboardManager.Instance != null)
                Object.DestroyImmediate(LeaderboardManager.Instance.gameObject);
            if (OnlineLeaderboard.Instance != null)
                Object.DestroyImmediate(OnlineLeaderboard.Instance.gameObject);
        }

        [Test]
        public void Process_IsIdempotent()
        {
            int before = PlayerStats.GetTotalGamesPlayed();
            RunEndPipeline.Process();
            int afterFirst = PlayerStats.GetTotalGamesPlayed();
            RunEndPipeline.Process();
            int afterSecond = PlayerStats.GetTotalGamesPlayed();

            Assert.AreEqual(before + 1, afterFirst);
            Assert.AreEqual(afterFirst, afterSecond);
        }

        [Test]
        public void Process_WritesLastPlayedPrefs()
        {
            RunEndPipeline.Process();

            Assert.AreEqual("addition", PlayerPrefs.GetString(GameConstants.PREF_LAST_PLAYED_MODE));
            Assert.AreEqual(100, PlayerPrefs.GetInt(GameConstants.PREF_LAST_PLAYED_SCORE));
            Assert.AreEqual(2, PlayerPrefs.GetInt(GameConstants.PREF_LAST_CORRECT_ANSWERS));
        }

        [Test]
        public void Process_RecordsGamePlayed()
        {
            Assert.AreEqual(0, PlayerStats.GetTotalGamesPlayed());
            RunEndPipeline.Process();
            Assert.AreEqual(1, PlayerStats.GetTotalGamesPlayed());
            Assert.AreEqual(1, PlayerStats.GetGamesPlayed("addition"));
        }

        [Test]
        public void Process_SubmitsLeaderboardScore()
        {
            var leaderboardGo = new GameObject("[LeaderboardManager_Test]");
            leaderboardGo.AddComponent<LeaderboardManager>();
            var onlineGo = new GameObject("[OnlineLeaderboard_Test]");
            onlineGo.AddComponent<OnlineLeaderboard>();

            RunEndPipeline.Process();

            var entries = LeaderboardManager.Instance.GetTopScores("addition", 1);
            Assert.AreEqual(1, entries.Count);
            Assert.AreEqual(100, entries[0].Score);
            Assert.AreEqual("addition", entries[0].Mode);
        }

        [Test]
        public void Init_ResetsPipelineGuard()
        {
            RunEndPipeline.Process();
            GameState.Init();
            GameState.SetQuestionType("addition");
            GameState.StartGame();
            RunEndPipeline.Process();
            Assert.AreEqual(2, PlayerStats.GetTotalGamesPlayed());
        }
    }

    [TestFixture]
    public class PowerUpSystemTests
    {
        private GameObject powerUpGo;

        [SetUp]
        public void SetUp()
        {
            GameState.Init();
            GameState.StartGame();
            powerUpGo = new GameObject("[PowerUpSystem_Test]");
            powerUpGo.AddComponent<PowerUpSystem>();
        }

        [TearDown]
        public void TearDown()
        {
            if (powerUpGo != null)
                Object.DestroyImmediate(powerUpGo);
        }

        [Test]
        public void ActivateShield_CanBeConsumed()
        {
            PowerUpSystem.Instance.ActivatePowerUp(PowerUpType.Shield);
            Assert.IsTrue(PowerUpSystem.Instance.HasActivePowerUp(PowerUpType.Shield));
            Assert.IsTrue(PowerUpSystem.Instance.TryConsumeShield());
            Assert.IsFalse(PowerUpSystem.Instance.HasActivePowerUp(PowerUpType.Shield));
            Assert.IsFalse(PowerUpSystem.Instance.TryConsumeShield());
        }

        [Test]
        public void DoublePoints_DoublesScoreMultiplier()
        {
            Assert.AreEqual(1, PowerUpSystem.Instance.GetScoreMultiplier());
            PowerUpSystem.Instance.ActivatePowerUp(PowerUpType.DoublePoints);
            Assert.AreEqual(2, PowerUpSystem.Instance.GetScoreMultiplier());
        }

        [Test]
        public void SlowMotion_HalvesSpeedMultiplier()
        {
            Assert.AreEqual(1f, PowerUpSystem.Instance.GetSpeedMultiplier(), 0.001f);
            PowerUpSystem.Instance.ActivatePowerUp(PowerUpType.SlowMotion);
            Assert.AreEqual(0.5f, PowerUpSystem.Instance.GetSpeedMultiplier(), 0.001f);
        }

        [Test]
        public void Collectible_SetType_UpdatesProperty()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var col = go.GetComponent<Collider>();
            col.isTrigger = true;
            var collectible = go.AddComponent<PowerUpCollectible>();
            collectible.SetType(PowerUpType.DoublePoints);
            Assert.AreEqual(PowerUpType.DoublePoints, collectible.PowerUpType);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void BossQuestion_ShouldSpawnEveryTen()
        {
            Assert.IsFalse(BossQuestion.ShouldSpawnBoss(1));
            Assert.IsTrue(BossQuestion.ShouldSpawnBoss(10));
            Assert.IsTrue(BossQuestion.ShouldSpawnBoss(20));
            Assert.IsFalse(BossQuestion.ShouldSpawnBoss(11));
        }

        [Test]
        public void Question_FromBoss_HasThreeAnswers()
        {
            var q = new Question(new BossQuestion());
            Assert.IsFalse(string.IsNullOrEmpty(q.Text));
            Assert.AreEqual(3, q.Numbers.Count);
            Assert.Contains(q.Answer, q.Numbers);
        }
    }
}
