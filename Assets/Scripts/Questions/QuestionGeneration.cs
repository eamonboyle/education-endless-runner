using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// the new generation class
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

    private int questionsSpawnedThisRun;
    private string lastSpokenQuestion;

    public enum QuestionType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division
    }
    public void AddQuestion(bool removeOldest = false)
    {
        questionsSpawnedThisRun++;
        Question question;
        if (BossQuestion.ShouldSpawnBoss(questionsSpawnedThisRun))
            question = new Question(new BossQuestion());
        else
            question = new Question();

        questions.Add(question);

        if (removeOldest)
        {
            SpawnNewQuestion();
            // Remove the question whose boxes were just destroyed. Must happen
            // exactly once per answered row or the HUD (questions[0].Text) drifts
            // ahead of the boxes still on the track.
            DeleteOldestQuestion();
        }
    }

    /// <summary>Removes the front question (index 0) from the buffer.</summary>
    public void DeleteOldestQuestion()
    {
        if (questions.Count > 0)
            questions.RemoveAt(0);
    }

    // Kept for any remaining callers; prefer DeleteOldestQuestion.
    public void DeleteLastQuestion()
    {
        DeleteOldestQuestion();
    }

    private void PlaceInitialQuestionBoxes()
    {
        int i = 1;

        foreach (Question question in questions)
        {
            float spawnZ = player.transform.position.z + (i * questionSpacing);
            question.SetZ(spawnZ);
            SpawnBoxesForQuestion(question);
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
        float spawnZ;
        if (questions.Count >= 2)
            spawnZ = questions[questions.Count - 2].ZPosition + questionSpacing;
        else if (player != null)
            spawnZ = player.transform.position.z + questionSpacing;
        else
            spawnZ = questionSpacing;

        question.SetZ(spawnZ);

        SpawnBoxesForQuestion(question);
    }

    private void SpawnBoxesForQuestion(Question question)
    {
        GameObject leftBox = Instantiate(questionBox, new Vector3(boxZ, boxHeight, question.ZPosition), Quaternion.identity, questionBoxParent.transform);
        leftBox.GetComponent<QuestionBox>().number = question.Numbers[0];
        leftBox.GetComponent<QuestionBox>().correctNumber = question.Answer;
        leftBox.GetComponent<QuestionBox>().questionText = question.Text;
        leftBox.GetComponentInChildren<TextMeshPro>().text = leftBox.GetComponent<QuestionBox>().number.ToString();

        GameObject centerBox = Instantiate(questionBox, new Vector3(0f, boxHeight, question.ZPosition), Quaternion.identity, questionBoxParent.transform);
        centerBox.GetComponent<QuestionBox>().number = question.Numbers[1];
        centerBox.GetComponent<QuestionBox>().correctNumber = question.Answer;
        centerBox.GetComponent<QuestionBox>().questionText = question.Text;
        centerBox.GetComponentInChildren<TextMeshPro>().text = centerBox.GetComponent<QuestionBox>().number.ToString();

        GameObject rightBox = Instantiate(questionBox, new Vector3(Math.Abs(boxZ), boxHeight, question.ZPosition), Quaternion.identity, questionBoxParent.transform);
        rightBox.GetComponent<QuestionBox>().number = question.Numbers[2];
        rightBox.GetComponent<QuestionBox>().correctNumber = question.Answer;
        rightBox.GetComponent<QuestionBox>().questionText = question.Text;
        rightBox.GetComponentInChildren<TextMeshPro>().text = rightBox.GetComponent<QuestionBox>().number.ToString();

        questionBoxes.Add(leftBox);
        questionBoxes.Add(centerBox);
        questionBoxes.Add(rightBox);
    }

    /// <summary>
    /// Rebuilds the question buffer and answer boxes ahead of the player.
    /// Call after Continue from game over so HUD text and lane answers match again.
    /// </summary>
    public void ResyncAfterContinue()
    {
        ClearWorldBoxes();

        if (questions == null)
            questions = new List<Question>();
        else
            questions.Clear();

        if (questionBoxes == null)
            questionBoxes = new List<GameObject>();
        else
            questionBoxes.Clear();

        for (int i = 0; i < amountOfQuestions; i++)
            AddQuestion();

        PlaceInitialQuestionBoxes();

        if (questionText != null)
        {
            var text = questionText.GetComponent<Text>();
            if (text != null && questions.Count > 0)
                text.text = questions[0].Text;
        }
    }

    private void ClearWorldBoxes()
    {
        if (questionBoxes != null)
        {
            for (int i = 0; i < questionBoxes.Count; i++)
            {
                if (questionBoxes[i] != null)
                    Destroy(questionBoxes[i]);
            }
            questionBoxes.Clear();
        }

        // Catch any orphans still tagged in the scene.
        foreach (GameObject box in GameObject.FindGameObjectsWithTag("QuestionBox"))
        {
            if (box != null)
                Destroy(box);
        }
    }

    private void Start()
    {
        Debug.Log("Start Question Generator");

        playerMovement = player.GetComponent<PlayerMovement>();
        questions = new List<Question>();
        if (questionBoxes == null)
            questionBoxes = new List<GameObject>();
        else
            questionBoxes.Clear();

        PreloadQuestions();
    }

    private void Update()
    {
        if (!GameState.IsRunning())
            return;

        if (questionText == null || questions == null || questions.Count == 0)
            return;

        var text = questionText.GetComponent<Text>();
        if (text != null)
        {
            string next = questions[0].Text;
            text.text = next;
            ApplyTextScale(text);
            if (next != lastSpokenQuestion)
            {
                lastSpokenQuestion = next;
                if (TextToSpeechManager.Instance != null)
                    TextToSpeechManager.Instance.SpeakQuestion(next);
            }
        }
    }

    private static void ApplyTextScale(Text text)
    {
        if (AccessibilityManager.Instance == null) return;
        float scale = AccessibilityManager.Instance.GetTextScale();
        text.fontSize = Mathf.RoundToInt(40f * scale);
        if (AccessibilityManager.Instance.HighContrastMode)
            text.color = Color.white;
    }
}