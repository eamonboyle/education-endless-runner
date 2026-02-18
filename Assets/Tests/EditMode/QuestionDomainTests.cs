using NUnit.Framework;
using UnityEngine;

public class QuestionDomainTests
{
    [SetUp]
    public void SetUp()
    {
        PlayerPrefs.DeleteAll();
        GameState.Init();
        GameState.SetQuestionType("addition");
        GameState.SetScore(50);
    }

    [TearDown]
    public void TearDown()
    {
        PlayerPrefs.DeleteAll();
    }

    [Test]
    public void NewQuestion_ShouldContainExactlyThreeOptionsIncludingAnswer()
    {
        Question question = new Question();

        Assert.AreEqual(3, question.Numbers.Count);
        CollectionAssert.Contains(question.Numbers, question.Answer);
    }

    [Test]
    public void NewQuestion_CorrectLane_ShouldMatchAnswerIndex()
    {
        Question question = new Question();
        int answerIndex = question.Numbers.IndexOf(question.Answer);

        PlayerMovement.Lane expectedLane = answerIndex switch
        {
            0 => PlayerMovement.Lane.Left,
            1 => PlayerMovement.Lane.Center,
            2 => PlayerMovement.Lane.Right,
            _ => PlayerMovement.Lane.Center
        };

        Assert.AreEqual(expectedLane, question.correctLane);
    }

    [Test]
    public void NewQuestion_ShouldUseModeSpecificOperatorInText()
    {
        GameState.SetQuestionType("subtraction");
        Question question = new Question();

        StringAssert.Contains("-", question.Text);
    }
}
