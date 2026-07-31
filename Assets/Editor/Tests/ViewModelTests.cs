using MathRunner.Data;
using MathRunner.UI.ViewModels;
using NUnit.Framework;
using UnityEngine;

namespace MathRunner.Tests
{
    [TestFixture]
    public class ViewModelTests
    {
        [SetUp]
        public void SetUp()
        {
            GameState.Init();
            PlayerPrefs.DeleteAll();
        }

        [Test]
        public void RunHudViewModel_ScoreDisplay_IncludesMultiplier()
        {
            var vm = new RunHudViewModel();
            int changed = 0;
            vm.Changed += () => changed++;

            vm.Score = 42;
            vm.Multiplier = 3;

            Assert.AreEqual("42  x3!", vm.ScoreDisplay);
            Assert.AreEqual("x3!", vm.ComboDisplay);
            Assert.GreaterOrEqual(changed, 2);
        }

        [Test]
        public void RunHudViewModel_LivesDisplay_UsesHearts()
        {
            var vm = new RunHudViewModel { MaxLives = 3, Lives = 2 };
            StringAssert.Contains("\u2665", vm.LivesDisplay);
            StringAssert.Contains("\u2661", vm.LivesDisplay);
        }

        [Test]
        public void RunHudViewModel_PowerUps_ClearRemoves()
        {
            var vm = new RunHudViewModel();
            vm.SetPowerUp(PowerUpType.Shield, 5f);
            Assert.IsTrue(vm.PowerUpTimers.ContainsKey(PowerUpType.Shield));
            vm.ClearPowerUps();
            Assert.AreEqual(0, vm.PowerUpTimers.Count);
        }

        [Test]
        public void ProgressionViewModel_Refresh_ReadsPlayerStats()
        {
            PlayerPrefs.SetInt(MathRunner.Core.GameConstants.PREF_GAMES_PLAYED, 5);
            var vm = new ProgressionViewModel();
            vm.RefreshFromPlayerStats();
            Assert.AreEqual(PlayerStats.GetTotalGamesPlayed(), vm.GamesPlayed);
            Assert.NotNull(vm.WeeklyScores);
            Assert.AreEqual(7, vm.WeeklyScores.Length);
        }

        [Test]
        public void GameState_OnScoreChanged_UpdatesValue()
        {
            int observed = -1;
            System.Action<int> handler = s => observed = s;
            GameState.OnScoreChanged += handler;
            try
            {
                GameState.SetScore(77);
                Assert.AreEqual(77, observed);
                Assert.AreEqual(77, GameState.GetScore());
            }
            finally
            {
                GameState.OnScoreChanged -= handler;
            }
        }
    }
}
