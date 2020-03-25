using System.Collections.Generic;

public class Question
{
    public int Answer { get; private set; }
    public int Number1 { get; private set; }
    public int Number2 { get; private set; }
    public List<int> Numbers { get; private set; }
    public string Text { get; private set; }
    public int Wrong1 { get; private set; }
    public int Wrong2 { get; private set; }
    public float ZPosition { get; private set; }

    public PlayerMovement.Lane correctLane;

    private char questionSymbol;

    private QuestionType questionType;

    public Question()
    {
        GetQuestionType();
        CreateQuestion();

        correctLane = FindCorrectLane();
    }

    public enum QuestionType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division
    }

    public void RandomizeList()
    {
        this.Numbers = RandomizeBoxPlacement(Numbers);
    }

    public void SetZ(float z)
    {
        ZPosition = z;
    }

    private void CreateQuestion()
    {
        GenerateNumbers();
        Numbers = RandomizeBoxPlacement(new List<int>() { Answer, Wrong1, Wrong2 });
    }

    private void GenerateNumbers()
    {
        // change the range to a number based on scores
        int score = GameState.GetScore();

        if (score == 0)
        {
            score = 5;
        }

        int number1 = UnityEngine.Random.Range(1, score);
        int number2 = UnityEngine.Random.Range(1, score);
        int answer = GetAnswer(number1, number2);

        int wrong1;

        do
        {
            wrong1 = UnityEngine.Random.Range(answer - 5, answer + 5);
        } while (wrong1 == answer);

        int wrong2;

        do
        {
            wrong2 = UnityEngine.Random.Range(answer - 5, answer + 5);
        } while (wrong2 == answer || wrong2 == wrong1);

        Text = number1.ToString() + " " + questionSymbol + " " + number2.ToString();
        Number1 = number1;
        Number2 = number2;
        Answer = answer;
        Wrong1 = wrong1;
        Wrong2 = wrong2;
        ZPosition = 0.0f;
    }

    private int GetAnswer(int number1, int number2)
    {
        int answer = number1 + number2;

        if (questionType == QuestionType.Subtraction)
        {
            answer = number1 - number2;
        }
        else if (questionType == QuestionType.Multiplication)
        {
            answer = number1 * number2;
        }
        else if (questionType == QuestionType.Division)
        {
            answer = number1 / number2;
        }

        return answer;
    }

    private PlayerMovement.Lane FindCorrectLane()
    {
        // find index of answer in Numbers
        int index = Numbers.IndexOf(Answer);

        switch (index)
        {
            case 0:
                return PlayerMovement.Lane.Left;

            case 1:
                return PlayerMovement.Lane.Center;

            case 2:
                return PlayerMovement.Lane.Right;

            default:
                return PlayerMovement.Lane.Center;
        }
    }

    private void GetQuestionType()
    {
        // get player prefs for now on which question type
        string mode = GameState.GetQuestionType();

        switch (mode)
        {
            case "addition":
                questionType = QuestionType.Addition;
                questionSymbol = '+';
                break;

            case "subtraction":
                questionType = QuestionType.Subtraction;
                questionSymbol = '-';
                break;

            case "multiply":
                questionType = QuestionType.Multiplication;
                questionSymbol = 'x';
                break;

            case "division":
                questionType = QuestionType.Division;
                questionSymbol = '÷';
                break;

            default:
                questionType = QuestionType.Addition;
                questionSymbol = '+';
                break;
        }
    }

    private List<int> RandomizeBoxPlacement(List<int> numbers)
    {
        for (int i = 0; i < numbers.Count; i++)
        {
            int temp = numbers[i];
            int randomIndex = UnityEngine.Random.Range(i, numbers.Count);
            numbers[i] = numbers[randomIndex];
            numbers[randomIndex] = temp;
        }

        return numbers;
    }

    private List<int> Swap(List<int> list, int i, int j)
    {
        List<int> returnList = list;
        var temp = returnList[i];
        returnList[i] = returnList[j];
        returnList[j] = temp;

        return returnList;
    }
}