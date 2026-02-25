using NUnit.Framework;

namespace MathRunner.Tests
{
    [TestFixture]
    public class GameStateTests
    {
        [SetUp]
        public void SetUp()
        {
            GameState.Init();
        }

        [Test]
        public void Init_ResetsScore()
        {
            GameState.SetScore(100);
            GameState.Init();
            Assert.AreEqual(0, GameState.GetScore());
        }

        [Test]
        public void Init_ResetsRunning()
        {
            GameState.SetRunning(true);
            GameState.Init();
            Assert.IsFalse(GameState.IsRunning());
        }

        [Test]
        public void Init_ResetsGameOver()
        {
            GameState.SetGameOver(true);
            GameState.Init();
            Assert.IsFalse(GameState.IsGameOver());
        }

        [Test]
        public void StartGame_SetsRunning()
        {
            GameState.StartGame();
            Assert.IsTrue(GameState.IsRunning());
            Assert.IsFalse(GameState.IsGameOver());
        }

        [Test]
        public void GameOver_StopsRunning()
        {
            GameState.StartGame();
            GameState.GameOver();
            Assert.IsFalse(GameState.IsRunning());
            Assert.IsTrue(GameState.IsGameOver());
        }

        [Test]
        public void SetGameOver_RespectsParameter()
        {
            GameState.SetGameOver(true);
            Assert.IsTrue(GameState.IsGameOver());
            GameState.SetGameOver(false);
            Assert.IsFalse(GameState.IsGameOver());
        }

        [Test]
        public void AddScore_IncreasesScore()
        {
            GameState.StartGame();
            GameState.AddScore(10);
            Assert.AreEqual(10, GameState.GetScore());
            GameState.AddScore(5);
            Assert.AreEqual(15, GameState.GetScore());
        }

        [Test]
        public void RecordAnswer_TracksCorrectly()
        {
            GameState.StartGame();
            GameState.RecordAnswer(true);
            GameState.RecordAnswer(true);
            GameState.RecordAnswer(false);
            Assert.AreEqual(3, GameState.GetQuestionsAnsweredThisGame());
            Assert.AreEqual(2, GameState.GetCorrectAnswersThisGame());
        }

        [Test]
        public void Accuracy_CalculatesCorrectly()
        {
            GameState.StartGame();
            GameState.RecordAnswer(true);
            GameState.RecordAnswer(false);
            Assert.AreEqual(50f, GameState.GetAccuracyThisGame(), 0.01f);
        }

        [Test]
        public void Accuracy_ZeroWhenNoQuestions()
        {
            GameState.StartGame();
            Assert.AreEqual(0f, GameState.GetAccuracyThisGame());
        }
    }
}
