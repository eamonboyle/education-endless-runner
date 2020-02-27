﻿using System.Collections;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

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

        public bool enabled = true;

        public GameObject player;
        public GameObject questionBox;
        public GameObject questionText;

        public List<GameObject> questionBoxes;

        public bool questionExists = false;

        private QuestionType questionType;
        private int number1;
        private int number2;
        private int wrongNumber1;
        private int wrongNumber2;
        private int answer;

        // Start is called before the first frame update
        void Start()
        {
            questionType = QuestionType.Addition;
        }

        // Update is called once per frame
        void Update()
        {
            if (!enabled)
            {
                return;
            }

            if (!questionExists)
            {
                //Debug.Log("GENERATE A QUESTION");

                GenerateQuestion();

                questionExists = true;
            }
        }

        public void GenerateQuestion()
        {
            CalculateNumbers();

            questionText.GetComponent<Text>().text = number1.ToString() + " + " + number2.ToString();

            // work out the zindex to spawn the boxes at
            // make this shorter when the difficulty increases
            float spawnZ = UnityEngine.Random.Range(player.transform.position.z + 15.0f, player.transform.position.z + 25.0f);


            // MAKE THE NUMBER SPAWN IN RANDOM LANES

            // put the numbers into a list
            List<int> numberList = new List<int>();
            numberList.Add(wrongNumber1);
            numberList.Add(wrongNumber2);
            numberList.Add(answer);

            // test randomMizer
            numberList = RandomizeBoxPlacement(numberList);

            //foreach (int testNUmber in numberList)
            //{
            //    Debug.Log("TEST NUMBER: " + testNUmber);
            //}

            //// randomly sort the list
            //numberList = numberList.OrderBy(a => System.Guid.NewGuid()).ToList();

            // right box
            GameObject rightBox = Instantiate(questionBox, new Vector3(1.586f, 1.3f, spawnZ), Quaternion.identity, null);
            rightBox.GetComponent<QuestionBox>().number = numberList[0];
            rightBox.GetComponent<QuestionBox>().correctNumber = answer;
            rightBox.GetComponentInChildren<TextMeshPro>().text = rightBox.GetComponent<QuestionBox>().number.ToString();

            // center
            int wrong1 = UnityEngine.Random.Range(0, 200);
            GameObject centerBox = Instantiate(questionBox, new Vector3(0f, 1.3f, spawnZ), Quaternion.identity, null);
            centerBox.GetComponent<QuestionBox>().number = numberList[1];
            centerBox.GetComponent<QuestionBox>().correctNumber = answer;
            centerBox.GetComponentInChildren<TextMeshPro>().text = centerBox.GetComponent<QuestionBox>().number.ToString();

            // left
            int wrong2 = UnityEngine.Random.Range(0, 200);
            GameObject leftBox = Instantiate(questionBox, new Vector3(-1.586f, 1.3f, spawnZ), Quaternion.identity, null);
            leftBox.GetComponent<QuestionBox>().number = numberList[2];
            leftBox.GetComponent<QuestionBox>().correctNumber = answer;
            leftBox.GetComponentInChildren<TextMeshPro>().text = leftBox.GetComponent<QuestionBox>().number.ToString();

            questionBoxes.Add(rightBox);
            questionBoxes.Add(leftBox);
            questionBoxes.Add(centerBox);
        }

        private void CalculateNumbers()
        {
            // TODO: increase these ranges as time passes
            number1 = UnityEngine.Random.Range(0, 70);
            number2 = UnityEngine.Random.Range(0, 70);

            if (number1 == number2)
            {
                number2 = UnityEngine.Random.Range(0, 70);
            }

            answer = number1 + number2;

            // as time goes on calculate this on a % range from the answer
            //wrongNumber1 = UnityEngine.Random.Range(0, 70);
            //wrongNumber2 = UnityEngine.Random.Range(0, 70);
            wrongNumber1 = UnityEngine.Random.Range(0, 70);
            wrongNumber2 = UnityEngine.Random.Range(0, 70);

            if (wrongNumber1 == wrongNumber2)
            {
                wrongNumber1 = UnityEngine.Random.Range(0, 70);
            }

            // debug
            Debug.Log(number1 + " + " + number2 + " = " + answer);
        }

        private List<int> RandomizeBoxPlacement(List<int> numbers)
        {
            System.Random rnd = new System.Random();

            for (int i = numbers.Count; i > 0; i--)
            {
                numbers = Swap(numbers, 0, rnd.Next(0, i));
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
    }
}