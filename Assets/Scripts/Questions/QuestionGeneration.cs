using System;
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

    private float questionSpacing = 50.0f;
    [SerializeField] private bool useQuestionBoxPooling = true;
    [SerializeField] private int prewarmQuestionBoxes = 9;

    private readonly Queue<GameObject> pooledQuestionBoxes = new Queue<GameObject>();

    public enum QuestionType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division
    }
    public void AddQuestion(bool removeLast = false)
    {
        //Debug.Log("Add Question");

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

            SpawnQuestionSet(question);

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

        SpawnQuestionSet(question);
    }

    private void Start()
    {
        Debug.Log("Start Question Generator");

        if (questionBoxes == null)
        {
            questionBoxes = new List<GameObject>();
        }
        else
        {
            questionBoxes.Clear();
        }

        questions = new List<Question>();
        PrewarmQuestionPool();

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

    public void ClearCurrentQuestionBoxes()
    {
        int boxesToClear = Mathf.Min(3, questionBoxes.Count);
        for (int i = 0; i < boxesToClear; i++)
        {
            ReleaseQuestionBox(questionBoxes[0]);
            questionBoxes.RemoveAt(0);
        }
    }

    private void SpawnQuestionSet(Question question)
    {
        GameObject leftBox = SpawnQuestionBox(boxZ, question, 0);
        GameObject centerBox = SpawnQuestionBox(0f, question, 1);
        GameObject rightBox = SpawnQuestionBox(Math.Abs(boxZ), question, 2);

        questionBoxes.Add(rightBox);
        questionBoxes.Add(leftBox);
        questionBoxes.Add(centerBox);
    }

    private GameObject SpawnQuestionBox(float xPosition, Question question, int numberIndex)
    {
        GameObject box = GetQuestionBox();
        box.transform.SetParent(questionBoxParent.transform, true);
        box.transform.SetPositionAndRotation(new Vector3(xPosition, boxHeight, question.ZPosition), Quaternion.identity);

        QuestionBox boxScript = box.GetComponent<QuestionBox>();
        boxScript.Initialize(this, question.Numbers[numberIndex], question.Answer);
        box.GetComponentInChildren<TextMeshPro>().text = question.Numbers[numberIndex].ToString();

        MeshRenderer rootRenderer = box.GetComponent<MeshRenderer>();
        if (rootRenderer != null)
        {
            rootRenderer.enabled = true;
        }

        if (box.transform.childCount > 0)
        {
            MeshRenderer childRenderer = box.transform.GetChild(0).GetComponent<MeshRenderer>();
            if (childRenderer != null)
            {
                childRenderer.enabled = true;
            }
        }

        return box;
    }

    private GameObject GetQuestionBox()
    {
        if (useQuestionBoxPooling && pooledQuestionBoxes.Count > 0)
        {
            GameObject pooledBox = pooledQuestionBoxes.Dequeue();
            pooledBox.SetActive(true);
            return pooledBox;
        }

        return Instantiate(questionBox, questionBoxParent.transform);
    }

    private void ReleaseQuestionBox(GameObject box)
    {
        if (!useQuestionBoxPooling)
        {
            Destroy(box);
            return;
        }

        box.SetActive(false);
        box.transform.SetParent(questionBoxParent.transform, true);
        pooledQuestionBoxes.Enqueue(box);
    }

    private void PrewarmQuestionPool()
    {
        if (!useQuestionBoxPooling)
        {
            return;
        }

        for (int i = 0; i < prewarmQuestionBoxes; i++)
        {
            GameObject pooledBox = Instantiate(questionBox, questionBoxParent.transform);
            pooledBox.SetActive(false);
            pooledQuestionBoxes.Enqueue(pooledBox);
        }
    }
}