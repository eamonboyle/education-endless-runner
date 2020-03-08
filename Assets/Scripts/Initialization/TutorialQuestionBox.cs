using Assets.Scripts.GameManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialQuestionBox : MonoBehaviour
{
    public int number;
    public int correctNumber;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        TutorialQuestions tutorialQuestions = GameObject.Find("GameManager").GetComponent<TutorialQuestions>();

        // delete these question boxes
        Destroy(tutorialQuestions.questionBoxes[0].gameObject);
        Destroy(tutorialQuestions.questionBoxes[1].gameObject);
        Destroy(tutorialQuestions.questionBoxes[2].gameObject);
        tutorialQuestions.questionBoxes.RemoveRange(0, 3);

        // work out if they were correct
        if (number != correctNumber)
        {
            AnsweredIncorrectly();
            return;
        }
        else
        {
            // play sound
            GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>().Play();
        }

        // increase the question count on tutorial questions class
        tutorialQuestions.currentQuestion++;

        // if answered the last question
        if (tutorialQuestions.currentQuestion > tutorialQuestions.tutorialQuestions.Count - 1)
        {
            // bring up the tutorial complete ui
            GameState.ShowTutorialCompleteUI();
        }
    }

    private void AnsweredIncorrectly()
    {
        GameState.ShowTutorialGameOver();

        // play a little fall animation?
        PlayFallAnimation();
    }

    private void PlayFallAnimation()
    {
        GameObject player = GameObject.Find("PlayerObject");
        player.GetComponent<Animator>().Play("stumbleBackwards");
        GameObject.Find("PlayerObject").GetComponent<Animator>().SetBool("isRunning", false);
    }
}
