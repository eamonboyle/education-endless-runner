using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionGenerator : MonoBehaviour
{
    public enum QuestionType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division
    }

    public GameObject questionBox;
    public GameObject questionText;

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

            questionText.GetComponent<Text>().text = number1.ToString() + " + " + number2.ToString();

            // right box
            GameObject rightBox = Instantiate(questionBox, new Vector3(1.586f, 1.3f, 19.0f), Quaternion.identity, null);
            rightBox.GetComponent<QuestionBox>().number = answer;
            rightBox.GetComponentInChildren<TextMeshPro>().text = answer.ToString();

            // center
            int wrong1 = Random.Range(0, 200);
            GameObject centerBox = Instantiate(questionBox, new Vector3(0f, 1.3f, 19.0f), Quaternion.identity, null);
            centerBox.GetComponent<QuestionBox>().number = wrong1;
            centerBox.GetComponentInChildren<TextMeshPro>().text = wrong1.ToString();

            // left
            int wrong2 = Random.Range(0, 200);
            GameObject leftBox = Instantiate(questionBox, new Vector3(-1.586f, 1.3f, 19.0f), Quaternion.identity, null);
            leftBox.GetComponent<QuestionBox>().number = wrong2;
            leftBox.GetComponentInChildren<TextMeshPro>().text = wrong2.ToString();



            questionExists = true;
        }
    }
}
