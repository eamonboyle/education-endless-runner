using System.Collections;
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

        public new bool generateQuestions = true;

        public GameObject player;
        public GameObject questionBox;
        public GameObject questionText;

        public List<GameObject> questionBoxes;

        private QuestionType questionType;
        private char questionSymbol;
        private int number1;
        private int number2;
        private int wrongNumber1;
        private int wrongNumber2;
        private int answer;

        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.FindWithTag("Player");

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

        // Update is called once per frame
        void Update()
        {
            if (!generateQuestions)
            {
                return;
            }

            if (!GameState.GetQuestionExists() && GameState.IsRunning())
            {
                GenerateQuestion();

                GameState.SetQuestionExists(true);
            }
        }

        public void GenerateQuestion()
        {
            CalculateNumbers();
            WriteQuestionText();


            // work out the zindex to spawn the boxes at
            // make this shorter when the difficulty increases
            //float spawnZ = UnityEngine.Random.Range(player.transform.position.z + 15.0f, player.transform.position.z + 25.0f);
            float spawnZ = player.transform.position.z + 35.0f;


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

        private void WriteQuestionText()
        {
            questionText.GetComponent<Text>().text = number1.ToString() + " " + questionSymbol + " " + number2.ToString();
        }

        private void CalculateNumbers()
        {
            /*
             * Question Equation Generator
            */

            // if score < 150 (ex.)
                // generate two random numbers (low range)
                    // if division, make sure number1 > number2
                // generate answer from two random numbers
                // generate two wrong answers with random range
            // if score > 151 && score < 450
                // generate two random numbers (higher range)
                    // if division, number1 > number2
                // generate answer from two random numbers
                // generate two wrong answers 
                    // calculate range based on % from answer
                        // use the score multiplier to help calculate this

            int score = GameState.GetScore();
            int firstRange = 5;
            int secondRange = 25;
            int firstDivisionRange = 1;
            int secondDivisionRange = 10;

            if (score < 60)
            {
                number1 = UnityEngine.Random.Range(firstRange, secondRange);
                number2 = UnityEngine.Random.Range(firstRange, secondRange);

                while (number1 == number2)
                {
                    number2 = UnityEngine.Random.Range(firstRange, secondRange);
                }

                if (questionType == QuestionType.Division)
                {
                    number1 = UnityEngine.Random.Range(firstDivisionRange, secondDivisionRange);
                    number2 = UnityEngine.Random.Range(firstDivisionRange, secondDivisionRange);

                    if (number1 == number2)
                    {
                        number2 = UnityEngine.Random.Range(firstDivisionRange, secondDivisionRange);
                    }

                    while (number1 < number2)
                    {
                        number2 = UnityEngine.Random.Range(firstDivisionRange, secondDivisionRange);
                    }
                }

                if (questionType == QuestionType.Multiplication)
                {
                    number1 = UnityEngine.Random.Range(firstDivisionRange, secondDivisionRange);
                    number2 = UnityEngine.Random.Range(firstDivisionRange, secondDivisionRange);
                }

                answer = number1 + number2;

                if (questionType == QuestionType.Subtraction)
                {
                    answer = number1 - number2;
                }
                else if (questionType == QuestionType.Multiplication)
                {
                    answer = number1 * number2;
                }
                else if (questionType == QuestionType.Division)
                {
                    answer = number1 / number2;
                }

                wrongNumber1 = UnityEngine.Random.Range(firstRange, secondRange);
                wrongNumber2 = UnityEngine.Random.Range(firstRange, secondRange);

                while (wrongNumber1 == wrongNumber2)
                {
                    wrongNumber2 = UnityEngine.Random.Range(firstRange, secondRange);
                }

                return;
            }
            else
            {
                firstRange = 25;
                secondRange = 50;
                firstDivisionRange = 5;
                secondDivisionRange = 13;

                number1 = UnityEngine.Random.Range(firstRange, secondRange);
                number2 = UnityEngine.Random.Range(firstRange, secondRange);

                while (number1 == number2)
                {
                    number2 = UnityEngine.Random.Range(firstRange, secondRange);
                }

                if (questionType == QuestionType.Division)
                {
                    number2 = UnityEngine.Random.Range(firstDivisionRange, secondDivisionRange);

                    while (number1 < number2)
                    {
                        number2 = UnityEngine.Random.Range(firstDivisionRange, secondDivisionRange);
                    }
                }

                if (questionType == QuestionType.Multiplication)
                {
                    number1 = UnityEngine.Random.Range(firstDivisionRange, secondDivisionRange);
                    number2 = UnityEngine.Random.Range(firstDivisionRange, secondDivisionRange);
                }

                answer = number1 + number2;

                if (questionType == QuestionType.Subtraction)
                {
                    answer = number1 - number2;
                }
                else if (questionType == QuestionType.Multiplication)
                {
                    answer = number1 * number2;
                }
                else if (questionType == QuestionType.Division)
                {
                    answer = number1 / number2;
                }

                // calculate wrong answers

                int eachWay = 15;

                if (score > 100)
                {
                    eachWay = 12;
                }
                else if (score > 120)
                {
                    eachWay = 11;
                }
                else if (score > 140)
                {
                    eachWay = 10;
                }
                else if (score > 160)
                {
                    eachWay = 9;
                }
                else if (score > 180)
                {
                    eachWay = 8;
                }
                else if (score > 200)
                {
                    eachWay = 7;
                }
                else if (score > 220)
                {
                    eachWay = 6;
                }
                else if (score > 240)
                {
                    eachWay = 5;
                }
                else if (score > 260)
                {
                    eachWay = 4;
                }
                else if (score > 280)
                {
                    eachWay = 3;
                }
                else if (score > 300)
                {
                    eachWay = 2;
                }
                else if (score > 320)
                {
                    eachWay = 1;
                }

                //int eachwayRand = UnityEngine.Random.Range(1, eachWay)

                int randomRangeMin = answer - eachWay;
                int randomRangeMax = answer + eachWay;

                wrongNumber1 = UnityEngine.Random.Range(randomRangeMin, randomRangeMax);
                wrongNumber2 = UnityEngine.Random.Range(randomRangeMin, randomRangeMax);

                while (wrongNumber1 == wrongNumber2)
                {
                    wrongNumber2 = UnityEngine.Random.Range(randomRangeMin, randomRangeMax);
                }
            }




            // TODO: increase these ranges as time passes
            //number1 = UnityEngine.Random.Range(0, 70);
            //number2 = UnityEngine.Random.Range(0, 70);

            //if (number1 == number2)
            //{
            //    number2 = UnityEngine.Random.Range(0, 70);
            //}

            //answer = number1 + number2;

            //// as time goes on calculate this on a % range from the answer
            ////wrongNumber1 = UnityEngine.Random.Range(0, 70);
            ////wrongNumber2 = UnityEngine.Random.Range(0, 70);
            //wrongNumber1 = UnityEngine.Random.Range(0, 70);
            //wrongNumber2 = UnityEngine.Random.Range(0, 70);

            //if (wrongNumber1 == wrongNumber2)
            //{
            //    wrongNumber1 = UnityEngine.Random.Range(0, 70);
            //}

            //// debug
            //Debug.Log(number1 + " " + questionSymbol + " " + number2 + " = " + answer);
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