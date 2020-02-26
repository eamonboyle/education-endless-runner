using System.Collections;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameManagement
{
    public class QuestionGenerator : MonoBehaviour
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

        public bool questionExists = false;

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

                GenerateQuestion();

                questionExists = true;
            }
        }

        public void GenerateQuestion()
        {
            // TODO: increase these ranges as time passes
            number1 = Random.Range(0, 50);
            number2 = Random.Range(0, 50);
            answer = number1 + number2;

            // debug
            Debug.Log(number1 + " + " + number2 + " = " + answer);

            questionText.GetComponent<Text>().text = number1.ToString() + " + " + number2.ToString();

            // work out the zindex to spawn the boxes at
            // make this shorter when the difficulty increases
            float spawnZ = Random.Range(player.transform.position.z + 15.0f, player.transform.position.z + 25.0f);


            // MAKE THE NUMBER SPAWN IN RANDOM LANES

            // put the numbers into a list
            List<int> numberList = new List<int>();
            numberList.Add(number1);
            numberList.Add(number2);
            numberList.Add(answer);

            // randomly sort the list
            numberList = numberList.OrderBy(a => System.Guid.NewGuid()).ToList();

            // right box
            GameObject rightBox = Instantiate(questionBox, new Vector3(1.586f, 1.3f, spawnZ), Quaternion.identity, null);
            rightBox.GetComponent<QuestionBox>().number = numberList[0];
            rightBox.GetComponent<QuestionBox>().correctNumber = answer;
            rightBox.GetComponentInChildren<TextMeshPro>().text = rightBox.GetComponent<QuestionBox>().number.ToString();

            // center
            int wrong1 = Random.Range(0, 200);
            GameObject centerBox = Instantiate(questionBox, new Vector3(0f, 1.3f, spawnZ), Quaternion.identity, null);
            centerBox.GetComponent<QuestionBox>().number = numberList[1];
            centerBox.GetComponent<QuestionBox>().correctNumber = answer;
            centerBox.GetComponentInChildren<TextMeshPro>().text = centerBox.GetComponent<QuestionBox>().number.ToString();

            // left
            int wrong2 = Random.Range(0, 200);
            GameObject leftBox = Instantiate(questionBox, new Vector3(-1.586f, 1.3f, spawnZ), Quaternion.identity, null);
            leftBox.GetComponent<QuestionBox>().number = numberList[2];
            leftBox.GetComponent<QuestionBox>().correctNumber = answer;
            leftBox.GetComponentInChildren<TextMeshPro>().text = leftBox.GetComponent<QuestionBox>().number.ToString();

            questionBoxes.Add(rightBox);
            questionBoxes.Add(leftBox);
            questionBoxes.Add(centerBox);
        }
    }
}