using Assets.Scripts.GameManagement;
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

        Debug.Log("COLLIDED WITH: " + other.gameObject.name);

        // work out if right or wrong
        if (number != correctNumber)
        {
            // wrong
            SceneManager.LoadScene(3);
            return;
        }

        // find the game manager gameobject
        QuestionGenerator questionGenerator = GameObject.FindWithTag("GameManager").GetComponent<QuestionGenerator>();

        // delete these question boxes
        Destroy(questionGenerator.questionBoxes[0].gameObject);
        Destroy(questionGenerator.questionBoxes[1].gameObject);
        Destroy(questionGenerator.questionBoxes[2].gameObject);
        questionGenerator.questionBoxes.RemoveRange(0, 3);

        // set the bool question to false
        // to generate a new question
        questionGenerator.questionExists = false;
    }
}
