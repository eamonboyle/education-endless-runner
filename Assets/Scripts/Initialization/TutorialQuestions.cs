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
    public GameObject swipeImage;

    public Sprite leftImage;
    public Sprite rightImage;

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
        
        tutorialQuestions.Add(new Question(1, 2, 3, 5, 6, Vector3.zero));
        tutorialQuestions.Add(new Question(2, 3, 5, 7, 4, Vector3.zero));
        tutorialQuestions.Add(new Question(3, 3, 6, 5, 7, Vector3.zero));

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
                    SetSwipeHint();
                }
                else if (tutorialQuestions[currentQuestion].correctLane == PlayerMovement.Lane.Right)
                {
                    SetSwipeHint(false);
                }
                else
                {
                    if (movement.currentLane == PlayerMovement.Lane.Left)
                    {
                        SetSwipeHint(false);
                    }
                    else
                    {
                        SetSwipeHint();
                    }
                }

                tutorialText.SetActive(true);
            }
        }
        else
        {
            tutorialText.SetActive(false);
            swipeImage.SetActive(false);
        }
    }

    private void SetSwipeHint(bool left = true)
    {
        if (left)
        {
            tutorialText.GetComponent<Text>().text = "Swipe Left";
            swipeImage.GetComponent<Image>().sprite = leftImage;
        }
        else
        {
            tutorialText.GetComponent<Text>().text = "Swipe right";
            swipeImage.GetComponent<Image>().sprite = rightImage;
        }

        swipeImage.SetActive(true);
    }

    private void InstantiateQuestionBoxes()
    {
        int i = 1;

        foreach (Question question in tutorialQuestions)
        {
            //Debug.Log("Question: " + i);
            //Debug.Log("Correct Lane is: " + question.correctLane);

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