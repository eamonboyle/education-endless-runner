using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionGeneration : MonoBehaviour
{
    public GameObject player;

    public GameObject questionBox;

    public List<GameObject> questionBoxes;

    public GameObject questionBoxParent;

    public List<Question> questions;

    public GameObject questionText;

    private int amountOfQuestions = 3;

    private float boxHeight = 1.3f;

    private float boxZ = -1.586f;

    private PlayerMovement playerMovement;

    private float questionSpacing = 50.0f;

    public enum QuestionType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division
    }
    public void AddQuestion(bool removeLast = false)
    {
        Debug.Log("Add Question");

        Question question = new Question();
        questions.Add(question);

        if (removeLast)
        {
            SpawnNewQuestion();
            DeleteLastQuestion();
        }
    }

    public void DeleteLastQuestion()
    {
        questions.RemoveAt(0);
    }

    private void PlaceInitialQuestionBoxes()
    {
        int i = 1;

        foreach (Question question in questions)
        {
            // set the position of the question boxes along the z axis
            float spawnZ = player.transform.position.z + (i * questionSpacing);
            question.SetZ(spawnZ);

            // left
            GameObject leftBox = Instantiate(questionBox, new Vector3(boxZ, boxHeight, question.ZPosition), Quaternion.identity, questionBoxParent.transform);
            leftBox.GetComponent<QuestionBox>().number = question.Numbers[0];
            leftBox.GetComponent<QuestionBox>().correctNumber = question.Answer;
            leftBox.GetComponentInChildren<TextMeshPro>().text = leftBox.GetComponent<QuestionBox>().number.ToString();

            // center
            GameObject centerBox = Instantiate(questionBox, new Vector3(0f, boxHeight, question.ZPosition), Quaternion.identity, questionBoxParent.transform);
            centerBox.GetComponent<QuestionBox>().number = question.Numbers[1];
            centerBox.GetComponent<QuestionBox>().correctNumber = question.Answer;
            centerBox.GetComponentInChildren<TextMeshPro>().text = centerBox.GetComponent<QuestionBox>().number.ToString();

            // right
            GameObject rightBox = Instantiate(questionBox, new Vector3(Math.Abs(boxZ), boxHeight, question.ZPosition), Quaternion.identity, questionBoxParent.transform);
            rightBox.GetComponent<QuestionBox>().number = question.Numbers[2];
            rightBox.GetComponent<QuestionBox>().correctNumber = question.Answer;
            rightBox.GetComponentInChildren<TextMeshPro>().text = rightBox.GetComponent<QuestionBox>().number.ToString();

            questionBoxes.Add(rightBox);
            questionBoxes.Add(leftBox);
            questionBoxes.Add(centerBox);

            i++;
        }
    }

    private void PreloadQuestions()
    {
        Debug.Log("Load 3 random questions ");

        for (int i = 0; i < amountOfQuestions; i++)
        {
            AddQuestion();
        }

        PlaceInitialQuestionBoxes();
    }

    private void SpawnNewQuestion()
    {
        Question question = questions[questions.Count - 1];

        // set the position of the question boxes along the z axis
        float spawnZ = questions[questions.Count - 2].ZPosition + questionSpacing;
        question.SetZ(spawnZ);

        // left
        GameObject leftBox = Instantiate(questionBox, new Vector3(boxZ, boxHeight, question.ZPosition), Quaternion.identity, questionBoxParent.transform);
        leftBox.GetComponent<QuestionBox>().number = question.Numbers[0];
        leftBox.GetComponent<QuestionBox>().correctNumber = question.Answer;
        leftBox.GetComponentInChildren<TextMeshPro>().text = leftBox.GetComponent<QuestionBox>().number.ToString();

        // center
        GameObject centerBox = Instantiate(questionBox, new Vector3(0f, boxHeight, question.ZPosition), Quaternion.identity, questionBoxParent.transform);
        centerBox.GetComponent<QuestionBox>().number = question.Numbers[1];
        centerBox.GetComponent<QuestionBox>().correctNumber = question.Answer;
        centerBox.GetComponentInChildren<TextMeshPro>().text = centerBox.GetComponent<QuestionBox>().number.ToString();

        // right
        GameObject rightBox = Instantiate(questionBox, new Vector3(Math.Abs(boxZ), boxHeight, question.ZPosition), Quaternion.identity, questionBoxParent.transform);
        rightBox.GetComponent<QuestionBox>().number = question.Numbers[2];
        rightBox.GetComponent<QuestionBox>().correctNumber = question.Answer;
        rightBox.GetComponentInChildren<TextMeshPro>().text = rightBox.GetComponent<QuestionBox>().number.ToString();

        questionBoxes.Add(rightBox);
        questionBoxes.Add(leftBox);
        questionBoxes.Add(centerBox);
    }

    private void Start()
    {
        Debug.Log("Start Question Generator");

        playerMovement = player.GetComponent<PlayerMovement>();
        questions = new List<Question>();

        PreloadQuestions();
    }

    private void Update()
    {
        if (!GameState.IsRunning())
        {
            return;
        }

        questionText.GetComponent<Text>().text = questions[0].Text;
    }
}