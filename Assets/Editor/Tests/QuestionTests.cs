using NUnit.Framework;
using System.Collections.Generic;

namespace MathRunner.Tests
{
    [TestFixture]
    public class QuestionTests
    {
        [SetUp]
        public void SetUp()
        {
            GameState.Init();
            GameState.SetQuestionType("addition");
        }

        [Test]
        public void Addition_AnswerIsCorrect()
        {
            GameState.SetQuestionType("addition");
            var question = new Question();
            Assert.AreEqual(question.Number1 + question.Number2, question.Answer);
        }

        [Test]
        public void Subtraction_AnswerIsCorrect()
        {
            GameState.SetQuestionType("subtraction");
            var question = new Question();
            Assert.AreEqual(question.Number1 - question.Number2, question.Answer);
        }

        [Test]
        public void Multiplication_AnswerIsCorrect()
        {
            GameState.SetQuestionType("multiply");
            var question = new Question();
            Assert.AreEqual(question.Number1 * question.Number2, question.Answer);
        }

        [Test]
        public void Division_AnswerIsCorrect()
        {
            GameState.SetQuestionType("division");
            var question = new Question();
            Assert.AreEqual(question.Number1 / question.Number2, question.Answer);
        }

        [Test]
        public void Division_NoDivisionByZero()
        {
            GameState.SetQuestionType("division");
            for (int i = 0; i < 100; i++)
            {
                var question = new Question();
                Assert.AreNotEqual(0, question.Number2);
            }
        }

        [Test]
        public void Division_ResultIsWholeNumber()
        {
            GameState.SetQuestionType("division");
            for (int i = 0; i < 100; i++)
            {
                var question = new Question();
                Assert.AreEqual(0, question.Number1 % question.Number2);
            }
        }

        [Test]
        public void WrongAnswers_DifferFromCorrect()
        {
            string[] modes = { "addition", "subtraction", "multiply", "division" };
            foreach (string mode in modes)
            {
                GameState.SetQuestionType(mode);
                var question = new Question();
                Assert.AreNotEqual(question.Answer, question.Wrong1);
                Assert.AreNotEqual(question.Answer, question.Wrong2);
                Assert.AreNotEqual(question.Wrong1, question.Wrong2);
            }
        }

        [Test]
        public void Numbers_ContainsThreeEntries()
        {
            var question = new Question();
            Assert.AreEqual(3, question.Numbers.Count);
        }

        [Test]
        public void Numbers_ContainsCorrectAnswer()
        {
            var question = new Question();
            Assert.IsTrue(question.Numbers.Contains(question.Answer));
        }

        [Test]
        public void Mixed_GeneratesValidQuestions()
        {
            GameState.SetQuestionType("mixed");
            for (int i = 0; i < 50; i++)
            {
                var question = new Question();
                Assert.IsNotNull(question.Text);
                Assert.AreEqual(3, question.Numbers.Count);
                Assert.IsTrue(question.Numbers.Contains(question.Answer));
            }
        }

        [Test]
        public void HighScore_QuestionsStillGenerate()
        {
            GameState.SetScore(5000);
            string[] modes = { "addition", "subtraction", "multiply", "division" };
            foreach (string mode in modes)
            {
                GameState.SetQuestionType(mode);
                var question = new Question();
                Assert.AreEqual(3, question.Numbers.Count);
            }
        }
    }
}
