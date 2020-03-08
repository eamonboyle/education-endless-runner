using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialQuestions : MonoBehaviour
{
    public GameObject player;
    public GameObject questionBox;
    public GameObject questionText;
    public GameObject tutorialText;

    public List<GameObject> questionBoxes;
    public List<Question> tutorialQuestions;

    public int currentQuestion = 0;

    private float questionSpacing = 50.0f;
    private float tutorialAt = 40.0f;
    private float boxZ = -1.586f;
    private float boxHeight = 1.3f;

    private PlayerMovement movement;

    void Start()
    {
        Debug.Log("LOAD TUTORIAL QUESTIONS");

        movement = player.GetComponent<PlayerMovement>();

        tutorialQuestions = new List<Question>();
        
        tutorialQuestions.Add(new Question(1, 2, 3, 5, 6));
        tutorialQuestions.Add(new Question(2, 3, 5, 7, 4));
        tutorialQuestions.Add(new Question(3, 3, 6, 5, 7));

        InstantiateQuestionBoxes();
    }

    private void Update()
    {
        if (currentQuestion > tutorialQuestions.Count - 1)
        {
            return;
        }

        questionText.GetComponent<Text>().text = tutorialQuestions[currentQuestion].Text;

        float playerZ = player.transform.position.z;
        float questionZ = tutorialQuestions[currentQuestion].zPosition;

        // help the player out
        if (movement.currentLane != tutorialQuestions[currentQuestion].correctLane)
        {
            if (playerZ >= (questionZ - tutorialAt))
            {
                if (tutorialQuestions[currentQuestion].correctLane == PlayerMovement.Lane.Left)
                {
                    tutorialText.GetComponent<Text>().text = "Swipe Left";
                }
                else if (tutorialQuestions[currentQuestion].correctLane == PlayerMovement.Lane.Right)
                {
                    tutorialText.GetComponent<Text>().text = "Swipe right";
                }
                else
                {
                    if (movement.currentLane == PlayerMovement.Lane.Left)
                    {
                        tutorialText.GetComponent<Text>().text = "Swipe right";
                    }
                    else
                    {
                        tutorialText.GetComponent<Text>().text = "Swipe Left";
                    }
                }

                tutorialText.SetActive(true);
            }
        }
        else
        {
            tutorialText.SetActive(false);
        }
    }

    private void InstantiateQuestionBoxes()
    {
        int i = 1;

        foreach (Question question in tutorialQuestions)
        {
            Debug.Log("Question: " + i);
            Debug.Log("Correct Lane is: " + question.correctLane);

            // set the position of the question boxes along the z axis
            float spawnZ = player.transform.position.z + (i * questionSpacing);
            question.SetZ(spawnZ);

            // left
            GameObject leftBox = Instantiate(questionBox, new Vector3(boxZ, boxHeight, question.zPosition), Quaternion.identity, null);
            leftBox.GetComponent<TutorialQuestionBox>().number = question.Numbers[0];
            leftBox.GetComponent<TutorialQuestionBox>().correctNumber = question.Answer;
            leftBox.GetComponentInChildren<TextMeshPro>().text = leftBox.GetComponent<TutorialQuestionBox>().number.ToString();

            // center
            GameObject centerBox = Instantiate(questionBox, new Vector3(0f, boxHeight, question.zPosition), Quaternion.identity, null);
            centerBox.GetComponent<TutorialQuestionBox>().number = question.Numbers[1];
            centerBox.GetComponent<TutorialQuestionBox>().correctNumber = question.Answer;
            centerBox.GetComponentInChildren<TextMeshPro>().text = centerBox.GetComponent<TutorialQuestionBox>().number.ToString();

            // right
            GameObject rightBox = Instantiate(questionBox, new Vector3(Math.Abs(boxZ), boxHeight, question.zPosition), Quaternion.identity, null);
            rightBox.GetComponent<TutorialQuestionBox>().number = question.Numbers[2];
            rightBox.GetComponent<TutorialQuestionBox>().correctNumber = question.Answer;
            rightBox.GetComponentInChildren<TextMeshPro>().text = rightBox.GetComponent<TutorialQuestionBox>().number.ToString();

            questionBoxes.Add(rightBox);
            questionBoxes.Add(leftBox);
            questionBoxes.Add(centerBox);

            i++;
        }
    }
}

public class Question
{
    public Question(int number1, int number2, int answer, int wrong1, int wrong2)
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

        //System.Random rnd = new System.Random();

        //for (int i = numbers.Count; i > 0; i--)
        //{
        //    numbers = Swap(numbers, 0, rnd.Next(0, i));
        //}

        //return numbers;
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