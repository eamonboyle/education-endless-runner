﻿using Assets.Scripts.GameManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestionBox : MonoBehaviour
{
    public int number;
    public int correctNumber;

    private new GameObject camera;

    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.FindWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        // delete the boxes once they go off camera
        //if ((transform.position.z - 5.0f) < camera.transform.position.z)
        //{
        //    Destroy(gameObject);
        //}
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        // find the game manager gameobject
        QuestionGenerator questionGenerator = GameObject.FindWithTag("GameManager").GetComponent<QuestionGenerator>();

        // delete these question boxes
        // TODO: Do this another way, where when it's offscreen it deletes?
        Destroy(questionGenerator.questionBoxes[0].gameObject);
        Destroy(questionGenerator.questionBoxes[1].gameObject);
        Destroy(questionGenerator.questionBoxes[2].gameObject);
        questionGenerator.questionBoxes.RemoveRange(0, 3);

        // work out if they were correct
        if (number != correctNumber)
        {
            AnsweredIncorrectly();
            return;
        }
        else
        {
            // set the bool question to false
            // to generate a new question
            questionGenerator.questionExists = false;

            // play sound
            GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>().Play();
        }
    }

    private void AnsweredIncorrectly()
    {
        GameState.ShowGameOverUI();

        // play a little fall animation?
        PlayFallAnimation();




        //SceneManager.LoadScene("GameOver");
    }

    private void PlayFallAnimation()
    {
        GameObject player = GameObject.Find("PlayerObject");
        player.GetComponent<Animator>().Play("stumbleBackwards");
        GameObject.Find("PlayerObject").GetComponent<Animator>().SetBool("isRunning", false);
    }
}
