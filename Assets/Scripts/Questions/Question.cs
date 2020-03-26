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

    private int score = 0;

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
        score = GameState.GetScore();

        if (score == 0)
        {
            score = 5;
        }

        int number1 = 0, number2 = 0, answer = 0, wrong1 = 0, wrong2 = 0;

        (int, int) rangeNumbers = GetNumberRanges();

        if (questionType == QuestionType.Addition)
        {
            number1 = UnityEngine.Random.Range(1, rangeNumbers.Item1);
            number2 = UnityEngine.Random.Range(1, rangeNumbers.Item1);

            answer = GetAnswer(number1, number2);

            do
            {
                if (rangeNumbers.Item2 != 0)
                {
                    wrong1 = UnityEngine.Random.Range(answer - rangeNumbers.Item2, answer + rangeNumbers.Item2);
                }
                else
                {
                    wrong1 = UnityEngine.Random.Range(1, answer);
                }
            } while (wrong1 == answer || wrong1 < 0);

            do
            {
                if (rangeNumbers.Item2 != 0)
                {
                    wrong2 = UnityEngine.Random.Range(answer - rangeNumbers.Item2, answer + rangeNumbers.Item2);
                }
                else
                {
                    wrong2 = UnityEngine.Random.Range(1, answer);
                }
            } while (wrong2 == answer || wrong2 == wrong1 || wrong2 < 0);
        }





        if (questionType == QuestionType.Subtraction)
        {
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






        if (questionType == QuestionType.Multiplication)
        {
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

        (int, int) test = GetNumberRanges();
    }

    private (int a, int b) GetNumberRanges()
    {
        (int, int) returnVal = (0, 0);

        if (questionType == QuestionType.Addition)
        {
            if (score < 50)
            {
                returnVal.Item1 = 5;
                returnVal.Item2 = 0;
            }
            else if (score < 100)
            {
                returnVal.Item1 = 10;
                returnVal.Item2 = 0;
            }
            else if (score < 200)
            {
                returnVal.Item1 = 20;
                returnVal.Item2 = 10;
            }
            else if (score < 300)
            {
                returnVal.Item1 = 30;
                returnVal.Item2 = 10;
            }
            else if (score < 400)
            {
                returnVal.Item1 = 40;
                returnVal.Item2 = 10;
            }
            else if (score < 500)
            {
                returnVal.Item1 = 50;
                returnVal.Item2 = 10;
            }
            else if (score < 600)
            {
                returnVal.Item1 = 60;
                returnVal.Item2 = 10;
            }
            else if (score < 700)
            {
                returnVal.Item1 = 70;
                returnVal.Item2 = 7;
            }
            else if (score < 800)
            {
                returnVal.Item1 = 80;
                returnVal.Item2 = 6;
            }
            else if (score < 900)
            {
                returnVal.Item1 = 90;
                returnVal.Item2 = 5;
            }
            else
            {
                returnVal.Item1 = score / 10;
                returnVal.Item2 = 4;
            }
        }
        else if (questionType == QuestionType.Addition)
        {

        }
        else if (questionType == QuestionType.Addition)
        {

        }
        else if (questionType == QuestionType.Addition)
        {

        }

        return returnVal;
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