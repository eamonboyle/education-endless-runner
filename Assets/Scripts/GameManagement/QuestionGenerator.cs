using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionGenerator : MonoBehaviour
{
    public enum QuestionType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division
    }

    private bool questionExists = false;
    private QuestionType questionType;
    private int number1;
    private int number2;
    private int answer;

    // Start is called before the first frame update
    void Start()
    {
        questionType = QuestionType.Addition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!questionExists)
        {
            Debug.Log("GENERATE A QUESTION");

            number1 = Random.Range(0, 200);
            number2 = Random.Range(0, 200);
            answer = number1 + number2;

            Debug.Log(number1 + " + " + number2 + " = " + answer);

            questionExists = true;
        }
    }
}
