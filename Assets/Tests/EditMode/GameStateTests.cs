using NUnit.Framework;
using UnityEngine;

public class GameStateTests
{
    [SetUp]
    public void SetUp()
    {
        PlayerPrefs.DeleteAll();
        GameState.Init();
    }

    [TearDown]
    public void TearDown()
    {
        PlayerPrefs.DeleteAll();
    }

    [Test]
    public void SetHighScore_ShouldPersistHighestScorePerQuestionMode()
    {
        GameState.SetQuestionType("addition");

        GameState.SetScore(10);
        GameState.SetHighScore();
        Assert.AreEqual(10, GameState.GetHighScore());

        GameState.SetScore(8);
        GameState.SetHighScore();
        Assert.AreEqual(10, GameState.GetHighScore());

        GameState.SetScore(12);
        GameState.SetHighScore();
        Assert.AreEqual(12, GameState.GetHighScore());
    }

    [Test]
    public void IsFirstLoad_ShouldSeedDefaultSettingsWhenFlagIsUnset()
    {
        bool isFirstLoad = GameState.IsFirstLoad();

        Assert.IsTrue(isFirstLoad);
        Assert.IsTrue(SettingState.GetSound());
        Assert.AreEqual("Medium", SettingState.GetGraphics());
    }
}
