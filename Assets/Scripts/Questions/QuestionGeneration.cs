using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionGeneration : MonoBehaviour
{
    public enum QuestionType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division
    }

    public GameObject player;
    public GameObject questionBox;
    public GameObject questionText;

    public List<GameObject> questionBoxes;
    public List<Question> questions;

    private float questionSpacing = 50.0f;
    private float boxZ = -1.586f;
    private float boxHeight = 1.3f;

    private int amountOfQuestions = 3;

    private char questionSymbol;

    private PlayerMovement playerMovement;
    private QuestionType questionType;

    private void Start()
    {
        Debug.Log("Start Question Generator");

        playerMovement = player.GetComponent<PlayerMovement>();
        questions = new List<Question>();

        GetQuestionType();
        PreloadQuestions();
    }

    private void Update()
    {
        if (!GameState.IsRunning())
        {
            return;
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

    private void PreloadQuestions()
    {
        Debug.Log("Load 3 random questions ");

        for (int i = 0; i < amountOfQuestions; i++)
        {
            AddQuestion();
        }

        PlaceQuestionBoxes();
    }

    private void PlaceQuestionBoxes()
    {
        Debug.Log("Placing [" + questions.Count + "] questionBoxes");
    }

    private void AddQuestion()
    {
        Debug.Log("Add Question");

        Question question = new Question(1, 2, 3, 4, 5, Vector3.zero);
        questions.Add(question);
    }

    private void DeleteLastQuestion()
    {
        questions.RemoveAt(0);
    }
}