using System.Collections.Generic;
using UnityEngine;

public class TutorialQuestion
{
    public TutorialQuestion(int number1, int number2, int answer, int wrong1, int wrong2)
    {
        Text = number1.ToString() + " + " + number2.ToString();
        Number1 = number1;
        Number2 = number2;
        Answer = answer;
        Wrong1 = wrong1;
        Wrong2 = wrong2;
        zPosition = 0.0f;
        Numbers = RandomizeBoxPlacement(new List<int>() { answer, wrong1, wrong2 });

        correctLane = FindCorrectLane();
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

    public void SetZ(float z)
    {
        zPosition = z;
    }

    public void RandomizeList()
    {
        this.Numbers = RandomizeBoxPlacement(Numbers);
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

    public string Text;
    public int Number1;
    public int Number2;
    public int Answer;
    public int Wrong1;
    public int Wrong2;
    public float zPosition;
    public List<int> Numbers;
    public PlayerMovement.Lane correctLane;
}