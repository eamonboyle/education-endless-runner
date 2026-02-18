using UnityEngine;

public class QuestionBox : MonoBehaviour
{
    public int number;
    public int correctNumber;
    [SerializeField] private QuestionGeneration questionGeneration;

    public void Initialize(QuestionGeneration owner, int displayedNumber, int answer)
    {
        questionGeneration = owner;
        number = displayedNumber;
        correctNumber = answer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        if (questionGeneration == null)
        {
            questionGeneration = GameObject.Find("QuestionManager").GetComponent<QuestionGeneration>();
        }

        // play animations on boxes here? or whatever, add a particle effect
        // can add a particle effect where the box was?
        questionGeneration.ClearCurrentQuestionBoxes();

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
        player.GetComponent<Animator>().SetBool("isRunning", false);
    }
}
