using System;
using System.Collections.Generic;

public class Question
{
    public PlayerMovement.Lane correctLane;
    private char questionSymbol;
    private QuestionType questionType;
    private int score = 0;

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

    public int Answer { get; private set; }
    public int Number1 { get; private set; }
    public int Number2 { get; private set; }
    public List<int> Numbers { get; private set; }
    public string Text { get; private set; }
    public int Wrong1 { get; private set; }
    public int Wrong2 { get; private set; }
    public float ZPosition { get; private set; }

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

    private void GenerateNumbers()
    {
        // change the range to a number based on scores
        score = GameState.GetScore();

        if (score == 0)
        {
            score = 5;
        }

        int number1 = 0, number2 = 0, answer = 0, wrong1 = 0, wrong2 = 0;

        if (questionType == QuestionType.Addition)
        {
            (int, int, int, int, int) numbers = GetNumbers_Addition();
            number1 = numbers.Item1;
            number2 = numbers.Item2;
            answer = numbers.Item3;
            wrong1 = numbers.Item4;
            wrong2 = numbers.Item5;
        }

        if (questionType == QuestionType.Subtraction)
        {
            (int, int, int, int, int) numbers = GetNumbers_Subtraction();
            number1 = numbers.Item1;
            number2 = numbers.Item2;
            answer = numbers.Item3;
            wrong1 = numbers.Item4;
            wrong2 = numbers.Item5;
        }

        if (questionType == QuestionType.Multiplication)
        {
            (int, int, int, int, int) numbers = GetNumbers_Multiplication();

            number1 = UnityEngine.Random.Range(1, score);
            number2 = UnityEngine.Random.Range(1, score);
            answer = GetAnswer(number1, number2);

            do
            {
                wrong1 = UnityEngine.Random.Range(answer - 5, answer + 5);
            } while (wrong1 == answer);

            do
            {
                wrong2 = UnityEngine.Random.Range(answer - 5, answer + 5);
            } while (wrong2 == answer || wrong2 == wrong1);
        }

        if (questionType == QuestionType.Division)
        {
            (int, int, int, int, int) numbers = GetNumbers_Division();

            number1 = UnityEngine.Random.Range(1, score);
            number2 = UnityEngine.Random.Range(1, score);
            answer = GetAnswer(number1, number2);

            do
            {
                wrong1 = UnityEngine.Random.Range(answer - 5, answer + 5);
            } while (wrong1 == answer);

            do
            {
                wrong2 = UnityEngine.Random.Range(answer - 5, answer + 5);
            } while (wrong2 == answer || wrong2 == wrong1);
        }

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

    private (int, int, int, int, int) GetNumbers_Addition()
    {
        int maxNumber = 10;
        int wrongRange = 0;
        int number1 = 0;
        int number2 = 0;
        int wrong1 = 0;
        int wrong2 = 0;
        int answer = 0;

        if (score < 100)
        {
            maxNumber = 10;
            wrongRange = 0;
        }
        else if (score < 200)
        {
            maxNumber = 20;
            wrongRange = 10;
        }
        else if (score < 300)
        {
            maxNumber = 30;
            wrongRange = 10;
        }
        else if (score < 400)
        {
            maxNumber = 40;
            wrongRange = 10;
        }
        else if (score < 500)
        {
            maxNumber = 50;
            wrongRange = 10;
        }
        else if (score < 600)
        {
            maxNumber = 60;
            wrongRange = 10;
        }
        else if (score < 700)
        {
            maxNumber = 70;
            wrongRange = 7;
        }
        else if (score < 800)
        {
            maxNumber = 80;
            wrongRange = 6;
        }
        else if (score < 900)
        {
            maxNumber = 90;
            wrongRange = 5;
        }
        else
        {
            maxNumber = score / 10;
            wrongRange = 4;
        }

        number1 = UnityEngine.Random.Range(1, maxNumber);
        number2 = UnityEngine.Random.Range(1, maxNumber);

        answer = GetAnswer(number1, number2);

        do
        {
            if (wrongRange != 0)
            {
                wrong1 = UnityEngine.Random.Range(answer - wrongRange, answer + wrongRange);
            }
            else
            {
                wrong1 = UnityEngine.Random.Range(1, answer);
            }
        } while (wrong1 == answer || wrong1 < 0);

        do
        {
            if (wrongRange != 0)
            {
                wrong2 = UnityEngine.Random.Range(answer - wrongRange, answer + wrongRange);
            }
            else
            {
                wrong2 = UnityEngine.Random.Range(1, answer);
            }
        } while (wrong2 == answer || wrong2 == wrong1 || wrong2 < 0);


        return (number1, number2, answer, wrong1, wrong2);
    }
    private (int, int, int, int, int) GetNumbers_Division()
    {
        return (0, 0, 0, 0, 0);
    }

    private (int, int, int, int, int) GetNumbers_Multiplication()
    {
        return (0, 0, 0, 0, 0);
    }

    private (int, int, int, int, int) GetNumbers_Subtraction()
    {
        int minNumberA = 0, maxNumberA = 0, minNumberB = 0, maxNumberB = 0, wrongRange = 0, number1 = 0, number2 = 0, wrong1 = 0, wrong2 = 0, answer = 0;

        if (score < 100)
        {
            minNumberA = 10;
            maxNumberA = 20;
            minNumberB = 1;
            maxNumberB = 9;
            wrongRange = 0;
        }
        else if (score < 200)
        {
            minNumberA = 15;
            maxNumberA = 25;
            minNumberB = 5;
            maxNumberB = 14;
            wrongRange = 10;
        }
        else if (score < 300)
        {
            minNumberA = 20;
            maxNumberA = 30;
            minNumberB = 5;
            maxNumberB = 19;
            wrongRange = 8;
        }
        else if (score < 400)
        {
            minNumberA = 25;
            maxNumberA = 40;
            minNumberB = 10;
            maxNumberB = 24;
            wrongRange = 6;
        }
        else if (score < 500)
        {
            minNumberA = 30;
            maxNumberA = 50;
            minNumberB = 15;
            maxNumberB = 29;
            wrongRange = 5;
        }
        else if (score < 600)
        {
            minNumberA = 35;
            maxNumberA = 60;
            minNumberB = 20;
            maxNumberB = 59;
            wrongRange = 3;
        }
        else
        {
            minNumberA = 40;
            maxNumberA = 150;
            minNumberB = 1;
            maxNumberB = 39;
            wrongRange = 5;
        }

        number1 = UnityEngine.Random.Range(minNumberA, maxNumberA);
        number2 = UnityEngine.Random.Range(minNumberB, maxNumberB);

        answer = GetAnswer(number1, number2);

        do
        {
            if (wrongRange != 0)
            {
                wrong1 = UnityEngine.Random.Range(answer - wrongRange, answer + wrongRange);
            }
            else
            {
                wrong1 = UnityEngine.Random.Range(1, maxNumberA);
            }
        } while (wrong1 == answer);

        do
        {
            if (wrongRange != 0)
            {
                wrong2 = UnityEngine.Random.Range(answer - wrongRange, answer + wrongRange);
            }
            else
            {
                wrong2 = UnityEngine.Random.Range(1, maxNumberA);
            }
        } while (wrong2 == answer || wrong2 == wrong1);


        return (number1, number2, answer, wrong1, wrong2);
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