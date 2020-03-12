using Assets.Scripts.GameManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestionBox : MonoBehaviour
{
    public int number;
    public int correctNumber;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        QuestionGeneration questionGeneration = GameObject.Find("QuestionManager").GetComponent<QuestionGeneration>();

        // play animations on boxes here? or whatever, add a particle effect
        // can add a particle effect where the box was?

        // delete this boxes
        Destroy(questionGeneration.questionBoxes[0].gameObject);
        Destroy(questionGeneration.questionBoxes[1].gameObject);
        Destroy(questionGeneration.questionBoxes[2].gameObject);

        questionGeneration.questionBoxes.RemoveRange(0, 3);

        if (number != correctNumber)
        {
            questionGeneration.DeleteLastQuestion();
            AnsweredIncorrectly();
            return;
        }
        else
        {
            // correct play sound
            GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>().Play();

            // spawn the next question boxes
            questionGeneration.AddQuestion(true);
        }
    }

    private void AnsweredIncorrectly()
    {
        GameState.ShowGameOverUI();

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
